using System;

namespace Dream.IO.Database.Entities.InterestRates
{
    public class RateCurveDataSetEntity
    {
        public int RateCurveDataSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string RateCurveDataSetDescription { get; set; }
    }
}
