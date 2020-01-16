namespace Dream.IO.Database.Entities.Collateral
{
    public class CollateralizedSecuritizationTrancheEntity
    {
        public int CollateralizedSecuritizationTrancheId { get; set; }
        public int CollateralizedSecuritizationDataSetId { get; set; }
        public int SecuritizationAnalysisDataSetId { get; set; }
        public int SecuritizationAnalysisVersionId { get; set; }
        public int SecuritizatizedTrancheDetailId { get; set; }
        public double SecuritizatizedTranchePercentage { get; set; }
    }
}
