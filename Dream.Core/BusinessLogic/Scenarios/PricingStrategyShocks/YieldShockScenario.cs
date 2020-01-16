using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Scenarios.PrincingStrategyShocks
{
    public class YieldShockScenario : PricingStrategyShockScenario
    {
        public override bool RequiresRunningCashFlows => false;

        protected override double GetShockValue(SecuritizationCashFlowsSummaryResult trancheCashFlowsSummaryResult)
        {
            return trancheCashFlowsSummaryResult.InternalRateOfReturn;
        }

        protected override double GetShockValue(ProjectedCashFlowsSummaryResult projectedCashFlowsSummaryResult)
        {
            return projectedCashFlowsSummaryResult.InternalRateOfReturn;
        }

        protected override PricingStrategy ShockPricingStrategy(
            PricingStrategy pricingStrategy,
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDataGrouping,
            ScenarioShock scenarioShock,
            double yield)
        {
            PricingStrategy shockedPricingStrategy;
            switch (scenarioShock.ShockStrategy)
            {
                case ShockStrategy.Additive:
                    shockedPricingStrategy = new YieldBasedPricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        scenarioShock.ShockValue + yield);
                    break;

                case ShockStrategy.Multiplicative:
                    shockedPricingStrategy = new YieldBasedPricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        scenarioShock.ShockValue * yield);
                    break;

                case ShockStrategy.Replacement:
                    shockedPricingStrategy = new YieldBasedPricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
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
