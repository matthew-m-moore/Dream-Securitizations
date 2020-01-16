using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationAnalysisDataSetEntity
    {
        public int SecuritizationAnalysisDataSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsResecuritization { get; set; }
        public string SecuritizationAnalysisDataSetDescription { get; set; }     
    }
}
