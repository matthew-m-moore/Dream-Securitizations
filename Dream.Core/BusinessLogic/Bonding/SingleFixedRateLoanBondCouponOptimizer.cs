using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Paydown;
using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Bonding
{
    public class SingleFixedRateLoanBondCouponOptimizer<T, U> : BondCouponOptimizer<T, U> 
        where T : FixedRateLoan
        where U : FixedRateLoanPaydownCalculator
    {
        private T _fixedRateLoan;  

        public SingleFixedRateLoanBondCouponOptimizer(
            List<PaydownScenario> paydownScenarios,
            T fixedRateLoan,
            U loanPaydownCalculator,
            DateTime collateralCutOffDate,
            DateTime bondPaymentStartDate,
            bool lockBondPrincipalPaydownToLoan,
            bool ignoreFirstPaymentForBondPrincipalPaydown) 
            : base (paydownScenarios, loanPaydownCalculator, collateralCutOffDate, bondPaymentStartDate, 
                    lockBondPrincipalPaydownToLoan, ignoreFirstPaymentForBondPrincipalPaydown)
        {
            _fixedRateLoan = fixedRateLoan;
        }

        public override double FindOptimalBondCoupon(
            PaydownScenario paydownScenario,
            List<ContractualCashFlow> loanContractualCashFlows)
        {
            var targetValue = 0.0;
            var floorValue = 0.0;
            var ceilingValue = _fixedRateLoan.InitialCouponRate;
            var precision = 1e-8;
            var maxIterations = 1000;

            PostPaydownContractualCashFlows = loanContractualCashFlows;
            var baseFixedRateLoan = (T) _fixedRateLoan.Copy();

            OptimalBondCoupon = ceilingValue;
            var sumOfCashFlowDeltaAtCeiling = CheckBondCashFlows(OptimalBondCoupon, baseFixedRateLoan, paydownScenario);

            if (sumOfCashFlowDeltaAtCeiling < 0.0)
            {
                OptimalBondCoupon = NumericalSearchUtility.NewtonRaphsonWithBisection(
                    optimalCoupon => CheckBondCashFlows(optimalCoupon, baseFixedRateLoan, paydownScenario),
                    targetValue,
                    precision,
                    floorValue,
                    ceilingValue,
                    maxIterations);
            }

            return OptimalBondCoupon;
        }

        private double CheckBondCashFlows(double bondCoupon, T baseFixedRateLoan, PaydownScenario paydownScenario)
        {
            var bondedFixedRateLoan = (T) baseFixedRateLoan.Copy();         

            // Get the initial contractual cashflows of the bonded single loan
            bondedFixedRateLoan.InitialCouponRate = bondCoupon;
            bondedFixedRateLoan.StartDate = _BondPaymentStartDate;
            bondedFixedRateLoan.InterestAccrualStartDate = _BondPaymentStartDate;
            bondedFixedRateLoan.FirstPaymentDate = _BondPaymentStartDate
                .AddMonths(bondedFixedRateLoan.PrincipalPaymentFrequencyInMonths);
            BondPrePaydownContractualCashFlows = bondedFixedRateLoan.GetContractualCashFlows();

            // Adjust the bonded single loan for any paydown, and get the post-paydown cash flows
            var bondPrincipalApplied = AdjustBondForPaydown(bondedFixedRateLoan, baseFixedRateLoan, paydownScenario);
            BondPostPaydownContractualCashFlows = bondedFixedRateLoan.GetContractualCashFlows();
            var bondCashFlowCount = BondPostPaydownContractualCashFlows.Count();

            var isFirstPayment = true;
            var firstPaymentCashSurplus = 0.0;
            var paymentCashSurplus = 0.0;
            var sumOfCashSurplusToBond = 0.0;

            for (var cashFlowCounter = 0; cashFlowCounter < bondCashFlowCount; cashFlowCounter++)
            {
                var bondCashOutflow = BondPostPaydownContractualCashFlows[cashFlowCounter].Payment;
                var loanCashInflow = PostPaydownContractualCashFlows[cashFlowCounter].Payment;

                // There is an assumption here that bond and loan payment periods will be lined up
                if (loanCashInflow > 0.0)
                {
                    var percentageFeeDollars = paydownScenario.PercentageFees * (loanCashInflow + paydownScenario.AdditionalCustomerFee);
                    loanCashInflow -= percentageFeeDollars;
                    loanCashInflow -= paydownScenario.FixedFees;

                    if (isFirstPayment && _PaydownCalculator.FirstPaymentIfAlreadyCollectedOrUnamended.HasValue)
                    {
                        var firstLoanCashInflow = 0.0;
                        var firstBondCashOutflow = bondCashOutflow;

                        if (paydownScenario.PaymentAlreadyMadeForPeriod)
                        {
                            firstLoanCashInflow = _PaydownCalculator.
                                DetermineCashInFlowWhenPaymentAlreadyCollected(
                                paydownScenario,
                                bondPrincipalApplied,
                                bondCoupon);

                            if (PostPaydownContractualCashFlows[cashFlowCounter].EndingBalance <= 0.0)
                            {
                                firstBondCashOutflow = bondCoupon *
                                    _PaydownCalculator.AccruedInterestPeriod *
                                    BondPostPaydownContractualCashFlows[cashFlowCounter].StartingBalance;

                                firstBondCashOutflow += BondPostPaydownContractualCashFlows[cashFlowCounter].StartingBalance;

                                var refundDue = baseFixedRateLoan.InitialCouponRate * 
                                    (1.0 - _PaydownCalculator.AccruedInterestPeriod) * 
                                    PostPaydownContractualCashFlows[cashFlowCounter].StartingBalance;

                                firstBondCashOutflow += refundDue;
                            }
                        }
                        else if (!paydownScenario.NextBillAmendedForPayment.GetValueOrDefault(true))
                        {
                            firstLoanCashInflow = _PaydownCalculator.
                                DetermineCashInFlowWhenPaymentIsUnamended(paydownScenario);

                            var unadjustedLoanCashInflow = PostPaydownContractualCashFlows[cashFlowCounter].Payment;
                            var unamendedTaxPaymentDue = _PaydownCalculator.FirstPaymentIfAlreadyCollectedOrUnamended.Value;

                            var refundDue = Math.Max(0, unamendedTaxPaymentDue - unadjustedLoanCashInflow);
                            firstBondCashOutflow += refundDue;
                        }



                        firstPaymentCashSurplus = firstLoanCashInflow - firstBondCashOutflow;
                        paymentCashSurplus = loanCashInflow - bondCashOutflow;

                        // Takes whichever surplus value is lower, or in case of a sign dispute go negative
                        var surplusToUse = Math.Min(firstPaymentCashSurplus, paymentCashSurplus);
                        if (firstPaymentCashSurplus < 0.0 && paymentCashSurplus >= 0.0)
                        {
                            return firstPaymentCashSurplus;
                        }

                        sumOfCashSurplusToBond += surplusToUse;
                        isFirstPayment = false;
                        continue;
                    }
                }

                paymentCashSurplus = loanCashInflow - bondCashOutflow;
                sumOfCashSurplusToBond += paymentCashSurplus;
            }

            return sumOfCashSurplusToBond;
        }

        protected virtual double? AdjustBondForPaydown(T bondedFixedRateLoan, T baseFixedRateLoan, PaydownScenario paydownScenario)
        {
            var monthsBetweenStartDates = DateUtility.MonthsBetweenTwoDates(
                baseFixedRateLoan.StartDate,
                _BondPaymentStartDate);

            var bondCallDate = paydownScenario.BondCallDate;
            var adjustedBondCallDate = bondCallDate.AddMonths(monthsBetweenStartDates);

            var bondPaydownCalculator = _PaydownCalculator.GetNewPaydownCalculator(
                BondPrePaydownContractualCashFlows,
                bondedFixedRateLoan.InterestAccrualDayCountConvention);

            bondPaydownCalculator.PrepareForCalculation(adjustedBondCallDate);
            
            var bondPrincipalApplied = DeterminePrincipalToApply(bondPaydownCalculator, paydownScenario, bondedFixedRateLoan);

            if (_PaydownCalculator.YearEndPrincipalToApply > 0.0)
            {
                var bondFirstCashFlowAfterPaydown = bondPaydownCalculator.FirstCashFlowAfterPaydown;
                bondPaydownCalculator.PrepareForCalculation(bondFirstCashFlowAfterPaydown.PeriodDate);

                var isPrincipalAmountTotal = true;
                var bondYearEndPrincipalToApply = DetermineYearEndPrincipalToApply(bondPaydownCalculator, paydownScenario, bondedFixedRateLoan);
                var excessCashFlow = Math.Max(0, _PaydownCalculator.TotalPaymentApplied - bondPaydownCalculator.TotalPaymentApplied);

                bondedFixedRateLoan.AddCustomPayment(bondFirstCashFlowAfterPaydown.PeriodDate, bondYearEndPrincipalToApply, -1 * excessCashFlow, isPrincipalAmountTotal);
            }

            return bondPrincipalApplied;   
        }

        protected double? DeterminePrincipalToApply(
            PaydownCalculator bondPaydownCalculator, 
            PaydownScenario paydownScenario, 
            FixedRateLoan bondedFixedRateLoan)
        {
            // If the principal paydown is locked to the loan, use that specific value instead
            if (_LockBondPrincipalPaydownToLoan)
            {
                var principalToApply = _PaydownCalculator.PrincipalToApply;
                bondPaydownCalculator.AdjustLoanForSpecificPaydown(_BondPaymentStartDate, bondedFixedRateLoan, principalToApply);
                return null;
            }
            else
            {
                if (!_IgnoreFirstPaymentForBondPrincipalPaydown)
                {
                    CalculatePaydownDollarAmount(paydownScenario, bondPaydownCalculator);
                }

                bondPaydownCalculator.CalculatePrincipalToApply(paydownScenario, bondedFixedRateLoan);
                bondPaydownCalculator.AdjustLoanForPaydown(_BondPaymentStartDate, bondedFixedRateLoan);

                paydownScenario.PaydownDollarAmount = null;
                return bondPaydownCalculator.PrincipalToApply;
            }
        }

        protected double DetermineYearEndPrincipalToApply(
            PaydownCalculator bondPaydownCalculator,
            PaydownScenario paydownScenario,
            FixedRateLoan bondedFixedRateLoan)
        {
            // If the principal paydown is locked to the loan, use that specific value instead
            if (_LockBondPrincipalPaydownToLoan)
            {
                return _PaydownCalculator.YearEndPrincipalToApply;
            }
            else
            {
                var yearEndPaydownScenario = new PaydownScenario(paydownScenario)
                {
                    PaymentAlreadyMadeForPeriod = true,
                    SendRefundIfPossible = true,
                    PaydownDollarAmount = _PaydownCalculator.YearEndPrincipalToApply
                };

                bondPaydownCalculator.CalculatePrincipalToApply(yearEndPaydownScenario, bondedFixedRateLoan);
                return bondPaydownCalculator.PrincipalToApply;
            }
        }

        private void CalculatePaydownDollarAmount(PaydownScenario paydownScenario, PaydownCalculator bondPaydownCalculator)
        {
            if (_PaydownCalculator.FirstPaymentIfAlreadyCollectedOrUnamended.HasValue)
            {
                var cashAlreadyCollected = _PaydownCalculator.DetermineCashAvailableWhenPaymentAlreadyCollectedOrUnamended(paydownScenario);
                var bondPaymentOriginallyDue = bondPaydownCalculator.FirstCashFlowAfterPaydown.Payment;
                var excessCashAvailable = Math.Max(cashAlreadyCollected - bondPaymentOriginallyDue, 0.0);

                var paydownAmount = paydownScenario.PaydownPercentageAmount * _PaydownCalculator.BalanceToCalculatePaydown;
                var paydownDollarAmount = paydownAmount + excessCashAvailable;

                paydownScenario.PaydownDollarAmount = paydownDollarAmount;
            }
        }
    }
}
