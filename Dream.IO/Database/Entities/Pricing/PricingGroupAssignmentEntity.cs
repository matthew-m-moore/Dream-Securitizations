namespace Dream.IO.Database.Entities.Pricing
{
    public class PricingGroupAssignmentEntity
    {
        public int PricingGroupAssignmentId { get; set; }
        public int PricingGroupDataSetId { get; set; }
        public string InstrumentIdentifier { get; set; }
        public double PricingValue { get; set; }
        public int PricingTypeId { get; set; }
        public int? PricingRateIndexId { get; set; }
        public int PricingDayCountConventionId { get; set; }
        public int PricingCompoundingConventionId { get; set; }
    }
}
