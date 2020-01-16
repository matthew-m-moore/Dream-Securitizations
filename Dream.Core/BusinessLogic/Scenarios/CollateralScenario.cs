using Dream.Core.Interfaces;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.ProductTypes;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class CollateralScenario : IScenarioLogic
    {
        private List<Loan> _scenarioListOfLoans;
        public List<Loan> ScenarioListOfLoans => _scenarioListOfLoans;

        public bool RequiresRunningCashFlows => true;
        public bool RequiresLoadingCollateral => true;

        public CollateralScenario(List<Loan> scenarioCollateral)
        {
            _scenarioListOfLoans = scenarioCollateral;
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            securitization.SetCollateral(_scenarioListOfLoans);
            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            loanPool.SetListOfLoans(_scenarioListOfLoans);
            return loanPool;
        }
    }
}
