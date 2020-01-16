namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationAnalysisOwnerEntity
    {
        public int SecuritizationAnalysisOwnerId { get; set; }
        public int SecuritizationAnalysisDataSetId { get; set; }
        public int? SecuritizationAnalysisVersionId { get; set; }
        public int ApplicationUserId { get; set; }
        public bool IsReadOnlyToOthers { get; set; }
    }
}
