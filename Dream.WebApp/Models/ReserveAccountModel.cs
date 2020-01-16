using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class ReserveAccountModel
    {
        public bool IsModified { get; set; }
        public List<ReserveAccountModelEntry> ReserveAccountModelEntries { get; set; }
    }
}