using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Converters.Excel.Securitization;
using Dream.Core.Interfaces;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dream.Core.Repositories.Excel
{
    public class DbrsStressSecuritizationExcelDataRepository : SecuritizationExcelDataRepository
    {
        private const string _dbrsStressModelInputs = "DBRS Stress Model";

        private Dictionary<string, DbrsStressModelInput> _dbrsStressModelInputsDictionary = new Dictionary<string, DbrsStressModelInput>();
        private ProjectedPerformanceAssumptions _projectedPerformanceAssumptions;

        public DbrsStressSecuritizationExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public DbrsStressSecuritizationExcelDataRepository(Stream fileStream) : base(fileStream) { }

        public override Securitization GetPaceSecuritizationWithoutNodesDefined(SecuritizationInput securitizationInput)
        {
            var rateEnvironmentRepository = new MarketRateEnvironmentExcelDataRepository(_ExcelFileReader);
            var marketRateEnvironment = rateEnvironmentRepository.GetMarketRateEnvironmentWithoutRateCurves(securitizationInput.SecuritizationStartDate);
            securitizationInput.MarketRateEnvironment = marketRateEnvironment;

            var paceExcelDataRepository = new PaceAssessmentExcelDataRepository(_ExcelFileReader);

            var aggregationGroupings = GetAggregationGroupings();
            var performanceAssumptions = GetProjectedPerformanceAssumptions();
            _projectedPerformanceAssumptions = performanceAssumptions;

            if (!_dbrsStressModelInputsDictionary.ContainsKey(securitizationInput.ScenarioDescription))
            {
                throw new Exception(string.Format("INTERNAL ERROR: No DBRS stress model input was provided for the scenario named '{0}'. Please report this error.",
                    securitizationInput.ScenarioDescription));
            }

            var selectedAssumptionsGrouping = securitizationInput.SelectedPerformanceAssumptionGrouping ?? string.Empty;
            var dbrsStressModelInput = _dbrsStressModelInputsDictionary[securitizationInput.ScenarioDescription];
            var projectedCashFlowLogic = new DbrsStressModelProjectedCashFlowLogic(performanceAssumptions, dbrsStressModelInput, selectedAssumptionsGrouping);

            var paceSecuritization = new Securitization(paceExcelDataRepository, securitizationInput, aggregationGroupings, projectedCashFlowLogic)
            { Description = _ExcelFileReader.FileName };

            return paceSecuritization;
        }

        public override List<SecuritizationInput> GetSecuritizationInputs(out SecuritizationInputsRecord baseSecuritizationInputsRecord)
        {
            var dbrsStressModelInputRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<DbrsStressModelInputsRecord>(_dbrsStressModelInputs);
            var securitizationInputs = SecuritizationInputExcelConverter.ConvertDbrsStressModelInputsRecord(dbrsStressModelInputRecords, out _dbrsStressModelInputsDictionary);

            baseSecuritizationInputsRecord = dbrsStressModelInputRecords.First();
            return securitizationInputs;
        }

        protected override void ProcessRemainingSecuritizationInputsAsScenarios(List<SecuritizationInput> securitizationInputs)
        {
            var listOfSecuritizationInputsScenarios = new List<ScenarioAnalysis>();
            foreach (var securitizationInputsForScenario in securitizationInputs.Skip(1))
            {
                var scenarioDescription = securitizationInputsForScenario.ScenarioDescription;
                var securitizationInputsScenario = new InputsScenario(securitizationInputsForScenario);
                var scenarioLogicList = new List<IScenarioLogic> { securitizationInputsScenario };

                if (_dbrsStressModelInputsDictionary.ContainsKey(scenarioDescription))
                {
                    var scenarioDbrsStressModelInput = _dbrsStressModelInputsDictionary[scenarioDescription];
                    var scenarioAssumptionsGrouping = securitizationInputsForScenario.SelectedPerformanceAssumptionGrouping;
                    var scenarioProjectedCashFlowLogic = new DbrsStressModelProjectedCashFlowLogic(_projectedPerformanceAssumptions, scenarioDbrsStressModelInput, scenarioAssumptionsGrouping);
                    var projectedCashFlowLogicScenario = new ProjectedCashFlowLogicScenario(scenarioProjectedCashFlowLogic);
                    scenarioLogicList.Add(projectedCashFlowLogicScenario);
                }

                var scenarioAnalysis = new ScenarioAnalysis(scenarioDescription, scenarioLogicList);
                listOfSecuritizationInputsScenarios.Add(scenarioAnalysis);
            }

            _ScenariosToAnalyze.AddRange(listOfSecuritizationInputsScenarios);
        }
    }
}
