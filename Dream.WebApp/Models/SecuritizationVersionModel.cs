namespace Dream.WebApp.Models
{
    public class SecuritizationVersionModel
    {
        public int SecuritizationDataSetId { get; set; }
        public int SecuritizationVersionId { get; set; }
        public string SecuritizationVersionComment { get; set; }
        public string SecuritizationVersionOwner { get; set; }
        public string TruncatedSecuritizationVersionComment { get; set; }
        public bool IsReadOnly { get; set; }
    }
}