namespace Dream.IO.Excel.Entities
{
    public class CashFlowPricingInputsRecord : CashFlowGenerationInputsRecord
    {
        public double? TotalDollarPriceTarget { get; set; }
        public string PricingMethodology { get; set; }
        public double PricingValue {get;set;}
        public string DayCounting { get; set; }
        public string Compounding { get; set; }
    }
}
