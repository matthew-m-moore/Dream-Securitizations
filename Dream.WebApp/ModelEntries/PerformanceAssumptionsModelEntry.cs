namespace Dream.WebApp.ModelEntries
{
    public class PerformanceAssumptionsModelEntry
    {
        public string PerformanceAssumptionDescription { get; set; }
        public PerformanceAssumptionsEntry PrepaymentAssumptionEntry { get; set; }
        public PerformanceAssumptionsEntry DefaultAssumptionEntry { get; set; }
        public PerformanceAssumptionsEntry DelinquencyAssumptionEntry { get; set; }
        public PerformanceAssumptionsEntry SeverityAssumptionEntry { get; set; }
        public bool UseDbrsModel { get; set; }
        public double? DbrsModelDefaultRate { get; set; }
        public double? DbrsModelLossGivenDefault { get; set; }
        public int? DbrsModelForeclosureTermInMonths { get; set; }
        public int? DbrsModelReperformanceTermInMonths { get; set; }
        public int? DbrsModelNumberOfDefaultSequences { get; set; }
    }
}