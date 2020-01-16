using System;

namespace Dream.IO.Database.Entities.InterestRates
{
    public class MarketDataEntity
    {
        public int MarketDataId { get; set; }
        public int MarketDataSetId { get; set; }
        public DateTime MarketDateTime { get; set; }
        public double DataValue { get; set; }
        public int RateIndexId { get; set; }
        public int MarketDataTypeId { get; set; }
    }
}
