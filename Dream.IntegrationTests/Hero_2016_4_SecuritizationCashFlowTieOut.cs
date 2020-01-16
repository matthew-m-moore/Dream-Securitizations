using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Excel;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Common.Enums;
using Dream.Common;
using System.Linq;
using Dream.IntegrationTests.Utilities;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class Hero_2016_4_SecuritizationCashFlowTieOut
    {
        private const string _inputsFile = "Dream.IntegrationTests.Resources.HERO-2016-4-Assessment-Level-Inputs.xlsx";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var paceSecuritization = GetSecuritization(inputFileStream);
            var securitizationResult = paceSecuritization.RunSecuritizationAnalysis();

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResult);

            CheckResults(securitizationResult);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_AfterUsingCopy()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var paceSecuritization = GetSecuritization(inputFileStream);

            var copiedPaceSecuritization = paceSecuritization.Copy();
            var securitizationResult = copiedPaceSecuritization.RunSecuritizationAnalysis();

            CheckResults(securitizationResult);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CheckResults(SecuritizationResult securitizationResult)
        {
            var precision = 0.01;

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResult);

            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(20630242064.60, totalEndingBalance, precision);
            Assert.AreEqual(113053003.31, totalScheduledPrincipal, precision);
            Assert.AreEqual(292406287.97, totalPricipal, precision);
            Assert.AreEqual(139541358.31, totalInterest, precision);
            Assert.AreEqual(431947646.28, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(113053003.31, totalScheduledPrincipal, precision);
            Assert.AreEqual(292406287.97, totalPricipal, precision);
            Assert.AreEqual(139541358.31, totalInterest, precision);
            Assert.AreEqual(431947646.28, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(9713482186.92, classA1EndingBalance, precision);
            Assert.AreEqual(140000000.00, classA1Principal, precision);
            Assert.AreEqual(28967506.34, classA1Interest, precision);
            Assert.AreEqual(168967506.34, classA1CashFlow, precision);
            Assert.AreEqual(237, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(9965616431.68, classA2EndingBalance, precision);
            Assert.AreEqual(143634000.00, classA2Principal, precision);
            Assert.AreEqual(35713252.65, classA2Interest, precision);
            Assert.AreEqual(179347252.65, classA2CashFlow, precision);
            Assert.AreEqual(237, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(1539249233.97, classBEndingBalance, precision);
            Assert.AreEqual(44500000.00, classBPrincipal, precision);
            Assert.AreEqual(6431552.37, classBInterest, precision);
            Assert.AreEqual(50931552.37, classBCashFlow, precision);
            Assert.AreEqual(69, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(29926959.61, residualCashFlow, precision);
            Assert.AreEqual(69, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(309, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2022, 9, 20).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
            Assert.AreEqual(new DateTime(2042, 9, 20).Ticks, lastPeriodOfResidualCashFlow.PeriodDate.Ticks);
            #endregion Notes Tie-Out
            #region Fees Tie-Out
            var absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var subAbsNoteTrusteeFee = trancheResultsDictionary["Sub ABS Note Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var subSpvAdministrationFee = trancheResultsDictionary["Sub SPV Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(1012111.11, absNoteTrusteeFee, precision);
            Assert.AreEqual(478473.23, spvAdministrationFee, precision);
            Assert.AreEqual(390000.00, portfolioAdministrationFee, precision);
            Assert.AreEqual(286000.00, subAbsNoteTrusteeFee, precision);
            Assert.AreEqual(353800.94, subSpvAdministrationFee, precision);
            Assert.AreEqual(273990.04, raAdvanceFee, precision);

            absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            subAbsNoteTrusteeFee = trancheResultsDictionary["Sub ABS Note Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            subSpvAdministrationFee = trancheResultsDictionary["Sub SPV Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(3254.38, absNoteTrusteeFee, precision);
            Assert.AreEqual(1538.50, spvAdministrationFee, precision);
            Assert.AreEqual(1254.02, portfolioAdministrationFee, precision);
            Assert.AreEqual(919.61, subAbsNoteTrusteeFee, precision);
            Assert.AreEqual(1137.62, subSpvAdministrationFee, precision);
            Assert.AreEqual(881.00, raAdvanceFee, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(753787659.18, reserveAccountEndingBalance, precision);
            Assert.AreEqual(5356553.67, reserveAccountReleases, precision);
            Assert.AreEqual(5336553.67, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out
        }

        public static Securitization GetSecuritization(string inputsFilePath)
        {
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFilePath);
            var paceSecuritization = GetSecuritization(securitizationDataRepository);
            return paceSecuritization;
        }

        public static Securitization GetSecuritization(Stream inputsFileStream)
        {
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFileStream);
            var paceSecuritization = GetSecuritization(securitizationDataRepository);
            return paceSecuritization;
        }

        private static Securitization GetSecuritization(SecuritizationExcelDataRepository securitizationDataRepository)
        {
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritizationWithoutNodesDefined();

            RedemptionLogic redemptionLogic;
            var securitizationNodes = GetSecuritizationNodesAndRedemptionLogic(out redemptionLogic);
            var priorityOfPayments = GetPriorityOfPayments();
            var redemptionPriorityOfPayments = GetRedemptionPriorityOfPayments();

            redemptionLogic.PriorityOfPayments = redemptionPriorityOfPayments;
            paceSecuritization.PriorityOfPayments = priorityOfPayments;
            paceSecuritization.SecuritizationNodes = securitizationNodes;

            paceSecuritization.RedemptionLogicList.Add(redemptionLogic);
            paceSecuritization.Inputs.MarketDataGroupingForNominalSpread = MarketDataGrouping.None;

            return paceSecuritization;
        }

        private static PriorityOfPayments GetPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "Portfolio Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "Class A1", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 4, "Class A2", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 5, "Class A1", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 5, "Class A2", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 6, "Liquidity Reserve Account", TrancheCashFlowType.Reserves),
                    new PriorityOfPaymentsEntry( 7, "Sub ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 7, "Sub SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 8, "RA Advance Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 9, "Class B",  TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry(10, "Class B",  TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry(11, "Preferred Shares", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static PriorityOfPayments GetRedemptionPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "Portfolio Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "Class A1", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 4, "Class A2", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 5, "Class A1", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 5, "Class A2", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 6, "Sub ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 6, "Sub SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 7, "RA Advance Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 8, "Class B",  TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 9, "Class B",  TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry(10, "Preferred Shares", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static List<SecuritizationNodeTree> GetSecuritizationNodesAndRedemptionLogic(out RedemptionLogic redemptionLogic)
        {
            var principalRemittancesAvailableFundsRetriever = new PrincipalRemittancesAvailableFundsRetriever(0.97);
            var irregularInterestRemittanceAvailableFundsRetriever = new IrregularInterestRemittanceAvailableFundsRetriever(new DateTime(2017, 3, 20));
            var allFundsAvailableFundsRetriever = new AllFundsAvailableFundsRetriever();

            #region Reserves Definition Region
            // Liquidity Reserve Account
            var liquidityReserveAccount = new PercentOfCollateralBalanceCappedReserveFundTranche("Liquidity Reserve Account",
                20000,
                irregularInterestRemittanceAvailableFundsRetriever)
            {
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1
            };

            liquidityReserveAccount.AddReserveFundBalanceCap(new DateTime(2016, 12, 15), 0.01);
            liquidityReserveAccount.AddReserveFundBalanceCap(new DateTime(2017, 3, 31), 0.02);
            liquidityReserveAccount.AddReserveFundBalanceFloor(new DateTime(2016, 12, 15), 0);
            liquidityReserveAccount.AddReserveFundBalanceFloor(new DateTime(2017, 9, 20), 3000000);
            #endregion Reserves
            #region Notes & Residual Definition Region
            // Residual Certificates
            var residualCertificate = new ResidualTranche("Preferred Shares",
                                new YieldBasedPricingStrategy(DayCountConvention.Actual365, CompoundingConvention.Annually, 0.12553),
                                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
            };

            residualCertificate.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Payment);
            residualCertificate.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.PaymentShortfall);

            // Subordinated Notes
            var classBNote = new FixedRateTranche("Class B",
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.05150),
                                allFundsAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.04990))
            {
                CurrentBalance = 44500000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2016, 12, 15),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 3, 20),
                MonthsToNextInterestPayment = 3,
                InterestPaymentFrequencyInMonths = 6,
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                AbsorbsRemainingAvailableFunds = true,
                AbsorbsAssociatedReservesReleased = true,
                IncludePaymentShortfall = true,
                IncludeInterestShortfall = true
            };

            classBNote.InitialBalance = classBNote.CurrentBalance.Value;
            classBNote.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Principal);
            classBNote.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.PrincipalShortfall);
            classBNote.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Interest);
            classBNote.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.InterestShortfall);
            classBNote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Principal);
            classBNote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.PrincipalShortfall);
            classBNote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Interest);
            classBNote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.InterestShortfall);

            // Senior Notes
            var classA1Note = new FixedRateTranche("Class A1",
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.03578),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.03570))
            {
                CurrentBalance = 140000000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2016, 12, 15),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 3, 20),
                MonthsToNextInterestPayment = 3,
                InterestPaymentFrequencyInMonths = 6,
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1,
                IncludePaymentShortfall = true,
                IncludeInterestShortfall = true
            };

            classA1Note.InitialBalance = classA1Note.CurrentBalance.Value;
            classA1Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Interest);
            classA1Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.InterestShortfall);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Principal);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.PrincipalShortfall);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Interest);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.InterestShortfall);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Principal);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.PrincipalShortfall);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Interest);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.InterestShortfall);

            var classA2Note = new FixedRateTranche("Class A2",
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.03828),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.04290))
            {
                CurrentBalance = 143634000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2016, 12, 15),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 3, 20),
                MonthsToNextInterestPayment = 3,
                InterestPaymentFrequencyInMonths = 6,
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1,
                IncludePaymentShortfall = true,
                IncludeInterestShortfall = true
            };

            classA2Note.InitialBalance = classA2Note.CurrentBalance.Value;
            classA2Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Interest);
            classA2Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.InterestShortfall);
            classA2Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Principal);
            classA2Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.PrincipalShortfall);
            classA2Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Interest);
            classA2Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.InterestShortfall);
            classA2Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Principal);
            classA2Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.PrincipalShortfall);
            classA2Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Interest);
            classA2Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.InterestShortfall);
            #endregion Notes & Residual Definition Region
            #region Fees Definition Region
            // Senior Fees
            var absNoteTrusteeFee = new BondCountBasedFeeTranche("ABS Note Trustee Fee",
                25.0 * Constants.MonthsInOneYear,
                500.0 * Constants.MonthsInOneYear,
                2000.0 * Constants.MonthsInOneYear,
                new DateTime(2017, 3, 20),
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                PaymentConvention.ProRated,
                DayCountConvention.Actual360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            absNoteTrusteeFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            absNoteTrusteeFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            absNoteTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            absNoteTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            absNoteTrusteeFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            absNoteTrusteeFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            absNoteTrusteeFee.AddBaseFee("Administration", 1250.0 * Constants.MonthsInOneYear);
            absNoteTrusteeFee.AddBaseFee("Reporting", 5000.0);

            var spvAdministrationFee = new PeriodicallyIncreasingFeeTranche("SPV Administration Fee", 0.03, 60,
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            spvAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            spvAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            spvAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            spvAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            spvAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            spvAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            spvAdministrationFee.AddIncreasingFee("Class A Administration", 6750 * 2);
            spvAdministrationFee.AddIncreasingFee("Class A FATCA Administrator", 750 * 2);
            spvAdministrationFee.AddIncreasingFee("Class A Registrated Office", 750 * 2);
            spvAdministrationFee.AddDelayedFee("Government", 426.83 * 2, new DateTime(2018, 1, 1));

            var portfolioAdministrationFee = new FlatFeeTranche("Portfolio Administration Fee",
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            portfolioAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            portfolioAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            portfolioAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            portfolioAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            portfolioAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            portfolioAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            portfolioAdministrationFee.AddBaseFee("Portfolio Administration", 15000);

            // Subordinated Fees
            var subordinatedAbsNoteTrusteeFee = new FlatFeeTranche("Sub ABS Note Trustee Fee",
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            subordinatedAbsNoteTrusteeFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            subordinatedAbsNoteTrusteeFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            subordinatedAbsNoteTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            subordinatedAbsNoteTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            subordinatedAbsNoteTrusteeFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            subordinatedAbsNoteTrusteeFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            subordinatedAbsNoteTrusteeFee.AddBaseFee("Class B ABS Note Trustee", 8000);
            subordinatedAbsNoteTrusteeFee.AddBaseFee("Preferred Share Paying Agent", 3000);

            var subordinatedSpvAdministrationFee = new PeriodicallyIncreasingFeeTranche("Sub SPV Administration Fee", 0.03, 60,
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            subordinatedSpvAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            subordinatedSpvAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            subordinatedSpvAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            subordinatedSpvAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            subordinatedSpvAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            subordinatedSpvAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            subordinatedSpvAdministrationFee.AddIncreasingFee("Class B Administration", 4250 * 2);
            subordinatedSpvAdministrationFee.AddIncreasingFee("Class B FATCA Administrator", 750 * 2);
            subordinatedSpvAdministrationFee.AddIncreasingFee("Class B Registrated Office", 750 * 2);
            subordinatedSpvAdministrationFee.AddIncreasingFee("Preferred Share Registrar", 250 * 2);
            subordinatedSpvAdministrationFee.AddDelayedFee("Government", 426.83 * 2, new DateTime(2018, 1, 1));

            // RA Advance Fees
            var raAdvanceFee = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche("RA Advance Fee", 0.00015, 12,
                new DateTime(2017, 12, 31),
                DayCountConvention.Actual360,
                DayCountConvention.Thirty360,
                PaymentConvention.ProRated,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 3,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            raAdvanceFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            raAdvanceFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            raAdvanceFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            raAdvanceFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            raAdvanceFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            raAdvanceFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);
            #endregion Fees Definition Region

            // Securitization Node Structure
            var securitizationNodes = new List<SecuritizationNodeTree>
            {
                new SecuritizationNodeTree(new InitialBalanceProRataDistributionRule())
                {
                    SecuritizationNodeName = "Senior",
                    SecuritizationTranches = new List<Tranche>
                    {
                        classA1Note,
                        classA2Note
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Mezzanine",
                    SecuritizationTranches = new List<Tranche>
                    {
                        classBNote
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Residual",
                    SecuritizationTranches = new List<Tranche>
                    {
                        residualCertificate
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Reserve Accounts",
                    SecuritizationTranches = new List<Tranche>
                    {
                       liquidityReserveAccount,
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Senior Fees",
                    SecuritizationTranches = new List<Tranche>
                    {
                       absNoteTrusteeFee,
                       spvAdministrationFee,
                       portfolioAdministrationFee
                    }
                },
                new SecuritizationNodeTree(new NextFeePaymentProRataDistributionRule())
                {
                    SecuritizationNodeName = "Subordinated Fees",
                    SecuritizationTranches = new List<Tranche>
                    {
                        subordinatedAbsNoteTrusteeFee,
                        subordinatedSpvAdministrationFee
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "RA Advance Fees",
                    SecuritizationTranches = new List<Tranche>
                    {
                        raAdvanceFee
                    }
                },
            };

            // Redemption Logic
            redemptionLogic = new TranchesCanBePaidOutFromAvailableFundsRedemptionLogic()
            {
                ListOfTranchesToBePaidOut = new List<Tranche>
                {
                    absNoteTrusteeFee,
                    spvAdministrationFee,
                    portfolioAdministrationFee,

                    subordinatedAbsNoteTrusteeFee,
                    subordinatedSpvAdministrationFee,
                    raAdvanceFee,

                    classA1Note,
                    classA2Note,
                    classBNote
                }
            };

            return securitizationNodes;
        }
    }
}
