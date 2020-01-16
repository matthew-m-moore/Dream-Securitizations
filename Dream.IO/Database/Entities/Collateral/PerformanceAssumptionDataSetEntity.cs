using System;

namespace Dream.IO.Database.Entities.Collateral
{
    public class PerformanceAssumptionDataSetEntity
    {
        public int PerformanceAssumptionDataSetId { get; set; }
        public DateTime? CutOffDate { get; set; }
        public string PerformanceAssumptionDataSetDescription { get; set; }
    }
}
