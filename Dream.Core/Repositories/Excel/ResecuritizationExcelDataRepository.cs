using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.IO.Excel.Entities.SecuritizationRecords;

namespace Dream.Core.Repositories.Excel
{
    public class ResecuritizationExcelDataRepository : SecuritizationExcelDataRepository
    {
        private const string _collateralizedTrancheNames = "CollateralizedSecuritizations";

        public ResecuritizationExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public ResecuritizationExcelDataRepository(Stream fileStream) : base(fileStream) { }

        /// <summary>
        /// Retrieves a resecuritization of a securitization of PACE assessemnts from an Excel workbook inputs file.
        /// </summary>
        /// <returns></returns>
        public Resecuritization GetPaceResecuritizaion()
        {
            var securitization = GetPaceSecuritization();
            var resecuritization = securitization as Resecuritization;

            AddCollateralizedPaceSecuritizations(resecuritization);
            return resecuritization;
        }

        /// <summary>
        /// Retrieves a resecuritization of a securitization of PACE Assessments from an Excel workbook inputs file using a specified securitization input.
        /// Does not define the tranche structure of the securitization.
        /// </summary>
        public override Securitization GetPaceSecuritizationWithoutNodesDefined(SecuritizationInput securitizationInput)
        {
            var rateEnvironmentRepository = new MarketRateEnvironmentExcelDataRepository(_ExcelFileReader);
            var marketRateEnvironment = rateEnvironmentRepository.GetMarketRateEnvironmentWithoutRateCurves(securitizationInput.SecuritizationStartDate);
            securitizationInput.MarketRateEnvironment = marketRateEnvironment;

            var paceSecuritization = new Resecuritization(securitizationInput) { Description = _ExcelFileReader.FileName };
            return paceSecuritization;
        }

        private void AddCollateralizedPaceSecuritizations(Resecuritization resecuritization)
        {
            var listOfCollateralizedSecuritizationRecords = GetListOfCollateralizedTrancheNames();
            foreach (var collaterlizedSecuritizationRecord in listOfCollateralizedSecuritizationRecords)
            {
                var securitizationName = collaterlizedSecuritizationRecord.SecuritizationName;
                var securitizationInputsFilePath = collaterlizedSecuritizationRecord.PathToUnderlyingSecuritization;
                var collateralizedTrancheName = collaterlizedSecuritizationRecord.CollateralizedTrancheName;
                var collateralizedTranchePercentage = collaterlizedSecuritizationRecord.PercentageStake;

                if (!resecuritization.CollateralizedSecuritizationsDictionary.ContainsKey(securitizationName))
                {
                    Console.WriteLine(string.Format("Loading Data for Collateralized Securitization '{0}'...", securitizationName));
                    var securitizationDataRepository = new SecuritizationExcelDataRepository(securitizationInputsFilePath);
                    var securitization = securitizationDataRepository.GetPaceSecuritization();

                    var performanceAssumptions = GetProjectedPerformanceAssumptions();
                    var selectedAssumptionsGrouping = securitization.Inputs.SelectedPerformanceAssumptionGrouping ?? string.Empty;
                    var projectedCashFlowLogic = new ProjectedCashFlowLogic(performanceAssumptions, selectedAssumptionsGrouping);

                    resecuritization.ProjectedCashFlowLogicDictionary.Add(securitizationName, projectedCashFlowLogic);
                    resecuritization.CollateralizedSecuritizationsDictionary.Add(securitizationName, securitization);
                    Console.WriteLine(string.Format("Data Loaded for Collateralized Securitization '{0}'.", securitizationName));
                }
                                
                resecuritization.AddCollaterizedTrancheNameAndPercentage(securitizationName, collateralizedTrancheName, collateralizedTranchePercentage);
            }
        }

        private List<CollateralizedSecuritizationRecord> GetListOfCollateralizedTrancheNames()
        {
            var collateralizedTrancheNameRecords = _ExcelFileReader.GetDataFromSpecificTab<CollateralizedSecuritizationRecord>(_collateralizedTrancheNames);
            var collateralizedTrancheNames = collateralizedTrancheNameRecords
                .Where(r => !string.IsNullOrEmpty(r.CollateralizedTrancheName))
                .ToList();

            return collateralizedTrancheNames;
        }
    }
}
