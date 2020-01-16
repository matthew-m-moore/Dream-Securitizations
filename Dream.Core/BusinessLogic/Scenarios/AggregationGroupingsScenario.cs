using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Aggregation;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class AggregationGroupingsScenario : IScenarioLogic
    {
        private AggregationGroupings _scenarioAggregationGroupings;

        public bool RequiresRunningCashFlows => true;
        public bool RequiresLoadingCollateral => false;

        public AggregationGroupingsScenario(AggregationGroupings scenarioAggregationGroupings)
        {
            _scenarioAggregationGroupings = scenarioAggregationGroupings;
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            securitization.AggregationGroupings = _scenarioAggregationGroupings;
            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            loanPool.AggregationGroupings = _scenarioAggregationGroupings;
            return loanPool;
        }
    }
}
