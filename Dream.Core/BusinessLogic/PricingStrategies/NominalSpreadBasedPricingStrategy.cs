using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public class NominalSpreadBasedPricingStrategy : PricingStrategy
    {
        public double SpreadOverPointOnCurve { get; set; }
        public MarketRateEnvironment MarketRateEnvironment { get; set; }
        public MarketDataGrouping MarketDataGrouping { get; set; }

        public NominalSpreadBasedPricingStrategy(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingCovention,
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDataGrouping,
            double nominalSpread) 
            : base(dayCountConvention, compoundingCovention, Constants.BpsPerOneHundredPercentagePoints)
        {
            SpreadOverPointOnCurve = nominalSpread;
            MarketRateEnvironment = marketRateEnvironment;
            MarketDataGrouping = marketDataGrouping;

            MarketDataUsedForNominalSpread = marketDataGrouping;
            NominalSpread = nominalSpread;         
        }

        public override PricingStrategy Copy()
        {
            return new NominalSpreadBasedPricingStrategy(
                DayCountConvention,
                CompoundingConvention,
                MarketRateEnvironment.Copy(),
                MarketDataGrouping,
                SpreadOverPointOnCurve);
        }

        public override double CalculatePresentValue<T>(List<T> cashFlows)
        {
            // Note, the interpolated rate should be calculated just once for the total present value
            var linearlyInterpolatedRate = InterpolatedRate 
                ?? GetLinearlyInterpolatedRate(cashFlows, MarketRateEnvironment, MarketDataGrouping);

            var yieldToMaturity = linearlyInterpolatedRate + SpreadOverPointOnCurve;
            var presentValue = DiscountCashFlows(yieldToMaturity, cashFlows);

            InterpolatedRate = linearlyInterpolatedRate;
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

            var linearlyInterpolatedRate = GetLinearlyInterpolatedRate(cashFlows, MarketRateEnvironment, MarketDataGrouping);

            var shockedYield = linearlyInterpolatedRate + SpreadOverPointOnCurve + shockSizeInDecimal;
            shockedPresentValue = DiscountCashFlows(shockedYield, cashFlows);

            return shockedPresentValue;
        }

        public override double CalculateNominalSpread<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, MarketDataGrouping marketDataGrouping)
        {
            InterpolatedRate = InterpolatedRate ?? GetLinearlyInterpolatedRate(cashFlows, rateEnvironment, marketDataGrouping);
            return SpreadOverPointOnCurve;
        }

        public override double CalculateInternalRateOfReturn<T>(List<T> cashFlows)
        {
            var linearlyInterpolatedRate = GetLinearlyInterpolatedRate(cashFlows, MarketRateEnvironment, MarketDataGrouping);

            var yieldToMaturity = linearlyInterpolatedRate + SpreadOverPointOnCurve;

            InterpolatedRate = linearlyInterpolatedRate;
            InternalRateOfReturn = yieldToMaturity;
            return yieldToMaturity;
        }

        public override void ClearCachedValues()
        {
            Price = null;
            PresentValue = null;
            InternalRateOfReturn = null;
            InterpolatedRate = null;
            SpreadOverCurve = null;
            SpreadDuration = null;

            CurveTypeUsedForSpreadCalculation = InterestRateCurveType.None;
            CurveUsedForSpreadCalculation = null;

            MacaulayDuration = null;
            ModifiedDuration = null;
            EffectiveDuration = null;
            DollarDuration = null;
            EffectiveDollarDuration = null;
        }
    }
}
