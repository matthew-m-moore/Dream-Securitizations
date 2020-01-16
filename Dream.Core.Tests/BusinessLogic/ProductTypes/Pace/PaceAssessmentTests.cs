using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.IO.Database.Entities.Collateral;
using Dream.Core.Converters.Database.Collateral;
using System.Linq;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;
using Dream.IO.Excel;

namespace Dream.Core.Tests.BusinessLogic.ProductTypes.Pace
{
    [TestClass]
    public class PaceAssessmentTests
    {
        private PaceAssessment CreateSamplePaceAssessmentFromDatabaseEntity()
        {
            var paceAssessmentRecordEntity = new PaceAssessmentRecordEntity
            {
                Balance = 25000,
                CouponRate = 0.0835,
                TermInYears = 25,

                FundingDate = new DateTime(2017, 10, 15),
                BondFirstPaymentDate = new DateTime(2018, 3, 2),
                BondFirstPrincipalPaymentDate = new DateTime(2019, 9, 2),
                BondMaturityDate = new DateTime(2043, 9, 2),

                CashFlowStartDate = new DateTime(2017, 11, 2),
                InterestAccrualStartDate = new DateTime(2017, 12, 2),

                InterestPaymentFrequencyInMonths = 6,
                PrincipalPaymentFrequencyInMonths = 12
            };

            var collateralCutOffDate = new DateTime(2017, 11, 2);
            var propertyStateConverter = new PropertyStateDatabaseConverter(new Dictionary<int, string>());
            var paceAssessmentDatabaseConverter = new PaceAssessmentDatabaseConverter(collateralCutOffDate, null, null, false, propertyStateConverter, null, null);

            var paceAssessment = paceAssessmentDatabaseConverter.ConvertPaceAssessmentRecordEntity(paceAssessmentRecordEntity);
            return paceAssessment;
        }

        [TestMethod]
        public void GenerateContractualCashFlowsOnSamplePaceAssesment()
        {
            var paceAssessment = CreateSamplePaceAssessmentFromDatabaseEntity();
            var contractualCashFlows = paceAssessment.GetContractualCashFlows();

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
