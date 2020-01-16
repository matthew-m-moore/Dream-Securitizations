namespace Dream.IO.Database.Entities.Collateral
{
    public class PerformanceAssumptionAssignmentEntity
    {
        public int PerformanceAssumptionAssignmentId { get; set; }
        public int PerformanceAssumptionDataSetId { get; set; }
        public string InstrumentIdentifier { get; set; }
        public string PerformanceAssumptionGrouping { get; set; }
        public string PerformanceAssumptionIdentifier { get; set; }
        public int PerformanceAssumptionTypeId { get; set; }
        public int VectorParentId { get; set; }
    }
}
