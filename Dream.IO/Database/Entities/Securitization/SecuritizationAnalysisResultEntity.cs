namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationAnalysisResultEntity
    {
        public int SecuritizationAnalysisResultId { get; set; }
        public int SecuritizationAnalysisDataSetId { get; set; }
        public int SecuritizationAnalysisVersionId { get; set; }
        public int SecuritizationAnalysisScenarioId { get; set; }
        public int? SecuritizationTrancheDetailId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public int SecuritizationResultTypeId { get; set; }
        public double SecuritizationResultValue { get; set; }
    }
}
