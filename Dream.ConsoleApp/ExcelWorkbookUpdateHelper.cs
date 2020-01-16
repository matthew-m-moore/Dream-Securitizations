using System.Collections.Generic;
using System.Linq;
using Dream.Core.Reporting.Results;
using Dream.Core.Reporting.Excel;
using Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;

namespace Dream.ConsoleApp
{
    public class ExcelWorkbookUpdateHelper
    {
        public static void UpdateSecuritizationSummary(
            Workbook activeExcelWorkbook,
            XLWorkbook reportingExcelWorkbook,
            Dictionary<string, SecuritizationResult> securitizationResultsDictionary)
        {
            SecuritizationTranchesSummaryExcelReport.AddReportTab(reportingExcelWorkbook, securitizationResultsDictionary);
            var reportTabName = reportingExcelWorkbook.Worksheets.First().Name;

            UpdateExcelWorksheet(activeExcelWorkbook, reportingExcelWorkbook, reportTabName);
        }

        public static void UpdateSecuritizationWaterfall(
            string additionalYieldScenarioDescription,
            Workbook activeExcelWorkbook,
            XLWorkbook reportingExcelWorkbook,
            Dictionary<string, SecuritizationResult> securitizationResultsDictionary)
        {
            var additionalYieldScenarioResults = securitizationResultsDictionary[additionalYieldScenarioDescription];
            SecuritizationCashFlowsExcelReport.AddReportTab(reportingExcelWorkbook, additionalYieldScenarioResults);
            var reportTabName = reportingExcelWorkbook.Worksheets.Last().Name;

            UpdateExcelWorksheet(activeExcelWorkbook, reportingExcelWorkbook, reportTabName);
        }

        private static void UpdateExcelWorksheet(Workbook activeExcelWorkbook, XLWorkbook reportingExcelWorkbook, string reportTabName)
        {
            var excelApplication = activeExcelWorkbook.Application;

            if (!CheckThatSheetExists(excelApplication.ActiveWorkbook.Sheets, reportTabName)) return;
            var worksheetToUpdate = excelApplication.ActiveWorkbook.Sheets[reportTabName];
            worksheetToUpdate = worksheetToUpdate as Worksheet;
            worksheetToUpdate.UsedRange.Cells.ClearContents();

            var worksheetToCopy = reportingExcelWorkbook.Worksheet(reportTabName);
            var firstCellPopulated = worksheetToCopy.FirstCellUsed();
            var lastCellPopulated = worksheetToCopy.LastCellUsed();
            var rangeUsed = worksheetToCopy.Range(firstCellPopulated.Address, lastCellPopulated.Address);

            var rangeToUpdate = worksheetToUpdate.Range(firstCellPopulated.Address.ToString(), lastCellPopulated.Address.ToString());

            // Creating an object array up front makes the update process go much faster
            var objectArray = new object[rangeUsed.RowCount(), rangeUsed.ColumnCount()];
            for (var row = firstCellPopulated.Address.RowNumber; row <= lastCellPopulated.Address.RowNumber; row++)
            {
                for (var column = firstCellPopulated.Address.ColumnNumber; column <= lastCellPopulated.Address.ColumnNumber; column++)
                {
                    var relativeRow = row - firstCellPopulated.Address.RowNumber;
                    var relativeColumn = column - firstCellPopulated.Address.ColumnNumber;

                    objectArray[relativeRow, relativeColumn] = worksheetToCopy.Cell(row, column).Value;
                }
            }

            rangeToUpdate.Value = objectArray;
        }

        private static bool CheckThatSheetExists(Sheets excelWorksheets, string reportTabName)
        {
            foreach (var excelWorksheet in excelWorksheets)
            {
                var worksheetToCheck = excelWorksheet as Worksheet;
                if (worksheetToCheck.Name == reportTabName) return true;
            }

            return false;
        }
    }
}
