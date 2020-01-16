namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationNodeEntity
    {
        public int SecuritizationNodeId { get; set; }
        public int SecuritizationNodeDataSetId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public string SecuritizationChildNodeName { get; set; }
        public int FundsDistributionTypeId { get; set; }
        public int? TrancheDetailId { get; set; }
        public int? TranchePricingTypeId { get; set; }
        public int? TranchePricingRateIndexId { get; set; }
        public double? TranchePricingValue { get; set; }
        public int? TranchePricingDayCountConventionId { get; set; }
        public int? TranchePricingCompoundingConventionId { get; set; }
    }
}
