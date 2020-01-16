using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.LoanStages
{
    public abstract class LoanStage
    {
        public int StageOrderId { get; set; }
        public int StageDurationInMonths { get; set; }
        public int StageStartMonth { get; set; }
        public double StageStartingBalance { get; set; }
        public bool IsFinalLoanStage { get; set; }

        public double AccruedInterest { get; set; }
        public DateTime InterestAccrualStartDate { get; set; }
        public DayCountConvention InterestAccrualDayCountConvention { get; set; }
        public List<Month> ListOfInterestAccrualEndMonths { get; set; }

        public int MonthsToNextInterestPayment { get; set; }
        public int MonthsToNextPrincipalPayment { get; set; }
        public int InterestPaymentFrequencyInMonths { get; set; }
        public int PrincipalPaymentFrequencyInMonths { get; set; }

        public List<ContractualCashFlow> PriorStagesContractualCashFlows = new List<ContractualCashFlow>();
        public List<CustomPayment> CustomPaymentSchedule { get; set; }

        public abstract List<ContractualCashFlow> CalculateScheduledPayments();

        /// <summary>
        /// Determines the interest amount due for a given monthly period of a loan stage.
        /// The monthly period is typically adjusted to correspond to the total periods elapsed.
        /// </summary>
        protected double DetermineInterestAmountDue(int monthlyPeriod, bool isFinalPeriod)
        {
            if (monthlyPeriod < MonthsToNextInterestPayment)
            {
                return 0.0;
            }

            var isFirstInterestPayment = monthlyPeriod == MonthsToNextInterestPayment;
            var isInterestPaymentMonth = MathUtility.CheckDivisibilityOfIntegers(
                monthlyPeriod - MonthsToNextInterestPayment,
                InterestPaymentFrequencyInMonths);

            if (isFirstInterestPayment || isInterestPaymentMonth || (IsFinalLoanStage && isFinalPeriod))
            {
                if (ListOfInterestAccrualEndMonths.Any())
                {
                    monthlyPeriod = AdjustMonthlyPeriodToSpecifiedEndMonth(monthlyPeriod);
                }

                var minimumMonthlyPeriodForLookback = Math.Max(monthlyPeriod - InterestPaymentFrequencyInMonths, 0);

                // In the final period of the final stage, balloon out all remaining accrued interest
                if (IsFinalLoanStage && isFinalPeriod && !ListOfInterestAccrualEndMonths.Any())
                {
                    var lastCashFlowWithAnInterestPayment = PriorStagesContractualCashFlows.LastOrDefault(c => c.Interest > 0);
                    if (lastCashFlowWithAnInterestPayment != null)
                    {
                        minimumMonthlyPeriodForLookback = lastCashFlowWithAnInterestPayment.Period;
                    }
                }

                var previouslyAccruedInterest = CalculatePreviouslyAccruedInterest(minimumMonthlyPeriodForLookback, monthlyPeriod);

                var interestPayment = previouslyAccruedInterest;
                return interestPayment;
            }

            return 0.0;
        }

        protected double CalculatePreviouslyAccruedInterest(int priorMonthlyPeriod, int currentMonthlyPeriod)
        {
            var previouslyAccruedInterest = PriorStagesContractualCashFlows
                .Where(c => c.Period >= priorMonthlyPeriod
                         && c.Period < currentMonthlyPeriod)
                .Sum(s => s.AccruedInterest);

            return previouslyAccruedInterest;
        }


        protected void ApplyCustomPrincipalPayments(ref double scheduledPrincipalPayment, DateTime currentMonthlyPeriodDate)
        {
            if (CustomPaymentSchedule == null || !CustomPaymentSchedule.Any()) return;

            var previousMonthlyPeriodDate = PriorStagesContractualCashFlows.Last().PeriodDate;

            if (CustomPaymentSchedule.Any(c => c.PaymentDate.Ticks > previousMonthlyPeriodDate.Ticks
                                            && c.PaymentDate.Ticks <= currentMonthlyPeriodDate.Ticks
                                            && c.PrincipalAmount > 0.0))
            {
                var relevantCustomPayments = CustomPaymentSchedule.Where(c => c.PaymentDate.Ticks > previousMonthlyPeriodDate.Ticks
                                                                           && c.PaymentDate.Ticks <= currentMonthlyPeriodDate.Ticks);
                foreach (var customPayment in relevantCustomPayments)
                {
                    if (customPayment.IsPrincipalAmountTotal)
                    {
                        scheduledPrincipalPayment = customPayment.PrincipalAmount;
                    }
                    else
                    {
                        scheduledPrincipalPayment += customPayment.PrincipalAmount;
                    }

                    if (scheduledPrincipalPayment > PriorStagesContractualCashFlows.Last().EndingBalance)
                    {
                        scheduledPrincipalPayment = PriorStagesContractualCashFlows.Last().EndingBalance;
                    }
                }
            }
        }

        protected void ApplyCustomInterestPayments(ref double scheduledinterestPayment, DateTime currentMonthlyPeriodDate)
        {
            if (CustomPaymentSchedule == null || !CustomPaymentSchedule.Any()) return;

            var previousMonthlyPeriodDate = PriorStagesContractualCashFlows.Last().PeriodDate;

            if (CustomPaymentSchedule.Any(c => c.PaymentDate.Ticks > previousMonthlyPeriodDate.Ticks
                                            && c.PaymentDate.Ticks <= currentMonthlyPeriodDate.Ticks))
            {
                var relevantCustomPayments = CustomPaymentSchedule.Where(c => c.PaymentDate.Ticks > previousMonthlyPeriodDate.Ticks
                                                                           && c.PaymentDate.Ticks <= currentMonthlyPeriodDate.Ticks);
                foreach (var customPayment in relevantCustomPayments)
                {
                    if (customPayment.IsInterestAmountTotal)
                    {
                        scheduledinterestPayment = customPayment.InterestAmount;
                    }
                    else
                    {
                        scheduledinterestPayment += customPayment.InterestAmount;
                    }
                }
            }
        }

        private int AdjustMonthlyPeriodToSpecifiedEndMonth(int monthlyPeriod)
        {
            var lastCashFlowWithAnInterestPayment = PriorStagesContractualCashFlows.LastOrDefault(c => c.Interest > 0);
            if (lastCashFlowWithAnInterestPayment == null) lastCashFlowWithAnInterestPayment = PriorStagesContractualCashFlows.First();

            var periodDateOfLastInterestPayment = lastCashFlowWithAnInterestPayment.PeriodDate;
            var periodOfLastInterestPayment = lastCashFlowWithAnInterestPayment.Period;

            var monthsSinceLastInterestPayment = monthlyPeriod - periodOfLastInterestPayment;
            var endMonthOfInterestAccrual = DetermineInterestAccrualEndMonth(periodDateOfLastInterestPayment);

            var interestAccrualEndPeriodCashFlow = PriorStagesContractualCashFlows
                .Where(c => c.Period >= periodOfLastInterestPayment)
                .FirstOrDefault(s => s.PeriodDate.Month == (int) endMonthOfInterestAccrual);

            if (interestAccrualEndPeriodCashFlow == null) return 0;

            var interestAccrualEndPeriod = interestAccrualEndPeriodCashFlow.Period; 
            interestAccrualEndPeriod++;

            return interestAccrualEndPeriod;
        }

        private Month DetermineInterestAccrualEndMonth(DateTime periodDateOfLastInterestPayment)
        {
            var monthOfLastInterestPayment = periodDateOfLastInterestPayment.Month;
            var listOfMonthsToIncrement = ListOfInterestAccrualEndMonths
                .Select(m => (int) m - monthOfLastInterestPayment).ToList();

            var integerMonthsToIncrement = 0;
            if (listOfMonthsToIncrement.Any(i => i > 0))
            {
                integerMonthsToIncrement = listOfMonthsToIncrement.Where(i => i > 0).Min();
            }
            else
            {
                integerMonthsToIncrement = listOfMonthsToIncrement.Min();
            }

            var integerEndMonth = monthOfLastInterestPayment + integerMonthsToIncrement;

            var enumEndMonth = (Month) integerEndMonth;
            return enumEndMonth;
        }
    }
}
