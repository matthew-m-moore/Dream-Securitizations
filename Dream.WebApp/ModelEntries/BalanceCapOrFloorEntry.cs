using System;

namespace Dream.WebApp.ModelEntries
{
    public class BalanceCapOrFloorEntry
    {
        public string BalanceCapOrFloor { get; set; }
        public string PercentageOrDollarAmount { get; set; }
        public double CapOrFloorValue { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}