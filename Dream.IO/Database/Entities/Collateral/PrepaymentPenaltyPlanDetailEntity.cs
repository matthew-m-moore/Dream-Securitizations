namespace Dream.IO.Database.Entities.Collateral
{
    public class PrepaymentPenaltyPlanDetailEntity
    {
        public int PrepaymentPenaltyPlanDetailId { get; set; }
        public int PrepaymentPenaltyPlanId { get; set; }
        public int EndingMonthlyPeriodOfPenalty { get; set; }
        public double PenaltyAmount { get; set; }
        public string PenaltyType { get; set; }
    }
}
