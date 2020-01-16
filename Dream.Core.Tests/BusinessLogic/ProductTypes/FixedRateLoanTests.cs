using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.IO.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Tests.BusinessLogic.ProductTypes
{
    [TestClass]
    public class FixedRateLoanTests
    {
        private FixedRateLoan CreateSampleFixedRateLoan()
        {
            // Note, this sample loan has a balloon payment
            var fixedRateFullyAmortizingLoan = new FixedRateLoan
            {
                StartDate = new DateTime(2014, 5, 1),
                Balance = 408000,
                InitialCouponRate = 0.04375,

                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2014, 5, 1),

                InterestPaymentFrequencyInMonths = 1,
                PrincipalPaymentFrequencyInMonths = 1,

                MonthsToNextInterestPayment = 1,
                MonthsToNextPrincipalPayment = 1,

                AmortizationTermInMonths = 360,
                MaturityTermInMonths = 179,
            };

            return fixedRateFullyAmortizingLoan;
        }

        [TestMethod]
        public void GenerateContractualCashFlowsOnSampleFixedRateLoan()
        {
            var fixedRateLoan = CreateSampleFixedRateLoan();
            var contractualCashFlows = fixedRateLoan.GetContractualCashFlows();

            // Feel free to comment this out if needed once the tests are set up
            ExportContractualCashFlows(contractualCashFlows);

            Assert.IsTrue(contractualCashFlows.Any());
        }

        /// <summary>
        /// This is a private helper method to double-check cash flows in Excel, if needed.
        /// </summary>
        private static void ExportContractualCashFlows(List<ContractualCashFlow> contractualCashFlows)
        {
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            excelFileWriter.AddWorksheetForListOfData(contractualCashFlows, "Contractual Cash Flows");
            excelFileWriter.ExportWorkbook();
        }
    }
}
