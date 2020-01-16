using Dream.Common;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Paydown;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Reporting.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Bonding
{
    public abstract class BondCouponOptimizer<T, U> 
        where T : Loan 
        where U : PaydownCalculator
    {
        public double OptimalBondCoupon = double.NaN;
        public List<ContractualCashFlow> PrePaydownContractualCashFlows { get; protected set; }
        public List<ContractualCashFlow> PostPaydownContractualCashFlows { get; protected set; }
        public List<ContractualCashFlow> BondPrePaydownContractualCashFlows { get; protected set; }
        public List<ContractualCashFlow> BondPostPaydownContractualCashFlows { get; protected set; }

        public List<PaydownScenario> PaydownScenarios { get; protected set; }
        public List<BondCouponOptimizationResult> OptimizationResults { get; protected set; }

        protected DateTime _CollateralCutOffDate;
        protected DateTime _BondPaymentStartDate;

        protected bool _LockBondPrincipalPaydownToLoan;
        protected bool _IgnoreFirstPaymentForBondPrincipalPaydown;

        protected U _PaydownCalculator;

        public BondCouponOptimizer(
            List<PaydownScenario> paydownScenarios,
            U paydownCalculator,    
            DateTime collateralCutOffDate,
            DateTime bondPaymentStartDate,
            bool lockBondPrincipalPaydownToLoan,
            bool ignoreFirstPaymentForBondPrincipalPaydown)
        {
            PaydownScenarios = paydownScenarios;

            _PaydownCalculator = paydownCalculator;

            _CollateralCutOffDate = collateralCutOffDate;
            _BondPaymentStartDate = bondPaymentStartDate;

            _LockBondPrincipalPaydownToLoan = lockBondPrincipalPaydownToLoan;
            _IgnoreFirstPaymentForBondPrincipalPaydown = ignoreFirstPaymentForBondPrincipalPaydown;

            BondPrePaydownContractualCashFlows = new List<ContractualCashFlow>();
            BondPostPaydownContractualCashFlows = new List<ContractualCashFlow>();

            OptimizationResults = new List<BondCouponOptimizationResult>();
        }

        public abstract double FindOptimalBondCoupon(
            PaydownScenario paydownScenario,
            List<ContractualCashFlow> loanContractualCashFlows);

        public void OptimizeBondCouponsForSingleLoan(T loan, List<ContractualCashFlow> prePaydownContractualCashFlows)
        {
            PrePaydownContractualCashFlows = prePaydownContractualCashFlows;

            foreach (var paydownScenario in PaydownScenarios)
            {
                var clonedPaydownScenario = new PaydownScenario(paydownScenario);
                var paydownScenarioResult = AnalyzePaydownScenario(loan, clonedPaydownScenario);
                OptimizationResults.Add(paydownScenarioResult);
            }
        }

        private BondCouponOptimizationResult AnalyzePaydownScenario(T loan, PaydownScenario paydownScenario)
        {
            var copiedLoan = (T)loan.Copy();
            var firstPaymentDate = copiedLoan.FirstPaymentDate;
            var maturityDate = PrePaydownContractualCashFlows.Last().PeriodDate;

            _PaydownCalculator = (U)_PaydownCalculator.GetNewPaydownCalculator(
                PrePaydownContractualCashFlows,
                copiedLoan.InterestAccrualDayCountConvention);

            var resultsList = new List<BondCouponOptimizationResult>();

            do
            {
                PostPaydownContractualCashFlows = new List<ContractualCashFlow>();

                var inputValueAtSuccess = double.NaN;
                var greatestBondCoupon = double.NaN;

                if (paydownScenario.SolveForMaxPaydownPercentage)
                {
                    var targetPrecision = 1e-4;
                    var floorValue = 0.0;
                    var ceilingValue = 1.5;

                    paydownScenario.PaydownPercentageAmount = floorValue;
                    var greatestBondCouponAtFloor = RunBondCouponOptimizationForSingleLoan(copiedLoan, paydownScenario);

                    if (!double.IsNaN(greatestBondCouponAtFloor))
                    {
                        greatestBondCoupon = NumericalSearchUtility.BisectionWithNotANumber(
                            paydownPercent =>
                            {
                                paydownScenario.PaydownPercentageAmount = paydownPercent;
                                return RunBondCouponOptimizationForSingleLoan(copiedLoan, paydownScenario);
                            },
                            targetPrecision,
                            out inputValueAtSuccess,
                            floorValue,
                            ceilingValue);
                    }

                    if (!double.IsNaN(inputValueAtSuccess) && greatestBondCoupon < greatestBondCouponAtFloor)
                    {
                        paydownScenario.PaydownPercentageAmount = inputValueAtSuccess;
                    }
                    else
                    {
                        paydownScenario.PaydownPercentageAmount = floorValue;
                    }
                }

                greatestBondCoupon =
                    RunBondCouponOptimizationForSingleLoan(copiedLoan, paydownScenario);

                double? couponToReport = null;
                if (!double.IsNaN(greatestBondCoupon) && greatestBondCoupon > 0.0) couponToReport = greatestBondCoupon;

                var paydownScenarioResult = new BondCouponOptimizationResult
                {
                    ScenarioName = paydownScenario.ScenarioName,
                    PaydownPercentage = paydownScenario.PaydownPercentageAmount * Constants.OneHundredPercentagePoints,
                    AssessmentCoupon = loan.InitialCouponRate * Constants.OneHundredPercentagePoints,
                    MaxBondCoupon = couponToReport * Constants.OneHundredPercentagePoints,
                    AssessmentTerm = loan.MaturityTermInYears - 1,

                    PercentageFeeCollection = paydownScenario.PercentageFees * Constants.OneHundredPercentagePoints,
                    AdditionalCustomerFee = paydownScenario.AdditionalCustomerFee,
                    FixedFeeCollection = paydownScenario.FixedFees,

                    PostPaydownContractualCashFlows = PostPaydownContractualCashFlows,
                    BondPrePaydownContractualCashFlows = BondPrePaydownContractualCashFlows,
                    BondPostPaydownContractualCashFlows = BondPostPaydownContractualCashFlows,
                };

                resultsList.Add(paydownScenarioResult);

                paydownScenario.BondCallDate =
                    paydownScenario.BondCallDate.AddMonths(loan.PrincipalPaymentFrequencyInMonths);
            }
            while (paydownScenario.TryAllFutureBondCallDates &&
                   paydownScenario.BondCallDate < maturityDate);

            if (resultsList.Any(r => r.MaxBondCoupon.HasValue))
            {
                var minimumCouponResult = resultsList
                    .Where(r => r.MaxBondCoupon.HasValue)
                    .Min(r => r.MaxBondCoupon);

                return resultsList
                    .Where(r => r.MaxBondCoupon.HasValue)
                    .First(r => r.MaxBondCoupon.Value == minimumCouponResult.Value);
            }

            return resultsList.First();
        }

        private double RunBondCouponOptimizationForSingleLoan(T loan, PaydownScenario paydownScenario)
        {
            var copiedLoanToAdjustForPaydown = (T) loan.Copy();
            GetPostPaydownContractualCashFlowsForSingleLoan(
                copiedLoanToAdjustForPaydown,
                paydownScenario,          
                PrePaydownContractualCashFlows);

            if (PostPaydownContractualCashFlows.First().StartingBalance <= 0.0 ||
                double.IsNaN(PostPaydownContractualCashFlows.First().StartingBalance))
            {
                return double.NaN;
            }

            var greatestBondCoupon = double.NaN;
            if (copiedLoanToAdjustForPaydown.Balance > 0.0)
            {
                greatestBondCoupon =
                    FindOptimalBondCoupon(paydownScenario, PostPaydownContractualCashFlows);
            }

            return greatestBondCoupon;
        }

        private void GetPostPaydownContractualCashFlowsForSingleLoan(T loan,
            PaydownScenario paydownScenario,
            List<ContractualCashFlow> prePaydownContractualCashFlows)
        {
            _PaydownCalculator.PrepareForCalculation(paydownScenario.BondCallDate);
            _PaydownCalculator.CalculatePrincipalToApply(paydownScenario, loan);
            _PaydownCalculator.AdjustLoanForPaydown(_CollateralCutOffDate, loan);

            PostPaydownContractualCashFlows = loan.GetContractualCashFlows();
            _PaydownCalculator.DetermineFirstPaymentIfAlreadyCollectedOrUnamended(
                paydownScenario,
                prePaydownContractualCashFlows,
                PostPaydownContractualCashFlows);
        }
    }
}
