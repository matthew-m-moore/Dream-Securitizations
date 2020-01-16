using System;

namespace Dream.IO.Database.Entities.Collateral
{
    public class AggregationGroupDataSetEntity
    {
        public int AggregationGroupDataSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string AggregationGroupDataSetDescription { get; set; }
    }
}
