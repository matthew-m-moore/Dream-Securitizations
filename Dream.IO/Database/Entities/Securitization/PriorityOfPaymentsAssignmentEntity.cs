namespace Dream.IO.Database.Entities.Securitization
{
    public class PriorityOfPaymentsAssignmentEntity
    {
        public int PriorityOfPaymentsAssignmentId { get; set; }
        public int PriorityOfPaymentsSetId { get; set; }
        public int SeniorityRanking { get; set; }
        public int TrancheDetailId { get; set; }
        public int TrancheCashFlowTypeId { get; set; }
    }
}
