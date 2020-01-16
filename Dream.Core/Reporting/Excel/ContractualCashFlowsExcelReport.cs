using ClosedXML.Excel;
using Dream.Common;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Reporting.Excel
{
    public class ContractualCashFlowsExcelReport
    {
        private const string _reportTabName = "Contractual Cash Flows";
        private const string _fontName = "Open Sans";

        private const string _shortDateFormat = "m/d/yyyy";
        private const string _percentageNumberFormat = "0.00%";
        private const string _noDecimalsNumberFormat = "#,##0";
        private const string _accountingNumberFormat = @"_(* #,##0.00_);_(* (#,##0.00);_(* "" - ""??_);_(@_)";

        private const int _baseFontSize = 8;
        private const int _blankColumnsOffset = 2;

        private static int _blankRowsOffset = 2;

        private static List<string> _headersRow = new List<string>
        {
            "Date",
            "Beginning Balance",
            "Payment",
            "Interest Pmt",
            "Principal Pmt",
            "Prepayment",
            "Ending Balance",
            "Coupon",
            "Buy-Down Rate",
            "Term Count",
            "Term",
            "Bond ID",
        };

        public static void AddReportTab(
            XLWorkbook excelWorkbook, 
            List<Loan> listOfLoans, 
            Dictionary<string, List<ContractualCashFlow>> contractualCashFlowsDictionary)
        {
            var cashFlowDisplayResults = ConvertListOfLoansToDisplayResults(listOfLoans, contractualCashFlowsDictionary);
            var excelWorksheet = excelWorkbook.Worksheets.Add(_reportTabName);

            AddReportHeadersLine(excelWorksheet);

            var dataRowOffsetCounter = 1;
            foreach (var cashFlowDisplayResult in cashFlowDisplayResults)
            {
                AddReportLine(excelWorksheet, cashFlowDisplayResult, dataRowOffsetCounter);
                dataRowOffsetCounter++;
            }

            SetSheetFormatting(excelWorksheet, cashFlowDisplayResults.Count);
        }

        private static List<AbbreviatedCashFlowDisplayResult> ConvertListOfLoansToDisplayResults(List<Loan> listOfLoans, Dictionary<string, List<ContractualCashFlow>> contractualCashFlowsDictionary)
        {
            var cashFlowDisplayResults = new List<AbbreviatedCashFlowDisplayResult>();

            foreach (var loan in listOfLoans)
            {             
                if (contractualCashFlowsDictionary.ContainsKey(loan.StringId))
                {
                    var contractualCashFlows = contractualCashFlowsDictionary[loan.StringId];
                    var abbreviatedContractualCashFlows = contractualCashFlows.Where(c => c.Payment > 0.0 || c.Period == 0).ToList();

                    var abbreviatedCashFlowDisplayResults = ConvertListOfCashFlowsToDisplayResults(loan, abbreviatedContractualCashFlows);
                    cashFlowDisplayResults.AddRange(abbreviatedCashFlowDisplayResults);
                }
            }

            return cashFlowDisplayResults;
        }

        private static List<AbbreviatedCashFlowDisplayResult> ConvertListOfCashFlowsToDisplayResults(Loan loan, List<ContractualCashFlow> abbreviatedContractualCashFlows)
        {
            var cashFlowDisplayResults = new List<AbbreviatedCashFlowDisplayResult>();
            var termCount = 1;

            foreach (var abbreviatedContractualCashFlow in abbreviatedContractualCashFlows)
            {
                var abbreviatedCashFlowDisplayResult = ConvertContractualCashFlowToDisplayResult(loan, abbreviatedContractualCashFlow);

                abbreviatedCashFlowDisplayResult.TermCount = termCount;
                cashFlowDisplayResults.Add(abbreviatedCashFlowDisplayResult);

                if (abbreviatedCashFlowDisplayResult.PrincipalPayment > 0.0) termCount++;
            }

            return cashFlowDisplayResults;
        }

        private static AbbreviatedCashFlowDisplayResult ConvertContractualCashFlowToDisplayResult(Loan loan, ContractualCashFlow contractualCashFlow)
        {
            var couponRate = (contractualCashFlow.Period > 0)
                ? contractualCashFlow.Coupon / Constants.OneHundredPercentagePoints
                : loan.InitialCouponRate;

            var buyDownRate = (loan is PaceAssessment paceAssessment && paceAssessment.RatePlan != null)
                ? paceAssessment.RatePlan.BuyDownRate
                : 0.0;

            var abbreviatedCashFlowDisplayResult = new AbbreviatedCashFlowDisplayResult
            {
                Identifier = loan.StringId,
                Date = contractualCashFlow.PeriodDate,

                BeginningBalance = contractualCashFlow.StartingBalance + loan.ActualPrepayments,
                Payment = contractualCashFlow.Payment + loan.ActualPrepayments,
                InterestPayment = contractualCashFlow.Interest,
                PrincipalPayment = contractualCashFlow.Principal,
                Prepayment = loan.ActualPrepayments,
                EndingBalance = contractualCashFlow.EndingBalance,

                Coupon = couponRate,
                BuyDownRate = buyDownRate,

                TermInYears = loan.AmortizationTermInYears
            };

            return abbreviatedCashFlowDisplayResult;
        }

        private static void AddReportHeadersLine(IXLWorksheet excelWorksheet)
        {
            // Set up the headers
            foreach (var headerIndex in Enumerable.Range(0, _headersRow.Count))
            {
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Value = _headersRow[headerIndex];
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.Bold = true;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Font.FontSize = _baseFontSize;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.TopBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Border.BottomBorderColor = XLColor.Black;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                excelWorksheet.Cell(_blankRowsOffset, headerIndex + _blankColumnsOffset).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        private static void AddReportLine(IXLWorksheet excelWorksheet, AbbreviatedCashFlowDisplayResult cashFlowDisplayResult, int dataRowOffsetCounter)
        {
            var dataColumnOffsetCounter = 0;

            // Date
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.Date;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.NumberFormat.Format = _shortDateFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataColumnOffsetCounter++;

            // Beginning Balance
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.BeginningBalance;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.NumberFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Payment
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.Payment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Interest Pmt
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.InterestPayment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Principal Pmt
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.PrincipalPayment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Prepayment
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.Prepayment;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Ending Balance
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.EndingBalance;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _accountingNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            dataColumnOffsetCounter++;

            // Coupon
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.Coupon;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _percentageNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataColumnOffsetCounter++;

            // Buy-Down Rate
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.BuyDownRate;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _percentageNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataColumnOffsetCounter++;

            // Term Count
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.TermCount;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _noDecimalsNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataColumnOffsetCounter++;

            // Term
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.TermInYears;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.DateFormat.Format = _noDecimalsNumberFormat;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataColumnOffsetCounter++;

            // Bond ID
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Value = cashFlowDisplayResult.Identifier;
            excelWorksheet.Cell(_blankRowsOffset + dataRowOffsetCounter, _blankColumnsOffset + dataColumnOffsetCounter).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataColumnOffsetCounter++;
        }

        private static void SetSheetFormatting(IXLWorksheet excelWorksheet, int cashFlowCount)
        {
            // Setup font name and size
            excelWorksheet.Cells().Style.Font.FontName = _fontName;
            excelWorksheet.Rows(_blankRowsOffset + 1, _blankRowsOffset + cashFlowCount + 1).Style.Font.FontSize = _baseFontSize + 1;
            excelWorksheet.Rows(_blankRowsOffset - 1, _blankRowsOffset + cashFlowCount + 1).Height = 14.5;

            // Auto-fit and adjust column-widths
            excelWorksheet.Columns().AdjustToContents();
            foreach (var columnIndex in Enumerable.Range(1, _blankColumnsOffset - 1))
            {
                excelWorksheet.Column(columnIndex).Width = 2;
            }

            // Freeze panes and set tab color
            excelWorksheet.SheetView.Freeze(_blankRowsOffset, _blankColumnsOffset);
            excelWorksheet.SetTabColor(XLColor.White);
        }
    }
}
