namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class LedgerAccountAggregationGroupMappingEntity
    {
        public int LedgerAccountAggregationGroupMappingId { get; set; }
        public int LedgerAccountAggregationGroupId { get; set; }
        public int LedgerAccountKey { get; set; }
        public string LedgerAccountAggregationGroupIdentifier { get; set; }
    }
}
