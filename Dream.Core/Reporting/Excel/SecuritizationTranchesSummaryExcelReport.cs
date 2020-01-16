using ClosedXML.Excel;
using Dream.Common;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Reporting.Excel
{
    public class SecuritizationTranchesSummaryExcelReport
    {
        private const string _reportTabName = "Securitization Summary";
        private const string _fontName = "Open Sans";
        private const string _scenarioDescription = "Scenario";

        public const char enDash = (char) 0x2013;

        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatNoDecimals = @"_(* #,##0_);_(* (#,##0);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatThreeDecimals = @"_(* #,##0.000_);_(* (#,##0.000);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;
        private const int _blankRowsOffset = 2;

        private static int _dataRowOffsetCounter = 1;

        private static List<string> _headersRow = new List<string>
        {
            "Node",
            "Tranche Name",             
            "Size ($)",                 
            "Size (%)",                 
            "Coupon (%)",                
            "WAL (years)",             
            "Mac. Dur.",                
            "Mod. Dur.",                
            "DV01 ($)",
            "Prin. Window",             
            "Benchmark Yield (%)",     
            "Nominal Spread (bps)",     
            "Yield (%)",                
            "Market Value ($)",         
            "MV (% Face)",              
            "MV (% Collat)",            
            "Total Cashflow ($)",      
            "Total Principal ($)",      
            "Total Interest ($)",
            "Total Prin. Short-Fall ($)",
            "Has Int. Short-Fall?",
            "Day-Counting",             
            "Compounding",              
            "Nominal Spread Data",      
        };

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            // There is a baked-in assumption here that the scenario description will never be null along this code path
            if (!_headersRow.Contains(_scenarioDescription)) _headersRow.Insert(0, _scenarioDescription);

            IXLWorksheet excelWorksheet;
            if (excelWorkbook.Worksheets.Any(w => w.Name == _reportTabName))
            {
                excelWorksheet = excelWorkbook.Worksheet(_reportTabName);
            }
            else
            {
                excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);
            }

            var initialDataRowOffsetCounter = _dataRowOffsetCounter;

            foreach (var scenarioDescription in dictionaryOfSecuritizationResults.Keys)
            {
                var securitizationResult = dictionaryOfSecuritizationResults[scenarioDescription];
                AddReportTab(excelWorksheet, securitizationResult);
            }

            _dataRowOffsetCounter = initialDataRowOffsetCounter;
        }

        public static void AddReportTab(XLWorkbook excelWorkbook, SecuritizationResult securitizationResult)
        {
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);
            var initialDataRowOffsetCounter = _dataRowOffsetCounter;

            AddReportTab(excelWorksheet, securitizationResult);
            _dataRowOffsetCounter = initialDataRowOffsetCounter;
        }

        private static void AddReportTab(IXLWorksheet excelWorksheet, SecuritizationResult securitizationResults)
        {
            var securitizationResultsDictionary = securitizationResults.SecuritizationResultsDictionary;
            var securitizationDisplayResults = new List<SecuritizationCashFlowsSummaryResult>();
            foreach (var trancheOrSecuritizationNodeName in securitizationResultsDictionary.Keys)
            {
                var securitizationResult = securitizationResultsDictionary[trancheOrSecuritizationNodeName];
                if (!string.IsNullOrEmpty(securitizationResult.TrancheName))
                {
                    securitizationDisplayResults.Add(securitizationResult);
                }
            }

            // First, assemble the header rows.
            foreach (var headerIndex in Enumerable.Range(0, _headersRow.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Value = _headersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // The very first headers always have a left alignment, instead of center
                var leftAlignedIndex = (securitizationResults.ScenarioDescription == null) ? 0 : 1;
                if (headerIndex > leftAlignedIndex)
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }

            // Now, keep track of the number of rows below the headers to proceed;
            var dataColumnOffsetCounter = 0;
            var totalCollateralBalance = securitizationResults.CollateralCashFlowsResultsDictionary[AggregationGroupings.TotalAggregationGroupName].ProjectedCashFlows.First().StartingBalance;
            
            // Second, grab the data, element-by-element
            foreach (var trancheDisplayResult in securitizationDisplayResults)
            {
                // Scenario
                if (securitizationResults.ScenarioDescription != null)
                {
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = securitizationResults.ScenarioDescription;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    dataColumnOffsetCounter++;
                }

                // Node
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.SecuritizationNodeName;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                dataColumnOffsetCounter++;

                // Tranche Name
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.TrancheName;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                dataColumnOffsetCounter++;

                // Size ($)
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.Balance;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Size (%)
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * (trancheDisplayResult.Balance / totalCollateralBalance);
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Coupon
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * trancheDisplayResult.ForwardWeightedAverageCoupon;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // WAL (years)
                var weightedAverageLife = (double.IsNaN(trancheDisplayResult.WeightedAverageLife)) ? 0.0 : trancheDisplayResult.WeightedAverageLife;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = weightedAverageLife;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Mac. Dur.
                var macaulayDuration = (double.IsNaN(trancheDisplayResult.MacaulayDuration)) ? 0.0 : trancheDisplayResult.MacaulayDuration;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = macaulayDuration;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Mod. Dur.
                var modifiedDuration = (double.IsNaN(trancheDisplayResult.ModifiedDurationAnalytical)) ? 0.0 : trancheDisplayResult.ModifiedDurationAnalytical;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = modifiedDuration;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // DV01 ($)
                var dollarDuration = (double.IsNaN(trancheDisplayResult.DollarDuration)) ? 0.0 : trancheDisplayResult.DollarDuration;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = dollarDuration;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prin. Window
                var paymentWindowString = "'" + trancheDisplayResult.PaymentCorridor.ToString();
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = paymentWindowString;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataColumnOffsetCounter++;

                // Benchmark Yield (%)
                var benchmarkYield = (double.IsNaN(trancheDisplayResult.NominalBenchmarkRate.GetValueOrDefault())) ? 0.0 : Constants.OneHundredPercentagePoints * trancheDisplayResult.NominalBenchmarkRate;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = benchmarkYield;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatThreeDecimals;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Nominal Spread (bps)
                var nominalSpread = (double.IsNaN(trancheDisplayResult.NominalSpread.GetValueOrDefault())) ? 0.0 : Constants.BpsPerOneHundredPercentagePoints * trancheDisplayResult.NominalSpread;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = nominalSpread;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Yield (%)
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * trancheDisplayResult.InternalRateOfReturn;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatThreeDecimals;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Market Value ($)
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.PresentValue;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // MV (% Face)
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * trancheDisplayResult.DollarPrice;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // MV (% Collat)
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * (trancheDisplayResult.PresentValue / totalCollateralBalance);
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Total Cashflow
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.TotalCashFlow;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Total Principal
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.TotalPrincipal;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Total Interest
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.TotalInterest;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Total Prin. Short-Fall
                var principalShortFall = trancheDisplayResult.Balance.GetValueOrDefault() - trancheDisplayResult.TotalPrincipal;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = principalShortFall;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Has Int. Short-Fall?
                var interestShortfallRecordString = (trancheDisplayResult.InterestShortfallRecord == null) ? string.Empty : trancheDisplayResult.InterestShortfallRecord.ToString();
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = interestShortfallRecordString;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataColumnOffsetCounter++;

                // Day-Counting
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.DayCountConvention.GetFriendlyDescription();
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Compounding
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.CompoundingConvention.GetFriendlyDescription();
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Nominal Spread Data
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = trancheDisplayResult.MarketDataUseForNominalSpread.GetFriendlyDescription();
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                dataColumnOffsetCounter = 0;
                _dataRowOffsetCounter++;
            }

            var scenarioOffset = (securitizationResults.ScenarioDescription != null) ? 1 : 0;

            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Cells().Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Row(_blankRowsOffset).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Row(_blankRowsOffset).Height = 15;
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, _blankColumnsOffset + scenarioOffset + 1);
            excelWorksheet.SetTabColor(XLColor.White);

            // Auto-fit and adjust column-widths
            excelWorksheet.Columns().AdjustToContents();
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }

            // TO DO: When available, hide the gridlines and set the zoom scale to 85%
        }
    }
}
