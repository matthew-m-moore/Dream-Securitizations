using System;

namespace Dream.WebApp.ModelEntries
{
    public class MarketRateDataEntry
    {
        public string RateIndexName { get; set; }
        public string RateIndexGrouping { get; set; }
        public DateTime MarketRateDataDate { get; set; }
        public double MarketRateDataValue { get; set; }
    }
}