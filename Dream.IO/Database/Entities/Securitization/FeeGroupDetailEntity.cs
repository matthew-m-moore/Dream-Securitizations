namespace Dream.IO.Database.Entities.Securitization
{
    public class FeeGroupDetailEntity
    {
        public int FeeGroupDetailId { get; set; }
        public string FeeGroupName { get; set; }
        public double? FeeRate { get; set; }
        public double? FeePerUnit { get; set; }
        public double? FeeMinimum { get; set; }
        public double? FeeMaximum { get; set; }
        public double? FeeIncreaseRate { get; set; }
        public int? FeeRateUpdateFrequencyInMonths { get; set; }
        public int? FeeRollingAverageInMonths { get; set; }
        public bool? UseStartingBalanceToDetermineFee { get; set; }
    }
}
