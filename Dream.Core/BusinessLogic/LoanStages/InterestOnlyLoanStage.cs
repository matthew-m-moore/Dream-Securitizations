using System;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Common.Utilities;
using Dream.Core.Interfaces;
using System.Linq;

namespace Dream.Core.BusinessLogic.LoanStages
{
    public class InterestOnlyLoanStage : LoanStage, IRevolvingCredit
    {
        public int RevolvingTermInMonths { get; set; }
        public Coupon LoanCoupon { get; set; }

        public override List<ContractualCashFlow> CalculateScheduledPayments()
        {
            var listOfContractualCashFlows = new List<ContractualCashFlow>();

            for (var monthlyPeriod = 0; monthlyPeriod < StageDurationInMonths; monthlyPeriod++)
            {
                var isFinalPeriod = monthlyPeriod == StageDurationInMonths - 1;
                var adjustedMonthlyPeriod = monthlyPeriod + StageStartMonth;
                var monthlyPeriodDate = DateUtility.AddMonthsForCashFlowPeriods(InterestAccrualStartDate, monthlyPeriod);

                var accruedInterest = 0.0;
                var interestPayment = CalculateInterestPayment(
                    adjustedMonthlyPeriod, 
                    monthlyPeriodDate, 
                    isFinalPeriod,
                    out accruedInterest);

                ApplyCustomInterestPayments(ref interestPayment, monthlyPeriodDate);

                var bondCount = PriorStagesContractualCashFlows.Last().BondCount;

                // Note that an interest-only stage will not have any principal payments
                var contractualCashFlow = new ContractualCashFlow
                {
                    Period = adjustedMonthlyPeriod,
                    PeriodDate = monthlyPeriodDate,
                    BondCount = bondCount,

                    StartingBalance = StageStartingBalance,
                    EndingBalance = StageStartingBalance,
                    Principal = 0.0,

                    AccruedInterest = accruedInterest,
                    Interest = interestPayment,
                };

                listOfContractualCashFlows.Add(contractualCashFlow);
                PriorStagesContractualCashFlows.Add(contractualCashFlow);
            }

            return listOfContractualCashFlows;
        }

        private double CalculateInterestPayment(
            int adjustedMonthlyPeriod, 
            DateTime interestAccrualDate,
            bool isFinalPeriod,
            out double accruedInterest)
        {
            var couponRate = LoanCoupon.GetCouponForSpecificMonthlyPeriod(adjustedMonthlyPeriod, InterestAccrualStartDate);

            var monthlyTimePeriodInYears = DateUtility.CalculateTimePeriodInYearsForOneMonth(
                InterestAccrualDayCountConvention,
                interestAccrualDate);

            var accruedInterestFactor = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(
                monthlyTimePeriodInYears,
                couponRate);

            // The starting balance for the stage should never change for an interest-only stage
            accruedInterest = accruedInterestFactor * StageStartingBalance;

            var interestPayment = DetermineInterestAmountDue(adjustedMonthlyPeriod, isFinalPeriod);
            return interestPayment;
        }
    }
}
