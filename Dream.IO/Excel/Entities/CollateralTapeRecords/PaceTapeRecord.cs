using System;

namespace Dream.IO.Excel.Entities.CollateralTapeRecords
{
    public class PaceTapeRecord
    {
        public double Balance { get; set; }
        public double CouponRate { get; set; }
        public double BuyDownRate { get; set; }
        public int TermInYears { get; set; }  
        public string RatePlan { get; set; }     
        public string PropertyState { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? InterestStartDate { get; set; }
        public DateTime? LastPreFundingDate { get; set; }
        public DateTime? PreFundingStartDate { get; set; }
        public int? InterestPaymentFrequency { get; set; }
        public int? PrincipalPaymentFrequency { get; set; }
        public double ActualPrepayments { get; set; }
        public int InterestAccrualEndMonth { get; set; }
        public bool? RunFlag { get; set; }
        public bool? IsPrefundingRepline { get; set; }
    }
}
