using System.Collections.Generic;
using System.Linq;
using System;
using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.LoanStages;
using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.Utilities;


namespace Dream.Core.BusinessLogic.ProductTypes
{
    public abstract class Loan : Product, IGeneratesContractualCashFlows
    {
        public DateTime StartDate { get; set; }
        public DateTime FirstPaymentDate { get; set; }
        public PropertyState PropertyState { get; set; }

        public double Balance { get; set; }
        public double OriginalIssueDiscount { get; set; }
        public double InitialCouponRate { get; set; }

        public double ActualPrepayments { get; set; }
        public double AccruedInterest { get; set; }
        public DateTime InterestAccrualStartDate { get; set; }
        public DayCountConvention InterestAccrualDayCountConvention { get; set; }

        public readonly List<Month> ListOfInterestAccrualEndMonths = new List<Month>();
        public bool IsLoanStartSameDateAsInterestAccrualStart => StartDate.Date == InterestAccrualStartDate.Date;

        public int MonthsToNextInterestPayment { get; set; }
        public int MonthsToNextPrincipalPayment { get; set; }
        public int InterestPaymentFrequencyInMonths { get; set; }
        public int PrincipalPaymentFrequencyInMonths { get; set; }

        public int SeasoningInYears => SeasoningInMonths / Constants.MonthsInOneYear;
        public int SeasoningInMonths { get; set; }
        public int AmortizationTermInYears => AmortizationTermInMonths / Constants.MonthsInOneYear;
        public int AmortizationTermInMonths { get; set; }
        public int MaturityTermInYears => MaturityTermInMonths / Constants.MonthsInOneYear;
        public int MaturityTermInMonths { get; set; }

        public PrepaymentPenaltyPlan PrepaymentPenaltyPlan { get; set; }
        public List<CustomPayment> CustomPaymentSchedule { get; set; }

        public abstract Loan Copy();
        public abstract List<ContractualCashFlow> GetContractualCashFlows();
        public abstract ContractualCashFlow PrepareZerothPeriodCashFlow();
        protected abstract List<LoanStage> GenerateLoanStages();

        public Loan() { }

        public Loan(Loan loan) : base(loan)
        {
            StartDate = loan.StartDate;
            FirstPaymentDate = loan.FirstPaymentDate;
            PropertyState = loan.PropertyState;

            Balance = loan.Balance;
            OriginalIssueDiscount = loan.OriginalIssueDiscount;
            InitialCouponRate = loan.InitialCouponRate;

            ActualPrepayments = loan.ActualPrepayments;
            AccruedInterest = loan.AccruedInterest;
            InterestAccrualStartDate = loan.InterestAccrualStartDate;
            InterestAccrualDayCountConvention = loan.InterestAccrualDayCountConvention;
            ListOfInterestAccrualEndMonths = loan.ListOfInterestAccrualEndMonths;

            MonthsToNextInterestPayment = loan.MonthsToNextInterestPayment;
            MonthsToNextPrincipalPayment = loan.MonthsToNextPrincipalPayment;
            InterestPaymentFrequencyInMonths = loan.InterestPaymentFrequencyInMonths;
            PrincipalPaymentFrequencyInMonths = loan.PrincipalPaymentFrequencyInMonths;

            AmortizationTermInMonths = loan.AmortizationTermInMonths;
            MaturityTermInMonths = loan.MaturityTermInMonths;

            CustomPaymentSchedule = CustomPaymentSchedule != null 
                ? loan.CustomPaymentSchedule.Select(c => new CustomPayment(c)).ToList()
                : null;
        }

        /// <summary>
        /// Allows addition of a custom payment to the contracual cash flows of a loan on a specific payment date.
        /// </summary>
        public void AddCustomPayment(DateTime paymentDate, double principalAmount, double interestAmount, 
            bool isPrincipalAmountTotal = false,
            bool isInterestAmountTotal = false)
        {
            if (CustomPaymentSchedule == null) CustomPaymentSchedule = new List<CustomPayment>();

            CustomPaymentSchedule.Add(
                new CustomPayment(paymentDate, principalAmount, interestAmount, isPrincipalAmountTotal, isInterestAmountTotal));
        }

        /// <summary>
        /// Sets an ending month for interest accrual, even if this month does not coincide with an interest payment.
        /// </summary>
        public void SetInterestAccrualEndMonths(Month interestAccuralEndMonth)
        {
            if (interestAccuralEndMonth == Month.None) return;
            ListOfInterestAccrualEndMonths.Add(interestAccuralEndMonth);

            var initialEndMonth = (int) interestAccuralEndMonth;
            var nextEndMonth = initialEndMonth - InterestPaymentFrequencyInMonths;

            while (nextEndMonth != initialEndMonth)
            {
                if ((Month) nextEndMonth == Month.None) break;

                ListOfInterestAccrualEndMonths.Add((Month) nextEndMonth);
                nextEndMonth -= InterestPaymentFrequencyInMonths;

                if (nextEndMonth <= 0) nextEndMonth += Constants.MonthsInOneYear;
            }
        }

        /// <summary>
        /// Adjust a the loan to a new start date, potentially with a new blance and initial coupon.
        /// </summary>
        public void AdjustLoanToNewStartDate(DateTime oldStartDate, DateTime newStartDate, DateTime newFirstPaymentDate, 
            double? newBalance = null, 
            double? newCouponRate = null)
        {
            StartDate = newStartDate;
            FirstPaymentDate = newFirstPaymentDate;
            Balance = newBalance ?? Balance;
            InitialCouponRate = newCouponRate ?? InitialCouponRate;

            AdjustLoanToCashFlowStartDate(oldStartDate);
            InterestAccrualStartDate = newFirstPaymentDate;
        }

        /// <summary>
        /// Adjust a loan to begin a specific starting date, assuming its terms are as-of the specified collteral cut-off date.
        /// </summary>
        public void AdjustLoanToCashFlowStartDate(DateTime collateralCutOffDate)
        {
            var monthsExpiredSinceCutOffDate = DateUtility.MonthsBetweenTwoDates(
                collateralCutOffDate,
                StartDate);

            if (monthsExpiredSinceCutOffDate < 0)
            {
                throw new Exception(string.Format("ERROR: The start date cannot precede the collateral cut-off date start date. Loan {0}:{1} starts {2}, cut-off date is {3}.",
                    IntegerId,
                    StringId,
                    StartDate,
                    collateralCutOffDate));
            }

            if (monthsExpiredSinceCutOffDate > 0)
            {
                var monthsToInterestAccrualStartDate = DateUtility.MonthsBetweenTwoDates(
                    StartDate,
                    InterestAccrualStartDate);

                // Move the interest accrual start date only if it is not in the past relative to the new start date.
                // This is to protect against the idea that some users may think it is necessary to adjust this date as well.
                if (monthsToInterestAccrualStartDate < 0)
                {
                    InterestAccrualStartDate =
                        InterestAccrualStartDate.AddMonths(monthsExpiredSinceCutOffDate);
                }

                MonthsToNextInterestPayment = AdjustLoanMonthsToNextInterestPayment(
                    monthsExpiredSinceCutOffDate);

                MonthsToNextPrincipalPayment = AdjustMonthsToNextPayment(
                    MonthsToNextPrincipalPayment,
                    PrincipalPaymentFrequencyInMonths,
                    monthsExpiredSinceCutOffDate);

                // The amortization term will be capped at the length of the maturity term
                MaturityTermInMonths -= monthsExpiredSinceCutOffDate;
                AmortizationTermInMonths = Math.Min(
                    AmortizationTermInMonths,
                    MaturityTermInMonths);
            }
        }

        protected int AdjustMonthsToNextPayment(
            int monthsToNextPayment,
            int paymentFrequencyInMonths,
            int monthsExpiredSinceStartDate)
        {
            if (monthsExpiredSinceStartDate <= monthsToNextPayment)
            {
                var adjustedMonthsToNextPayment = monthsToNextPayment - monthsExpiredSinceStartDate;
                return adjustedMonthsToNextPayment;
            }

            if (monthsExpiredSinceStartDate > monthsToNextPayment)
            {
                var monthsElapsedPastPayment = monthsExpiredSinceStartDate - monthsToNextPayment;
                var newMonthsToNextPayment = paymentFrequencyInMonths - (monthsElapsedPastPayment % paymentFrequencyInMonths);
                return newMonthsToNextPayment;
            }

            return monthsToNextPayment;
        }

        protected int AdjustLoanMonthsToNextInterestPayment(int monthsExpiredSinceStartDate)
        {
            var monthsToNextInterestPayment = AdjustMonthsToNextPayment(
                    MonthsToNextInterestPayment,
                    InterestPaymentFrequencyInMonths,
                    monthsExpiredSinceStartDate);

            // This would indicate that the collateral has rolled past the point where all
            // pre-analysis interest was paid out
            if (monthsExpiredSinceStartDate > MonthsToNextInterestPayment)
            {
                AccruedInterest = 0.0;
            }

            return monthsToNextInterestPayment;
        }
    }
}
