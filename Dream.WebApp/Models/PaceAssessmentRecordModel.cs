using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class PaceAssessmentRecordModel
    {
        public bool IsModified { get; set; }
        public List<PaceAssessmentRecordModelEntry> PaceAssessmentRecordModelEntries { get; set; }
    }
}