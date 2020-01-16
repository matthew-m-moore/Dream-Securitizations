using Dream.Common.Enums;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public class SpecificMarketValuePricingStrategy : FixedValuePricingStrategy
    {
        public double SpecificMarketValue { get; set; }

        public SpecificMarketValuePricingStrategy(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingCovention,
            double specificMarketValue) 
            : base(dayCountConvention, compoundingCovention)
        {
            SpecificMarketValue = specificMarketValue;
            PresentValue = specificMarketValue;
        }

        public override PricingStrategy Copy()
        {
            return new SpecificMarketValuePricingStrategy(
                DayCountConvention,
                CompoundingConvention,
                SpecificMarketValue);
        }

        public override double CalculatePresentValue<T>(List<T> cashFlows)
        {
            return SpecificMarketValue;
        }

        public override void ClearCachedValues()
        {
            PresentValue = null;
            InternalRateOfReturn = null;
            InterpolatedRate = null;
            NominalSpread = null;
            SpreadOverCurve = null;
            SpreadDuration = null;

            CurveTypeUsedForSpreadCalculation = InterestRateCurveType.None;
            MarketDataUsedForNominalSpread = MarketDataGrouping.None;
            CurveUsedForSpreadCalculation = null;

            MacaulayDuration = null;
            ModifiedDuration = null;
            EffectiveDuration = null;
            DollarDuration = null;
            EffectiveDollarDuration = null;
        }
    }
}
