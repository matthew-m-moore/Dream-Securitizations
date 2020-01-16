using ClosedXML.Excel;
using Dream.Common;
using Dream.Common.ExtensionMethods;
using Dream.Core.Reporting.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Reporting.Excel
{
    public class SecuritizationNodesPriceYieldTablesExcelReport
    {
        private const string _reportTabName = "Node Price-Yield Tables";
        private const string _fontName = "Open Sans";

        private const string _priceHeader = "Price (%)";
        private const string _marketValueHeader = "MV ($)";
        private const string _pricedToCall = "Call";

        public const char enDash = (char)0x2013;

        private const string _weightedAverageLifeHeader = "WAL (years)";
        private const string _modifiedDurationHeader = "Mid. Mod. Dur.";
        private const string _principalWindowHeader = "Prin. Window";
        private const string _dollarDurationHeader = "Mid. DV01 ($)";

        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatNoDecimals = @"_(* #,##0_);_(* (#,##0);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;

        private static int _blankRowsOffset = 4;

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            // Assumes that only the concatenated scenaro keys are of interest for the tables
            var dictionaryOfTableResults = dictionaryOfSecuritizationResults
                .Where(entry => entry.Key.Contains('|'))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (dictionaryOfTableResults.Keys.Any(key => key.Count(c => c == '|') > 1))
            {
                throw new Exception("ERROR: The pipe character is used separate scenario descriptions. " +
                    "Either there are pipes used in the scenario descriptions, or " +
                    "there are more layers of nested scenarios than can be supported in a two-dimentional table representation.");
            }

            var initialBlankRowsOffset = _blankRowsOffset;
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);
            var listOfColumnHeadersInPriceYieldTables = dictionaryOfTableResults.Keys
                .Select(key => key.Split('|').First())
                .Distinct().ToList();

            var nodesForPriceYieldTables = dictionaryOfTableResults.Values
                .SelectMany(r => r.SecuritizationResultsDictionary.Values
                    .Where(t => string.IsNullOrEmpty(t.TrancheName) && 
                               !string.IsNullOrEmpty(t.SecuritizationNodeName) && 
                                Math.Abs(t.PresentValue) > 0.0))
                .Select(x => x.SecuritizationNodeName).Distinct().ToList();

            if (listOfColumnHeadersInPriceYieldTables.Any(h => h.Contains(_pricedToCall)))
            {
                var columnsNotPricedToCall = listOfColumnHeadersInPriceYieldTables.Where(h => !h.Contains(_pricedToCall)).ToList();
                AddPriceYieldTables(excelWorksheet, nodesForPriceYieldTables, columnsNotPricedToCall, dictionaryOfTableResults);

                var distinctCallPercentages = listOfColumnHeadersInPriceYieldTables.Where(h => h.Contains(_pricedToCall))
                    .Select(c => c.Split('-').Last()).Distinct().ToList();

                foreach (var distinctCallPercentage in distinctCallPercentages)
                {
                    var columnsPricedToCall = listOfColumnHeadersInPriceYieldTables.Where(h => h.Contains(distinctCallPercentage)).ToList();
                    AddPriceYieldTables(excelWorksheet, nodesForPriceYieldTables, columnsPricedToCall, dictionaryOfTableResults);
                }
            }
            else
            {
                AddPriceYieldTables(excelWorksheet, nodesForPriceYieldTables, listOfColumnHeadersInPriceYieldTables, dictionaryOfTableResults);
            }

            // Auto-fit and adjust column-widths
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.SetTabColor(XLColor.White);
            excelWorksheet.Column(_blankColumnsOffset).Width = 11;
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }

            _blankRowsOffset = initialBlankRowsOffset;
        }

        private static void AddPriceYieldTables(
            IXLWorksheet excelWorksheet,
            List<string> nodesForPriceYieldTables,
            List<string> listOfColumnHeadersInPriceYieldTables,
            Dictionary<string, SecuritizationResult> dictionaryOfTableResults)
        {
            foreach (var securitizationNodeName in nodesForPriceYieldTables)
            {
                AddPriceYieldTable(
                    excelWorksheet,
                    securitizationNodeName,
                    listOfColumnHeadersInPriceYieldTables,
                    dictionaryOfTableResults);
            }
        }

        private static void AddPriceYieldTable(
            IXLWorksheet excelWorksheet,
            string securitizationNodeName,
            List<string> listOfColumnHeadersInPriceYieldTables,
            Dictionary<string, SecuritizationResult> dictionaryOfTableResults)
        {
            // Title
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Value = securitizationNodeName;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Font.Bold = true;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize + 3;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            var dataRowOffsetCounter = 0;

            foreach (var columnIndex in Enumerable.Range(0, listOfColumnHeadersInPriceYieldTables.Count))
            {
                // Scenario Column Headers
                var columnHeader = listOfColumnHeadersInPriceYieldTables[columnIndex];
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Value = listOfColumnHeadersInPriceYieldTables[columnIndex];
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                excelWorksheet.Cell(_blankRowsOffset, columnIndex + _blankColumnsOffset + 1).Style.Alignment.WrapText = true;

                dataRowOffsetCounter = 1;

                var securitizationNodeResult = new SecuritizationCashFlowsSummaryResult(null);
                var modifiedDurationList = new List<double>();
                var dollarDurationList = new List<double>();

                foreach (var scenarioResultKeyValuePair in dictionaryOfTableResults.Where(kvp => kvp.Key.Split('|').First() == columnHeader))
                {
                    var scenarioResult = scenarioResultKeyValuePair.Value;
                    securitizationNodeResult = scenarioResult.SecuritizationResultsDictionary[securitizationNodeName];

                    modifiedDurationList.Add(securitizationNodeResult.ModifiedDurationAnalytical);
                    dollarDurationList.Add(securitizationNodeResult.DollarDuration);

                    // Leading Header (i.e. Price or MV)
                    if (excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).IsEmpty())
                    {
                        if (securitizationNodeResult.Balance > 0.0)
                        {
                            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Value = _priceHeader;
                        }
                        else
                        {
                            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Value = _marketValueHeader;
                        }

                        excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Font.Bold = true;
                        excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                        excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    // Leading Header Value
                    if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).IsEmpty())
                    {
                        if (securitizationNodeResult.Balance > 0.0)
                        {
                            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * securitizationNodeResult.DollarPrice;
                        }
                        else
                        {
                            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Value = securitizationNodeResult.PresentValue;
                        }

                        excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.Font.Bold = true;
                        excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                        excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                        excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    // Yield Value
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, columnIndex + _blankColumnsOffset + 1).Value = Constants.OneHundredPercentagePoints * securitizationNodeResult.InternalRateOfReturn;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, columnIndex + _blankColumnsOffset + 1).Style.Font.FontSize = _baseFontSize + 1;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, columnIndex + _blankColumnsOffset + 1).Style.NumberFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    dataRowOffsetCounter++;
                }

                // WAL (years) Header
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, _blankColumnsOffset).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, _blankColumnsOffset).Value = _weightedAverageLifeHeader;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, _blankColumnsOffset).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                // WAL (years)
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, columnIndex + _blankColumnsOffset + 1).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, columnIndex + _blankColumnsOffset + 1).Value = securitizationNodeResult.WeightedAverageLife;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, columnIndex + _blankColumnsOffset + 1).Style.Font.FontSize = _baseFontSize + 1;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, columnIndex + _blankColumnsOffset + 1).Style.NumberFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 1, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                // Avg. Mod. Dur Header
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, _blankColumnsOffset).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, _blankColumnsOffset).Value = _modifiedDurationHeader;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, _blankColumnsOffset).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                // Avg. Mod. Dur
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, columnIndex + _blankColumnsOffset + 1).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, columnIndex + _blankColumnsOffset + 1).Value = modifiedDurationList.Middle();
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, columnIndex + _blankColumnsOffset + 1).Style.Font.FontSize = _baseFontSize + 1;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, columnIndex + _blankColumnsOffset + 1).Style.NumberFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 2, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                // Prin. Window Header
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, _blankColumnsOffset).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, _blankColumnsOffset).Value = _principalWindowHeader;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, _blankColumnsOffset).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                // Prin. Window
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, columnIndex + _blankColumnsOffset + 1).IsEmpty())
                {
                    var paymentWindowString = "'" + securitizationNodeResult.PaymentCorridor.ToString();
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, columnIndex + _blankColumnsOffset + 1).Value = paymentWindowString;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, columnIndex + _blankColumnsOffset + 1).Style.Font.FontSize = _baseFontSize + 1;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 3, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                // Avg. DV01 ($) Header
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, _blankColumnsOffset).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, _blankColumnsOffset).Value = _dollarDurationHeader;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, _blankColumnsOffset).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                // Avg. DV01 ($)
                if (excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, columnIndex + _blankColumnsOffset + 1).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, columnIndex + _blankColumnsOffset + 1).Value = dollarDurationList.Middle();
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, columnIndex + _blankColumnsOffset + 1).Style.Font.FontSize = _baseFontSize + 1;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, columnIndex + _blankColumnsOffset + 1).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter + 4, columnIndex + _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
            }

            dataRowOffsetCounter += 8;
            _blankRowsOffset += dataRowOffsetCounter;
        }
    }
}
