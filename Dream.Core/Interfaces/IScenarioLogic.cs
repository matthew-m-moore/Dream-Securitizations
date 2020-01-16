using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Reporting.Results;

namespace Dream.Core.Interfaces
{
    public interface IScenarioLogic
    {
        bool RequiresRunningCashFlows { get; }
        bool RequiresLoadingCollateral { get; }
        Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult);
        LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult);
    }
}
