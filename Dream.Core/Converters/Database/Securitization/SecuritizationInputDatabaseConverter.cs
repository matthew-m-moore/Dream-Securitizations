using Dream.Common.Enums;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Repositories.Database;
using Dream.IO.Database.Entities.Securitization;
using System;

namespace Dream.Core.Converters.Database.Securitization
{
    public class SecuritizationInputDatabaseConverter
    {
        public static SecuritizationInput ConvertToSecuritizationInput(
            SecuritizationAnalysisInputEntity securitizationAnalysisInputEntity,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository)
        {
            if (securitizationAnalysisInputEntity == null)
            {
                throw new Exception("INTERNAL ERROR: The database is missing a securitization analysis input record. Please report this error.");
            }

            var marketDataGroupingForNominalSpread = default(MarketDataGrouping);
            if (securitizationAnalysisInputEntity.NominalSpreadRateIndexGroupId.HasValue)
                marketDataGroupingForNominalSpread = typesAndConventionsDatabaseRepository.MarketDataGroupings[securitizationAnalysisInputEntity.NominalSpreadRateIndexGroupId.Value];

            var curveTypeForSpreadCalculation = default(InterestRateCurveType);
            if (securitizationAnalysisInputEntity.CurveSpreadRateIndexId.HasValue)
                curveTypeForSpreadCalculation = typesAndConventionsDatabaseRepository.InterestRateCurveTypes[securitizationAnalysisInputEntity.CurveSpreadRateIndexId.Value];

            var securitizationInput = new SecuritizationInput
            {
                ScenarioDescription = securitizationAnalysisInputEntity.SecuritizationAnalysisScenarioDescription,
                AdditionalYieldScenarioDescription = securitizationAnalysisInputEntity.AdditionalAnalysisScenarioDescription,

                CollateralCutOffDate = securitizationAnalysisInputEntity.CollateralCutOffDate,
                CashFlowStartDate = securitizationAnalysisInputEntity.CashFlowStartDate,
                InterestAccrualStartDate = securitizationAnalysisInputEntity.InterestAccrualStartDate,

                SelectedAggregationGrouping = securitizationAnalysisInputEntity.SelectedAggregationGrouping,
                SelectedPerformanceAssumption = securitizationAnalysisInputEntity.SelectedPerformanceAssumption,
                SelectedPerformanceAssumptionGrouping = securitizationAnalysisInputEntity.SelectedPerformanceAssumptionGrouping,

                MarketDataGroupingForNominalSpread = marketDataGroupingForNominalSpread,
                CurveTypeForSpreadCalcultion = curveTypeForSpreadCalculation,

                CashFlowPricingStrategy = new DoNothingPricingStrategy(),

                PreFundingPercentageAmount = securitizationAnalysisInputEntity.PreFundingAmount,
                BondCountPerPreFunding = securitizationAnalysisInputEntity.PreFundingBondCount.GetValueOrDefault(),

                SecuritizationStartDate = securitizationAnalysisInputEntity.SecuritizationClosingDate,
                SecuritizationFirstCashFlowDate = securitizationAnalysisInputEntity.SecuritizationFirstCashFlowDate,

                UseReplines = securitizationAnalysisInputEntity.UseReplines,
                UsePreFundingStartDate = securitizationAnalysisInputEntity.UsePreFundingStartDate,

                SeparatePrepaymentInterest = securitizationAnalysisInputEntity.SeparatePrepaymentInterest,
                CleanUpCallPercentage = securitizationAnalysisInputEntity.CleanUpCallPercentage
            };

            return securitizationInput;
        }
    }
}
