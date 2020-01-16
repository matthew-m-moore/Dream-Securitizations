using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Converters.Excel;
using Dream.Core.Converters.Excel.Collateral;
using Dream.IO.Excel.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dream.Core.Repositories.Excel
{
    public class LoanPoolExcelDataRepository : ExcelDataRepository
    {
        private const string _projectedCashFlowInputs = "ProjectedCashFlowsInputs";
        private const string _performanceAssumptions = "PerformanceAssumptions";
        private const string _performanceAssumptionsMapping = "PerformanceAssumptionsMapping";

        protected const string _AggregationMapping = "AggregationMapping";

        public LoanPoolExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public LoanPoolExcelDataRepository(Stream fileStream) : base(fileStream) { }

        /// <summary>
        /// Gets a loan pool object populated with the list of loans supplied for the pool.
        /// </summary>
        public LoanPool GetLoanPool(List<Loan> listOfLoansInLoanPool)
        {
            var cashFlowGenerationInputs = GetCashFlowGenerationInput(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord);

            var aggregationGroupings = GetAggregationGroupings();
            var performanceAssumptions = GetProjectedPerformanceAssumptions();
            var selectedAssumptionsGrouping = cashFlowPricingInputsRecord.PerformanceAssumptionGrouping ?? string.Empty;
            var projectedCashFlowLogic = new ProjectedCashFlowLogic(performanceAssumptions, selectedAssumptionsGrouping);

            var loanPool = new LoanPool(null, cashFlowGenerationInputs, aggregationGroupings, projectedCashFlowLogic);
            loanPool.SetListOfLoans(listOfLoansInLoanPool);
            return loanPool;
        }

        /// <summary>
        /// Gets a loan pool object populated with the list of loans supplied for the pool.
        /// </summary>
        public LoanPool GetLoanPoolOfPaceAssessments(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord)
        {
            var cashFlowGenerationInput = GetCashFlowGenerationInput(out cashFlowPricingInputsRecord);
            var paceExcelDataRepository = new PaceAssessmentExcelDataRepository(_ExcelFileReader);

            var aggregationGroupings = GetAggregationGroupings();
            var performanceAssumptions = GetProjectedPerformanceAssumptions();
            var selectedAssumptionsGrouping = cashFlowPricingInputsRecord.PerformanceAssumptionGrouping ?? string.Empty;
            var projectedCashFlowLogic = new ProjectedCashFlowLogic(performanceAssumptions, selectedAssumptionsGrouping);

            var loanPool = new LoanPool(paceExcelDataRepository, cashFlowGenerationInput, aggregationGroupings, projectedCashFlowLogic);
            return loanPool;
        }

        /// <summary>
        /// Retrieves a cash-flow generation business object and a pricing strategy from an Excel inputs file.
        /// </summary>
        public CashFlowGenerationInput GetCashFlowGenerationInput(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord)
        {
            var cashFlowPricingInputsRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<CashFlowPricingInputsRecord>(_projectedCashFlowInputs);
            cashFlowPricingInputsRecord = cashFlowPricingInputsRecords.First();

            var rateEnvironmentRepository = new MarketRateEnvironmentExcelDataRepository(_ExcelFileReader);
            var marketRateEnvironment = rateEnvironmentRepository.GetMarketRateEnvironmentWithoutRateCurves(_CutOffDate);

            var cashFlowGenerationInput = CashFlowGenerationInputExcelConverter
                .ConvertCashFlowPricingInputsRecord(cashFlowPricingInputsRecord, marketRateEnvironment);

            cashFlowGenerationInput.MarketRateEnvironment = marketRateEnvironment;
            cashFlowGenerationInput.MarketDataGroupingForNominalSpread = MarketDataGrouping.Swaps;

            return cashFlowGenerationInput;
        }

        /// <summary>
        /// Retrieves aggregation groupings from an Excel tab.
        /// </summary>
        public virtual AggregationGroupings GetAggregationGroupings()
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_AggregationMapping);
            var aggregationGroupings = AggregationGroupingsExcelConverter.ConvertExcelRowsToAggregationGroupings(excelDataRows);
            return aggregationGroupings;
        }

        /// <summary>
        /// Retrieves projected performance assumptions (e.g. CPR and CDR) from an Excel tab.
        /// </summary>
        public ProjectedPerformanceAssumptions GetProjectedPerformanceAssumptions()
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_performanceAssumptions);
            var projectedPerformanceAssumptions =
                ProjectedPerformanceAssumptionsExcelConverter.ConvertExcelRowsToProjectedPeformanceAssumptions(excelDataRows);

            // All assumptions will be used on a monthly basis in most cases
            var convertAssumptionsToAnnual = false;
            var convertAssumptionsToMonthly = true;

            var performanceAssumptionsMapping = GetProjectedPerformanceAssumptionsMapping(
                    convertAssumptionsToAnnual,
                    convertAssumptionsToMonthly);

            projectedPerformanceAssumptions.PerformanceAssumptionsMapping = performanceAssumptionsMapping;
            return projectedPerformanceAssumptions;
        }

        /// <summary>
        /// Retrieves projected performance assumptions mapping from an Excel tab.
        /// </summary>
        private PerformanceAssumptionsMapping GetProjectedPerformanceAssumptionsMapping(
            bool convertAssumptionsToAnnual,
            bool convertAssumptionsToMonthly)
        {
            var performanceAssumptionsMappingRecords =
                _ExcelFileReader.GetDataFromSpecificTab<PerformanceAssumptionMappingRecord>(_performanceAssumptionsMapping);

            var performanceAssumptionsMappingConverter =
                new PerformanceAssumptionsMappingExcelConverter(convertAssumptionsToMonthly, convertAssumptionsToAnnual);

            var performanceAssumptionsMapping =
                performanceAssumptionsMappingConverter.ConvertToPeformanceAssumptionsMapping(performanceAssumptionsMappingRecords);

            return performanceAssumptionsMapping;
        }
    }
}
