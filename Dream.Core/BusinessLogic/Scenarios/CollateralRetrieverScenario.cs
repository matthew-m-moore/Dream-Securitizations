using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Interfaces;
using Dream.Core.Reporting.Results;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class CollateralRetrieverScenario : IScenarioLogic
    {
        private ICollateralRetriever _scenarioCollateralRetriever;
        public ICollateralRetriever ScenarioCollateralRetriever => _scenarioCollateralRetriever;

        public bool RequiresRunningCashFlows => true;
        public bool RequiresLoadingCollateral => true;

        public CollateralRetrieverScenario(ICollateralRetriever scenarioCollateralRetriever)
        {
            _scenarioCollateralRetriever = scenarioCollateralRetriever;
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            securitization.CollateralRetriever = _scenarioCollateralRetriever;
            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            loanPool.ListOfLoansRetriever = _scenarioCollateralRetriever;
            return loanPool;
        }
    }
}
