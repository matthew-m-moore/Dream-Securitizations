using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System.Data.Entity;

namespace Dream.Core.Savers
{
    public class SecuritizationInputDatabaseSaver : DatabaseSaver
    {
        private SecuritizationInput _securitizationInput;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public SecuritizationInputDatabaseSaver(
            SecuritizationInput securitizationInput, 
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository)
        {
            _securitizationInput = securitizationInput;
            _typesAndConventionsDatabaseRepository = typesAndConventionsDatabaseRepository;
        }

        public int SaveSecuritizationInput()
        {
            int? nominalSpreadRateIndexGroupId = null;
            if (_securitizationInput.MarketDataGroupingForNominalSpread != default(MarketDataGrouping))
                nominalSpreadRateIndexGroupId = _typesAndConventionsDatabaseRepository.MarketDataGroupingsReversed[_securitizationInput.MarketDataGroupingForNominalSpread];

            int? curveSpreadRateIndexId = null;
            if (_securitizationInput.CurveTypeForSpreadCalcultion != default(InterestRateCurveType))
                curveSpreadRateIndexId = _typesAndConventionsDatabaseRepository.InterestRateCurveTypesReversed[_securitizationInput.CurveTypeForSpreadCalcultion];

            var securitizationAnalysisInputEntity = new SecuritizationAnalysisInputEntity
            {
                SecuritizationAnalysisScenarioDescription = _securitizationInput.ScenarioDescription,
                AdditionalAnalysisScenarioDescription = _securitizationInput.AdditionalYieldScenarioDescription,

                CashFlowStartDate = _securitizationInput.CashFlowStartDate,
                CollateralCutOffDate = _securitizationInput.CollateralCutOffDate,
                InterestAccrualStartDate = _securitizationInput.InterestAccrualStartDate,

                SecuritizationClosingDate = _securitizationInput.SecuritizationStartDate,
                SecuritizationFirstCashFlowDate = _securitizationInput.SecuritizationFirstCashFlowDate,

                SelectedAggregationGrouping = _securitizationInput.SelectedAggregationGrouping,
                SelectedPerformanceAssumption = _securitizationInput.SelectedPerformanceAssumption,
                SelectedPerformanceAssumptionGrouping = _securitizationInput.SelectedPerformanceAssumptionGrouping,

                NominalSpreadRateIndexGroupId = nominalSpreadRateIndexGroupId,
                CurveSpreadRateIndexId = curveSpreadRateIndexId,
                
                UseReplines = _securitizationInput.UseReplines,
                UsePreFundingStartDate = _securitizationInput.UsePreFundingStartDate,

                SeparatePrepaymentInterest = _securitizationInput.SeparatePrepaymentInterest,
                PreFundingAmount = _securitizationInput.PreFundingPercentageAmount,
                PreFundingBondCount = _securitizationInput.BondCountPerPreFunding,
                CleanUpCallPercentage = _securitizationInput.CleanUpCallPercentage
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.SecuritizationAnalysisInputEntities.Add(securitizationAnalysisInputEntity);
                securitizationEngineContext.SaveChanges();
            }

            return securitizationAnalysisInputEntity.SecuritizationAnalysisInputId;
        }
    }
}
