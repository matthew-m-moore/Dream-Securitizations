using System;
using System.Collections.Generic;
using System.Linq;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Common.Utilities;
using Dream.Common;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.LoanStages
{
    public class FixedRateAmortizingLoanStage : LoanStage
    {
        public int AmortizationTermInMonths { get; set; }
        public FixedRateCoupon LoanCoupon { get; set; }

        private double _fixedCouponRate;

        public override List<ContractualCashFlow> CalculateScheduledPayments()
        {
            // TODO: Make a different loan stage class to handle non-30/360 day-counting conventions
            if (InterestAccrualDayCountConvention != DayCountConvention.Thirty360)
            {
                throw new Exception(string.Format("ERROR: Fixed amortizing payments are not supported for anything besides 30/360 day-counting."));
            }

            var listOfContractualCashFlows = new List<ContractualCashFlow>();
            var monthlyPeriodStartingBalance = StageStartingBalance;

            for (var monthlyPeriod = 0; monthlyPeriod < StageDurationInMonths; monthlyPeriod++)
            {
                var isFinalPeriod = monthlyPeriod == StageDurationInMonths - 1;
                var adjustedMonthlyPeriod = monthlyPeriod + StageStartMonth;             
                var monthlyPeriodDate = DateUtility.AddMonthsForCashFlowPeriods(InterestAccrualStartDate, monthlyPeriod);
            
                // This value should not change, since this is a fixed rate stage
                _fixedCouponRate = LoanCoupon.GetCouponForSpecificMonthlyPeriod(adjustedMonthlyPeriod, InterestAccrualStartDate);
              
                var principalPayment = CalculatePrincipalPayment(
                    monthlyPeriodStartingBalance,
                    adjustedMonthlyPeriod,
                    monthlyPeriod);

                ApplyCustomPrincipalPayments(ref principalPayment, monthlyPeriodDate);

                var accruedInterest = 0.0;
                var interestPayment = CalculateInterestPayment(
                    adjustedMonthlyPeriod,
                    monthlyPeriodDate,
                    principalPayment,
                    isFinalPeriod,
                    out accruedInterest);

                ApplyCustomInterestPayments(ref interestPayment, monthlyPeriodDate);

                var bondCount = PriorStagesContractualCashFlows.Last().BondCount;

                var contractualCashFlow = new ContractualCashFlow
                {
                    Period = adjustedMonthlyPeriod,
                    PeriodDate = monthlyPeriodDate,
                    BondCount = bondCount,
                    StartingBalance = monthlyPeriodStartingBalance
                };

                // If this is the final stage, then a balloon payment would be required for any
                // un-amortized portion of the balance remaining
                if (IsFinalLoanStage && monthlyPeriod == StageDurationInMonths - 1)
                {
                    contractualCashFlow.Principal = monthlyPeriodStartingBalance;
                    contractualCashFlow.Interest = interestPayment;
                }
                else
                {
                    contractualCashFlow.EndingBalance = monthlyPeriodStartingBalance - principalPayment;
                    contractualCashFlow.Principal = principalPayment;

                    contractualCashFlow.AccruedInterest = accruedInterest;
                    contractualCashFlow.Interest = interestPayment;
                }

                monthlyPeriodStartingBalance = contractualCashFlow.EndingBalance;

                listOfContractualCashFlows.Add(contractualCashFlow);
                PriorStagesContractualCashFlows.Add(contractualCashFlow);
            }

            return listOfContractualCashFlows;
        }

        private double CalculatePrincipalPayment(
            double periodStartingBalance,
            int adjustedMonthlyPeriod,
            int monthlyPeriod)
        {
            if (adjustedMonthlyPeriod < MonthsToNextPrincipalPayment)
            {
                return 0.0;
            }

            var isFirstPrincipalPayment = adjustedMonthlyPeriod == MonthsToNextPrincipalPayment;
            var isPrincipalPaymentMonth = MathUtility.CheckDivisibilityOfIntegers(
                adjustedMonthlyPeriod - MonthsToNextPrincipalPayment,
                PrincipalPaymentFrequencyInMonths);

            if (isFirstPrincipalPayment || isPrincipalPaymentMonth)
            {
                var amortizingPeriodsElapsed = (monthlyPeriod + 1) - PrincipalPaymentFrequencyInMonths;
                var remainingMonthlyPeriodsOfAmortization = AmortizationTermInMonths - amortizingPeriodsElapsed;

                var isPrincipalFrequencyValid = MathUtility.CheckDivisibilityOfIntegers(
                        remainingMonthlyPeriodsOfAmortization,
                        PrincipalPaymentFrequencyInMonths);

                if (!isPrincipalFrequencyValid && !isFirstPrincipalPayment)
                {
                    throw new Exception(string.Format("ERROR: Remaining periods of {0} months are not an integer multiple of the principal payment frequency of {1} months.",
                        remainingMonthlyPeriodsOfAmortization,
                        PrincipalPaymentFrequencyInMonths));
                }

                var periodicCouponRate = (_fixedCouponRate / Constants.MonthsInOneYear) * PrincipalPaymentFrequencyInMonths;
                var remainingPeriodsToMaturity = remainingMonthlyPeriodsOfAmortization / PrincipalPaymentFrequencyInMonths;

                var fixedAnnuityPayment = MathUtility.CalculateFixedAnnuityPayment(
                    periodStartingBalance,
                    periodicCouponRate,
                    remainingPeriodsToMaturity);

                // If there are not enough months of accrued interest to sum back through, the interest will need to be recalculated for the full period
                var accruedInterestInFixedMonthlyPayment = 0.0;
                var minimumMonthlyPeriodForLookback = adjustedMonthlyPeriod - PrincipalPaymentFrequencyInMonths;
                if (minimumMonthlyPeriodForLookback > 0)
                {
                    accruedInterestInFixedMonthlyPayment = CalculatePreviouslyAccruedInterest(minimumMonthlyPeriodForLookback, adjustedMonthlyPeriod);
                }
                else
                {
                    // Note, this logic will only work for a 30/360 day-counting convention
                    var firstContractualCashFlow = PriorStagesContractualCashFlows.First();
                    accruedInterestInFixedMonthlyPayment = firstContractualCashFlow.StartingBalance * periodicCouponRate;
                }

                var principalPayment = fixedAnnuityPayment - accruedInterestInFixedMonthlyPayment;
                return principalPayment;
            }

            return 0.0;
        }

        private double CalculateInterestPayment(
            int adjustedMonthlyPeriod,
            DateTime interestAccrualDate,
            double principalPayment,
            bool isFinalPeriod,
            out double accruedInterest)
        {
            var priorPeriodCashFlow = PriorStagesContractualCashFlows.Last();

            var monthlyTimePeriodInYears = DateUtility.CalculateTimePeriodInYearsForOneMonth(
                InterestAccrualDayCountConvention,
                interestAccrualDate);

            var accruedInterestFactor = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(
                monthlyTimePeriodInYears,
                _fixedCouponRate);

            accruedInterest = accruedInterestFactor * (priorPeriodCashFlow.EndingBalance - principalPayment);

            var interestPayment = DetermineInterestAmountDue(adjustedMonthlyPeriod, isFinalPeriod);
            return interestPayment;
        }
    }
}
