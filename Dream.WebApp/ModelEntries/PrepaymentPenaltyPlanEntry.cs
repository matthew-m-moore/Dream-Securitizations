using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class PrepaymentPenaltyPlanEntry
    {
        public string PrepaymentPenaltyPlanDescription { get; set; }
        public List<PrepaymentPenaltyPlanDetailEntry> PrepaymentPenaltyPlanDetailEntries { get; set; }
    }
}