using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Scenarios.PrincingStrategyShocks
{
    public class PriceShockScenario : PricingStrategyShockScenario
    {
        public override bool RequiresRunningCashFlows => false;

        protected override double GetShockValue(SecuritizationCashFlowsSummaryResult trancheCashFlowsSummaryResult)
        {
            return trancheCashFlowsSummaryResult.DollarPrice.GetValueOrDefault();
        }

        protected override double GetShockValue(ProjectedCashFlowsSummaryResult projectedCashFlowsSummaryResult)
        {
            return projectedCashFlowsSummaryResult.DollarPrice.GetValueOrDefault();
        }

        protected override PricingStrategy ShockPricingStrategy(
            PricingStrategy pricingStrategy,
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDataGrouping,
            ScenarioShock scenarioShock, 
            double price)
        {
            PricingStrategy shockedPricingStrategy;
            switch (scenarioShock.ShockStrategy)
            {
                case ShockStrategy.Additive:
                    shockedPricingStrategy = new PercentOfBalancePricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        scenarioShock.ShockValue + price);
                    break;

                case ShockStrategy.Multiplicative:
                    shockedPricingStrategy = new PercentOfBalancePricingStrategy(
                        pricingStrategy.DayCountConvention,
                        pricingStrategy.CompoundingConvention,
                        scenarioShock.ShockValue * price);
                    break;

                case ShockStrategy.Replacement:
                    shockedPricingStrategy = new PercentOfBalancePricingStrategy(
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
