using Dream.Common.Curves;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Converters.Database.Collateral;
using Dream.IO.Database.Entities.Collateral;
using Dream.IO.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Tests.BusinessLogic.ProjectedCashFlows
{
    [TestClass]
    public class ProjectedCashFlowLogicTests
    {
        [TestMethod]
        public void GetProjectedCashFlowsOnSamplePaceAssessment()
        {
            var projectedCashFlowLogic = CreateSampleProjectedCashFlowLogic();
            var performanceAssumptionsIdentifier = "Flat 10.0% CPR";

            projectedCashFlowLogic.ProjectedPerformanceAssumptions.ConvertAllCurvesToMonthlyRate();
            MapPerformanceAssumptions(projectedCashFlowLogic.ProjectedPerformanceAssumptions, performanceAssumptionsIdentifier);       

            var paceAssessment = CreateSamplePaceAssessmentFromDatabaseEntity();
            var projectedCashFlowGenerator = new ProjectedCashFlowGenerator<ProjectedCashFlow>(projectedCashFlowLogic, new CashFlowGenerationInput());
            var projectedCashFlows = projectedCashFlowGenerator.GenerateCashFlowsOnSingleLoan(paceAssessment);

            // Feel free to comment this out if needed once the tests are set up
            ExportProjectedCashFlows(projectedCashFlows);

            Assert.IsTrue(projectedCashFlows.Any());
        }

        [TestMethod]
        public void GetProjectedCashFlowsOnSampleFixedRateLoan()
        {
            var projectedCashFlowLogic = CreateSampleProjectedCashFlowLogic();
            var performanceAssumptionsIdentifier = "Flat 10.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD";

            projectedCashFlowLogic.ProjectedPerformanceAssumptions.ConvertAllCurvesToMonthlyRate();
            MapPerformanceAssumptions(projectedCashFlowLogic.ProjectedPerformanceAssumptions, performanceAssumptionsIdentifier);      

            var fixedRateLoan = CreateSampleFixedRateLoan();
            var projectedCashFlowGenerator = new ProjectedCashFlowGenerator<ProjectedCashFlow>(projectedCashFlowLogic, new CashFlowGenerationInput());
            var projectedCashFlows = projectedCashFlowGenerator.GenerateCashFlowsOnSingleLoan(fixedRateLoan);

            // Feel free to comment this out if needed once the tests are set up
            ExportProjectedCashFlows(projectedCashFlows);

            Assert.IsTrue(projectedCashFlows.Any());
        }

        private ProjectedCashFlowLogic CreateSampleProjectedCashFlowLogic()
        {
            var flat00_CPR = new Curve<double>(0.00);
            var flat10_CPR = new Curve<double>(0.10);
            var flat08_CDR = new Curve<double>(0.08);
            var flat05_LGD = new Curve<double>(0.05);

            var projectedPerformanceAssumptions = new ProjectedPerformanceAssumptions();

            projectedPerformanceAssumptions["Flat 10.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD", PerformanceCurveType.Cpr] = flat10_CPR;
            projectedPerformanceAssumptions["Flat 10.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD", PerformanceCurveType.Cdr] = flat08_CDR;
            projectedPerformanceAssumptions["Flat 10.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD", PerformanceCurveType.Lgd] = flat05_LGD;

            projectedPerformanceAssumptions["Flat 0.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD", PerformanceCurveType.Cpr] = flat00_CPR;
            projectedPerformanceAssumptions["Flat 0.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD", PerformanceCurveType.Cdr] = flat08_CDR;
            projectedPerformanceAssumptions["Flat 0.0% CPR, Flat 8.0% CDR, Flat 5.0% LGD", PerformanceCurveType.Lgd] = flat05_LGD;

            projectedPerformanceAssumptions["Flat 10.0% CPR", PerformanceCurveType.Cpr] = flat10_CPR;

            return new ProjectedCashFlowLogic(projectedPerformanceAssumptions, string.Empty);
        }

        private void MapPerformanceAssumptions(ProjectedPerformanceAssumptions projectedPerformanceAssumptions, string performanceAssumptionsIdentifier)
        {
            projectedPerformanceAssumptions.PerformanceAssumptionsMapping[string.Empty, "Sample Fixed-Rate Loan", PerformanceCurveType.Smm] = performanceAssumptionsIdentifier;
            projectedPerformanceAssumptions.PerformanceAssumptionsMapping[string.Empty, "Sample Fixed-Rate Loan", PerformanceCurveType.Mdr] = performanceAssumptionsIdentifier;
            projectedPerformanceAssumptions.PerformanceAssumptionsMapping[string.Empty, "Sample Fixed-Rate Loan", PerformanceCurveType.Lgd] = performanceAssumptionsIdentifier;

            projectedPerformanceAssumptions.PerformanceAssumptionsMapping[string.Empty, "Sample Pace Assessment", PerformanceCurveType.Smm] = performanceAssumptionsIdentifier;
            projectedPerformanceAssumptions.PerformanceAssumptionsMapping[string.Empty, "Sample Pace Assessment", PerformanceCurveType.Mdr] = performanceAssumptionsIdentifier;
            projectedPerformanceAssumptions.PerformanceAssumptionsMapping[string.Empty, "Sample Pace Assessment", PerformanceCurveType.Lgd] = performanceAssumptionsIdentifier;
        }

        private FixedRateLoan CreateSampleFixedRateLoan()
        {
            var fixedRateFullyAmortizingLoan = new FixedRateLoan
            {
                StringId = "Sample Fixed-Rate Loan",

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
                MaturityTermInMonths = 360,
            };

            return fixedRateFullyAmortizingLoan;
        }

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
            paceAssessment.StringId = "Sample Pace Assessment";
            return paceAssessment;
        }

        /// <summary>
        /// This is a private helper method to double-check cash flows in Excel, if needed.
        /// </summary>
        private static void ExportProjectedCashFlows(List<ProjectedCashFlow> contractualCashFlows)
        {
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            excelFileWriter.AddWorksheetForListOfData(contractualCashFlows, "Projected Cash Flows");
            excelFileWriter.ExportWorkbook();
        }
    }
}
