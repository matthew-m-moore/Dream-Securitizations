using System;

namespace Dream.WebApp.ModelEntries
{
    public class PaceAssessmentRecordModelEntry
    {
        public int? LoanId { get; set; }
        public string BondId { get; set; }
        public string ReplineId { get; set; }
        public string PropertyState { get; set; }
        public double Balance { get; set; }
        public double? ProjectCost { get; set; }
        public double CouponRate { get; set; }
        public double? BuyDownRate { get; set; }
        public int TermInYears { get; set; }
        public DateTime? FundingDate { get; set; }
        public DateTime BondFirstPaymentDate { get; set; }
        public DateTime? BondFirstPrincipalPaymentDate { get; set; }
        public DateTime? BondMaturityDate { get; set; }
        public DateTime? CashFlowStartDate { get; set; }
        public DateTime? InterestAccrualStartDate { get; set; }
        public int? InterestAccrualEndMonth { get; set; }
        public int? InterestPaymentFrequencyInMonths { get; set; }
        public int? PrincipalPaymentFrequencyInMonths { get; set; }
        public int? NumberOfUnderlyingBonds { get; set; }
        public double? AccruedInterest { get; set; }
        public double? ActualPrepaymentsReceived { get; set; }
        public PrepaymentPenaltyPlanEntry PrepaymentPenaltyPlanEntry { get; set; }
        public PaceAssessmentRatePlanTermSetEntry PaceAssessmentRatePlanTermSetEntry { get; set; }
    }
}