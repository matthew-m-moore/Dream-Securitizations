namespace Dream.WebApp.ModelEntries
{
    public class SecuritizationSummaryModelEntry
    {
        public int SecuritizationNodeId { get; set; }
        public int? SecuritizationNodeTrancheId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public string SecuritizationNodeOrTrancheName { get; set; }
        public string SecuritizationNodeOrTrancheType { get; set; }
        public string SecuritizationNodeOrTrancheRating { get; set; }
        public double? SecuritizationNodeOrTrancheBalance { get; set; }
        public double? SecuritizationNodeOrTrancheSizePercentage { get; set; }
        public double? SecuritizationNodeOrTrancheInitialCoupon { get; set; }
        public double WeightedAverageLife { get; set; }
        public double ModifiedDuration { get; set; }
        public string PaymentWindow { get; set; }
        public double? BenchmarkYield { get; set; }
        public double? NominalSpread { get; set; }
        public double InternalRateOfReturn { get; set; }
        public double PresentValue { get; set; }
        public double? DollarPrice { get; set; }
        public double PercentageOfCollateral { get; set; }
        public bool IsSecuritizationNode { get; set; }
    }
}