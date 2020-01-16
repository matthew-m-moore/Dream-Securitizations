using ClosedXML.Excel;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.Reporting.Excel
{
    public class SecuritizationCashFlowsExcelReport
    {
        private static string _reportTabName = "Securitization Waterfall";

        private const string _columnTotals = "Column Totals:";
        private const string _fontName = "Open Sans";

        private const string _shortDateFormat = "m/d/yyyy";
        private const string _noDecimalsNumberFormat = "#,##0";
        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;

        private static int _blankRowsOffset = 2;

        // These are column width of 10
        private static List<string> _initialColumnHeadersRow = new List<string>
        {
            "Period",
            "Payment Date",
        };

        // These have column width of 14
        private static List<string> _availableFundsHeadersRow = new List<string>
        {

            "Total Principal",
            "Total Interest",          
            "Total Cash Flow",
            "Principal Balance",
            "Scheduled Principal",
            "Scheduled Interest",
            "Prepayment Interest",
            "Prepayments",
            "Prepayment Penalties",
            "Recoveries",
            "Collections Account Balance",
        };

        // Fees have a column width of 10.5
        // Fees always have a column header equal to the fee's name

        // These have column width of 14
        private static List<string> _interestPayingTrancheHeadersRow = new List<string>
        {
            "Principal",
            "Interest",
            "Cash Flow",
            "Balance",
            "Prin. Short-Fall",
            "Int. Short-Fall",
            "Acc. Int. on Short-Fall"
        };

        // These have column width of 13
        private static List<string> _reserveAccountHeadersRow = new List<string>
        {
            "Release",
            "Contribution",
            "Balance"
        };

        // Resdiual has a column width of 14
        // Note that each block will be separated by an empty column of width equal to 2

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            var scenarioCounter = 1;
            foreach (var scenarioDescription in dictionaryOfSecuritizationResults.Keys)
            {
                var originalReportTabName = _reportTabName;
                var securitizationResult = dictionaryOfSecuritizationResults[scenarioDescription];
                AddReportTab(excelWorkbook, securitizationResult, scenarioCounter);

                _reportTabName = originalReportTabName;
                scenarioCounter++;
            }
        }

        public static void AddReportTab(XLWorkbook excelWorkbook, SecuritizationResult securitizationResult, int? scenarioCounter = null)
        {
            if (scenarioCounter != null) _reportTabName += (" (" + scenarioCounter + ")");

            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);
            var initialBlackRowsOffsetValue = _blankRowsOffset;

            // Add a scenario heading if needed
            if (securitizationResult.ScenarioDescription != null)
            {
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Value = securitizationResult.ScenarioDescription;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                _blankRowsOffset += 2;
            }

            // Column totals will span across the top of the report
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Value = _columnTotals;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Font.Bold = true;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            _blankRowsOffset += 2;

            // Start with the initial columns                
            foreach (var headerIndex in Enumerable.Range(0, _initialColumnHeadersRow.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Value = _initialColumnHeadersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                excelWorksheet.Column(headerIndex + _blankColumnsOffset).Width = 10;
            }

            // Add the empty spacer column and set width
            var dataColumnOffsetCounter = 0;
            dataColumnOffsetCounter += _initialColumnHeadersRow.Count + 1;
            excelWorksheet.Column(_blankColumnsOffset + dataColumnOffsetCounter - 1).Width = 2;

            // Available funds comes next
            foreach (var headerIndex in Enumerable.Range(0, _availableFundsHeadersRow.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Value = _availableFundsHeadersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                excelWorksheet.Column(headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Width = 14;
            }

            var dataRowOffsetCounter = 1;
            var initialDataColumnOffsetCounterValue = dataColumnOffsetCounter;
            var availableFundsCashFlows = securitizationResult.AvailableFundsCashFlows;
            var totalCollateralCashFlows = securitizationResult.ProjectedCashFlowsOnCollateral;
            foreach (var availableFundsCashFlow in availableFundsCashFlows)
            {
                var availableFundsPeriod = availableFundsCashFlow.Period;

                // Period
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Value = totalCollateralCashFlows[availableFundsPeriod].Period;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.NumberFormat.Format = _noDecimalsNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Payment Date
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + 1).Value = totalCollateralCashFlows[availableFundsPeriod].PeriodDate;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + 1).Style.DateFormat.Format = _shortDateFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Total Principal
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = availableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                var totalPrincipal = availableFundsCashFlow.AvailablePrincipal + availableFundsCashFlow.AvailablePrepayments + availableFundsCashFlow.AvailablePrincipalRecoveries;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalPrincipal;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Total Interest
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = availableFundsCashFlows.Sum(c => c.AvailableInterest);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                var totalInterest = availableFundsCashFlow.AvailableInterest + availableFundsCashFlow.AvailablePrepaymentInterest + availableFundsCashFlow.AvailableInterestRecoveries;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalInterest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Total Cash Flow
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = availableFundsCashFlows.Sum(c => c.Payment);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = availableFundsCashFlow.Payment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Principal Balance
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].EndingBalance;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Scheduled Principal
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows.Sum(c => c.Principal);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].Principal;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Scheduled Interest
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows.Sum(c => c.Interest);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].Interest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prepayment Interest
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows.Sum(c => c.PrepaymentInterest);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].PrepaymentInterest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prepayments  
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows.Sum(c => c.Prepayment);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].Prepayment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prepayment Penalties  
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows.Sum(c => c.PrepaymentPenalty);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].PrepaymentPenalty;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Recoveries  
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows.Sum(c => c.Recovery + c.InterestRecovery);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = totalCollateralCashFlows[availableFundsPeriod].Recovery + totalCollateralCashFlows[availableFundsPeriod].InterestRecovery;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Collections Account Balance
                var collectionsAccountBalances = availableFundsCashFlow.AvailableReserveFundsDictionary.Values.Where(v => !v.IsReserveTranche).ToList();
                var collectionsAccountBalance = availableFundsCashFlow.AvailableReserveFundsDictionary.Where(kvp => !kvp.Value.IsReserveTranche).Sum(kvp => kvp.Value.FundStartingBalance);
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = collectionsAccountBalance;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                dataColumnOffsetCounter = initialDataColumnOffsetCounterValue;
                dataRowOffsetCounter++;
            }

            // Reset data and column row counters
            dataRowOffsetCounter = 1;
            dataColumnOffsetCounter += _availableFundsHeadersRow.Count + 1;

            // The securitization waterfall is generally displayed in a manner that follows the non-shortfall entries of the priority of payments
            var distinctOrderedPriorityOfPaymentsEntries = securitizationResult.PriorityOfPayments.OrderedListOfEntries
                .Where(e => e.TrancheCashFlowType == TrancheCashFlowType.Payment   ||
                            e.TrancheCashFlowType == TrancheCashFlowType.Principal ||
                            e.TrancheCashFlowType == TrancheCashFlowType.Fees      ||
                            e.TrancheCashFlowType == TrancheCashFlowType.Reserves).OrderBy(p => p.SeniorityRanking).ToList();

            Type previousTrancheType = typeof(Tranche);
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;
            foreach (var priorityOfPaymentsEntry in distinctOrderedPriorityOfPaymentsEntries)
            {
                var trancheName = priorityOfPaymentsEntry.TrancheName;
                if (!trancheResultsDictionary.ContainsKey(trancheName)) continue;
                var trancheResult = trancheResultsDictionary[trancheName];
                var trancheCashFlows = trancheResult.TrancheCashFlows;

                // Do not add spacer columns between fee tranches
                if (previousTrancheType.IsSubclassOf(typeof(FeeTranche)) && trancheResult.TrancheType.IsSubclassOf(typeof(FeeTranche)))
                {
                    dataColumnOffsetCounter--;   
                }
                else
                {
                    excelWorksheet.Column(_blankColumnsOffset + dataColumnOffsetCounter - 1).Width = 2;
                }

                // Add the block corresponding to the type of tranche appearing next
                if (trancheResult.TrancheType.IsSubclassOf(typeof(InterestPayingTranche)))
                {
                    AddInterestPayingTrancheBlock(excelWorksheet, trancheCashFlows, trancheName, ref dataRowOffsetCounter, ref dataColumnOffsetCounter);
                }
                else if (trancheResult.TrancheType.IsSubclassOf(typeof(ReserveFundTranche)))
                {
                    AddReserveFundBlock(excelWorksheet, trancheCashFlows, trancheName, ref dataRowOffsetCounter, ref dataColumnOffsetCounter);
                }
                else if (trancheResult.TrancheType.IsSubclassOf(typeof(FeeTranche)) ||
                         trancheResult.TrancheType == typeof(ResidualTranche))
                {
                    var columnWidth = 14.0;
                    if (trancheResult.TrancheType.IsSubclassOf(typeof(FeeTranche))) columnWidth = 10.5;

                    AddPaymentOnlyBlock(excelWorksheet, trancheCashFlows, trancheName, columnWidth, ref dataRowOffsetCounter, ref dataColumnOffsetCounter);
                }
                else
                {
                    AddGenericTrancheBlock(excelWorksheet, trancheCashFlows, trancheName, ref dataRowOffsetCounter, ref dataColumnOffsetCounter);
                }

                previousTrancheType = trancheResult.TrancheType;
            }

            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Cells().Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Rows(1, _blankRowsOffset).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Rows(1, _blankRowsOffset - 1).Height = 14.5;

            if (securitizationResult.ScenarioDescription != null)
            {
                excelWorksheet.Row(initialBlackRowsOffsetValue).Style.Font.FontSize = _baseFontSize + 4;
            }

            // Auto-fit and adjust column-widths
            excelWorksheet.Row(_blankRowsOffset).Style.Alignment.WrapText = true;
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }

            // Freeze panes and color tab
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, _blankColumnsOffset + _initialColumnHeadersRow.Count - 1);
            excelWorksheet.SetTabColor(XLColor.White);

            // Reset blank rows offset
            _blankRowsOffset = initialBlackRowsOffsetValue;
        }

        private static void AddGenericTrancheBlock(
            IXLWorksheet excelWorksheet, 
            List<SecuritizationCashFlow> trancheCashFlows,
            string trancheName,
            ref int dataRowOffsetCounter, 
            ref int dataColumnOffsetCounter)
        {
            throw new NotImplementedException();
        }

        private static void AddPaymentOnlyBlock(
            IXLWorksheet excelWorksheet,
            List<SecuritizationCashFlow> trancheCashFlows,
            string trancheName,
            double columnWidth,
            ref int dataRowOffsetCounter,
            ref int dataColumnOffsetCounter)
        {
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheName;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            excelWorksheet.Column(_blankColumnsOffset + dataColumnOffsetCounter).Width = columnWidth;     

            var initialDataColumnOffsetCounterValue = dataColumnOffsetCounter;
            foreach (var trancheCashFlow in trancheCashFlows)
            {
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.Payment);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.Payment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataRowOffsetCounter++;
            }

            // Reset data row counter
            dataRowOffsetCounter = 1;
            dataColumnOffsetCounter += 2;
        }

        private static void AddReserveFundBlock(
            IXLWorksheet excelWorksheet,
            List<SecuritizationCashFlow> trancheCashFlows,
            string trancheName,
            ref int dataRowOffsetCounter,
            ref int dataColumnOffsetCounter)
        {
            foreach (var headerIndex in Enumerable.Range(0, _reserveAccountHeadersRow.Count))
            {
                var headerText = trancheName + " " + _reserveAccountHeadersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Value = headerText;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                excelWorksheet.Column(headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Width = 13;
            }

            var initialDataColumnOffsetCounterValue = dataColumnOffsetCounter;
            foreach (var trancheCashFlow in trancheCashFlows)
            {
                // Release
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.Interest);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.Interest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Contribution
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.Payment);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.Payment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Balance
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.EndingBalance;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                dataColumnOffsetCounter = initialDataColumnOffsetCounterValue;
                dataRowOffsetCounter++;
            }

            // Reset data row counter
            dataRowOffsetCounter = 1;
            dataColumnOffsetCounter += _reserveAccountHeadersRow.Count + 1;
        }

        private static void AddInterestPayingTrancheBlock(
            IXLWorksheet excelWorksheet,
            List<SecuritizationCashFlow> trancheCashFlows,
            string trancheName,
            ref int dataRowOffsetCounter,
            ref int dataColumnOffsetCounter)
        {
            foreach (var headerIndex in Enumerable.Range(0, _interestPayingTrancheHeadersRow.Count))
            {
                var headerText = trancheName + " " + _interestPayingTrancheHeadersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Value = headerText;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                excelWorksheet.Column(headerIndex + _blankColumnsOffset + dataColumnOffsetCounter).Width = 14;
            }

            var initialDataColumnOffsetCounterValue = dataColumnOffsetCounter;
            foreach (var trancheCashFlow in trancheCashFlows)
            {
                // Principal
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.Principal);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.Principal;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Interest
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.Interest);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.Interest;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Cash Flow
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.Payment);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.Payment;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Balance
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.EndingBalance;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Prin. Short-Fall
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.PaymentShortfall);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.PaymentShortfall;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Int. Short-Fall
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.InterestShortfall);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.InterestShortfall;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataColumnOffsetCounter++;

                // Acc. Int. on Short-Fall
                if (excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).IsEmpty())
                {
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlows.Sum(c => c.InterestAccruedOnInterestShortfall);
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                    excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = trancheCashFlow.InterestAccruedOnInterestShortfall;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                dataColumnOffsetCounter = initialDataColumnOffsetCounterValue;
                dataRowOffsetCounter++;
            }

            // Reset data row counter
            dataRowOffsetCounter = 1;
            dataColumnOffsetCounter += _interestPayingTrancheHeadersRow.Count + 1;
        }
    }
}
