using ClosedXML.Excel;
using Dream.Common;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Reporting.Excel
{
    public class CollateralSummaryExcelReport
    {
        private const string _reportTabName = "Collateral Summary";
        private const string _fontName = "Open Sans";
        private const string _scenarioDescription = "Scenario";

        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatNoDecimals = @"_(* #,##0_);_(* (#,##0);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatOneDecmial = @"_(* #,##0.0_);_(* (#,##0.0);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatThreeDecimals = @"_(* #,##0.000_);_(* (#,##0.000);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;
        private const int _blankRowsOffset = 2;

        private static int _dataRowOffsetCounter = 1;

        private static string _currentScenario;

        private static List<string> _headersRow = new List<string>
        {
            "Aggregation Group",
            "Bond Count",
            "Count",
            "Balance ($)",
            "Coupon (%)",
            "WAL (years)",
            "Life CPR (%)",
            "Total Cashflow ($)",
            "Total Interest ($)",
            "Total Principal ($)",
            "Total Prepayment ($)",
            "Total Prepayment Penalty ($)",
            "Total Default ($)",
            "Total Recovery ($)",
            "Total Loss ($)",
            "Severity (%)",
            "Total Prepayment (%)",
            "Total Default (%)",
            "Total Recovery (%)",
            "Total Loss (%)",
        };

        private static List<string> _headersRowWithPricing = new List<string>
        {
            "Aggregation Group",
            "Bond Count",
            "Count",       
            "Balance ($)",                 
            "Coupon (%)",                   
            "WAL (years)",              
            "Mac. Dur.",                
            "Mod. Dur.",      
            "DV01 ($)",
            "Life CPR (%)",
            "Benchmark Yield (%)",      
            "Nominal Spread (bps)",     
            "Yield (%)",                
            "Market Value ($)",         
            "Price (%)",
            "Total Cashflow ($)",
            "Total Interest ($)",
            "Total Principal ($)",
            "Total Prepayment ($)",
            "Total Prepayment Penalty ($)",
            "Total Default ($)",
            "Total Recovery ($)",
            "Total Loss ($)",
            "Severity (%)",
            "Total Prepayment (%)",
            "Total Default (%)",
            "Total Recovery (%)",
            "Total Loss (%)",
            "Day-Counting",             
            "Compounding",              
            "Nominal Spread Data",      
        };

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            // There is a baked-in assumption here that the scenario description will never be null along this code path
            _headersRow.Insert(0, _scenarioDescription);
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);

            foreach (var scenarioDescription in dictionaryOfSecuritizationResults.Keys)
            {
                _currentScenario = scenarioDescription;
                var securitizationResult = dictionaryOfSecuritizationResults[scenarioDescription];
                var collateralCashFlowsDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

                AddReportTab(excelWorksheet, collateralCashFlowsDictionary);
            }

            _currentScenario = null;
            _dataRowOffsetCounter = 1;
        }

        public static void AddReportTab(XLWorkbook excelWorkbook, SecuritizationResult securitizationResult)
        {
            var collateralCashFlowsDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);

            AddReportTab(excelWorksheet, collateralCashFlowsDictionary);
            _dataRowOffsetCounter = 1;
        }

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfResults)
        {
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);

            AddReportTab(excelWorksheet, dictionaryOfResults);
            _dataRowOffsetCounter = 1;
        }

        public static void AddReportTabWithPricing(XLWorkbook excelWorkbook, Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfResults)
        {
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);

            AddReportTabWithPricing(excelWorksheet, dictionaryOfResults);
            _dataRowOffsetCounter = 1;
        }

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, Dictionary<string, ProjectedCashFlowsSummaryResult>> dictionaryOfScenarioResults)
        {
            // There is a baked-in assumption here that the scenario description will never be null along this code path
            _headersRow.Insert(0, _scenarioDescription);
            _headersRowWithPricing.Insert(0, _scenarioDescription);

            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);

            foreach (var scenarioDescription in dictionaryOfScenarioResults.Keys)
            {
                _currentScenario = scenarioDescription;
                var dictionaryOfResults = dictionaryOfScenarioResults[scenarioDescription];

                AddReportTabWithPricing(excelWorksheet, dictionaryOfResults);
            }

            _currentScenario = null;
            _dataRowOffsetCounter = 1;
        }

        private static void AddReportTab(IXLWorksheet excelWorksheet, Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfResults)
        {
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
                var leftAlignedIndex = (_currentScenario == null) ? 0 : 1;
                if (headerIndex > leftAlignedIndex)
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }

            // Loop through all the aggregation groupings
            foreach (var keyValuePair in dictionaryOfResults.OrderBy(kvp => kvp.Key))
            {
                if (keyValuePair.Key == AggregationGroupings.TotalAggregationGroupName) continue;

                var collateralCashFlows = keyValuePair.Value.ProjectedCashFlows;
                var aggregationGroupIdentifier = keyValuePair.Key;

                AddReportLine(excelWorksheet, collateralCashFlows, aggregationGroupIdentifier, _dataRowOffsetCounter);
                _dataRowOffsetCounter++;
            }

            // Always pick out the "Total" grouping and display it last, if it is present
            if (dictionaryOfResults.ContainsKey(AggregationGroupings.TotalAggregationGroupName))
            {
                var collateralCashFlows = dictionaryOfResults[AggregationGroupings.TotalAggregationGroupName].ProjectedCashFlows;
                var aggregationGroupIdentifier = AggregationGroupings.TotalAggregationGroupName;

                AddReportLine(excelWorksheet, collateralCashFlows, aggregationGroupIdentifier, _dataRowOffsetCounter);
                excelWorksheet.Row(_blankRowsOffset + _dataRowOffsetCounter).Style.Font.Bold = true;
                _dataRowOffsetCounter++;
            }

            var scenarioOffset = (_currentScenario != null) ? 1 : 0;

            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Cells().Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Row(_blankRowsOffset).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Row(_blankRowsOffset).Height = 15;
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, _blankColumnsOffset + scenarioOffset);
            excelWorksheet.SetTabColor(XLColor.White);

            // Auto-fit and adjust column-widths
            excelWorksheet.Columns().AdjustToContents();
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }
        }

        private static void AddReportTabWithPricing(IXLWorksheet excelWorksheet, Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfResults)
        {
            // First, assemble the header rows.
            foreach (var headerIndex in Enumerable.Range(0, _headersRowWithPricing.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Value = _headersRowWithPricing[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // The very first headers always have a left alignment, instead of center
                var leftAlignedIndex = (_currentScenario == null) ? 0 : 1;
                if (headerIndex > leftAlignedIndex)
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else
                {
                    excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }

            // Loop through all the aggregation groupings
            foreach (var keyValuePair in dictionaryOfResults.OrderBy(kvp => kvp.Key))
            {
                if (keyValuePair.Key == AggregationGroupings.TotalAggregationGroupName) continue;

                var cashFlowsSummaryResult = keyValuePair.Value;
                var aggregationGroupIdentifier = keyValuePair.Key;

                AddReportLineWithPricing(excelWorksheet, cashFlowsSummaryResult, aggregationGroupIdentifier, _dataRowOffsetCounter);
                _dataRowOffsetCounter++;
            }

            // Always pick out the "Total" grouping and display it last, if it is present
            if (dictionaryOfResults.ContainsKey(AggregationGroupings.TotalAggregationGroupName))
            {
                var totalCashFlowsSummaryResult = dictionaryOfResults[AggregationGroupings.TotalAggregationGroupName];
                var aggregationGroupIdentifier = AggregationGroupings.TotalAggregationGroupName;

                AddReportLineWithPricing(excelWorksheet, totalCashFlowsSummaryResult, aggregationGroupIdentifier, _dataRowOffsetCounter);
                excelWorksheet.Row(_blankRowsOffset + _dataRowOffsetCounter).Style.Font.Bold = true;
                _dataRowOffsetCounter++;
            }

            var scenarioOffset = (_currentScenario != null) ? 1 : 0;

            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Cells().Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Row(_blankRowsOffset).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Row(_blankRowsOffset).Height = 15;
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, _blankColumnsOffset + scenarioOffset);
            excelWorksheet.SetTabColor(XLColor.White);

            // Auto-fit and adjust column-widths
            excelWorksheet.Columns().AdjustToContents();
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }
        }

        private static void AddReportLine(
            IXLWorksheet excelWorksheet,
            List<ProjectedCashFlow> collateralCashFlows,
            string aggregationGroupIdentifier,
            int dataRowOffsetCounter)
        {
            var dataColumnOffsetCounter = 0;
            var totalCollateralBalance = collateralCashFlows.First().StartingBalance;

            // Scenario
            if (_currentScenario != null)
            {
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = _currentScenario;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                dataColumnOffsetCounter++;
            }

            // Aggregation Group
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = aggregationGroupIdentifier;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            dataColumnOffsetCounter++;

            // Bond Count
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = collateralCashFlows.First().BondCount;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Count
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = collateralCashFlows.First().Count;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Balance ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalCollateralBalance;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Coupon (%)
            var forwardWeightedAverageCoupon = CashFlowMetrics.CalculateForwardWeightedAverageCoupon(collateralCashFlows);
            forwardWeightedAverageCoupon = (double.IsNaN(forwardWeightedAverageCoupon) || double.IsInfinity(forwardWeightedAverageCoupon)) ? 0.0 : Constants.OneHundredPercentagePoints * forwardWeightedAverageCoupon;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = forwardWeightedAverageCoupon;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // WAL (years)
            var weightedAverageLife = CashFlowMetrics.CalculateWeightedAverageLife(collateralCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = weightedAverageLife;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Life CPR (%)
            var lifetimeConstantPrepaymentRate = CashFlowMetrics.CalculateLifetimeConstantPrepaymentRate(collateralCashFlows);
            lifetimeConstantPrepaymentRate = (double.IsNaN(lifetimeConstantPrepaymentRate)) ? 0.0 : Constants.OneHundredPercentagePoints * lifetimeConstantPrepaymentRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = lifetimeConstantPrepaymentRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatOneDecmial;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Cashflow ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = collateralCashFlows.Sum(c => c.Payment);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Interest ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = collateralCashFlows.Sum(c => c.Interest);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Principal ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = collateralCashFlows.Sum(c => c.Principal);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Prepayment ($)
            var totalPrepayment = collateralCashFlows.Sum(c => c.Prepayment);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalPrepayment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Prepayment ($)
            var totalPrepaymentPenalty = collateralCashFlows.Sum(c => c.PrepaymentPenalty);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalPrepaymentPenalty;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Default ($)
            var totalDefault = collateralCashFlows.Sum(c => c.Default);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalDefault;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Recovery ($)
            var totalRecovery = collateralCashFlows.Sum(c => c.Recovery);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalRecovery;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Loss ($)
            var totalLoss = collateralCashFlows.Sum(c => c.Loss);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalLoss;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Severity (%)
            var severity = (totalDefault > 0.0) ? (totalLoss / totalDefault) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * severity;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Prepayment (%)
            var totalPrepaymentPercentage = (totalCollateralBalance > 0.0) ? Constants.OneHundredPercentagePoints * (totalPrepayment / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalPrepaymentPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Default (%)
            var totalDefaultPercentage = (totalCollateralBalance > 0.0) ? Constants.OneHundredPercentagePoints * (totalDefault / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalDefaultPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Recovery (%)
            var totalRecoveryPercentage = (totalCollateralBalance > 0.0) ? Constants.OneHundredPercentagePoints * (totalRecovery / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalRecoveryPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Loss (%)
            var totalLossPercentage = (totalCollateralBalance > 0.0) ? Constants.OneHundredPercentagePoints * (totalLoss / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalLossPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }

        private static void AddReportLineWithPricing(
            IXLWorksheet excelWorksheet,
            ProjectedCashFlowsSummaryResult cashFlowsSummaryResult,
            string aggregationGroupIdentifier,
            int dataRowOffsetCounter)
        {
            var dataColumnOffsetCounter = 0;
            var projectedCashFlows = cashFlowsSummaryResult.ProjectedCashFlows;
            var totalCollateralBalance = projectedCashFlows.First().StartingBalance;

            // Scenario
            if (_currentScenario != null)
            {
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = _currentScenario;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                dataColumnOffsetCounter++;
            }

            // Aggregation Group
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = aggregationGroupIdentifier;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            dataColumnOffsetCounter++;

            // Bond Count
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = projectedCashFlows.First().BondCount;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Count
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = projectedCashFlows.First().Count;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Balance ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalCollateralBalance;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Coupon (%)
            var forwardWeightedAverageCoupon = CashFlowMetrics.CalculateForwardWeightedAverageCoupon(projectedCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * forwardWeightedAverageCoupon;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // WAL (years)
            var weightedAverageLife = CashFlowMetrics.CalculateWeightedAverageLife(projectedCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = weightedAverageLife;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Mac. Dur.       
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.MacaulayDuration;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Mod. Dur.          
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.ModifiedDurationAnalytical;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // DV01 ($)          
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.DollarDuration;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Life CPR (%)
            var lifetimeConstantPrepaymentRate = CashFlowMetrics.CalculateLifetimeConstantPrepaymentRate(projectedCashFlows);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * lifetimeConstantPrepaymentRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatOneDecmial;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Benchmark Yield (%)         
            var nominalBenchmarkRate = (double.IsNaN(cashFlowsSummaryResult.NominalBenchmarkRate.GetValueOrDefault())) ? 0.0 : cashFlowsSummaryResult.NominalBenchmarkRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * nominalBenchmarkRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatThreeDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Nominal Spread (bps)
            var nominalSpread = (double.IsNaN(cashFlowsSummaryResult.NominalSpread.GetValueOrDefault())) ? 0.0 : cashFlowsSummaryResult.NominalSpread;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.BpsPerOneHundredPercentagePoints * nominalSpread;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Yield (%)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * cashFlowsSummaryResult.InternalRateOfReturn;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormatThreeDecimals;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Market Value ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.PresentValue;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Price (%)
            var dollarPrice = (double.IsInfinity(cashFlowsSummaryResult.DollarPrice.GetValueOrDefault())) ? 0.0 : cashFlowsSummaryResult.DollarPrice;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * dollarPrice;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Cashflow ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = projectedCashFlows.Sum(c => c.Payment);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Interest ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = projectedCashFlows.Sum(c => c.Interest);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Principal ($)
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = projectedCashFlows.Sum(c => c.Principal);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Prepayment ($)
            var totalPrepayment = projectedCashFlows.Sum(c => c.Prepayment);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalPrepayment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Prepayment ($)
            var totalPrepaymentPenalty = projectedCashFlows.Sum(c => c.PrepaymentPenalty);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalPrepaymentPenalty;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Default ($)
            var totalDefault = projectedCashFlows.Sum(c => c.Default);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalDefault;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Recovery ($)
            var totalRecovery = projectedCashFlows.Sum(c => c.Recovery);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalRecovery;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Loss ($)
            var totalLoss = projectedCashFlows.Sum(c => c.Loss);
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = totalLoss;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Severity (%)
            var severity = (totalDefault > 0.0) ? (totalLoss / totalDefault) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * severity;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Prepayment (%)
            var totalPrepaymentPercentage = (totalCollateralBalance > 0.0) ? (totalPrepayment / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * totalPrepaymentPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Default (%)
            var totalDefaultPercentage = (totalCollateralBalance > 0.0) ? (totalDefault / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * totalDefaultPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Recovery (%)
            var totalRecoveryPercentage = (totalCollateralBalance > 0.0) ? (totalRecovery / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * totalRecoveryPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Total Loss (%)
            var totalLossPercentage = (totalCollateralBalance > 0.0) ? (totalLoss / totalCollateralBalance) : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = Constants.OneHundredPercentagePoints * totalLossPercentage;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Day-Counting
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.DayCountConvention.GetFriendlyDescription();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Compounding
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.CompoundingConvention.GetFriendlyDescription();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Nominal Spread Data
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Value = cashFlowsSummaryResult.MarketDataUseForNominalSpread.GetFriendlyDescription();
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, dataColumnOffsetCounter + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }
    }
}
