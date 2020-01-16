using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;
using System.Linq;
using Dream.Common.Enums;
using Dream.Common;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public class PercentOfBalancePricingStrategy : FixedValuePricingStrategy
    {
        public double PriceAsPercentOfBalance { get; set; }

        public PercentOfBalancePricingStrategy(
            DayCountConvention dayCountConvention, 
            CompoundingConvention compoundingCovention,
            double priceAsPercentOfBalance) 
            : base(dayCountConvention, compoundingCovention, Constants.OneHundredPercentagePoints)
        {
            PriceAsPercentOfBalance = priceAsPercentOfBalance;
            Price = priceAsPercentOfBalance;
        }

        public override PricingStrategy Copy()
        {
            return new PercentOfBalancePricingStrategy(
                DayCountConvention,
                CompoundingConvention,
                PriceAsPercentOfBalance);
        }

        public override double CalculatePresentValue<T>(List<T> cashFlows)
        {
            var contractualCashFlow = cashFlows.First() as ContractualCashFlow;
            if (contractualCashFlow == null) return double.NaN;

            var balance = contractualCashFlow.StartingBalance;

            var presentValue = balance * PriceAsPercentOfBalance;

            PresentValue = presentValue;
            return presentValue;
        }

        public override double CalculatePrice<T>(List<T> cashFlows)
        {
            return PriceAsPercentOfBalance;
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
