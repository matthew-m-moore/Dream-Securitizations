namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class ProductLineAggregationGroupMappingEntity
    {
        public int ProductLineAggregationGroupMappingId { get; set; }
        public int ProductLineAggregationGroupId { get; set; }
        public int ProductLineKey { get; set; }
        public string ProductLineAggregationGroupIdentifier { get; set; }
    }
}
