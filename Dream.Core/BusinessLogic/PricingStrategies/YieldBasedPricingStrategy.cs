using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public class YieldBasedPricingStrategy : PricingStrategy
    {
        public double YieldToMaturity { get; set; }

        public YieldBasedPricingStrategy(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingCovention,
            double yieldToMaturity) 
            : base(dayCountConvention, compoundingCovention)
        {
            YieldToMaturity = yieldToMaturity;
            InternalRateOfReturn = yieldToMaturity;
        }

        public override PricingStrategy Copy()
        {
            return new YieldBasedPricingStrategy(
                DayCountConvention,
                CompoundingConvention,
                YieldToMaturity);
        }

        public override double CalculatePresentValue<T>(List<T> cashFlows)
        {
            var presentValue = DiscountCashFlows(YieldToMaturity, cashFlows);

            PresentValue = presentValue;
            return presentValue;
        }

        public override double CalculateShockedPresentValue<T>(List<T> cashFlows, double shockSizeInDecimal)
        {
            var shockedPresentValue = 0.0;
            if (SpreadOverCurve.HasValue)
            {
                var shockedSpread = SpreadOverCurve.Value + shockSizeInDecimal;
                var discountingFunction = DetermineDiscountingFunctionForSpread(CurveUsedForSpreadCalculation, cashFlows);
                shockedPresentValue = discountingFunction(shockedSpread, CurveUsedForSpreadCalculation, cashFlows);

                return shockedPresentValue;
            }

            var shockedYield = YieldToMaturity + shockSizeInDecimal;
            shockedPresentValue = DiscountCashFlows(shockedYield, cashFlows);

            return shockedPresentValue;
        }

        public override double CalculateInternalRateOfReturn<T>(List<T> cashFlows)
        {
            return YieldToMaturity;
        }

        public override void ClearCachedValues()
        {
            Price = null;
            PresentValue = null;
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
