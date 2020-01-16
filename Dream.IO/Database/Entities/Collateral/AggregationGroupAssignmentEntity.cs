namespace Dream.IO.Database.Entities.Collateral
{
    public class AggregationGroupAssignmentEntity
    {
        public int AggregationGroupAssignmentId { get; set; }
        public int AggregationGroupDataSetId { get; set; }
        public string InstrumentIdentifier { get; set; }
        public string AggregationGroupingIdentifier { get; set; }
        public string AggregationGroupName { get; set; }
    }
}
