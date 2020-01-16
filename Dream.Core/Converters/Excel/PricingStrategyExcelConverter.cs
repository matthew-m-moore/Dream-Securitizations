using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.IO.Excel.Entities;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;

namespace Dream.Core.Converters.Excel
{
    public class PricingStrategyExcelConverter
    {
        private MarketRateEnvironment _marketRateEnivironment;

        public PricingStrategyExcelConverter(MarketRateEnvironment marketRateEnvironment)
        {
            _marketRateEnivironment = marketRateEnvironment;
        }

        public PricingStrategy ExtractPricingStrategyFromCashFlowPricingRecord(CashFlowPricingInputsRecord cashFlowPricingRecord)
        {
            var pricingMethodology = cashFlowPricingRecord.PricingMethodology;
            var pricingValue = cashFlowPricingRecord.PricingValue;

            var dayCountConvention = DayCountConventionExcelConverter.ConvertString(cashFlowPricingRecord.DayCounting);
            var compoundingConvention = CompoundingConventionExcelConverter.ConvertString(cashFlowPricingRecord.Compounding);

            return DeterminePricingStrategy(
                pricingMethodology,
                pricingValue,
                dayCountConvention,
                compoundingConvention);
        }

        public PricingStrategy ExtractPricingStrategyFromTrancheStructureRecord(TrancheStructureRecord trancheStructureRecord)
        {
            var pricingMethodology = trancheStructureRecord.PricingMethodology;
            var pricingValue = trancheStructureRecord.PricingValue;

            var dayCountConvention = DayCountConventionExcelConverter.ConvertString(trancheStructureRecord.DayCounting);
            var compoundingConvention = CompoundingConventionExcelConverter.ConvertString(trancheStructureRecord.Compounding);

            return DeterminePricingStrategy(
                pricingMethodology,
                pricingValue,
                dayCountConvention,
                compoundingConvention);
        }

        private PricingStrategy DeterminePricingStrategy(
            string pricingMethodology, 
            double pricingValue,
            DayCountConvention dayCountConvention, 
            CompoundingConvention compoundingConvention)
        {
            switch (pricingMethodology)
            {
                case Constants.NominalSpreadBasedPricing:
                    return new NominalSpreadBasedPricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        _marketRateEnivironment,
                        MarketDataGrouping.Swaps,
                        pricingValue / Constants.BpsPerOneHundredPercentagePoints);

                case Constants.YieldBasedPricing:
                    return new YieldBasedPricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        pricingValue);

                case Constants.MarketValueBasedPricing:
                    return new SpecificMarketValuePricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        pricingValue);

                case Constants.PercentOfBalanceBasedPricing:
                    return new PercentOfBalancePricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        pricingValue);

                default:
                    throw new Exception(string.Format("ERROR: The pricing methodology provided named '{0}' is not supported.",
                        pricingMethodology));
            }
        }
    }
}
