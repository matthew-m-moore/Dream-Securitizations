using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dream.WebApp.ModelEntries
{
    public class CollateralizedTranchesModelEntry
    {
        public int SecuritizationDataSetId { get; set; }
        public int SecuritizationVersionId { get; set; }
        public string SecuritizationName { get; set; }
        public string CollateralizedTrancheName { get; set; }
        public double CollateralizedTrancheBalance { get; set; }
        public double CollateralizedTranchePercentage { get; set; }
    }
}