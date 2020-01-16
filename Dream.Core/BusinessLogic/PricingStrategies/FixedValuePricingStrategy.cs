using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.PricingStrategies
{
    public abstract class FixedValuePricingStrategy : PricingStrategy
    {
        public FixedValuePricingStrategy(DayCountConvention dayCountConvention, CompoundingConvention compoundingCovention)
            : base(dayCountConvention, compoundingCovention)
        { }

        public FixedValuePricingStrategy(DayCountConvention dayCountConvention, CompoundingConvention compoundingCovention, double pricingValueMulitplier) 
            : base(dayCountConvention, compoundingCovention, pricingValueMulitplier)
        { }

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

            var internalRateOfReturn = InternalRateOfReturn ?? CalculateInternalRateOfReturn(cashFlows);
            var shockedYield = internalRateOfReturn + shockSizeInDecimal;
            shockedPresentValue = DiscountCashFlows(shockedYield, cashFlows);

            return shockedPresentValue;
        }

        public override double CalculateMacaulayDuration<T>(List<T> cashFlows)
        {
            var firstCashFlow = cashFlows.First();
            var totalPresentValue = PresentValue ?? CalculatePresentValue(cashFlows);
            if(!InternalRateOfReturn.HasValue) CalculateInternalRateOfReturn(cashFlows);

            var sumOfTimeWeightedPresentValues = 0.0;
            foreach (var cashFlow in cashFlows)
            {
                var timeExpiredInYears = DateUtility.CalculateTimePeriodInYears(
                    DayCountConvention,
                    firstCashFlow.PeriodDate,
                    cashFlow.PeriodDate);

                var specificCashFlow = new List<CashFlow> { firstCashFlow, cashFlow };
                var presentValueOfSpecificCashFlow = CalculateShockedPresentValue(specificCashFlow, 0.0);
                var timeWeightedPresentValue = timeExpiredInYears * presentValueOfSpecificCashFlow;
                sumOfTimeWeightedPresentValues += timeWeightedPresentValue;
            }

            var macaulayDuration = sumOfTimeWeightedPresentValues / totalPresentValue;

            ClearCachedValues();
            MacaulayDuration = macaulayDuration;
            return macaulayDuration;
        }
    }
}
