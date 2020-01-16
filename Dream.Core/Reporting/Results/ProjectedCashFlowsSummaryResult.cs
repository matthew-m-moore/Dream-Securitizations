using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;

namespace Dream.Core.Reporting.Results
{
    public class ProjectedCashFlowsSummaryResult : CashFlowsSummaryResult
    {
        public double TotalPrepayment { get; set; }
        public double TotalDefault { get; set; }
        public double TotalRecovery { get; set; }
        public double TotalLoss { get; set; }

        public double LossGivenDefault => 
            TotalDefault > 0.0 
                ? TotalLoss / TotalDefault
                : 0.0;

        public List<ProjectedCashFlow> ProjectedCashFlows { get; }

        public ProjectedCashFlowsSummaryResult(List<ProjectedCashFlow> projectedCashFlows)
        {
            ProjectedCashFlows = projectedCashFlows;
        }
    }
}
