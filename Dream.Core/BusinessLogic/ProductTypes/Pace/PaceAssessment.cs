using System;
using System.Collections.Generic;
using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.LoanStages;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Common.Utilities;
using Dream.Common;

namespace Dream.Core.BusinessLogic.ProductTypes
{
    public class PaceAssessment : FixedRateLoan, IGeneratesContractualCashFlows
    {
        public string MunicipalBondId { get; set; }
        public double BondCount { get; set; }    
        public PaceRatePlan RatePlan { get; set; }
        public DateTime FundingDate { get; set; }

        public DateTime? LastPreFundingDate { get; set; }
        public DateTime? PreFundingStartDate { get; set; }
        public double? PreFundingPrepaymentProRating { get; set; }

        public PaceAssessment() { }

        public PaceAssessment(PaceAssessment paceAssessment) : base(paceAssessment)
        {
            MunicipalBondId = paceAssessment.MunicipalBondId;
            BondCount = paceAssessment.BondCount;

            RatePlan = (paceAssessment.RatePlan != null) ? paceAssessment.RatePlan.Copy() : null;
            FundingDate = new DateTime(FundingDate.Ticks);

            PreFundingStartDate = (paceAssessment.PreFundingStartDate != null) ? new DateTime?(paceAssessment.PreFundingStartDate.Value) : null;
            LastPreFundingDate = (paceAssessment.LastPreFundingDate != null) ? new DateTime?(paceAssessment.LastPreFundingDate.Value) : null;
            PreFundingPrepaymentProRating = paceAssessment.PreFundingPrepaymentProRating;
        }

        public override List<ContractualCashFlow> GetContractualCashFlows()
        {
            if (PreFundingStartDate.HasValue)
            {
                var copiedLoan = Copy();
                var newStartDate = PreFundingStartDate.Value;

                StartDate = newStartDate.AddMonths(-1);
                FirstPaymentDate = newStartDate;

                AdjustLoanToCashFlowStartDate(copiedLoan.StartDate);
                InterestAccrualStartDate = newStartDate;

                AddAccruedInterestFromLastPreFundingDate();
            }

            return base.GetContractualCashFlows();
        }

        protected override List<LoanStage> GenerateLoanStages()
        {
            var loanStages = new List<LoanStage>();

            // Interest accrual for the stage can be bumped one month in this case
            var interestAccrualStartDate = InterestAccrualStartDate;
            if (IsLoanStartSameDateAsInterestAccrualStart)
            {
                interestAccrualStartDate = InterestAccrualStartDate.AddMonths(1);
            }

            var interestOnlyStageDuration = MaturityTermInMonths - AmortizationTermInMonths;
            var interestOnlyStage = new InterestOnlyLoanStage
            {
                StageOrderId = 1,
                StageStartMonth = 1,
                StageDurationInMonths = interestOnlyStageDuration,

                LoanCoupon = new FixedRateCoupon(InitialCouponRate),
                StageStartingBalance = Balance,
                AccruedInterest = AccruedInterest,

                InterestAccrualStartDate = interestAccrualStartDate,
                InterestAccrualDayCountConvention = InterestAccrualDayCountConvention,

                MonthsToNextInterestPayment = MonthsToNextInterestPayment,
                InterestPaymentFrequencyInMonths = InterestPaymentFrequencyInMonths,

                CustomPaymentSchedule = CustomPaymentSchedule,
                ListOfInterestAccrualEndMonths = ListOfInterestAccrualEndMonths,
            };

            var fixedRateFullyAmortizingStageDuration = MaturityTermInMonths - interestOnlyStageDuration;
            var fixedRateFullyAmortizingStage = new FixedRateAmortizingLoanStage
            {
                StageOrderId = 2,
                StageStartMonth = interestOnlyStageDuration + 1,
                StageDurationInMonths = AmortizationTermInMonths,
                IsFinalLoanStage = true,

                LoanCoupon = new FixedRateCoupon(InitialCouponRate),
                AmortizationTermInMonths = AmortizationTermInMonths,

                InterestAccrualStartDate = interestAccrualStartDate.AddMonths(interestOnlyStageDuration),
                InterestAccrualDayCountConvention = InterestAccrualDayCountConvention,

                MonthsToNextInterestPayment = MonthsToNextInterestPayment,
                MonthsToNextPrincipalPayment = MonthsToNextPrincipalPayment,
                InterestPaymentFrequencyInMonths = InterestPaymentFrequencyInMonths,
                PrincipalPaymentFrequencyInMonths = PrincipalPaymentFrequencyInMonths,

                CustomPaymentSchedule = CustomPaymentSchedule,
                ListOfInterestAccrualEndMonths = ListOfInterestAccrualEndMonths,
            };

            loanStages.Add(interestOnlyStage);
            loanStages.Add(fixedRateFullyAmortizingStage);

            return loanStages;
        }

        public override ContractualCashFlow PrepareZerothPeriodCashFlow()
        {
            var zerothPeriodCashFlow = base.PrepareZerothPeriodCashFlow();
            zerothPeriodCashFlow.BondCount = BondCount;           
            return zerothPeriodCashFlow; 
        }

        public override Loan Copy()
        {
            return new PaceAssessment(this);
        }

        private void AddAccruedInterestFromLastPreFundingDate()
        {
            if (LastPreFundingDate.HasValue)
            {
                var lastPrefundingDate = LastPreFundingDate.Value;
                var interestAccrualStartDate = PreFundingStartDate.Value;
                var yearsOfAccruedInterest = DateUtility.CalculateTimePeriodInYears(InterestAccrualDayCountConvention, lastPrefundingDate, interestAccrualStartDate);
                PreFundingPrepaymentProRating = yearsOfAccruedInterest * Constants.MonthsInOneYear;

                var assessmentBalance = Balance + ActualPrepayments;
                AccruedInterest = yearsOfAccruedInterest * assessmentBalance * InitialCouponRate;
            }
        }
    }
}
