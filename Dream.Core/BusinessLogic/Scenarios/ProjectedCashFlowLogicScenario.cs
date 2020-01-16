using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Interfaces;
using Dream.Core.Reporting.Results;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class ProjectedCashFlowLogicScenario : IScenarioLogic
    {
        private ProjectedCashFlowLogic _scenarioProjectedCashFlowLogic;
        public ProjectedCashFlowLogic ScenarioProjectedCashFlowLogic => _scenarioProjectedCashFlowLogic;

        public bool RequiresRunningCashFlows => true;
        public bool RequiresLoadingCollateral => false;

        public ProjectedCashFlowLogicScenario(ProjectedCashFlowLogic scenarioProjectedCashFlowLogic)
        {
            _scenarioProjectedCashFlowLogic = scenarioProjectedCashFlowLogic;
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            securitization.ProjectedCashFlowLogic = _scenarioProjectedCashFlowLogic;
            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            loanPool.ProjectedCashFlowLogic = _scenarioProjectedCashFlowLogic;
            return loanPool;
        }
    }
}
