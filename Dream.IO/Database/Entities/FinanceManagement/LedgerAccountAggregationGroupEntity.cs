namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class LedgerAccountAggregationGroupEntity
    {
        public int LedgerAccountAggregationGroupId { get; set; }
        public string LedgerAccountAggregationGroupDescription { get; set; }
        public bool IsAggregationGroupActive { get; set; }
    }
}
