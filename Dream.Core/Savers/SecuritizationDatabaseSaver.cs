using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Interfaces;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Savers
{
    public class SecuritizationDatabaseSaver : DatabaseSaver
    {
        protected Securitization _Securitization;
        protected List<ScenarioAnalysis> _ScenariosToAnalyze;

        private int _securitizationAnalysisDataSetId;
        private int _securitizationAnalysisVersionId;

        protected virtual bool _IsResecuritization => false;
        protected const int _BaseId = SecuritizationDatabaseRepository.BaseScenarioId;

        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;
        private SecuritizationDatabaseRepository _securitizationDatabaseRepository;
        private Dictionary<string, int> _trancheDetailIdsDictionary;

        public List<SecuritizationAnalysisEntity> SecuritizationAnalysisEntities { get; set; }

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public int SecuritizationAnalysisDataSetId => _securitizationAnalysisDataSetId;
        public int SecuritizationAnalysisVersionId => _securitizationAnalysisVersionId;

        public SecuritizationDatabaseSaver(Securitization securitization, List<ScenarioAnalysis> scenariosToAnalyze, 
            bool isNewVersionOfExistingSecuritization = false,
            int? securitizationAnalysisDataSetId = null)
        : base(securitization.Inputs.SecuritizationStartDate, securitization.Description)
        {
            _Securitization = securitization;
            _ScenariosToAnalyze = scenariosToAnalyze ?? new List<ScenarioAnalysis>();

            _typesAndConventionsDatabaseRepository = new TypesAndConventionsDatabaseRepository();
            _securitizationDatabaseRepository = new SecuritizationDatabaseRepository();

            SecuritizationAnalysisEntities = new List<SecuritizationAnalysisEntity>();

            if (isNewVersionOfExistingSecuritization && !securitizationAnalysisDataSetId.HasValue)
            {
                throw new Exception("INTERNAL ERROR: Securitization data set ID must be provided in order to save a new version of an existing securitization. Please report this error");
            }

            if (isNewVersionOfExistingSecuritization)
            {
                _securitizationAnalysisDataSetId = securitizationAnalysisDataSetId.Value;

                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    var maximumExistingVersionId = securitizationEngineContext.SecuritizationAnalysisEntities
                        .Where(e => e.SecuritizationAnalysisDataSetId == _securitizationAnalysisDataSetId)
                        .Max(s => s.SecuritizationAnalysisVersionId);

                    _securitizationAnalysisVersionId = maximumExistingVersionId + 1;
                }
            }
            else
            {
                // Note that template securitizations cannot be saved by this code path, they should be created in the database directly
                var securitizationAnalysisDataSetEntity = new SecuritizationAnalysisDataSetEntity
                {
                    CutOffDate = _CutOffDate,
                    SecuritizationAnalysisDataSetDescription = _Description,
                    IsTemplate = false,
                    IsResecuritization = _IsResecuritization,
                };

                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.SecuritizationAnalysisDataSetEntities.Add(securitizationAnalysisDataSetEntity);
                    securitizationEngineContext.SaveChanges();
                }

                _securitizationAnalysisDataSetId = securitizationAnalysisDataSetEntity.SecuritizationAnalysisDataSetId;
                _securitizationAnalysisVersionId = _BaseId;
            }

            _Securitization.SecuritizationAnalysisDataSetId = _securitizationAnalysisDataSetId;
            _Securitization.SecuritizationAnalysisVersionId = _securitizationAnalysisVersionId;
        }

        public void SaveSecuritization()
        {
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.SecuritizationAnalysisEntities.AddRange(SecuritizationAnalysisEntities);
                securitizationEngineContext.SaveChanges();
            }

            _Securitization.SecuritizationAnalysisDataSetId = _securitizationAnalysisDataSetId;
            _Securitization.SecuritizationAnalysisVersionId = _securitizationAnalysisVersionId;
        }

        public void SaveSecuritizationInput() { SaveSecuritizationInput(_BaseId, null); }

        private void SaveSecuritizationInput(int scenarioId = _BaseId, SecuritizationInput securitizationInput = null)
        {
            securitizationInput = securitizationInput ?? _Securitization.Inputs;
            var securitizationInputDatabaseSaver = new SecuritizationInputDatabaseSaver(securitizationInput, _typesAndConventionsDatabaseRepository);
            var securitizationInputDataSetId = securitizationInputDatabaseSaver.SaveSecuritizationInput();

            var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                SecuritizationDatabaseRepository.SecuritizationAnalysisInput,
                securitizationInputDataSetId,
                scenarioId);

            SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
        }

        public void SaveMarketRateEnvironment() { SaveMarketRateEnvironment(_BaseId, null); }

        private void SaveMarketRateEnvironment(int scenarioId = _BaseId, MarketRateEnvironment marketRateEnvironment = null)
        {
            marketRateEnvironment = marketRateEnvironment ?? _Securitization.Inputs.MarketRateEnvironment;
            var marketRateEnvironmentDatabaseSaver = new MarketRateEnvironmentDatabaseSaver(
                marketRateEnvironment, 
                _typesAndConventionsDatabaseRepository,
                _CutOffDate,
                _Description);

            var securitizationInputDataSetId = marketRateEnvironmentDatabaseSaver.SaveMarketRateEnvironment();
            var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                SecuritizationDatabaseRepository.MarketRateEnvironment,
                securitizationInputDataSetId,
                scenarioId);

            SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
        }

        public void SavePaceAssessmentCollateral() { SavePaceAssessmentCollateral(_BaseId, null); }

        private void SavePaceAssessmentCollateral(int scenarioId = _BaseId, ICollateralRetriever paceAssessmentCollateralRetriever = null)
        {
            paceAssessmentCollateralRetriever = paceAssessmentCollateralRetriever ?? _Securitization.CollateralRetriever;
            var paceAssessmentDatabaseSaver = new PaceAssessmentDatabaseSaver(
                paceAssessmentCollateralRetriever,
                _CutOffDate,
                _Description);

            var securitizationInputDataSetId = paceAssessmentDatabaseSaver.SavePaceAssessments();
            var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                SecuritizationDatabaseRepository.PaceAssessmentCollateral,
                securitizationInputDataSetId,
                scenarioId);

            SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
        }

        public void SavePerformanceAssumptions() { SavePerformanceAssumptions(_BaseId, null); }

        private void SavePerformanceAssumptions(int scenarioId = _BaseId, ProjectedCashFlowLogic performingCashFlowLogic = null)
        {
            performingCashFlowLogic = performingCashFlowLogic ?? _Securitization.ProjectedCashFlowLogic;
            var performanceAssumptionsDatabaseSaver = new PerformanceAssumptionsDatabaseSaver(
                performingCashFlowLogic,
                _typesAndConventionsDatabaseRepository,
                _CutOffDate,
                _Description);

            var performanceAssumptionsDataSetId = performanceAssumptionsDatabaseSaver.SavePerformanceAssumptions();
            var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                SecuritizationDatabaseRepository.PerformanceAssumptions,
                performanceAssumptionsDataSetId,
                scenarioId);

            SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
        }



        public void SaveSecuritizationScenarios()
        {
            var scenarioId = _BaseId + 1;
            foreach(var scenario in _ScenariosToAnalyze)
            {
                scenario.SecuritizationAnalysisScenarioId = scenarioId;
                foreach (var scenarioLogic in scenario.ListOfScenarioLogicToApply)
                {
                    if (scenarioLogic is InputsScenario inputsScenario)
                        SaveSecuritizationInput(scenarioId, inputsScenario.ScenarioSecuritizationInputs);

                    else if (scenarioLogic is MarketRateEnvironmentScenario marketRateEnvironmentScenario)
                        SaveMarketRateEnvironment(scenarioId, marketRateEnvironmentScenario.ScenarioMarketRateEnvironment);

                    else if (scenarioLogic is CollateralRetrieverScenario collateralRetrieverScenario)
                        SavePaceAssessmentCollateral(scenarioId, collateralRetrieverScenario.ScenarioCollateralRetriever);

                    else if (scenarioLogic is ProjectedCashFlowLogicScenario projectedCashFlowLogicScenario)
                        SavePerformanceAssumptions(scenarioId, projectedCashFlowLogicScenario.ScenarioProjectedCashFlowLogic);
                }
                scenarioId++;
            }
        }

        public void SaveSecuritizationResultsAndSummary()
        {
            if (_Securitization.ResultsDictionary == null || !_Securitization.ResultsDictionary.Any()) return;

            var scenarioDescriptionsDictionary = _ScenariosToAnalyze.ToDictionary(s => s.Description, s => s.SecuritizationAnalysisScenarioId);
            scenarioDescriptionsDictionary.Add(_Securitization.Inputs.ScenarioDescription, _BaseId);

            var securitizationResultDatabaseSaver = new SecuritizationResultDatabaseSaver(
                _Securitization,
                _securitizationDatabaseRepository,
                scenarioDescriptionsDictionary);

            securitizationResultDatabaseSaver.SaveSecuritizationResults();

            var securitizationSummaryDatabaseSaver = new SecuritizationSummaryDatabaseSaver(_Securitization, scenarioDescriptionsDictionary);

            securitizationSummaryDatabaseSaver
                .PrepareSecuritizationNodeSummary(securitizationResultDatabaseSaver.SecuritizationNodeNamesOfResultsToSave);

            securitizationSummaryDatabaseSaver
                .PrepareSecuritizationTrancheSummary(securitizationResultDatabaseSaver.SecuritizationTrancheNamesOfResultsToSave);

            securitizationSummaryDatabaseSaver.SaveSecuritizationSummaries();
        }

        public void SaveSecuritizationStructure()
        {
            SaveSecuritizationNodeTrancheStructure();
            SavePriorityOfPayments();
            SaveRedemptionLogicList();
        }

        private void SaveSecuritizationNodeTrancheStructure()
        {
            var securitizationTrancheDatabaseSaver = new SecuritizationTrancheDatabaseSaver(
                _Securitization,
                _typesAndConventionsDatabaseRepository,
                _CutOffDate,
                _Description);

            _trancheDetailIdsDictionary = securitizationTrancheDatabaseSaver.TrancheDetailIdsDictionary;

            var securitizationNodesDatabaseId = securitizationTrancheDatabaseSaver.SaveSecuritizationTrancheStructure();
            var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                SecuritizationDatabaseRepository.SecuritizationNodesStructure,
                securitizationNodesDatabaseId,
                _BaseId);

            SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
        }

        private void SavePriorityOfPayments()
        {
            var priorityOfPaymentsDatabaseSaver = new PriorityOfPaymentsDatabaseSaver(
                _Securitization.PriorityOfPayments,
                _trancheDetailIdsDictionary,
                _typesAndConventionsDatabaseRepository,
                _CutOffDate,
                _Description);

            var priorityOfPaymentsSetId = priorityOfPaymentsDatabaseSaver.SavePriorityOfPayments();
            var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                SecuritizationDatabaseRepository.PriorityOfPayments,
                priorityOfPaymentsSetId,
                _BaseId);

            SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
        }

        private void SaveRedemptionLogicList()
        {
            var redemptionLogicDatabaseSaver = new RedemptionLogicDatabaseSaver(
                _Securitization.RedemptionLogicList,
                _trancheDetailIdsDictionary,
                _typesAndConventionsDatabaseRepository,
                _CutOffDate,
                _Description);

            var redemptionLogicDataSetIds = redemptionLogicDatabaseSaver.SaveRedemptionLogic();
            foreach(var redemptionLogicDataSetId in redemptionLogicDataSetIds)
            {
                var securitizationAnalysisEntity = CreateSecuritizationAnalysisEntity(
                    SecuritizationDatabaseRepository.RedemptionLogic,
                    redemptionLogicDataSetId,
                    _BaseId);

                SecuritizationAnalysisEntities.Add(securitizationAnalysisEntity);
            }
        }

        protected SecuritizationAnalysisEntity CreateSecuritizationAnalysisEntity(
            string inputTypeDescription, 
            int securitizationInputDataSetId, 
            int scenarioId)
        {
            var securitizationInputTypeId = _securitizationDatabaseRepository.SecuritizationInputTypes[inputTypeDescription];
            var securitizationAnalysisEntity = new SecuritizationAnalysisEntity
            {
                SecuritizationAnalysisDataSetId = _securitizationAnalysisDataSetId,
                SecuritizationAnalysisVersionId = _securitizationAnalysisVersionId,
                SecuritizationAnalysisScenarioId = scenarioId,
                SecuritizationInputTypeId = securitizationInputTypeId,
                SecuritizationInputTypeDataSetId = securitizationInputDataSetId
            };

            return securitizationAnalysisEntity;
        }
    }
}
