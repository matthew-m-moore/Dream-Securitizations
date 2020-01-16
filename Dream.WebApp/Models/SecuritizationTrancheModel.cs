using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class SecuritizationTrancheModel
    {
        public bool IsModified { get; set; }
        public List<SecuritizationTrancheModelEntry> SecuritizationTrancheModelEntries { get; set; }
    }
}