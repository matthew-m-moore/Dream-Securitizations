using Dream.Common;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Converters.Excel.Scenarios;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dream.Core.Repositories.Excel
{
    public class ScenarioAnalysisExcelDataRepository : ExcelDataRepository
    {
        private const string _tabName = "Price-Yield Tables";

        private const string _scenarios = "Scenarios";

        private const string _firstScenario = "1";
        private const string _trancheNames = "TrancheName";
        private const string _callOrMaturity = "CallOrMaturity";
        private const string _callPercentage = "CallPercentage";
        private const string _pricingMethod = "PricingMethod";

        private const string _priceToCall = "Call";
        private const string _replacement = "Replace Value";

        public ScenarioAnalysisExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public ScenarioAnalysisExcelDataRepository(Stream fileStream) : base(fileStream) { }

        public List<ScenarioAnalysis> GetPriceYieldTableScenarios(CashFlowGenerationInput baseInputs)
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_tabName);
            var inputIdentifiers = excelDataRows.Select((row, i) => new KeyValuePair<string, int>(row.FirstCell().GetValue<string>(), i))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Behold the magical power of LINQ
            var groupingIdentifiers = excelDataRows.Skip(inputIdentifiers[_trancheNames]).First().Cells().Skip(1)
                .Select(c => c.GetValue<string>()).Where(s => s != _scenarios).ToList();
            var numberOfGroupings = groupingIdentifiers.Count;

            // Scenario inputs
            var pricingMethods = excelDataRows.Skip(inputIdentifiers[_pricingMethod]).First().Cells().Skip(1).Take(numberOfGroupings)
                .Select(c => c.GetValue<string>()).ToList();

            // Need to keep these default lists for backward compatibility
            var priceToCall = new List<bool>(Enumerable.Repeat(default(bool), numberOfGroupings));
            if (inputIdentifiers.ContainsKey(_callOrMaturity))
            {
                priceToCall = excelDataRows.Skip(inputIdentifiers[_callOrMaturity]).First().Cells().Skip(1).Take(numberOfGroupings)
                    .Select(c => c.GetValue<string>() == _priceToCall).ToList();
            }

            var callPercentages = new List<double>(Enumerable.Repeat(default(double), numberOfGroupings));
            if (inputIdentifiers.ContainsKey(_callPercentage))
            {
                callPercentages = excelDataRows.Skip(inputIdentifiers[_callPercentage]).First().Cells().Skip(1).Take(numberOfGroupings)
                    .Select(c => !string.IsNullOrEmpty(c.GetValue<string>()) ? double.Parse(c.GetValue<string>()) : 0.0).ToList();
            }

            // Note, need to capture two pieces of logic here
            var listOfPrepaymentShocks = new List<double>();
            var listOfSecuritizationPricingScenarioRecords = new List<PricingScenarioRecord>();

            foreach (var excelDataRow in excelDataRows.Skip(inputIdentifiers[_firstScenario]))
            {
                // First, grab the prepayment shock, if any is given in this row
                if (!excelDataRow.LastCell().IsEmpty())
                {
                    var prepaymentShock = excelDataRow.LastCell().GetValue<double>();
                    listOfPrepaymentShocks.Add(prepaymentShock);
                }

                var scenarioNumber = excelDataRow.FirstCell().GetValue<int>();
                for (var cellCounter = 1; cellCounter <= numberOfGroupings; cellCounter++)
                {
                    // Skip adding a scenario if the cell is empty
                    if (!excelDataRow.Cell(cellCounter + 1).IsEmpty())
                    {
                        var groupingIdentifier = groupingIdentifiers[cellCounter - 1];
                        var pricingMethod = pricingMethods[cellCounter - 1];
                        var pricedToCall = priceToCall[cellCounter - 1];
                        var callPercentage = callPercentages[cellCounter - 1];
                        var pricingValue = excelDataRow.Cell(cellCounter + 1).GetValue<double>();

                        if (pricingMethod == Constants.PercentOfBalanceBasedPricing) pricingValue /= Constants.OneHundredPercentagePoints;

                        var securitizationPricingScenarioRecord = new PricingScenarioRecord
                        {
                            ScenarioNumber = scenarioNumber,
                            ScenarioStrategy = _replacement,
                            GroupingIdentifier = groupingIdentifier,
                            PriceToCall = pricedToCall,
                            CleanUpCallPercentage = callPercentage,
                            PricingMethodology = pricingMethod,
                            ScenarioValue = pricingValue
                        };

                        listOfSecuritizationPricingScenarioRecords.Add(securitizationPricingScenarioRecord);
                    }
                }
            }

            var scenarioAnalysisConverter = new ScenarioAnalysisExcelConverter(baseInputs);
            scenarioAnalysisConverter.AddPricingScenarioRecords(listOfSecuritizationPricingScenarioRecords);

            var distinctCallPercentages = listOfSecuritizationPricingScenarioRecords.Select(r => r.CleanUpCallPercentage).Distinct().ToList();
            var scenariosToAnalyze = scenarioAnalysisConverter.GetPrepaymentShockScenarioAnalyses(distinctCallPercentages, listOfPrepaymentShocks, _replacement);
            return scenariosToAnalyze;
        }
    }
}
