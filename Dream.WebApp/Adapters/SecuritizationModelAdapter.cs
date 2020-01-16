using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.Converters.Database.Securitization;
using Dream.WebApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dream.WebApp.Adapters
{
    public class SecuritizationModelAdapter
    {
        private Securitization _securitizationBusinessObject;
        private List<ScenarioAnalysis> _securitizationScenarios;

        private bool _isResecuritization;

        public SecuritizationModelAdapter(Securitization securitization, List<ScenarioAnalysis> securitizationScenarios)
        {
            _securitizationBusinessObject = securitization;
            _securitizationScenarios = securitizationScenarios;
            _isResecuritization = (_securitizationBusinessObject is Resecuritization);
        }

        public SecuritizationModel CreateSecuritizationModel()
        {
            // Rather than serialize the business object as a binary, the path forward here is to make something compatible with JSON that is
            // freely editable in the browser window. That can then be post back to the server and coverted back into a business object.
          
            //var securitizationNodeModel = SecuritizationNodeModelAdapter.GetSecuritizationNodeModel(_securitizationBusinessObject);
            //var priorityOfPaymentsModel = PriorityOfPaymentsModelAdapter.GetPriorityOfPaymentsModel(_securitizationBusinessObject);
            //var marketRateEnvironmentModel = MarketRateEnvironmentModelAdapter.GetMarketRateEnvironmentModel(_securitizationBusinessObject);
            //var performanceAssumptionsModel = PerformanceAssumptionsModelAdapter.GetPerformanceAssumptionsModel(_securitizationBusinessObject);
            //var scenarioOptionsModel = ScenarioOptionsModelAdapter.GetScenarioOptionsModel(_securitizationBusinessObject);

            var securitzationSummaryModel = SecuritizationSummaryModelAdapter.GetSecuritizationSummaryModel(_securitizationBusinessObject, _isResecuritization);
            //var paceAssessmentRecordModel = PaceAssessmentRecordModelAdapter.GetPaceAssessmentRecordModel(_securitizationBusinessObject, _isResecuritization);
            //var collateralizedTranchesModel = CollateralizedTranchesModelAdapter.GetCollateralizedTranchesModel(_securitizationBusinessObject, _isResecuritization);

            //var securitizationTrancheModelAdapter = new SecuritizationTrancheModelAdapter(_securitizationBusinessObject, _isResecuritization);
            //var securitizationTrancheModel = securitizationTrancheModelAdapter.SecuritizationTrancheModel;
            //var feeTrancheModel = securitizationTrancheModelAdapter.FeeTrancheModel;
            //var reserveAccountModel = securitizationTrancheModelAdapter.ReserveAccountModel;

            //// Just take the first redemption logic in the list for now, since selecting mulitple redemption logic options from the UI will take some time to build out
            //var redemptionLogicDescription = RedemptionLogicTypeDatabaseConverter.ConvertTypeToDescription(typeof(DoNothingRedemptionLogic));
            //if (_securitizationBusinessObject.RedemptionLogicList.FirstOrDefault() == null)
            //{
            //    redemptionLogicDescription = RedemptionLogicTypeDatabaseConverter
            //        .ConvertTypeToDescription(_securitizationBusinessObject.RedemptionLogicList.FirstOrDefault().GetType());
            //}

            // This object should be completely serializable using JSON, any updates to it will be tracked and pushed back into a business object
            return new SecuritizationModel
            {
                SecuritizationDescription = _securitizationBusinessObject.Description,
                SecuritizationDataSetId = _securitizationBusinessObject.SecuritizationAnalysisDataSetId.Value,
                SecuritizationVersionId = _securitizationBusinessObject.SecuritizationAnalysisVersionId.Value,

                SecuritizationSummaryModel = securitzationSummaryModel,
                //SecuritizationNodeModel = securitizationNodeModel,
                //SecuritizationTrancheModel = securitizationTrancheModel,
                //FeeTrancheModel = feeTrancheModel,
                //ReserveAccountModel = reserveAccountModel,
                //PriorityOfPaymentsModel = priorityOfPaymentsModel,
                //MarketRateEnvironmentModel = marketRateEnvironmentModel,
                //PerformanceAssumptionsModel = performanceAssumptionsModel,
                //ScenarioOptionsModel = scenarioOptionsModel,
                //PaceAssessmentRecordModel = paceAssessmentRecordModel,
                //CollateralizedTranchesModel = collateralizedTranchesModel,

                CollateralCutOffDate = _securitizationBusinessObject.Inputs.CollateralCutOffDate,
                CashFlowStartDate = _securitizationBusinessObject.Inputs.CashFlowStartDate,
                InterestAccrualStartDate = _securitizationBusinessObject.Inputs.InterestAccrualStartDate,
                SecuritizationClosingDate = _securitizationBusinessObject.Inputs.SecuritizationStartDate,
                SecuritizationFirstCashFlowDate = _securitizationBusinessObject.Inputs.SecuritizationFirstCashFlowDate,

                //RedemptionLogicDescription = redemptionLogicDescription,
                NominalSpreadRateIndexGroup = _securitizationBusinessObject.Inputs.MarketDataGroupingForNominalSpread.ToString(),
                CurveSpreadRateIndexGroup = _securitizationBusinessObject.Inputs.CurveTypeForSpreadCalcultion.ToString(),

                IsResecuritization = _isResecuritization,
                UseReplines = _securitizationBusinessObject.Inputs.UseReplines,
                SeparatePrepaymentInterest = _securitizationBusinessObject.Inputs.SeparatePrepaymentInterest,

                PreFundingAmount = _securitizationBusinessObject.Inputs.PreFundingPercentageAmount,
                PreFundingBondCount = _securitizationBusinessObject.Inputs.BondCountPerPreFunding,
                CleanUpCallPercentage = _securitizationBusinessObject.Inputs.CleanUpCallPercentage,
            };
        }
    }
}