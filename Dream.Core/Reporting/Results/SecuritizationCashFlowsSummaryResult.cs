using Dream.Core.BusinessLogic.Containers.CashFlows;
using System;
using System.Collections.Generic;

namespace Dream.Core.Reporting.Results
{
    public class SecuritizationCashFlowsSummaryResult : CashFlowsSummaryResult
    {
        public int ChildToParentOrder { get; set; }

        public string FullyQualifiedName => (!string.IsNullOrEmpty(TrancheName)) 
            ? SecuritizationNodeName + " - " + TrancheName
            : SecuritizationNodeName;

        public string SecuritizationNodeName { get; set; }

        public string TrancheName = string.Empty;
        public Type TrancheType { get; set; }

        public PaymentCorridor PaymentCorridor { get; set; }
        public List<SecuritizationCashFlow> TrancheCashFlows { get; }

        public InterestShortfallRecord InterestShortfallRecord { get; set; }

        public SecuritizationCashFlowsSummaryResult(List<SecuritizationCashFlow> trancheCashFlows)
        {
            TrancheCashFlows = trancheCashFlows;
        }
    }
}