using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Interfaces;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class MarketRateEnvironmentScenario : IScenarioLogic
    {
        private MarketRateEnvironment _scenarioMarketRateEnvironment;
        public MarketRateEnvironment ScenarioMarketRateEnvironment => _scenarioMarketRateEnvironment;

        public bool RequiresRunningCashFlows => true;
        public bool RequiresLoadingCollateral => false;

        public MarketRateEnvironmentScenario(MarketRateEnvironment scenarioMarketRateEnvironment)
        {
            _scenarioMarketRateEnvironment = scenarioMarketRateEnvironment;
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            securitization.Inputs.MarketRateEnvironment = _scenarioMarketRateEnvironment;
            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            loanPool.Inputs.MarketRateEnvironment = _scenarioMarketRateEnvironment;
            return loanPool;
        }
    }
}
