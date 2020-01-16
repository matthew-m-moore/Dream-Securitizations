using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Scenarios.PrincingStrategyShocks
{
    public class MarketValueShockScenario : PricingStrategyShockScenario
    {
        public override bool RequiresRunningCashFlows => false;

        protected override double GetShockValue(SecuritizationCashFlowsSummaryResult trancheCashFlowsSummaryResult)
        {
            return trancheCashFlowsSummaryResult.PresentValue;
        }

        protected override double GetShockValue(ProjectedCashFlowsSummaryResult projectedCashFlowsSummaryResult)
        {
            return projectedCashFlowsSummaryResult.PresentValue;
        }

        protected override PricingStrategy ShockPricingStrategy(
            PricingStrategy pricingStrategy,
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDateGrouping,
            ScenarioShock scenarioShock,
            double presentValue)
        {
            PricingStrategy shockedPricingStrategy;
            switch (scenarioShock.ShockStrategy)
            {
                case ShockStrategy.Additive:
                    shockedPricingStrategy = new SpecificMarketValuePricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        scenarioShock.ShockValue + presentValue);
                    break;

                case ShockStrategy.Multiplicative:
                    shockedPricingStrategy = new SpecificMarketValuePricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        scenarioShock.ShockValue * presentValue);
                    break;

                case ShockStrategy.Replacement:
                    shockedPricingStrategy = new SpecificMarketValuePricingStrategy(
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
