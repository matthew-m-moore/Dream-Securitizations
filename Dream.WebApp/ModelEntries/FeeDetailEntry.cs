using System;

namespace Dream.WebApp.ModelEntries
{
    public class FeeDetailEntry
    {
        public string FeeDetailName { get; set; }
        public string FeeTrancheName { get; set; }
        public double AnnualFeeAmount { get; set; }
        public DateTime? FeeEffectiveDate { get; set; }
        public bool? IsPeriodicallyIncreasingFee { get; set; }
    }
}