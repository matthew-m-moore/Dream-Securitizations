namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class CostCenterAggregationGroupMappingEntity
    {
        public int CostCenterAggregationGroupMappingId { get; set; }
        public int CostCenterAggregationGroupId { get; set; }
        public int CostCenterKey { get; set; }
        public string CostCenterAggregationGroupIdentifier { get; set; }
    }
}
