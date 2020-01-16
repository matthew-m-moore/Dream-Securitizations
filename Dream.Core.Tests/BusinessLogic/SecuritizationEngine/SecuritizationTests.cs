using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.Reporting.Excel;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dream.Core.Tests.BusinessLogic.SecuritizationEngine
{
    [TestClass]
    public class SecuritizationTests
    {
        private const string _inputsFile = "Dream.Core.Tests.Resources.Pace-Assessment-Sample-Collateral.xlsx";

        [TestMethod]
        public void GenerateCashFlowsOnSamplePaceAssessmentSecuritization()
        {
            var paceSecuritization = GetSamplePaceSecuritizationWithFixedRateTranche();
            var securitizationResults = paceSecuritization.RunSecuritizationAnalysis();

            var fixedRateTrancheCashFlows = securitizationResults.SecuritizationResultsDictionary["Fixed Rate Tranche"].TrancheCashFlows;
            var residualCertificateCashFlows = securitizationResults.SecuritizationResultsDictionary["Equity"].TrancheCashFlows;

            // Feel free to comment this out if needed once the tests are set up
            ExportSecuritizationResults(new Dictionary<string, SecuritizationResult> { [string.Empty] = securitizationResults });

            Assert.IsTrue(fixedRateTrancheCashFlows.Any());
            Assert.IsTrue(residualCertificateCashFlows.Any());
        }

        private Securitization GetSamplePaceSecuritizationWithFixedRateTranche()
        {
            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFileStream);
            var paceSecuritization = GetSecuritization(securitizationDataRepository);

            return paceSecuritization;
        }

        private static Securitization GetSecuritization(SecuritizationExcelDataRepository securitizationDataRepository)
        {
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritizationWithoutNodesDefined();

            var securitizationNodes = GetSecuritizationNodeStructure();
            var priorityOfPayments = GetPriorityOfPayments();

            paceSecuritization.PriorityOfPayments = priorityOfPayments;
            paceSecuritization.SecuritizationNodes = securitizationNodes;

            paceSecuritization.Inputs.MarketDataGroupingForNominalSpread = MarketDataGrouping.Swaps;

            return paceSecuritization;
        }

        private static PriorityOfPayments GetPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry(1, "Fixed Rate Tranche", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry(2, "Fixed Rate Tranche", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry(3, "Equity", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static List<SecuritizationNodeTree> GetSecuritizationNodeStructure()
        {
            var principalRemittancesAvailableFundsRetriever = new PrincipalRemittancesAvailableFundsRetriever(1.00);
            var allFundsAvailableFundsRetriever = new AllFundsAvailableFundsRetriever();

            var fixedRateTranche = new FixedRateTranche("Fixed Rate Tranche",
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.0675),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.0450))
            {
                CurrentBalance = 200000000,
                InitialBalance = 200000000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2017, 4, 28),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 9, 20),
                MonthsToNextInterestPayment = 5,
                InterestPaymentFrequencyInMonths = 6,
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1,
                IncludePaymentShortfall = true,
                IncludeInterestShortfall = true,
                IsShortfallPaidFromReserves = true,
            };

            var residualCertificate = new ResidualTranche("Equity",
                    new YieldBasedPricingStrategy(DayCountConvention.Actual365, CompoundingConvention.Annually, 0.1350),
                    allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6,
            };

            var securitizationNodes = new List<SecuritizationNodeTree>
            {
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Sample Node",
                    SecuritizationTranches = new List<Tranche>
                    {
                        fixedRateTranche,
                        residualCertificate
                    }
                },
            };

            return securitizationNodes;
        }

        /// <summary>
        /// This is a private helper method to double-check cash flows in Excel, if needed.
        /// </summary>
        private static void ExportSecuritizationResults(Dictionary<string, SecuritizationResult> securitizationResultsDictionary)
        {
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            SecuritizationCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);

            excelFileWriter.ExportWorkbook();
        }
    }
}
