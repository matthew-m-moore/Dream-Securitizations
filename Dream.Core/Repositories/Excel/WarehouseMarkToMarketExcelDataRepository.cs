using Dream.Common;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Converters.Excel.Securitization;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dream.Core.Repositories.Excel
{
    public class WarehouseMarkToMarketExcelDataRepository : LoanPoolExcelDataRepository
    {
        private const string _warehouseMarkToMarketInputs = "WarehouseMarkToMarketInputs";

        public WarehouseMarkToMarketExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public WarehouseMarkToMarketExcelDataRepository(Stream fileStream) : base(fileStream) { }

        /// <summary>
        /// Retrieves a securitization of PACE Assessments from an Excel workbook inputs file.
        /// Does not define the tranche structure of the securitization.
        /// </summary>
        public Securitization GetMockPaceSecuritizationForMarkToMarket(out WarehouseMarkToMarketInput warehouseMarkToMarketInput)
        {
            var securitizationInput = GetMarkToMarketAnalysisInputs(out warehouseMarkToMarketInput);
            securitizationInput.UseReplines = false;

            var rateEnvironmentRepository = new MarketRateEnvironmentExcelDataRepository(_ExcelFileReader);
            var marketRateEnvironment = rateEnvironmentRepository.GetMarketRateEnvironmentWithoutRateCurves(securitizationInput.CollateralCutOffDate);
            securitizationInput.MarketRateEnvironment = marketRateEnvironment;

            var collateralCutOffDate = securitizationInput.CollateralCutOffDate;
            var cashFlowStartDate = securitizationInput.CashFlowStartDate;
            var interestAccrualStartDate = securitizationInput.InterestAccrualStartDate;

            var paceExcelDataRepository = new PaceAssessmentExcelDataRepository(_ExcelFileReader, collateralCutOffDate, cashFlowStartDate, interestAccrualStartDate);
            var paceAssessments = paceExcelDataRepository.GetAllPaceAssessments();

            var aggregationGroupings = GetAggregationGroupings(securitizationInput, paceAssessments);
            var performanceAssumptions = new ProjectedPerformanceAssumptions();
            var projectedCashFlowLogic = new ProjectedCashFlowLogic(performanceAssumptions, string.Empty);

            var dictionaryOfIdentifiers = aggregationGroupings.GetDictionaryOfIdentifersByGrouping();
            foreach (var groupingIdentifier in dictionaryOfIdentifiers.Keys)
            {
                var loansIdentifiersInGivenGrouping = dictionaryOfIdentifiers[groupingIdentifier];
                var loansAssociatedWithGrouping = paceAssessments.Where(p => loansIdentifiersInGivenGrouping.Contains(p.StringId)).ToList();
                WarehouseMarkToMarketPerformanceAssumptions.SetupPerformanceAssumptions(projectedCashFlowLogic, loansAssociatedWithGrouping);
            }

            var mockPaceSecuritization = new Securitization(paceExcelDataRepository, securitizationInput, aggregationGroupings, projectedCashFlowLogic);
            mockPaceSecuritization.SetCollateral(paceAssessments);
            return mockPaceSecuritization;
        }

        /// <summary>
        /// Retrieves the high-level mark-to-market analysis inputs from Excel
        /// Assumes there is only one set of inputs provided.
        /// </summary>
        private SecuritizationInput GetMarkToMarketAnalysisInputs(out WarehouseMarkToMarketInput warehouseMarkToMarketInput)
        {
            var securitizationInputRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<WarehouseMarkToMarketInputsRecord>(_warehouseMarkToMarketInputs);
            var securitizationInputRecord = securitizationInputRecords.First();

            var securitizationInput = SecuritizationInputExcelConverter.ConvertWarehouseMarkToMarketInputsRecord(securitizationInputRecord, out warehouseMarkToMarketInput);

            return securitizationInput;
        }

        /// <summary>
        /// Retrieves aggregation groupings from an Excel tab.
        /// </summary>
        private AggregationGroupings GetAggregationGroupings(SecuritizationInput securitizationInput, List<Loan> loans)
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_AggregationMapping);
            var selectedAggregationGrouping = securitizationInput.SelectedAggregationGrouping;

            AggregationGroupings aggregationGroupings;
            if (selectedAggregationGrouping == null || selectedAggregationGrouping == Constants.Automatic)
            {
                aggregationGroupings = AggregationGroupings.SetupAutomaticPaceAssessmentFundingDateAggregation(loans);
                securitizationInput.SelectedAggregationGrouping = AggregationGroupings.FundingDateAggregationGroupingIdentifier;
            }
            // If no aggregation grouping is selected, or the "Automatic" aggregation is picked, then take specific action
            else
            {
                aggregationGroupings = GetAggregationGroupings();
            }

            return aggregationGroupings;
        }
    }
}
