using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Scenarios.PrincingStrategyShocks
{
    public class NominalSpreadShockScenario : PricingStrategyShockScenario
    {
        public override bool RequiresRunningCashFlows => false;

        protected override double GetShockValue(SecuritizationCashFlowsSummaryResult trancheCashFlowsSummaryResult)
        {
            return trancheCashFlowsSummaryResult.NominalSpread.GetValueOrDefault();
        }

        protected override double GetShockValue(ProjectedCashFlowsSummaryResult projectedCashFlowsSummaryResult)
        {
            return projectedCashFlowsSummaryResult.NominalSpread.GetValueOrDefault();
        }

        protected override PricingStrategy ShockPricingStrategy(
            PricingStrategy pricingStrategy,
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDataGrouping,
            ScenarioShock scenarioShock, 
            double nominalSpread)
        {
            PricingStrategy shockedPricingStrategy;
            switch (scenarioShock.ShockStrategy)
            {
                case ShockStrategy.Additive:
                    shockedPricingStrategy = new NominalSpreadBasedPricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        marketRateEnvironment,
                        marketDataGrouping,
                        scenarioShock.ShockValue + nominalSpread);
                    break;

                case ShockStrategy.Multiplicative:
                    shockedPricingStrategy = new NominalSpreadBasedPricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        marketRateEnvironment,
                        marketDataGrouping,
                        scenarioShock.ShockValue * nominalSpread);
                    break;

                case ShockStrategy.Replacement:
                    shockedPricingStrategy = new NominalSpreadBasedPricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        marketRateEnvironment,
                        marketDataGrouping,
                        scenarioShock.ShockValue);
                    break;

                default:
                    shockedPricingStrategy = pricingStrategy;
                    break;
            }

            return shockedPricingStrategy;
        }
    }
}
