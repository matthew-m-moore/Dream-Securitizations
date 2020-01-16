using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class CollateralizedTranchesModel
    {
        public bool IsModified { get; set; }
        public List<CollateralizedTranchesModelEntry> CollateralizedTranchesModelEntries { get; set; }
    }
}