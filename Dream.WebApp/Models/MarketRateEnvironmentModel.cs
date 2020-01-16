namespace Dream.WebApp.Models
{
    public class MarketRateEnvironmentModel
    {
        public bool IsModified { get; set; }
        public MarketRateDataModel MarketRateDataModel { get; set; }
        public RateCurveDataModel RateCurveDataModel { get; set; }
    }
}