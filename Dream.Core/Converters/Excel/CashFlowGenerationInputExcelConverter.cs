using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.IO.Excel.Entities;

namespace Dream.Core.Converters.Excel
{
    public class CashFlowGenerationInputExcelConverter
    {
        /// <summary>
        /// Converts a cash-flow pricing input record into a cash-flow generation business object and a pricing strategy object.
        /// </summary>
        public static CashFlowGenerationInput ConvertCashFlowPricingInputsRecord(
            CashFlowPricingInputsRecord cashFlowPricingInputsRecord,
            MarketRateEnvironment marketRateEnvironment)
        {
            var pricingStrategyConverter = new PricingStrategyExcelConverter(marketRateEnvironment);
            var pricingStrategy = pricingStrategyConverter
                .ExtractPricingStrategyFromCashFlowPricingRecord(cashFlowPricingInputsRecord);

            var cashFlowGenerationInput = new CashFlowGenerationInput
            {
                CollateralCutOffDate = cashFlowPricingInputsRecord.CollateralCutOffDate,
                CashFlowStartDate = cashFlowPricingInputsRecord.StartDate,
                InterestAccrualStartDate = cashFlowPricingInputsRecord.InterestStartDate,

                SelectedAggregationGrouping = cashFlowPricingInputsRecord.AggregationGrouping,
                SelectedPerformanceAssumption = cashFlowPricingInputsRecord.PerformanceAssumption,
                SelectedPerformanceAssumptionGrouping = cashFlowPricingInputsRecord.PerformanceAssumptionGrouping,
                UseReplines = cashFlowPricingInputsRecord.TurnOnReplining.GetValueOrDefault(true),

                CashFlowPricingStrategy = pricingStrategy
            };

            return cashFlowGenerationInput;
        }

        /// <summary>
        /// Converts a cash-flow pricing input record into a cash-flow generation business object.
        /// </summary>
        public static CashFlowGenerationInput ConvertCashFlowPricingInputsRecord(
            CashFlowPricingInputsRecord cashFlowPricingInputsRecord)
        {
            var cashFlowGenerationInput = new CashFlowGenerationInput
            {
                CollateralCutOffDate = cashFlowPricingInputsRecord.CollateralCutOffDate,
                CashFlowStartDate = cashFlowPricingInputsRecord.StartDate,
                InterestAccrualStartDate = cashFlowPricingInputsRecord.InterestStartDate,

                SelectedAggregationGrouping = cashFlowPricingInputsRecord.AggregationGrouping,
                SelectedPerformanceAssumption = cashFlowPricingInputsRecord.PerformanceAssumption,
                SelectedPerformanceAssumptionGrouping = cashFlowPricingInputsRecord.PerformanceAssumptionGrouping,
                UseReplines = cashFlowPricingInputsRecord.TurnOnReplining.GetValueOrDefault(true),
            };

            return cashFlowGenerationInput;
        }
    }
}
