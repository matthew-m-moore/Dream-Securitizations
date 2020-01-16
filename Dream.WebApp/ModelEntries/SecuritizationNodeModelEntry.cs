using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class SecuritizationNodeModelEntry
    {
        public SecuritizationNodeModelEntry ParentSecuritizationNodeModelEntry { get; set; }
        public List<SecuritizationNodeModelEntry> SecuritizationNodeModelEntries { get; set; }
        public int SecuritizationNodeId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public string SecuritizationNodeType { get; set; }
        public string SecuritizationNodeRating { get; set; }
        public string AvailableFundsDistributionRuleDescription { get; set; }
    }
}