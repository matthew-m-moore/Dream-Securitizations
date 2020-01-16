namespace Dream.IO.Database.Entities.InterestRates
{
    public class RateIndexEntity
    {
        public int RateIndexId { get; set; }
        public string RateIndexDescription { get; set; }
        public int? TenorInMonths { get; set; }
        public int? RateIndexGroupId { get; set; }
    }
}
