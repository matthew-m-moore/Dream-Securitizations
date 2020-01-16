using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class FeeTrancheModel
    {
        public bool IsModified { get; set; }
        public List<FeeTrancheModelEntry> FeeTrancheModelEntries { get; set; }
    }
}