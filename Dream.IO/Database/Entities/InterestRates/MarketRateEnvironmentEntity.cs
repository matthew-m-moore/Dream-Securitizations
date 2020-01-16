using System;

namespace Dream.IO.Database.Entities.InterestRates
{
    public class MarketRateEnvironmentEntity
    {
        public int MarketRateEnvironmentId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string MarketRateEnvironmentDescription { get; set; }
        public int? MarketDataSetId { get; set; }
        public int? RateCurveDataSetId { get; set; }
    }
}
