using System;
using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class RateCurveDataModelEntry
    {
        public string RateIndexName { get; set; }
        public string RateIndexGrouping { get; set; }
        public string RateCurveTypeDescription { get; set; }
        public List<RateCurveDataEntry> RateCurveDataEntry { get; set; }
        public string DayCountConvention { get; set; }
        public DateTime MarketDate { get; set; }
    }
}