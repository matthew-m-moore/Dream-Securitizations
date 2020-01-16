namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationAnalysisCommentEntity
    {
        public int SecuritizationAnalysisCommentId { get; set; }
        public int SecuritizationAnalysisDataSetId { get; set; }
        public int? SecuritizationAnalysisVersionId { get; set; }
        public bool IsVisible { get; set; }
        public string CommentText { get; set; }
    }
}
