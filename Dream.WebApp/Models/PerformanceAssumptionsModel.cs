using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class PerformanceAssumptionsModel
    {
        public bool IsModified { get; set; }
        public List<PerformanceAssumptionsModelEntry> PerformanceAssumptionsModelEntries { get; set; }
    }
}