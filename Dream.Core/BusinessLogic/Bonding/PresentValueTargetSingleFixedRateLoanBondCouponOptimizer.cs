using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Paydown;
using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dream.Core.BusinessLogic.Bonding
{
    public class PresentValueTargetSingleFixedRateLoanBondCouponOptimizer<T, U> : SingleFixedRateLoanBondCouponOptimizer<T, U>
        where T : FixedRateLoan
        where U : FixedRateLoanPaydownCalculator
    {
        public PresentValueTargetSingleFixedRateLoanBondCouponOptimizer(
            List<PaydownScenario> paydownScenarios,
            T fixedRateLoan,
            U loanPaydownCalculator,
            DateTime collateralCutOffDate,
            DateTime bondPaymentStartDate,
            bool lockBondPrincipalPaydownToLoan,
            bool ignoreFirstPaymentForBondPrincipalPaydown) 
            : base (paydownScenarios, fixedRateLoan, loanPaydownCalculator, collateralCutOffDate, bondPaymentStartDate, 
                    lockBondPrincipalPaydownToLoan, ignoreFirstPaymentForBondPrincipalPaydown)
        { }

        protected override double? AdjustBondForPaydown(T bondedFixedRateLoan, T baseFixedRateLoan, PaydownScenario paydownScenario)
        {
            var monthsBetweenStartDates = DateUtility.MonthsBetweenTwoDates(
                baseFixedRateLoan.StartDate,
                _BondPaymentStartDate);

            var bondCallDate = paydownScenario.BondCallDate;
            var adjustedBondCallDate = bondCallDate.AddMonths(monthsBetweenStartDates);

            var bondPaydownCalculator = new EnsureSpecificPresentValuePaydownCalculator(
                    BondPrePaydownContractualCashFlows,
                    bondedFixedRateLoan.InterestAccrualDayCountConvention);

            bondPaydownCalculator.PrepareForCalculation(adjustedBondCallDate);

            var targetPresentValue = GetPresentValueTarget(baseFixedRateLoan, bondPaydownCalculator);
            bondPaydownCalculator.SetTargetPresentValue(targetPresentValue);
            bondPaydownCalculator.SetBaseFixedRateLoanCoupon(baseFixedRateLoan.InitialCouponRate);
            bondPaydownCalculator.SetBaseFixedRateLoanBalance(PostPaydownContractualCashFlows.First().StartingBalance);

            var bondPrincipalApplied = DeterminePrincipalToApply(bondPaydownCalculator, paydownScenario, bondedFixedRateLoan);
            var excessCashFlow = Math.Max(0, _PaydownCalculator.TotalPaymentApplied - bondPaydownCalculator.TotalPaymentApplied);

            if (_PaydownCalculator.YearEndPrincipalToApply > 0.0)
            {
                var bondFirstCashFlowAfterPaydown = bondPaydownCalculator.FirstCashFlowAfterPaydown;
                bondPaydownCalculator.PrepareForCalculation(bondFirstCashFlowAfterPaydown.PeriodDate);
                bondPaydownCalculator.BalanceToCalculatePaydown -= bondPrincipalApplied.GetValueOrDefault();

                var baseFixedRateLoanYearEndBalance = PostPaydownContractualCashFlows
                    .Single(c => c.PeriodDate == bondFirstCashFlowAfterPaydown.PeriodDate).EndingBalance;
                bondPaydownCalculator.SetBaseFixedRateLoanBalance(baseFixedRateLoanYearEndBalance);

                var isPrincipalAmountTotal = true;
                var bondYearEndPrincipalToApply = DetermineYearEndPrincipalToApply(bondPaydownCalculator, paydownScenario, bondedFixedRateLoan);

                bondedFixedRateLoan.AddCustomPayment(bondFirstCashFlowAfterPaydown.PeriodDate, bondYearEndPrincipalToApply, -1 * excessCashFlow, isPrincipalAmountTotal);
            }

            return bondPrincipalApplied;
        }

        private double GetPresentValueTarget(T baseFixedRateLoan, EnsureSpecificPresentValuePaydownCalculator bondPaydownCalculator)
        {
            var fixedPaymentAmount = PostPaydownContractualCashFlows[baseFixedRateLoan.PrincipalPaymentFrequencyInMonths].Payment;
            var loanCouponPerPeriod = baseFixedRateLoan.InitialCouponRate;

            var presentValueTarget = MathUtility.CalculatePresentValueOfAnnuity(
                fixedPaymentAmount,
                baseFixedRateLoan.InitialCouponRate,
                bondPaydownCalculator.TotalMaturityTermInYears);

            return presentValueTarget;
        }
    }
}
