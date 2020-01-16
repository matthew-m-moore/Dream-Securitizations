using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class PerformanceAssumptionsEntry
    {
        public double? FlatCurveValue { get; set; }
        public bool IsVector { get; set; }
        public List<VectorEntry> VectorEntries { get; set; }
    }
}