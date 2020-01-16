using System.Collections.Generic;
using Dream.Core.Interfaces;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Scenarios.PrincingStrategyShocks
{
    public abstract class PricingStrategyShockScenario : IScenarioLogic
    {
        public ScenarioShock GlobalScenarioShock { get; protected set; }
        public Dictionary<string, ScenarioShock> ScenarioShockDictionary { get; protected set; }

        public abstract bool RequiresRunningCashFlows { get; }
        public bool RequiresLoadingCollateral => false;

        public PricingStrategyShockScenario()
        {
            ScenarioShockDictionary = new Dictionary<string, ScenarioShock>();
        }

        protected abstract PricingStrategy ShockPricingStrategy(
            PricingStrategy pricingStrategy, 
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDataGrouping,
            ScenarioShock scenarioShock, 
            double presentValue);

        protected abstract double GetShockValue(SecuritizationCashFlowsSummaryResult trancheCashFlowsSummaryResult);
        protected abstract double GetShockValue(ProjectedCashFlowsSummaryResult projectedCashFlowsSummaryResult);

        public virtual Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            var tranchesDictionary = securitization.TranchesDictionary;
            var securitizationNodesDictionary = securitization.NodesDictionary;
            var marketRateEnvironment = securitizationResult.MarketRateEnvironment;

            foreach (var trancheName in tranchesDictionary.Keys)
            {
                if (!ScenarioShockDictionary.ContainsKey(trancheName) ||
                    !securitizationResult.SecuritizationResultsDictionary.ContainsKey(trancheName)) continue;

                var scenarioShock = GlobalScenarioShock ?? ScenarioShockDictionary[trancheName];
                var pricingStrategy = tranchesDictionary[trancheName].Tranche.PricingStrategy;
                var shockValue = GetShockValue(securitizationResult.SecuritizationResultsDictionary[trancheName]);
                var marketDataGrouping = securitizationResult.SecuritizationResultsDictionary[trancheName].MarketDataUseForNominalSpread;

                var shockedPricingStrategy = ShockPricingStrategy(pricingStrategy, marketRateEnvironment, marketDataGrouping, scenarioShock, shockValue);

                securitization.TranchesDictionary[trancheName].Tranche.PricingStrategy = shockedPricingStrategy;
            }

            foreach (var securitizationNodeName in securitizationNodesDictionary.Keys)
            {
                if (!ScenarioShockDictionary.ContainsKey(securitizationNodeName) ||
                    !securitizationResult.SecuritizationResultsDictionary.ContainsKey(securitizationNodeName)) continue;

                var scenarioShock = GlobalScenarioShock ?? ScenarioShockDictionary[securitizationNodeName];
                var pricingStrategy = securitizationNodesDictionary[securitizationNodeName].PricingStrategy;
                var shockValue = GetShockValue(securitizationResult.SecuritizationResultsDictionary[securitizationNodeName]);
                var marketDataGrouping = securitization.Inputs.MarketDataGroupingForNominalSpread;

                var shockedPricingStrategy = ShockPricingStrategy(pricingStrategy, marketRateEnvironment, marketDataGrouping, scenarioShock, shockValue);

                securitization.NodesDictionary[securitizationNodeName].PricingStrategy = shockedPricingStrategy;
            }

            if (GlobalScenarioShock != null)
            {
                var cashFlowPricingStrategy = securitization.Inputs.CashFlowPricingStrategy;
                var shockValue = GetShockValue(securitizationResult.CollateralCashFlowsResultsDictionary[AggregationGroupings.TotalAggregationGroupName]);
                var marketDataGrouping = securitization.Inputs.MarketDataGroupingForNominalSpread;

                var shockedPricingStrategy = ShockPricingStrategy(cashFlowPricingStrategy, marketRateEnvironment, marketDataGrouping, GlobalScenarioShock, shockValue);

                securitization.Inputs.CashFlowPricingStrategy = shockedPricingStrategy;
            }

            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            var scenarioShock = GlobalScenarioShock;
            var pricingStrategy = loanPool.Inputs.CashFlowPricingStrategy;
            var shockValue = GetShockValue(totalProjectedCashFlowSummaryResult);
            var marketDataGrouping = loanPool.Inputs.MarketDataGroupingForNominalSpread;
            var marketRateEnvironment = loanPool.Inputs.MarketRateEnvironment;

            var shockedPricingStrategy = ShockPricingStrategy(pricingStrategy, marketRateEnvironment, marketDataGrouping, scenarioShock, shockValue);
            loanPool.Inputs.CashFlowPricingStrategy = shockedPricingStrategy;

            return loanPool;
        }

        public void SetGlobalScenarioShock(double shockValue, ShockStrategy shockStrategy)
        {
            var scenarioShock = new ScenarioShock(shockStrategy, shockValue);
            GlobalScenarioShock = scenarioShock;
        }

        public void AddToScenarioShockDictionary(string groupingIdentifier, double shockValue, ShockStrategy shockStrategy)
        {
            var scenarioShock = new ScenarioShock(shockStrategy, shockValue);

            if (ScenarioShockDictionary.ContainsKey(groupingIdentifier))
            {
                ScenarioShockDictionary[groupingIdentifier] = scenarioShock;
            }
            else
            {
                ScenarioShockDictionary.Add(groupingIdentifier, scenarioShock);
            }
        }        
    }
}
