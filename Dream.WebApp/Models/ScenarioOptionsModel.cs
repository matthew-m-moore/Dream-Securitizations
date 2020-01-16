using Dream.WebApp.ModelEntries;
using System.Collections.Generic;

namespace Dream.WebApp.Models
{
    public class ScenarioOptionsModel
    {
        public bool IsModified { get; set; }
        public List<ScenarioOptionsModelEntry> ScenarioOptionsModelEntries { get; set; }
    }
}