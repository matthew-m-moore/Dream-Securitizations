using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class SecuritizationSummaryModel
    {
        public bool IsModified { get; set; }
        public List<SecuritizationSummaryModelEntry> SecuritizationSummaryModelEntries { get; set; }
    }
}