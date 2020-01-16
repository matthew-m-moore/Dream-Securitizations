using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.Paydown
{
    public class FixedRateLoanPaydownCalculator : PaydownCalculator
    {
        public FixedRateLoanPaydownCalculator() : base() { }

        protected FixedRateLoanPaydownCalculator(
            List<ContractualCashFlow> contractualCashFlows, 
            DayCountConvention interestAccrualDayCountConvention) 
            : base(contractualCashFlows, interestAccrualDayCountConvention)
        { }

        public override PaydownCalculator GetNewPaydownCalculator(
            List<ContractualCashFlow> contractualCashFlows,
            DayCountConvention interestAccrualDayCountConvention)
        {
            return new FixedRateLoanPaydownCalculator(
                contractualCashFlows,
                interestAccrualDayCountConvention)
            {
                FloorPaydownBalanceAtZero = FloorPaydownBalanceAtZero
            };
        }

        public override void CalculatePrincipalToApply(PaydownScenario paydownScenario, Loan loan)
        {
            if (!_IsCalculationPrepared)
            {
                throw new Exception("ERROR: The paydown calculator may not have been prepared before usage. Cannot proceed");
            }

            var fixedRateLoan = loan as FixedRateLoan;
            if (fixedRateLoan == null)
            {
                throw new Exception("ERROR: The fixed rate loan paydown calculator can only operate on fixed rate loans");
            }

            // It is possible that a specific dollar paydown amount might be supplied as well
            var paydownAmount = paydownScenario.PaydownPercentageAmount * BalanceToCalculatePaydown;
            if (paydownScenario.PaydownDollarAmount.HasValue)
            {
                paydownAmount = paydownScenario.PaydownDollarAmount.Value;
            }

            var loanCoupon = fixedRateLoan.InitialCouponRate;
            var annuityFactor = CalculateAnnuityFactor(fixedRateLoan);

            var principalToApply = 0.0;

            if (_IsInInterestOnlyPeriod)
            {
                var divisor = 1.0 + (loanCoupon * AccruedInterestPeriod);
                principalToApply = paydownAmount / divisor;
            }
            else
            {
                if (!paydownScenario.PaymentAlreadyMadeForPeriod)
                {
                    if (paydownScenario.NextBillAmendedForPayment == null)
                    {
                        throw new Exception("ERROR: Whether or not the tax bill has been amended must be specified " +
                            "when taxes have already been paid for the year.");
                    }

                    var divisor = 1.0 + (loanCoupon * AccruedInterestPeriod);

                    if (!paydownScenario.NextBillAmendedForPayment.Value)
                    {
                        principalToApply = paydownAmount / divisor;
                        YearEndPrincipalToApply = principalToApply * annuityFactor;
                    }
                    else
                    {
                        var multiplier = 1.0 + annuityFactor;
                        principalToApply = multiplier * (paydownAmount / divisor);
                    }
                }
                else
                {
                    if (paydownScenario.SendRefundIfPossible)
                    {
                        principalToApply = paydownAmount;
                    }
                    else
                    {
                        var divisor = 1.0 - annuityFactor + (loanCoupon * AccruedInterestPeriod);
                        if (divisor < 0.0) divisor = 1.0;
                        principalToApply = paydownAmount / divisor;
                    }
                }
            }

            PrincipalToApply = Math.Min(fixedRateLoan.Balance, principalToApply);

            TotalPaymentApplied = PrincipalToApply;
            TotalPaymentApplied *= (1.0 + (loanCoupon * AccruedInterestPeriod));

            _IsPrincipalAppliedCalculated = true;
        }

        private double CalculateAnnuityFactor(FixedRateLoan fixedRateLoan)
        {
            var assessmentCoupon = fixedRateLoan.InitialCouponRate;

            var principalPaymentFrequency = fixedRateLoan.PrincipalPaymentFrequencyInMonths;

            var remainingNumberOfPricipalPayments = (double) _RemainingTermInMonths / principalPaymentFrequency;
            var remainingIntegerNumberOfPricipalPayments = (int) Math.Ceiling(remainingNumberOfPricipalPayments);

            var periodicCouponRate = (assessmentCoupon / Constants.MonthsInOneYear) * principalPaymentFrequency;
            var annuityFactor = MathUtility.CalculateAnnuityFactor(periodicCouponRate, remainingIntegerNumberOfPricipalPayments);

            return annuityFactor;
        }
    }
}
