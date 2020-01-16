namespace Dream.IO.Database.Entities.InterestRates
{
    public class MarketDataTypeEntity
    {
        public int MarketDataTypeId { get; set; }
        public string MarketDataTypeDescription { get; set; }
        public double? StandardDivisor { get; set; }
    }
}
