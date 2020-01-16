namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class PricingScenarioRecord
    {
        public int ScenarioNumber { get; set; }
        public string GroupingIdentifier { get; set; }
        public bool PriceToCall { get; set; }
        public double CleanUpCallPercentage { get; set; }
        public string PricingMethodology { get; set; }
        public double ScenarioValue { get; set; }
        public string ScenarioStrategy { get; set; }
    }
}
