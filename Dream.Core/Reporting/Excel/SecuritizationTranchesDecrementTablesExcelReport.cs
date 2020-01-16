using ClosedXML.Excel;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.Reporting.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Common;

namespace Dream.Core.Reporting.Excel
{
    public class SecuritizationTranchesDecrementTablesExcelReport
    {
        private const string _reportTabName = "Tranche Decrement Tables";
        private const string _fontName = "Open Sans";

        private const string _periodDateHeader = "Period Date";
        private const string _initialPeriodHeader = "Initial";

        private const string _weightedAverageLifeFooter = "WAL (years)";
        private const string _maturityDateFooter = "Maturity";

        private const string _shortDateFormat = "m/d/yyyy";
        private const string _monthYearDateFormat = "mmm yyyy";
        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";
        private const string _accountingNumberFormatNoDecimals = @"_(* #,##0_);_(* (#,##0);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 9;
        private const int _blankRowsOffset = 4;

        private static int _blankColumnsOffset = 2;
        private static int _dataRowOffsetCounter = 1;

        private static int _cashFlowStartingPeriod;
        private static int _cashFlowMonthlyPeriodIncrementer;

        public static void AddReportTab(XLWorkbook excelWorkbook, Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults,
            int cashFlowMonthlyPeriodIncrement = 6)
        {
            var initialBlankColumnsOffsetCounterValue = _blankColumnsOffset;

            var tranchesForDecrementTables = dictionaryOfSecuritizationResults.Values
                .SelectMany(r => r.SecuritizationResultsDictionary.Values.Where(t => !string.IsNullOrEmpty(t.TrancheName)))
                .Select(x => (TrancheName: x.TrancheName, TrancheType: x.TrancheType)).Distinct().ToList();

            var listOfTrancheNames = tranchesForDecrementTables.Select(t => t.TrancheName).ToList();
            var startingPeriodsOfCashFlows = dictionaryOfSecuritizationResults.Values
                .SelectMany(r => (r.SecuritizationResultsDictionary.Values.Where(t => listOfTrancheNames.Contains(t.TrancheName))))
                .Select(DetermineFirstPaymentPeriod).ToList();

            _cashFlowStartingPeriod = startingPeriodsOfCashFlows.Min();
            _cashFlowMonthlyPeriodIncrementer = cashFlowMonthlyPeriodIncrement;

            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);
            foreach (var trancheNameAndType in tranchesForDecrementTables)
            {
                AddDecrementTables(
                    excelWorksheet,
                    trancheNameAndType,
                    dictionaryOfSecuritizationResults);
            }

            _blankColumnsOffset = initialBlankColumnsOffsetCounterValue;

            excelWorksheet.Column(1).Width = 2;
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, 0);
            excelWorksheet.SetTabColor(XLColor.White);
        }

        private static int DetermineFirstPaymentPeriod(SecuritizationCashFlowsSummaryResult summaryResult)
        {
            SecuritizationCashFlow firstNonZeroPaymentCashFlow;
            if (summaryResult.TrancheType.IsSubclassOf(typeof(InterestPayingTranche)))
            {
                firstNonZeroPaymentCashFlow = summaryResult.TrancheCashFlows.FirstOrDefault(c => c.Interest > 0.0);                
            }
            else
            {
                firstNonZeroPaymentCashFlow = summaryResult.TrancheCashFlows.FirstOrDefault(c => c.Payment > 0.0);
            }

            if (firstNonZeroPaymentCashFlow != null) return firstNonZeroPaymentCashFlow.Period;
            return 1;
        }

        private static void AddDecrementTables(
            IXLWorksheet excelWorksheet,
            (string TrancheName, Type TrancheType) trancheNameAndType,
            Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            // Title of the Table
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Value = trancheNameAndType.TrancheName;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Font.Bold = true;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Font.FontSize = _baseFontSize + 3;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            excelWorksheet.Cell(_blankRowsOffset - 2, _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            AddDecrementTable(excelWorksheet, trancheNameAndType, dictionaryOfSecuritizationResults);

            excelWorksheet.Column(_blankColumnsOffset).Width = 4;
            _blankColumnsOffset += 1;
        }

        private static void AddDecrementTable(
            IXLWorksheet excelWorksheet, 
            (string TrancheName, Type TrancheType) trancheNameAndType, 
            Dictionary<string, SecuritizationResult> dictionaryOfSecuritizationResults)
        {
            var initialDataRowOffsetCounterValue = _dataRowOffsetCounter;
            var dataColumnOffsetCounter = 0;

            foreach (var scenarioDescription in dictionaryOfSecuritizationResults.Keys)
            {
                var isFirstScenario = (dataColumnOffsetCounter == 0);

                // Date column header
                if (isFirstScenario)
                {
                    dataColumnOffsetCounter++;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = _periodDateHeader;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.TopBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.BottomBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;                   
                }

                // Scenario column header
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Value = scenarioDescription;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var securitizationResult = dictionaryOfSecuritizationResults[scenarioDescription];
                var securitizationTrancheResult = securitizationResult.SecuritizationResultsDictionary[trancheNameAndType.TrancheName];

                var trancheCashFlows = securitizationTrancheResult.TrancheCashFlows;
                if (trancheNameAndType.TrancheType.IsSubclassOf(typeof(InterestPayingTranche)))
                {
                    PopulateInterestPayingTrancheDecrementTable(excelWorksheet, trancheCashFlows, dataColumnOffsetCounter, isFirstScenario);
                }
                else if (trancheNameAndType.TrancheType == typeof(ResidualTranche))
                {
                    var residualPresentValue = securitizationTrancheResult.PresentValue;
                    PopulateResidualTrancheDecrementTable(excelWorksheet, trancheCashFlows, residualPresentValue, dataColumnOffsetCounter, isFirstScenario);
                }
                else
                {
                    throw new Exception(string.Format("INTERNAL ERROR: Decrement tables have not been explicitely described for tranche of type: '{0}'. Please report this error.",
                        trancheNameAndType.TrancheType.ToString()));
                }

                if (isFirstScenario)
                {
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = _weightedAverageLifeFooter;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.DateFormat.Format = _monthYearDateFormat;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.TopBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.BottomBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = _maturityDateFooter;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.Bold = true;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.DateFormat.Format = _monthYearDateFormat;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.TopBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Border.BottomBorderColor = XLColor.Black;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter + 1, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                var weightedAverageLife = (double.IsNaN(securitizationTrancheResult.WeightedAverageLife)) ? 0.0 : securitizationTrancheResult.WeightedAverageLife;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = weightedAverageLife;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                _dataRowOffsetCounter++;

                var lastNonZeroCashFlow = trancheCashFlows.LastOrDefault(c => c.Payment > 0);
                var maturityDate = (lastNonZeroCashFlow != null) ? (DateTime?) lastNonZeroCashFlow.PeriodDate : null;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = maturityDate;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _shortDateFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                _dataRowOffsetCounter = initialDataRowOffsetCounterValue;
                dataColumnOffsetCounter++;
            }

            _blankColumnsOffset += dataColumnOffsetCounter;
        }

        private static void PopulateInterestPayingTrancheDecrementTable(
            IXLWorksheet excelWorksheet,
            List<SecuritizationCashFlow> trancheCashFlows,
            int dataColumnOffsetCounter,
            bool isFirstScenario)
        {
            var firstTrancheCashFlow = trancheCashFlows.First();
            var initialBalance = firstTrancheCashFlow.StartingBalance;

            if (isFirstScenario)
            {
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = _initialPeriodHeader;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.DateFormat.Format = _monthYearDateFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }

            var firstValue = (initialBalance > 0.0) ? firstTrancheCashFlow.EndingBalance / initialBalance : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = firstValue * Constants.OneHundredPercentagePoints;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            _dataRowOffsetCounter++;

            var trancheCashFlowCount = trancheCashFlows.Count;
            for (var cashFlowCounter = _cashFlowStartingPeriod; cashFlowCounter < trancheCashFlowCount; cashFlowCounter += _cashFlowMonthlyPeriodIncrementer)
            {
                var trancheCashFlow = trancheCashFlows[cashFlowCounter];

                if (isFirstScenario)
                {
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = trancheCashFlow.PeriodDate;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.DateFormat.Format = _monthYearDateFormat;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                var displayValue = (initialBalance > 0.0) ? trancheCashFlow.EndingBalance / initialBalance : 0.0;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = displayValue * Constants.OneHundredPercentagePoints;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormatNoDecimals;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                _dataRowOffsetCounter++;
            }
        }

        private static void PopulateResidualTrancheDecrementTable(
            IXLWorksheet excelWorksheet, 
            List<SecuritizationCashFlow> trancheCashFlows, 
            double residualPresentValue,
            int dataColumnOffsetCounter, 
            bool isFirstScenario)
        {
            var firstTrancheCashFlow = trancheCashFlows.First();

            if (isFirstScenario)
            {
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = _initialPeriodHeader;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.DateFormat.Format = _monthYearDateFormat;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }

            var firstValue = (residualPresentValue > 0.0) ? firstTrancheCashFlow.Payment / residualPresentValue : 0.0;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = firstValue * Constants.OneHundredPercentagePoints;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormatNoDecimals;
            excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            _dataRowOffsetCounter++;

            var totalCashReturned = firstTrancheCashFlow.Payment;
            var trancheCashFlowCount = trancheCashFlows.Count;

            for (var cashFlowCounter = _cashFlowStartingPeriod; cashFlowCounter < trancheCashFlowCount; cashFlowCounter += _cashFlowMonthlyPeriodIncrementer)
            {
                var trancheCashFlow = trancheCashFlows[cashFlowCounter];
                totalCashReturned = trancheCashFlows.Where(c => c.Period <= cashFlowCounter).Sum(s => s.Payment);

                if (isFirstScenario)
                {
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Value = trancheCashFlow.PeriodDate;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Font.FontSize = _baseFontSize;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.DateFormat.Format = _monthYearDateFormat;
                    excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                var displayValue = (residualPresentValue > 0.0) ? totalCashReturned / residualPresentValue : 0.0;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = displayValue * Constants.OneHundredPercentagePoints;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormatNoDecimals;
                excelWorksheet.Cell(_blankRowsOffset + _dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                _dataRowOffsetCounter++;
            }
        }
    }
}
