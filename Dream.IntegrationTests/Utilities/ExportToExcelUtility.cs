using Dream.Core.Reporting.Excel;
using Dream.Core.Reporting.Results;
using Dream.IO.Excel;
using System.Collections.Generic;

namespace Dream.IntegrationTests.Utilities
{
    public class ExportToExcelUtility
    {
        public static void ExportRawSecuritizationResults(SecuritizationResult securitizationResult)
        {
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            excelFileWriter.AddWorksheetForListOfData(securitizationResult.AvailableFundsCashFlows, "Available Funds");

            var trancheDisplayResults = new List<SecuritizationCashFlowsSummaryResult>();
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;
            foreach (var trancheName in trancheResultsDictionary.Keys)
            {
                var trancheResult = trancheResultsDictionary[trancheName];
                excelFileWriter.AddWorksheetForListOfData(trancheResult.TrancheCashFlows, trancheName);

                if (trancheResult.TrancheName != null) trancheDisplayResults.Add(trancheResult);
            }

            excelFileWriter.AddWorksheetForListOfData(trancheDisplayResults, "Tranche Results");

            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;
            var listOfCashFlows = cashFlowDictionary["Total"].ProjectedCashFlows;
            excelFileWriter.AddWorksheetForListOfData(listOfCashFlows, "Total Collateral");

            foreach (var aggregationGroupingIdentifier in cashFlowDictionary.Keys)
            {
                if (aggregationGroupingIdentifier == "Total") continue;
                listOfCashFlows = cashFlowDictionary[aggregationGroupingIdentifier].ProjectedCashFlows;              
                excelFileWriter.AddWorksheetForListOfData(listOfCashFlows, aggregationGroupingIdentifier);
            }

            excelFileWriter.ExportWorkbook();
        }

        public static void ExportSecuritizationResults(SecuritizationResult securitizationResult)
        {
            var securitizationResultsDictionary = new Dictionary<string, SecuritizationResult> { [string.Empty] = securitizationResult };
            ExportSecuritizationResults(securitizationResultsDictionary);
        }

        public static void ExportSecuritizationResults(Dictionary<string, SecuritizationResult> securitizationResultsDictionary)
        {
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            SecuritizationCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);

            excelFileWriter.ExportWorkbook();
        }
    }
}
