using System.Collections.Generic;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public class DoNothingPricingStrategy : PricingStrategy
    {
        public DoNothingPricingStrategy() : base(DayCountConvention.Actual365, CompoundingConvention.Annually) { }

        public override PricingStrategy Copy()
        {
            return new DoNothingPricingStrategy();
        }

        public override double CalculateInternalRateOfReturn<T>(List<T> cashFlows)
        {
            return 0.0;
        }

        public override double CalculatePresentValue<T>(List<T> cashFlows)
        {
            return 0.0;
        }

        public override double CalculateShockedPresentValue<T>(List<T> cashFlows, double shockSizeInDecimal)
        {
            return 0.0;
        }

        public override double CalculateSpread<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, InterestRateCurveType interestRateCurve)
        {
            return 0.0;
        }

        public override double CalculateNominalSpread<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, MarketDataGrouping marketDataGrouping)
        {
            return 0.0;
        }

        public override double CalculateMacaulayDuration<T>(List<T> cashFlows)
        {
            return 0.0;
        }

        public override double CalculateModifiedDuration<T>(List<T> cashFlows)
        {
            return 0.0;
        }

        public override double CalculateModifiedDuration<T>(List<T> cashFlows, double shockSizeInDecimal)
        {
            return 0.0;
        }
    }
}
