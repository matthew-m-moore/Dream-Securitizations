using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public class SpreadOverCurveBasedPricingStrategy : PricingStrategy
    {
        public double SpreadOverRateCurve { get; set; }
        public MarketRateEnvironment MarketRateEnvironment { get; set; }
        public InterestRateCurveType InterestRateCurveType { get; set; }

        public SpreadOverCurveBasedPricingStrategy(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingCovention,
            MarketRateEnvironment marketRateEnvironment,
            InterestRateCurveType interestRateCurveType,
            double spreadOverRateCurve) 
            : base(dayCountConvention, compoundingCovention, Constants.BpsPerOneHundredPercentagePoints)
        {
            SpreadOverRateCurve = spreadOverRateCurve;
            MarketRateEnvironment = marketRateEnvironment;
            InterestRateCurveType = interestRateCurveType;

            CurveTypeUsedForSpreadCalculation = interestRateCurveType;
            SpreadOverCurve = spreadOverRateCurve;         
        }

        public override PricingStrategy Copy()
        {
            return new SpreadOverCurveBasedPricingStrategy(
                DayCountConvention,
                CompoundingConvention,
                MarketRateEnvironment.Copy(),
                InterestRateCurveType,
                SpreadOverRateCurve);
        }

        public override double CalculatePresentValue<T>(List<T> cashFlows)
        {
            var interestRateCurve = MarketRateEnvironment[InterestRateCurveType];
            var discountingFunction = DetermineDiscountingFunctionForSpread(interestRateCurve, cashFlows);

            var presentValue = discountingFunction(SpreadOverRateCurve, interestRateCurve, cashFlows);

            PresentValue = presentValue;
            return presentValue;
        }

        public override double CalculateShockedPresentValue<T>(List<T> cashFlows, double shockSizeInDecimal)
        {
            var interestRateCurve = MarketRateEnvironment[InterestRateCurveType];
            var discountingFunction = DetermineDiscountingFunctionForSpread(interestRateCurve, cashFlows);

            var shockedSpread = SpreadOverRateCurve + shockSizeInDecimal;
            var shockedPresentValue = discountingFunction(shockedSpread, interestRateCurve, cashFlows);

            return shockedPresentValue;
        }

        public override double CalculateSpread<T>(List<T> cashFlows, MarketRateEnvironment rateEnvironment, InterestRateCurveType interestRateCurveType)
        {
            return SpreadOverRateCurve;
        }

        public override void ClearCachedValues()
        {
            Price = null;
            PresentValue = null;
            InternalRateOfReturn = null;
            InterpolatedRate = null;
            NominalSpread = null;
            SpreadDuration = null;

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
