namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationAnalysisSummaryEntity
    {
        public int SecuritizationAnalysisSummaryId { get; set; }
        public int SecuritizationAnalysisDataSetId { get; set; }
	    public int SecuritizationAnalysisVersionId { get; set; }
        public int SecuritizationAnalysisScenarioId { get; set; }
        public int? SecuritizationTrancheDetailId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public string SecuritizationTrancheType { get; set; }
        public string SecuritizationTrancheRating { get; set; }
    }
}
