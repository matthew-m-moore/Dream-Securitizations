using System;

namespace Dream.IO.Database.Entities.InterestRates
{
    public class MarketDataSetEntity
    {
        public int MarketDataSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string MarketDataSetDescription { get; set; }
    }
}
