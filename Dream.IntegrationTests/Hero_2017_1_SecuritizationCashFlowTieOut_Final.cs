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
using Dream.IntegrationTests.Utilities;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Database;
using Dream.Core.Savers;
using Dream.Core.Savers.SaveManagers;
using Dream.IO.Database;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class Hero_2017_1_SecuritizationCashFlowTieOut_Final
    {
        private const string _inputsFile = "Dream.IntegrationTests.Resources.HERO-2017-1-Assessment-Level-Inputs-Final.xlsx";
        private const string _fullInputsFile = "Dream.IntegrationTests.Resources.HERO-2017-1-Dream-Excel-Application.xlsm";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest()
        {
            var precision = 1.00;

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var paceSecuritization = GetSecuritization(inputsFileStream);
            var securitizationResult = paceSecuritization.RunSecuritizationAnalysis();

            CheckResults12Cpr(securitizationResult, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_AfterUsingCopy()
        {
            var precision = 1.00;

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var paceSecuritization = GetSecuritization(inputsFileStream);

            var copiedPaceSecuritization = paceSecuritization.Copy();
            var securitizationResult = copiedPaceSecuritization.RunSecuritizationAnalysis();

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResult);

            CheckResults12Cpr(securitizationResult, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_WithScenarios()
        {
            var precision = 1.50;

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFileStream);
            var paceSecuritization = GetSecuritization(securitizationDataRepository);

            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            var securitizationResult10Cpr = securitizationResultsDictionary["10% CPR"];
            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            CheckResults10Cpr(securitizationResult10Cpr, precision);
            CheckResults12Cpr(securitizationResult12Cpr, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_FullLoadingFromExcel()
        {
            var precision = 0.01;

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_fullInputsFile);
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFileStream);
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritization();

            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            var securitizationResult10Cpr = securitizationResultsDictionary["10% CPR"];
            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];

            CheckResults10Cpr(securitizationResult10Cpr, precision);
            CheckResults12Cpr(securitizationResult12Cpr, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_FullLoadingFromDatabase()
        {
            var precision = 0.01;
            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);

            var securitizationDataRepository = new SecuritizationDatabaseRepository(1000, 1);
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritization();

            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult10Cpr = securitizationResultsDictionary["10% CPR"];
            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];

            CheckResults10Cpr(securitizationResult10Cpr, precision);
            CheckResults12Cpr(securitizationResult12Cpr, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_SaveToDatabase()
        {
            var precision = 0.01;
            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_fullInputsFile);
            var securitizationExcelDataRepository = new SecuritizationExcelDataRepository(inputsFileStream);
            var excelPaceSecuritization = securitizationExcelDataRepository.GetPaceSecuritization();
            var scenariosToAnalyze = securitizationExcelDataRepository.ScenariosToAnalyze;

            excelPaceSecuritization.Description = _fullInputsFile;
            excelPaceSecuritization.RunSecuritizationAnalysis(scenariosToAnalyze);

            var securitizationDatabaseSaver = new SecuritizationDatabaseSaver(excelPaceSecuritization, scenariosToAnalyze);
            var paceSecuritizationSaveManager = new PaceSecurititizationSaveManager(securitizationDatabaseSaver);

            paceSecuritizationSaveManager.SaveSecuritization();

            var securitizationAnalysisDataSetId = paceSecuritizationSaveManager.SecuritizationAnalysisDataSetId;
            var securitizationAnalysisVersionId = paceSecuritizationSaveManager.SecuritizationAnalysisVersionId;

            var securitizationDataRepository = new SecuritizationDatabaseRepository(securitizationAnalysisDataSetId, securitizationAnalysisVersionId);
            var databasePaceSecuritization = securitizationDataRepository.GetPaceSecuritization();

            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = databasePaceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult10Cpr = securitizationResultsDictionary["10% CPR"];
            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];

            CheckResults10Cpr(securitizationResult10Cpr, precision);
            CheckResults12Cpr(securitizationResult12Cpr, precision);
        }

        public void CheckResults10Cpr(SecuritizationResult securitizationResult, double precision)
        {
            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(17684017162.14, totalEndingBalance, precision);
            Assert.AreEqual(85123008.40, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.85, totalPricipal, precision);
            Assert.AreEqual(122640068.74, totalInterest, precision);
            Assert.AreEqual(362304253.58, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(85123008.40, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.85, totalPricipal, precision);
            Assert.AreEqual(122640068.74, totalInterest, precision);
            Assert.AreEqual(362304253.58, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(9250187197.72, classA1EndingBalance, precision);
            Assert.AreEqual(125474000.00, classA1Principal, precision);
            Assert.AreEqual(28532835.56, classA1Interest, precision);
            Assert.AreEqual(154006835.56, classA1CashFlow, precision);
            Assert.AreEqual(281, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(7888248004.81, classA2EndingBalance, precision);
            Assert.AreEqual(107000000.00, classA2Principal, precision);
            Assert.AreEqual(29250676.66, classA2Interest, precision);
            Assert.AreEqual(136250676.66, classA2CashFlow, precision);
            Assert.AreEqual(281, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(1185501533.68, classBEndingBalance, precision);
            Assert.AreEqual(36000000.00, classBPrincipal, precision);
            Assert.AreEqual(4904760.54, classBInterest, precision);
            Assert.AreEqual(40904760.54, classBCashFlow, precision);
            Assert.AreEqual(65, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(28815253.56, residualCashFlow, precision);
            Assert.AreEqual(65, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(305, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2022, 9, 20).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
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

            Assert.AreEqual(499314255.77, reserveAccountEndingBalance, precision);
            Assert.AreEqual(4341199.93, reserveAccountReleases, precision);
            Assert.AreEqual(4341199.93, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out

            #region Results Tie-Out
            var classA1Results = securitizationResult.SecuritizationResultsDictionary["Class A1"];
            Assert.AreEqual(classA1Results.DollarPrice.Value, 0.99959795, 0.00000001);
            Assert.AreEqual(classA1Results.PresentValue, 125423553.14, 0.01);
            Assert.AreEqual(classA1Results.InternalRateOfReturn, 0.03724, 0.00001);
            Assert.AreEqual(classA1Results.WeightedAverageLife, 6.121273, 0.000001);
            Assert.AreEqual(classA1Results.ForwardWeightedAverageCoupon, 0.037014821, 0.000000001);
            Assert.AreEqual(classA1Results.MacaulayDuration, 5.1782658, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationAnalytical, 5.0836090, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationNumerical, 5.0842267, 0.0000001);
            Assert.AreEqual(classA1Results.NominalBenchmarkRate.Value, 0.0197422, 0.0000001);
            Assert.AreEqual(classA1Results.NominalSpread.Value, 0.0174978, 0.0000001);
            Assert.AreEqual(classA1Results.TotalPrincipal, classA1Principal, 0.01);
            Assert.AreEqual(classA1Results.TotalInterest, classA1Interest, 0.01);
            Assert.AreEqual(classA1Results.TotalCashFlow, classA1CashFlow, 0.01);

            var classA2Results = securitizationResult.SecuritizationResultsDictionary["Class A2"];
            Assert.AreEqual(classA2Results.DollarPrice.Value, 1.02480419, 0.00000001);
            Assert.AreEqual(classA2Results.PresentValue, 109654048.30, 0.01);
            Assert.AreEqual(classA2Results.InternalRateOfReturn, 0.03974, 0.00001);
            Assert.AreEqual(classA2Results.WeightedAverageLife, 6.121273, 0.000001);
            Assert.AreEqual(classA2Results.ForwardWeightedAverageCoupon, 0.044497602, 0.000000001);
            Assert.AreEqual(classA2Results.MacaulayDuration, 5.1138557, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationAnalytical, 5.0142231, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationNumerical, 5.0148196, 0.0000001);
            Assert.AreEqual(classA2Results.NominalBenchmarkRate.Value, 0.0197422, 0.0000001);
            Assert.AreEqual(classA2Results.NominalSpread.Value, 0.0199978, 0.0000001);
            Assert.AreEqual(classA2Results.TotalPrincipal, classA2Principal, 0.01);
            Assert.AreEqual(classA2Results.TotalInterest, classA2Interest, 0.01);
            Assert.AreEqual(classA2Results.TotalCashFlow, classA2CashFlow, 0.01);

            var classBResults = securitizationResult.SecuritizationResultsDictionary["Class B"];
            Assert.AreEqual(classBResults.DollarPrice.Value, 0.99406799, 0.00000001);
            Assert.AreEqual(classBResults.PresentValue, 35786447.66, 0.01);
            Assert.AreEqual(classBResults.InternalRateOfReturn, 0.0525, 0.00001);
            Assert.AreEqual(classBResults.WeightedAverageLife, 2.721994, 0.000001);
            Assert.AreEqual(classBResults.ForwardWeightedAverageCoupon, 0.049647449, 0.000000001);
            Assert.AreEqual(classBResults.MacaulayDuration, 2.5249480, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationAnalytical, 2.4603635, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationNumerical, 2.4604084, 0.0000001);
            Assert.AreEqual(classBResults.NominalBenchmarkRate.Value, 0.0163041, 0.0000001);
            Assert.AreEqual(classBResults.NominalSpread.Value, 0.0361959, 0.0000001);
            Assert.AreEqual(classBResults.TotalPrincipal, classBPrincipal, 0.01);
            Assert.AreEqual(classBResults.TotalInterest, classBInterest, 0.01);
            Assert.AreEqual(classBResults.TotalCashFlow, classBCashFlow, 0.01);

            var residualResults = securitizationResult.SecuritizationResultsDictionary["Preferred Shares"];
            Assert.AreEqual(residualResults.PresentValue, 10036450.98, 0.01);
            Assert.AreEqual(residualResults.InternalRateOfReturn, 0.1200, 0.00001);
            Assert.AreEqual(residualResults.WeightedAverageLife, 10.245050, 0.000001);
            Assert.AreEqual(residualResults.MacaulayDuration, 8.5780218, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationAnalytical, 7.6589480, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationNumerical, 7.6598475, 0.0000001);
            Assert.AreEqual(residualResults.TotalCashFlow, residualCashFlow, 0.01);
            #endregion Results Tie-Out
        }

        public void CheckResults12Cpr(SecuritizationResult securitizationResult, double precision)
        {
            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(15848471583.75, totalEndingBalance, precision);
            Assert.AreEqual(71792196.80, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.85, totalPricipal, precision);
            Assert.AreEqual(110025731.84, totalInterest, precision);
            Assert.AreEqual(349689916.68, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(71792196.80, totalScheduledPrincipal, precision);
            Assert.AreEqual(239664184.85, totalPricipal, precision);
            Assert.AreEqual(110025731.84, totalInterest, precision);
            Assert.AreEqual(349689916.68, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(8279171351.45, classA1EndingBalance, precision);
            Assert.AreEqual(125474000.00, classA1Principal, precision);
            Assert.AreEqual(25530591.07, classA1Interest, precision);
            Assert.AreEqual(151004591.07, classA1CashFlow, precision);
            Assert.AreEqual(257, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(7060198404.49, classA2EndingBalance, precision);
            Assert.AreEqual(107000000.00, classA2Principal, precision);
            Assert.AreEqual(26172900.44, classA2Interest, precision);
            Assert.AreEqual(133172900.44, classA2CashFlow, precision);
            Assert.AreEqual(257, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(1202513772.82, classBEndingBalance, precision);
            Assert.AreEqual(36000000.00, classBPrincipal, precision);
            Assert.AreEqual(4975503.10, classBInterest, precision);
            Assert.AreEqual(40975503.10, classBCashFlow, precision);
            Assert.AreEqual(71, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(22232743.12, residualCashFlow, precision);
            Assert.AreEqual(71, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(305, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2023, 3, 20).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
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
            Assert.AreEqual(212973.99, raAdvanceFee, precision);

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
            Assert.AreEqual(693.73, raAdvanceFee, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(449217858.71, reserveAccountEndingBalance, precision);
            Assert.AreEqual(4250839.14, reserveAccountReleases, precision);
            Assert.AreEqual(4250839.14, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out

            #region Results Tie-Out
            var classA1Results = securitizationResult.SecuritizationResultsDictionary["Class A1"];
            Assert.AreEqual(classA1Results.DollarPrice.Value, 0.99966072, 0.00000001);
            Assert.AreEqual(classA1Results.PresentValue, 125431429.30, 0.01);
            Assert.AreEqual(classA1Results.InternalRateOfReturn, 0.03724, 0.00001);
            Assert.AreEqual(classA1Results.WeightedAverageLife, 5.476375, 0.000001);
            Assert.AreEqual(classA1Results.ForwardWeightedAverageCoupon, 0.037004560, 0.000000001);
            Assert.AreEqual(classA1Results.MacaulayDuration, 4.6998505, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationAnalytical, 4.6139390, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationNumerical, 4.6144245, 0.0000001);
            Assert.AreEqual(classA1Results.NominalBenchmarkRate.Value, 0.0192363, 0.0000001);
            Assert.AreEqual(classA1Results.NominalSpread.Value, 0.0180037, 0.0000001);
            Assert.AreEqual(classA1Results.TotalPrincipal, classA1Principal, 0.01);
            Assert.AreEqual(classA1Results.TotalInterest, classA1Interest, 0.01);
            Assert.AreEqual(classA1Results.TotalCashFlow, classA1CashFlow, 0.01);

            var classA2Results = securitizationResult.SecuritizationResultsDictionary["Class A2"];
            Assert.AreEqual(classA2Results.DollarPrice.Value, 1.02256246, 0.00000001);
            Assert.AreEqual(classA2Results.PresentValue, 109414183.04, 0.01);
            Assert.AreEqual(classA2Results.InternalRateOfReturn, 0.03974, 0.00001);
            Assert.AreEqual(classA2Results.WeightedAverageLife, 5.476375, 0.000001);
            Assert.AreEqual(classA2Results.ForwardWeightedAverageCoupon, 0.044485266, 0.000000001);
            Assert.AreEqual(classA2Results.MacaulayDuration, 4.6481262, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationAnalytical, 4.5575674, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationNumerical, 4.5580377, 0.0000001);
            Assert.AreEqual(classA2Results.NominalBenchmarkRate.Value, 0.0192363, 0.0000001);
            Assert.AreEqual(classA2Results.NominalSpread.Value, 0.0205037, 0.0000001);
            Assert.AreEqual(classA2Results.TotalPrincipal, classA2Principal, 0.01);
            Assert.AreEqual(classA2Results.TotalInterest, classA2Interest, 0.01);
            Assert.AreEqual(classA2Results.TotalCashFlow, classA2CashFlow, 0.01);

            var classBResults = securitizationResult.SecuritizationResultsDictionary["Class B"];
            Assert.AreEqual(classBResults.DollarPrice.Value, 0.99398991, 0.00000001);
            Assert.AreEqual(classBResults.PresentValue, 35783636.92, 0.01);
            Assert.AreEqual(classBResults.InternalRateOfReturn, 0.0525, 0.00001);
            Assert.AreEqual(classBResults.WeightedAverageLife, 2.761374, 0.000001);
            Assert.AreEqual(classBResults.ForwardWeightedAverageCoupon, 0.049651022, 0.000000001);
            Assert.AreEqual(classBResults.MacaulayDuration, 2.5557365, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationAnalytical, 2.4903645, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationNumerical, 2.4904121, 0.0000001);
            Assert.AreEqual(classBResults.NominalBenchmarkRate.Value, 0.0163616, 0.0000001);
            Assert.AreEqual(classBResults.NominalSpread.Value, 0.0361384, 0.0000001);
            Assert.AreEqual(classBResults.TotalPrincipal, classBPrincipal, 0.01);
            Assert.AreEqual(classBResults.TotalInterest, classBInterest, 0.01);
            Assert.AreEqual(classBResults.TotalCashFlow, classBCashFlow, 0.01);

            var residualResults = securitizationResult.SecuritizationResultsDictionary["Preferred Shares"];
            Assert.AreEqual(residualResults.PresentValue, 7683847.32, 0.01);
            Assert.AreEqual(residualResults.InternalRateOfReturn, 0.1200, 0.00001);
            Assert.AreEqual(residualResults.WeightedAverageLife, 10.279646, 0.000001);
            Assert.AreEqual(residualResults.MacaulayDuration, 8.6783051, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationAnalytical, 7.7484867, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationNumerical, 7.7493970, 0.0000001);
            Assert.AreEqual(residualResults.TotalCashFlow, residualCashFlow, 0.01);
            #endregion Results Tie-Out
        }

        public static Securitization GetSecuritization(string inputsFilePath, out SecuritizationExcelDataRepository securitizationDataRepository)
        {
            securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFilePath);
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
            paceSecuritization.Inputs.MarketDataGroupingForNominalSpread = MarketDataGrouping.Swaps;

            return paceSecuritization;
        }

        private static PriorityOfPayments GetPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "Cayman Governmental Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "Pref Shares Paying Agency Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "Portfolio Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 5, "Class A1", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 5, "Class A2", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 6, "Class A1", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 6, "Class A2", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 7, "Liquidity Reserve Account", TrancheCashFlowType.Reserves),
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

        private static PriorityOfPayments GetRedemptionPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "Cayman Governmental Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "ABS Note Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "Pref Shares Paying Agency Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "SPV Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "Portfolio Administration Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 5, "Class A1", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 5, "Class A2", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 6, "Class A1", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 6, "Class A2", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 7, "Cayman Governmental Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry( 8, "ABS Note Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry( 9, "Pref Shares Paying Agency Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(10, "SPV Administration Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(11, "Portfolio Administration Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(12, "RA Advance Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry(13, "Class B",  TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry(14, "Class B",  TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry(15, "Preferred Shares", TrancheCashFlowType.Payment)
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
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.05250),
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
                IncludeInterestShortfall = true,
                IsShortfallPaidFromReserves = true,
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
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.03724),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.03710))
            {
                CurrentBalance = 125474000,
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

            classA1Note.InitialBalance = classA1Note.CurrentBalance.Value;
            classA1Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Interest);
            classA1Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.InterestShortfall);
            classA1Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Principal);
            classA1Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.PrincipalShortfall);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Principal);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.PrincipalShortfall);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Interest);
            classA1Note.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.InterestShortfall);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Principal);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.PrincipalShortfall);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.Interest);
            classA1Note.AddAssociatedReserveAccount(classBNote, TrancheCashFlowType.InterestShortfall);

            var classA2Note = new FixedRateTranche("Class A2",
                                new YieldBasedPricingStrategy(DayCountConvention.Thirty360, CompoundingConvention.SemiAnnually, 0.03974),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.04460))
            {
                CurrentBalance = 107000000,
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

            classA2Note.InitialBalance = classA2Note.CurrentBalance.Value;
            classA2Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Interest);
            classA2Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.InterestShortfall);
            classA2Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.Principal);
            classA2Note.AddAssociatedReserveAccount(liquidityReserveAccount, TrancheCashFlowType.PrincipalShortfall);
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
                    SecuritizationNodeName = "Senior Fees A",
                    SecuritizationTranches = new List<Tranche>
                    {
                       caymanGovernmentalFee,
                    }
                },
                new SecuritizationNodeTree(new NextFeePaymentProRataDistributionRule())
                {
                    SecuritizationNodeName = "Senior Fees B",
                    SecuritizationTranches = new List<Tranche>
                    {
                       absNoteTrusteeFee,
                       preferredSharesPayingAgencyFee,
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Senior Fees C",
                    SecuritizationTranches = new List<Tranche>
                    {
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
