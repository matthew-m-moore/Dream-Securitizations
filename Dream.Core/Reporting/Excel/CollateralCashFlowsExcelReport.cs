using ClosedXML.Excel;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Reporting.Excel
{
    public class CollateralCashFlowsExcelReport
    {
        public static string ReportTabName = "Collat Cash Flows";      
        
        private const string _fontName = "Open Sans";

        private static string _byGroupingText = "By Grp";

        private const string _shortDateFormat = "m/d/yyyy";
        private const string _noDecimalsNumberFormat = "#,##0";
        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";
        private const string _noDecimalsAccountingNumberFormat = @"_(* #,##0_);_(* (#,##0);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;

        private static int _blankRowsOffset = 2;

        private static List<string> _initialHeadersRow = new List<string>
        {
            "Period",
        };

        private static List<string> _headersRow = new List<string>
        {
            "Period Date",
            "Bond Count",
            "Count",
            "Starting Balance",
            "Ending Balance",
            "Cash Flow",
            "Principal",
            "Prepayment",
            "Prepayment Penalty",
            "Delinquent Principal",
            "Default",
            "Recovery",
            "Loss",
            "Interest",
            "Prepaid Interest",
            "Interest Default",
            "Interest Recovery",
            "Interest Loss",
            "Accrued Interest",
            "Coupon",
        };

        public static void AddReportTabs(XLWorkbook excelWorkbook, Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            var scenarioCounter = 1;
            foreach (var scenarioDescription in dictionaryOfSecuritizationResults.Keys)
            {
                var originalByGroupingText = _byGroupingText;
                var securitizationResult = dictionaryOfSecuritizationResults[scenarioDescription];
                AddReportTabs(excelWorkbook, securitizationResult, scenarioCounter);

                _byGroupingText = originalByGroupingText;
                scenarioCounter++;
            }
        }

        public static void AddReportTabs(XLWorkbook excelWorkbook, SecuritizationResult securitizationResult, int? scenarioCounter = null)
        {
            var dictionaryOfResults = securitizationResult.CollateralCashFlowsResultsDictionary;
            var scenarioDescription = securitizationResult.ScenarioDescription;

            AddReportTabs(excelWorkbook, dictionaryOfResults, scenarioCounter, scenarioDescription);       
        }

        public static void AddReportTabs(
            XLWorkbook excelWorkbook, 
            Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfResults, 
            int? scenarioCounter = null,
            string scenarioDescription = null)
        {
            // Always pick out the "Total" grouping and display it first, if it is present
            var blockNumberOffset = 0;
            if (dictionaryOfResults.ContainsKey(AggregationGroupings.TotalAggregationGroupName))
            {              
                var collateralCashFlowsTotal = dictionaryOfResults[AggregationGroupings.TotalAggregationGroupName].ProjectedCashFlows;
                var aggregationGroupIdentifier = AggregationGroupings.TotalAggregationGroupName;
                if (scenarioCounter != null) aggregationGroupIdentifier += (", " + scenarioCounter); 

                var excelWorksheetTotal = excelWorkbook.Worksheets.Add(ReportTabName + " (" + aggregationGroupIdentifier + ")");

                var tabDataHeading = (scenarioDescription == null || scenarioDescription == string.Empty)
                    ? AggregationGroupings.TotalAggregationGroupName
                    : AggregationGroupings.TotalAggregationGroupName + " - " + scenarioDescription;

                PopulateReportTab(excelWorksheetTotal, collateralCashFlowsTotal, tabDataHeading, ref blockNumberOffset);
                SetSheetFormatting(excelWorksheetTotal, collateralCashFlowsTotal.Count);
                blockNumberOffset = 0;
            }

            // Loop through the rest of the collateral cash flows dictionary and place it in blocks on one tab     
            if (scenarioCounter != null) _byGroupingText += (", " + scenarioCounter);
            var excelWorksheet = excelWorkbook.Worksheets.Add(ReportTabName + " (" + _byGroupingText + ")");

            foreach (var keyValuePair in dictionaryOfResults.OrderBy(kvp => kvp.Key))
            {
                if (keyValuePair.Key == AggregationGroupings.TotalAggregationGroupName) continue;

                var collateralCashFlows = keyValuePair.Value.ProjectedCashFlows;
                var aggregationGroupIdentifier = keyValuePair.Key;

                var tabDataHeading = (scenarioDescription == null || scenarioDescription == string.Empty)
                    ? aggregationGroupIdentifier
                    : aggregationGroupIdentifier + " - " + scenarioDescription;

                PopulateReportTab(excelWorksheet, collateralCashFlows, tabDataHeading, ref blockNumberOffset);
            }

            var maxCashFlowCount = dictionaryOfResults.Values.Max(r => r.ProjectedCashFlows.Count);
            SetSheetFormatting(excelWorksheet, maxCashFlowCount);
        }

        private static void PopulateReportTab(
            IXLWorksheet excelWorksheet, 
            List<ProjectedCashFlow> collateralCashFlows, 
            string aggregationGroupIdentifier, 
            ref int blockNumberOffset)
        {            
            var initialBlankRowsOffsetValue = _blankRowsOffset;

            if (blockNumberOffset == 0)
            {
                // Set up the initial headers
                foreach (var headerIndex in Enumerable.Range(0, _initialHeadersRow.Count))
                {
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Value = _initialHeadersRow[headerIndex];
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.TopBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.BottomBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset + 1, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                blockNumberOffset += (_initialHeadersRow.Count + 1);
            }

            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + blockNumberOffset).Value = aggregationGroupIdentifier;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + blockNumberOffset).Style.Font.Bold = true;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + blockNumberOffset).Style.Font.FontSize = _baseFontSize + 4;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            _blankRowsOffset += 1;

            // Set up the headers
            foreach (var headerIndex in Enumerable.Range(0, _headersRow.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Value = _headersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            var dataRowOffsetCounter = 1;
            var dataColumnOffsetCounter = 0;

            foreach (var collateralCashFlow in collateralCashFlows)
            {
                // Period
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = collateralCashFlow.Period;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.NumberFormat.Format = _noDecimalsNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Period Date
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.PeriodDate;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _shortDateFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataColumnOffsetCounter++;

                // Bond Count
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.BondCount;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.NumberFormat.Format = _noDecimalsAccountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Count
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Count;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.NumberFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Starting Balance
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.StartingBalance;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Ending Balance
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.EndingBalance;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Cash Flow
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Payment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Principal
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Principal;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prepayment
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Prepayment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prepayment Penalty
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.PrepaymentPenalty;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Delinquent Principal
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.DelinquentPrincipal;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Default
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Default;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Recovery
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Recovery;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Loss
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Loss;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Interest
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Interest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prepaid Interest
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.PrepaymentInterest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Interest Default
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.InterestDefault;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Interest Recovery
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.InterestRecovery;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Interest Loss
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.InterestLoss;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Accrued Interest
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.AccruedInterest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Coupon
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Value = collateralCashFlow.Coupon;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter + blockNumberOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                dataColumnOffsetCounter = 0;
                dataRowOffsetCounter++;
            }

            // Reset blank rows offset
            blockNumberOffset += (_headersRow.Count + 1);
            _blankRowsOffset = initialBlankRowsOffsetValue;
        }

        private static void SetSheetFormatting(IXLWorksheet excelWorksheet, int cashFlowCount)
        {
            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Rows(_blankRowsOffset + 1, _blankRowsOffset + cashFlowCount + 1).Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Rows(_blankRowsOffset - 1, _blankRowsOffset + cashFlowCount + 1).Height = 14.5;

            // Auto-fit and adjust column-widths
            excelWorksheet.Columns().AdjustToContents(_blankRowsOffset + 1, _blankRowsOffset + cashFlowCount);
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }

            excelWorksheet.Column(_blankColumnsOffset + _initialHeadersRow.Count).Width = 2;

            // Freeze panes and set tab color
            excelWorksheet.SheetView.Freeze(_blankRowsOffset + 1, _blankColumnsOffset + 1);
            excelWorksheet.SetTabColor(XLColor.White);
        }
    }
}
