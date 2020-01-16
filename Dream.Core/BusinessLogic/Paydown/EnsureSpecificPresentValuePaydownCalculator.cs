using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Paydown
{
    public class EnsureSpecificPresentValuePaydownCalculator : FixedRateLoanPaydownCalculator
    {
        public int TotalMaturityTermInYears { get; private set; }
        public int RemainingMaturityTermInYears { get; private set; }

        private double _targetInitialPresentValue;
        private double _baseFixedRateLoanCoupon;
        private double _baseFixedRateLoanBalance;

        public EnsureSpecificPresentValuePaydownCalculator(
            List<ContractualCashFlow> contractualCashFlows,
            DayCountConvention interestAccrualDayCountConvention) 
            : base(contractualCashFlows, interestAccrualDayCountConvention)
        { }

        public override void PrepareForCalculation(DateTime paydownEffectiveDate)
        {
            base.PrepareForCalculation(paydownEffectiveDate);
            TotalMaturityTermInYears = CalculateTotalMaturityTerm();
            RemainingMaturityTermInYears = CalculateRemainingMaturityTerm(paydownEffectiveDate);
        }

        public void SetTargetPresentValue(double targetInitialPresentValue)
        {
            if (targetInitialPresentValue < 0.0)
            {
                throw new Exception("ERROR: Target present value cannot be negative");
            }

            _targetInitialPresentValue = targetInitialPresentValue;
        }

        public void SetBaseFixedRateLoanCoupon(double baseFixedRateLoanCoupon)
        {
            if (baseFixedRateLoanCoupon < 0.0)
            {
                throw new Exception("ERROR: Base fixed rate loan coupon cannot be negative");
            }

            _baseFixedRateLoanCoupon = baseFixedRateLoanCoupon;
        }

        public void SetBaseFixedRateLoanBalance(double baseFixedRateLoanBalance)
        {
            if (baseFixedRateLoanBalance < 0.0)
            {
                throw new Exception("ERROR: Base fixed rate loan coupon cannot be negative");
            }

            _baseFixedRateLoanBalance = baseFixedRateLoanBalance;
        }

        public override void CalculatePrincipalToApply(PaydownScenario paydownScenario, Loan loan)
        {
            if (!_IsCalculationPrepared)
            {
                throw new Exception("ERROR: The paydown calculator may not have been prepared before usage. Cannot proceed");
            }

            var currentFixedRateLoan = loan as FixedRateLoan;

            var currentRate = currentFixedRateLoan.InitialCouponRate;
            var maturityTerm = TotalMaturityTermInYears;
            var baseRate = _baseFixedRateLoanCoupon;
            var remainingTerm = RemainingMaturityTermInYears;

            // This factor can be derived from the constraint that the original value of the loan would be adjusted and need to match the value of the bond
            var appliedPrincipalFactor = (MathUtility.CalculateAnnuityFactor(currentRate, maturityTerm) / MathUtility.CalculateAnnuityFactor(currentRate, remainingTerm)) *
                                         (MathUtility.CalculateAnnuityFactor(baseRate, remainingTerm) / MathUtility.CalculateAnnuityFactor(baseRate, maturityTerm));

            var targetBalance = appliedPrincipalFactor * _baseFixedRateLoanBalance;

            var principalToApply = BalanceToCalculatePaydown - targetBalance;

            PrincipalToApply = Math.Min(currentFixedRateLoan.Balance, principalToApply);

            TotalPaymentApplied = PrincipalToApply;
            TotalPaymentApplied *= (1.0 + (currentRate * AccruedInterestPeriod));

            _IsPrincipalAppliedCalculated = true;
        }

        private int CalculateTotalMaturityTerm()
        {
            var totalNumberOfPrincipalPayments = _ContractualCashFlows
                .Where(c => c.Principal > 0.0)
                .Count();

            return totalNumberOfPrincipalPayments;
        }

        private int CalculateRemainingMaturityTerm(DateTime paydownEffectiveDate)
        {
            var remainingNumberOfPrincipalPayments = _ContractualCashFlows
                .Where(c => c.Principal > 0.0 && c.PeriodDate > paydownEffectiveDate)
                .Count();

            return remainingNumberOfPrincipalPayments;
        }

    }
}
