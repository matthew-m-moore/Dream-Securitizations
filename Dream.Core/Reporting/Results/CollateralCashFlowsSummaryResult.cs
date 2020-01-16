using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;

namespace Dream.Core.Reporting.Results
{
    public class CollateralCashFlowsSummaryResult : CashFlowsSummaryResult
    {
        public string Description { get; set; }

        public double TotalPrepayment { get; set; }
        public double TotalDefault { get; set; }
        public double TotalRecovery { get; set; }
        public double TotalLoss { get; set; }

        public List<ProjectedCashFlow> ProjectedCashFlows { get; }

        public CollateralCashFlowsSummaryResult(List<ProjectedCashFlow> projectedCashFlows)
        {
            ProjectedCashFlows = projectedCashFlows;
        }
    }
}
