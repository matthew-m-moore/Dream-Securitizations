using System;

namespace Dream.IO.Database.Entities.InterestRates
{
    public class RateCurveDataEntity
    {
        public int RateCurveDataId { get; set; }
        public int RateCurveDataSetId { get; set; }
        public DateTime MarketDateTime { get; set; }
        public DateTime ForwardDateTime { get; set; }
        public double RateCurveValue { get; set; }
        public int RateIndexId { get; set; }
        public int MarketDataTypeId { get; set; }
    }
}
