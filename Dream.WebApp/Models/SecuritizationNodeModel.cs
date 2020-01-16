using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class SecuritizationNodeModel
    {
        public bool IsModified { get; set; }
        public List<SecuritizationNodeModelEntry> SecuritizationNodeModelEntries { get; set; }
    }
}