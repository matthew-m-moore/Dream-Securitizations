using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using Dream.IO.Excel;
using Dream.IntegrationTests.Utilities;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class Hero_2017_1_SecuritizationCashFlowTieOut_Preliminary
    {
        private const string _inputsFile = "Dream.IntegrationTests.Resources.HERO-2017-1-Assessment-Level-Inputs-Prelim.xlsx";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest()
        {
            var precision = 0.01;

            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var paceSecuritization = GetSecuritization(inputFileStream);
            var securitizationResult = paceSecuritization.RunSecuritizationAnalysis();

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResult);

            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(17684017163.00, totalEndingBalance, precision);
            Assert.AreEqual( 85123008.40, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.86, totalPricipal, precision);
            Assert.AreEqual(122640068.74, totalInterest, precision);
            Assert.AreEqual(362304253.60, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual( 85123008.40, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.86, totalPricipal, precision);
            Assert.AreEqual(122640068.74, totalInterest, precision);
            Assert.AreEqual(362304253.60, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(9765467124.59, classA1EndingBalance, precision);
            Assert.AreEqual(132474000.00, classA1Principal, precision);
            Assert.AreEqual(14208606.74, classA1Interest, precision);
            Assert.AreEqual(146682606.74, classA1CashFlow, precision);
            Assert.AreEqual(280, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(7371610372.29, classA2EndingBalance, precision);
            Assert.AreEqual(100000000.00, classA2Principal, precision);
            Assert.AreEqual(15260970.14, classA2Interest, precision);
            Assert.AreEqual(115260970.14, classA2CashFlow, precision);
            Assert.AreEqual(280, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(784410020.77, classBEndingBalance, precision);
            Assert.AreEqual(36000000.00, classBPrincipal, precision);
            Assert.AreEqual(3236888.34, classBInterest, precision);
            Assert.AreEqual(39236888.34, classBCashFlow, precision);
            Assert.AreEqual(41, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(58797061.15, residualCashFlow, precision);
            Assert.AreEqual(41, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(305, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2020, 9, 20).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
            Assert.AreEqual(new DateTime(2042, 9, 20).Ticks, lastPeriodOfResidualCashFlow.PeriodDate.Ticks);
            #endregion Notes Tie-Out
            #region Fees Tie-Out
            var caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(21768.08, caymanGovernmentFee, precision);
            Assert.AreEqual(1068222.22, absNoteTrusteeFee, precision);
            Assert.AreEqual(76208.33, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(542506.32, spvAdministrationFee, precision);
            Assert.AreEqual(382500.00, portfolioAdministrationFee, precision);
            Assert.AreEqual(235522.29, raAdvanceFee, precision);

            caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Average(c => c.Payment);
            absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Average(c => c.Payment);
            spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(70.91, caymanGovernmentFee, precision);
            Assert.AreEqual(3479.55, absNoteTrusteeFee, precision);
            Assert.AreEqual(248.24, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(1767.12, spvAdministrationFee, precision);
            Assert.AreEqual(1245.93, portfolioAdministrationFee, precision);
            Assert.AreEqual(767.17, raAdvanceFee, precision);

            // This is the check that fee accrual logic accural portions out the fees at redemption
            caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Where(c => c.Payment > 0).Min(t => t.Payment);
            absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Where(c => c.Payment > 0).Min(t => t.Payment);
            preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Where(c => c.Payment > 0).Min(t => t.Payment);
            spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Where(c => c.Payment > 0).Min(t => t.Payment);
            portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Where(c => c.Payment > 0).Min(t => t.Payment);
            raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Where(c => c.Payment > 0).Min(t => t.Payment);

            Assert.AreEqual(71.14, caymanGovernmentFee, precision);
            Assert.AreEqual(2841.67, absNoteTrusteeFee, precision);
            Assert.AreEqual(250, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(1875.85, spvAdministrationFee, precision);
            Assert.AreEqual(1250.00, portfolioAdministrationFee, precision);
            Assert.AreEqual(18.77, raAdvanceFee, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(497920804.32, reserveAccountEndingBalance, precision);
            Assert.AreEqual(4339315.10, reserveAccountReleases, precision);
            Assert.AreEqual(4339315.10, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_WithRestrictedRedemptionLogic()
        {
            var precision = 0.01;

            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var paceSecuritization = GetSecuritization(inputFileStream);

            foreach (var redemptionLogic in paceSecuritization.RedemptionLogicList)
            {
                redemptionLogic.AddAllowedMonthForRedemption(Month.March);
                redemptionLogic.AddAllowedMonthForRedemption(Month.September);
            }

            var securitizationResult = paceSecuritization.RunSecuritizationAnalysis();

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResult);

            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(17684017163.00, totalEndingBalance, precision);
            Assert.AreEqual(85123008.40, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.86, totalPricipal, precision);
            Assert.AreEqual(122640068.74, totalInterest, precision);
            Assert.AreEqual(362304253.60, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(85123008.40, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.86, totalPricipal, precision);
            Assert.AreEqual(122640068.74, totalInterest, precision);
            Assert.AreEqual(362304253.60, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(9766240804.43, classA1EndingBalance, precision);
            Assert.AreEqual(132474000.00, classA1Principal, precision);
            Assert.AreEqual(14209735.02, classA1Interest, precision);
            Assert.AreEqual(146683735.02, classA1CashFlow, precision);
            Assert.AreEqual(281, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(7372194396.21, classA2EndingBalance, precision);
            Assert.AreEqual(100000000.00, classA2Principal, precision);
            Assert.AreEqual(15262181.99, classA2Interest, precision);
            Assert.AreEqual(115262181.99, classA2CashFlow, precision);
            Assert.AreEqual(281, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(784410020.77, classBEndingBalance, precision);
            Assert.AreEqual(36000000.00, classBPrincipal, precision);
            Assert.AreEqual(3236888.34, classBInterest, precision);
            Assert.AreEqual(39236888.34, classBCashFlow, precision);
            Assert.AreEqual(41, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(58794721.012, residualCashFlow, precision);
            Assert.AreEqual(41, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(305, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2020, 9, 20).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
            Assert.AreEqual(new DateTime(2042, 9, 20).Ticks, lastPeriodOfResidualCashFlow.PeriodDate.Ticks);
            #endregion Notes Tie-Out
            #region Fees Tie-Out
            var caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(21768.08, caymanGovernmentFee, precision);
            Assert.AreEqual(1068222.22, absNoteTrusteeFee, precision);
            Assert.AreEqual(76208.33, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(542506.32, spvAdministrationFee, precision);
            Assert.AreEqual(382500.00, portfolioAdministrationFee, precision);
            Assert.AreEqual(235522.29, raAdvanceFee, precision);

            caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Average(c => c.Payment);
            absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Average(c => c.Payment);
            spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(70.91, caymanGovernmentFee, precision);
            Assert.AreEqual(3479.55, absNoteTrusteeFee, precision);
            Assert.AreEqual(248.24, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(1767.12, spvAdministrationFee, precision);
            Assert.AreEqual(1245.93, portfolioAdministrationFee, precision);
            Assert.AreEqual(767.17, raAdvanceFee, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(499320804.32, reserveAccountEndingBalance, precision);
            Assert.AreEqual(4339315.10, reserveAccountReleases, precision);
            Assert.AreEqual(4339315.10, reserveAccountContributions, precision);
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
                    new PriorityOfPaymentsEntry( 1, "Cayman Governmental Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "Pref Shares Paying Agency Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 5, "Portfolio Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 6, "Class A1", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 6, "Class A2", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 7, "Class A1", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 7, "Class A2", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 8, "Liquidity Reserve Account", TrancheCashFlowType.Reserves),
                    new PriorityOfPaymentsEntry( 9, "Cayman Governmental Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(10, "ABS Note Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(11, "Pref Shares Paying Agency Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(12, "SPV Administration Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(13, "Portfolio Administration Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(14, "RA Advance Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry(15, "Class B",  TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry(16, "Class B",  TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry(17, "Preferred Shares", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static PriorityOfPayments GetRedemptionPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "Cayman Governmental Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "Pref Shares Paying Agency Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 5, "Portfolio Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 6, "Class A1", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 6, "Class A2", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 7, "Class A1", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 8, "Class A2", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 8, "Cayman Governmental Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry( 9, "ABS Note Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(10, "Pref Shares Paying Agency Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(11, "SPV Administration Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(12, "Portfolio Administration Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(13, "RA Advance Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry(14, "Class B",  TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry(15, "Class B",  TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry(16, "Preferred Shares", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static List<SecuritizationNodeTree> GetSecuritizationNodesAndRedemptionLogic(out RedemptionLogic redemptionLogic)
        {
            var principalRemittancesAvailableFundsRetriever = new PrincipalRemittancesAvailableFundsRetriever(0.97);
            var irregularInterestRemittanceAvailableFundsRetriever = new IrregularInterestRemittanceAvailableFundsRetriever(new DateTime(2017, 9, 20));
            var allFundsAvailableFundsRetriever = new AllFundsAvailableFundsRetriever();

            #region Reserves Definition Region
            // Liquidity Reserve Account
            var liquidityReserveAccount = new PercentOfCollateralBalanceCappedReserveFundTranche("Liquidity Reserve Account", 0,
                irregularInterestRemittanceAvailableFundsRetriever)
            {
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1
            };

            liquidityReserveAccount.AddReserveFundBalanceCap(new DateTime(2017, 4, 28), 0.01);
            liquidityReserveAccount.AddReserveFundBalanceCap(new DateTime(2017, 9, 30), 0.02);
            liquidityReserveAccount.AddReserveFundBalanceFloor(new DateTime(2017, 4, 28), 0);
            liquidityReserveAccount.AddReserveFundBalanceFloor(new DateTime(2018, 3, 20), 1400000);
            #endregion Reserves
            #region Notes & Residual Definition Region
            // Residual Certificates
            var residualCertificate = new ResidualTranche("Preferred Shares",
                                new YieldBasedPricingStrategy(DayCountConvention.Actual365, CompoundingConvention.Annually, 0.12000),
                                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6,
            };

            residualCertificate.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Payment);
            residualCertificate.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.PaymentShortfall);

            // Subordinated Notes
            var classBNote = new FixedRateTranche("Class B",
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.05350),
                                allFundsAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.04990))
            {
                CurrentBalance = 36000000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2017, 4, 28),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 9, 20),
                MonthsToNextInterestPayment = 5,
                InterestPaymentFrequencyInMonths = 6,
                MonthsToNextPayment = 5,
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
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.01750),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.01750))
            {
                CurrentBalance = 132474000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2017, 4, 28),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 9, 20),
                MonthsToNextInterestPayment = 5,
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
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.02000),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.02490))
            {
                CurrentBalance = 100000000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2017, 4, 28),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 9, 20),
                MonthsToNextInterestPayment = 5,
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
            var caymanGovernmentalFee = new FlatFeeTranche("Cayman Governmental Fee",
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6
            };

            caymanGovernmentalFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            caymanGovernmentalFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            caymanGovernmentalFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            caymanGovernmentalFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            caymanGovernmentalFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            caymanGovernmentalFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            caymanGovernmentalFee.AddBaseFee("Cayman Government", 853.65);

            var absNoteTrusteeFee = new BondCountBasedFeeTranche("ABS Note Trustee Fee",
                25.0 * Constants.MonthsInOneYear,
                500.0 * Constants.MonthsInOneYear,
                2000.0 * Constants.MonthsInOneYear,
                new DateTime(2017, 9, 20),
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                PaymentConvention.ProRated,
                DayCountConvention.Actual360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6
            };

            absNoteTrusteeFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            absNoteTrusteeFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            absNoteTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            absNoteTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            absNoteTrusteeFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            absNoteTrusteeFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            absNoteTrusteeFee.AddBaseFee("Administration", 1250.0 * Constants.MonthsInOneYear);
            absNoteTrusteeFee.AddBaseFee("Reporting", 5000.0);

            var preferredSharesPayingAgencyFee = new InitialPeriodSpecialAccrualFeeTranche("Pref Shares Paying Agency Fee",
                new DateTime(2017, 9, 20),
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                PaymentConvention.ProRated,
                DayCountConvention.Actual360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6
            };

            preferredSharesPayingAgencyFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            preferredSharesPayingAgencyFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            preferredSharesPayingAgencyFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            preferredSharesPayingAgencyFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            preferredSharesPayingAgencyFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            preferredSharesPayingAgencyFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            preferredSharesPayingAgencyFee.AddBaseFee("Preferred Shares Paying Agency", 1500 * 2);

            var spvAdministrationFee = new PeriodicallyIncreasingFeeTranche("SPV Administration Fee", 0.03, 60,
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6
            };

            spvAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            spvAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            spvAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            spvAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            spvAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            spvAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            spvAdministrationFee.AddIncreasingFee("Administration", 8250 * 2);
            spvAdministrationFee.AddIncreasingFee("Registered Office", 750 * 2);
            spvAdministrationFee.AddIncreasingFee("FATCA Administrator", 750 * 2);
            spvAdministrationFee.AddIncreasingFee("Preferred Shares Registrar", 250 * 2);

            var portfolioAdministrationFee = new FlatFeeTranche("Portfolio Administration Fee",
                PaymentConvention.SemiAnnual,
                DayCountConvention.Thirty360,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
                PaymentFrequencyInMonths = 6
            };

            portfolioAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Fees);
            portfolioAdministrationFee.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.FeesShortfall);
            portfolioAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            portfolioAdministrationFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            portfolioAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Fees);
            portfolioAdministrationFee.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.FeesShortfall);

            portfolioAdministrationFee.AddBaseFee("Portfolio Administration", 15000);

            // RA Advance Fees
            var raAdvanceFee = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche("RA Advance Fee", 0.00015, 12,
                new DateTime(2017, 12, 31),
                DayCountConvention.Actual360,
                DayCountConvention.Thirty360,
                PaymentConvention.SemiAnnual,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 5,
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
                       caymanGovernmentalFee,
                       absNoteTrusteeFee,
                       preferredSharesPayingAgencyFee,
                       spvAdministrationFee,
                       portfolioAdministrationFee
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
                    caymanGovernmentalFee,
                    absNoteTrusteeFee,
                    preferredSharesPayingAgencyFee,
                    spvAdministrationFee,
                    portfolioAdministrationFee,

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
