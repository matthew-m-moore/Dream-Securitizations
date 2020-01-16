using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Database;
using Dream.Core.Repositories.Excel;
using Dream.Core.Savers;
using Dream.Core.Savers.SaveManagers;
using Dream.IntegrationTests.Utilities;
using Dream.IO.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class Hero_2017_2_SecuritizationCashFlowTieOut
    {
        private const string _fullInputsFile = "Dream.IntegrationTests.Resources.HERO-2017-2-Dream-Excel-Application.xlsm";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_FullLoadingFromExcel()
        {
            var precision = 0.01;

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_fullInputsFile);
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFileStream);
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritization();

            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];
            var securitizationResult15Cpr = securitizationResultsDictionary["15% CPR"];

            CheckResults12Cpr(securitizationResult12Cpr, precision);
            CheckResults15Cpr(securitizationResult15Cpr, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_FullLoadingFromDatabase()
        {
            var precision = 0.01;
            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);

            var securitizationDataRepository = new SecuritizationDatabaseRepository(1001, 1);
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritization();

            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];
            var securitizationResult15Cpr = securitizationResultsDictionary["15% CPR"];

            CheckResults12Cpr(securitizationResult12Cpr, precision);
            CheckResults15Cpr(securitizationResult15Cpr, precision);
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

            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];
            var securitizationResult15Cpr = securitizationResultsDictionary["15% CPR"];

            CheckResults12Cpr(securitizationResult12Cpr, precision);
            CheckResults15Cpr(securitizationResult15Cpr, precision);
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

            Assert.AreEqual(12329997342.51, totalEndingBalance, precision);
            Assert.AreEqual(57418357.97, totalScheduledPrincipal, precision);
            Assert.AreEqual(187461563.01, totalPricipal, precision);
            Assert.AreEqual(75839790.96, totalInterest, precision);
            Assert.AreEqual(263301353.97, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(57418357.97, totalScheduledPrincipal, precision);
            Assert.AreEqual(187461563.01, totalPricipal, precision);
            Assert.AreEqual(75839790.96, totalInterest, precision);
            Assert.AreEqual(263301353.97, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(5886160322.29, classA1EndingBalance, precision);
            Assert.AreEqual(91000000.00, classA1Principal, precision);
            Assert.AreEqual(16229787.10, classA1Interest, precision);
            Assert.AreEqual(107229787.10, classA1CashFlow, precision);
            Assert.AreEqual(265, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(5875616980.17, classA2EndingBalance, precision);
            Assert.AreEqual(90837000.00, classA2Principal, precision);
            Assert.AreEqual(20102717.93, classA2Interest, precision);
            Assert.AreEqual(110939717.93, classA2CashFlow, precision);
            Assert.AreEqual(265, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(670410666.76, classBEndingBalance, precision);
            Assert.AreEqual(23000000.00, classBPrincipal, precision);
            Assert.AreEqual(2916027.73, classBInterest, precision);
            Assert.AreEqual(25916027.73, classBCashFlow, precision);
            Assert.AreEqual(61, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(18714761.64, residualCashFlow, precision);
            Assert.AreEqual(61, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(313, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2022, 9, 20).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
            Assert.AreEqual(new DateTime(2043, 9, 20).Ticks, lastPeriodOfResidualCashFlow.PeriodDate.Ticks);
            #endregion Notes Tie-Out
            #region Fees Tie-Out
            var caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(22621.73, caymanGovernmentFee, precision);
            Assert.AreEqual(221305.56, absNoteTrusteeFee, precision);
            Assert.AreEqual(264688.33, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(530000.00, spvAdministrationFee, precision);
            Assert.AreEqual(397500.00, portfolioAdministrationFee, precision);
            Assert.AreEqual(164943.95, raAdvanceFee, precision);

            caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Average(c => c.Payment);
            absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Average(c => c.Payment);
            spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(71.82, caymanGovernmentFee, precision);
            Assert.AreEqual(702.56, absNoteTrusteeFee, precision);
            Assert.AreEqual(840.28, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(1682.54, spvAdministrationFee, precision);
            Assert.AreEqual(1261.90, portfolioAdministrationFee, precision);
            Assert.AreEqual(523.63, raAdvanceFee, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(355511246.64, reserveAccountEndingBalance, precision);
            Assert.AreEqual(3444614.61, reserveAccountReleases, precision);
            Assert.AreEqual(2344614.61, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out

            #region Results Tie-Out
            var classA1Results = securitizationResult.SecuritizationResultsDictionary["Class A1"];
            Assert.AreEqual(classA1Results.DollarPrice.Value, 0.999869677068997, 0.00000001);
            Assert.AreEqual(classA1Results.PresentValue, 90988140.61, 0.01);
            Assert.AreEqual(classA1Results.InternalRateOfReturn, 0.03285, 0.00001);
            Assert.AreEqual(classA1Results.WeightedAverageLife, 5.43470114983608, 0.000001);
            Assert.AreEqual(classA1Results.ForwardWeightedAverageCoupon, 0.0330873497448339, 0.000000001);
            Assert.AreEqual(classA1Results.MacaulayDuration, 4.75088481791946, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationAnalytical, 4.67411251978205, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationNumerical, 4.67461009361684, 0.0000001);
            Assert.AreEqual(classA1Results.NominalBenchmarkRate.Value, 0.0198529660923443, 0.0000001);
            Assert.AreEqual(classA1Results.NominalSpread.Value, 0.0129970339076551, 0.0000001);
            Assert.AreEqual(classA1Results.TotalPrincipal, classA1Principal, 0.01);
            Assert.AreEqual(classA1Results.TotalInterest, classA1Interest, 0.01);
            Assert.AreEqual(classA1Results.TotalCashFlow, classA1CashFlow, 0.01);

            var classA2Results = securitizationResult.SecuritizationResultsDictionary["Class A2"];
            Assert.AreEqual(classA2Results.DollarPrice.Value, 1.02487350368532, 0.00000001);
            Assert.AreEqual(classA2Results.PresentValue, 93096434.45, 0.01);
            Assert.AreEqual(classA2Results.InternalRateOfReturn, 0.03535, 0.00001);
            Assert.AreEqual(classA2Results.WeightedAverageLife, 5.43470114983608, 0.000001);
            Assert.AreEqual(classA2Results.ForwardWeightedAverageCoupon, 0.041056558982158, 0.000000001);
            Assert.AreEqual(classA2Results.MacaulayDuration, 4.69703706738794, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationAnalytical, 4.61545883252309, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationNumerical, 4.61594048626222, 0.0000001);
            Assert.AreEqual(classA2Results.NominalBenchmarkRate.Value, 0.0198529660923443, 0.0000001);
            Assert.AreEqual(classA2Results.NominalSpread.Value, 0.0154970339076557, 0.0000001);
            Assert.AreEqual(classA2Results.TotalPrincipal, classA2Principal, 0.01);
            Assert.AreEqual(classA2Results.TotalInterest, classA2Interest, 0.01);
            Assert.AreEqual(classA2Results.TotalCashFlow, classA2CashFlow, 0.01);

            var classBResults = securitizationResult.SecuritizationResultsDictionary["Class B"];
            Assert.AreEqual(classBResults.DollarPrice.Value, 0.999976535625942, 0.00000001);
            Assert.AreEqual(classBResults.PresentValue, 22999460.32, 0.01);
            Assert.AreEqual(classBResults.InternalRateOfReturn, 0.05130, 0.00001);
            Assert.AreEqual(classBResults.WeightedAverageLife, 2.47346859935508, 0.000001);
            Assert.AreEqual(classBResults.ForwardWeightedAverageCoupon, 0.0521953700019364, 0.000000001);
            Assert.AreEqual(classBResults.MacaulayDuration, 2.31560951352971, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationAnalytical, 2.25769952082066, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationNumerical, 2.2577340801567, 0.0000001);
            Assert.AreEqual(classBResults.NominalBenchmarkRate.Value, 0.0167623663772067, 0.0000001);
            Assert.AreEqual(classBResults.NominalSpread.Value, 0.0345376336227839, 0.0000001);
            Assert.AreEqual(classBResults.TotalPrincipal, classBPrincipal, 0.01);
            Assert.AreEqual(classBResults.TotalInterest, classBInterest, 0.01);
            Assert.AreEqual(classBResults.TotalCashFlow, classBCashFlow, 0.01);

            var residualResults = securitizationResult.SecuritizationResultsDictionary["Preferred Shares"];
            Assert.AreEqual(residualResults.PresentValue, 7098609.26, 0.01);
            Assert.AreEqual(residualResults.InternalRateOfReturn, 0.12000, 0.00001);
            Assert.AreEqual(residualResults.WeightedAverageLife, 9.45769780326435, 0.000001);
            Assert.AreEqual(residualResults.MacaulayDuration, 7.86562156890313, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationAnalytical, 7.02287640080636, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationNumerical, 7.02361125040607, 0.0000001);
            Assert.AreEqual(residualResults.TotalCashFlow, residualCashFlow, 0.01);
            #endregion Results Tie-Out
        }

        public void CheckResults15Cpr(SecuritizationResult securitizationResult, double precision)
        {
            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(10606186744.03, totalEndingBalance, precision);
            Assert.AreEqual(45398397.78, totalScheduledPrincipal, precision);
            Assert.AreEqual(187461563.01, totalPricipal, precision);
            Assert.AreEqual(65154805.73, totalInterest, precision);
            Assert.AreEqual(252616368.74, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(45398397.78, totalScheduledPrincipal, precision);
            Assert.AreEqual(187461563.01, totalPricipal, precision);
            Assert.AreEqual(65154805.73, totalInterest, precision);
            Assert.AreEqual(252616368.74, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var classA1EndingBalance = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA1Principal = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Principal);
            var classA1Interest = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Interest);
            var classA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA1CashFlow = trancheResultsDictionary["Class A1"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(5047273937.15, classA1EndingBalance, precision);
            Assert.AreEqual(91000000.00, classA1Principal, precision);
            Assert.AreEqual(13936830.98, classA1Interest, precision);
            Assert.AreEqual(104936830.98, classA1CashFlow, precision);
            Assert.AreEqual(241, lastPeriodOfClassA1CashFlow.Period);

            var classA2EndingBalance = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classA2Principal = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Principal);
            var classA2Interest = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Interest);
            var classA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassA2CashFlow = trancheResultsDictionary["Class A2"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(5038233215.70, classA2EndingBalance, precision);
            Assert.AreEqual(90837000.00, classA2Principal, precision);
            Assert.AreEqual(17262591.32, classA2Interest, precision);
            Assert.AreEqual(108099591.32, classA2CashFlow, precision);
            Assert.AreEqual(241, lastPeriodOfClassA2CashFlow.Period);

            var classBEndingBalance = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var classBPrincipal = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Principal);
            var classBInterest = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Interest);
            var classBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfClassBCashFlow = trancheResultsDictionary["Class B"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(680774692.95, classBEndingBalance, precision);
            Assert.AreEqual(23000000.00, classBPrincipal, precision);
            Assert.AreEqual(2960247.58, classBInterest, precision);
            Assert.AreEqual(25960247.58, classBCashFlow, precision);
            Assert.AreEqual(61, lastPeriodOfClassBCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Preferred Shares"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(13175973.87, residualCashFlow, precision);
            Assert.AreEqual(61, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(301, lastPeriodOfResidualCashFlow.Period);
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

            Assert.AreEqual(22621.73, caymanGovernmentFee, precision);
            Assert.AreEqual(201305.56, absNoteTrusteeFee, precision);
            Assert.AreEqual(263985.48, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(521144.79, spvAdministrationFee, precision);
            Assert.AreEqual(390858.59, portfolioAdministrationFee, precision);
            Assert.AreEqual(143808.84, raAdvanceFee, precision);

            caymanGovernmentFee = trancheResultsDictionary["Cayman Governmental Fee"].TrancheCashFlows.Average(c => c.Payment);
            absNoteTrusteeFee = trancheResultsDictionary["ABS Note Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            preferredSharesPayingAgencyFee = trancheResultsDictionary["Pref Shares Paying Agency Fee"].TrancheCashFlows.Average(c => c.Payment);
            spvAdministrationFee = trancheResultsDictionary["SPV Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            portfolioAdministrationFee = trancheResultsDictionary["Portfolio Administration Fee"].TrancheCashFlows.Average(c => c.Payment);
            raAdvanceFee = trancheResultsDictionary["RA Advance Fee"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(71.82, caymanGovernmentFee, precision);
            Assert.AreEqual(639.07, absNoteTrusteeFee, precision);
            Assert.AreEqual(838.05, preferredSharesPayingAgencyFee, precision);
            Assert.AreEqual(1654.43, spvAdministrationFee, precision);
            Assert.AreEqual(1240.82, portfolioAdministrationFee, precision);
            Assert.AreEqual(456.54, raAdvanceFee, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Liquidity Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(315425274.88, reserveAccountEndingBalance, precision);
            Assert.AreEqual(3364249.37, reserveAccountReleases, precision);
            Assert.AreEqual(2264249.37, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out

            #region Results Tie-Out
            var classA1Results = securitizationResult.SecuritizationResultsDictionary["Class A1"];
            Assert.AreEqual(classA1Results.DollarPrice.Value, 0.999898125749371, 0.00000001);
            Assert.AreEqual(classA1Results.PresentValue, 90990729.4431928, 0.01);
            Assert.AreEqual(classA1Results.InternalRateOfReturn, 0.03285, 0.00001);
            Assert.AreEqual(classA1Results.WeightedAverageLife, 4.66649017443308, 0.000001);
            Assert.AreEqual(classA1Results.ForwardWeightedAverageCoupon, 0.0331351089494497, 0.000000001);
            Assert.AreEqual(classA1Results.MacaulayDuration, 4.14580721729262, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationAnalytical, 4.07881271839301, 0.0000001);
            Assert.AreEqual(classA1Results.ModifiedDurationNumerical, 4.07916671542034, 0.0000001);
            Assert.AreEqual(classA1Results.NominalBenchmarkRate.Value, 0.0191468448633267, 0.0000001);
            Assert.AreEqual(classA1Results.NominalSpread.Value, 0.0137031551366736, 0.0000001);
            Assert.AreEqual(classA1Results.TotalPrincipal, classA1Principal, 0.01);
            Assert.AreEqual(classA1Results.TotalInterest, classA1Interest, 0.01);
            Assert.AreEqual(classA1Results.TotalCashFlow, classA1CashFlow, 0.01);

            var classA2Results = securitizationResult.SecuritizationResultsDictionary["Class A2"];
            Assert.AreEqual(classA2Results.DollarPrice.Value, 1.02174440891958, 0.00000001);
            Assert.AreEqual(classA2Results.PresentValue, 92812196.8730282, 0.01);
            Assert.AreEqual(classA2Results.InternalRateOfReturn, 0.03535, 0.00001);
            Assert.AreEqual(classA2Results.WeightedAverageLife, 4.66649017443308, 0.000001);
            Assert.AreEqual(classA2Results.ForwardWeightedAverageCoupon, 0.0411158211659342, 0.000000001);
            Assert.AreEqual(classA2Results.MacaulayDuration, 4.10649374884239, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationAnalytical, 4.03517208228795, 0.0000001);
            Assert.AreEqual(classA2Results.ModifiedDurationNumerical, 4.03551604092376, 0.0000001);
            Assert.AreEqual(classA2Results.NominalBenchmarkRate.Value, 0.0191468448633267, 0.0000001);
            Assert.AreEqual(classA2Results.NominalSpread.Value, 0.0162031551366736, 0.0000001);
            Assert.AreEqual(classA2Results.TotalPrincipal, classA2Principal, 0.01);
            Assert.AreEqual(classA2Results.TotalInterest, classA2Interest, 0.01);
            Assert.AreEqual(classA2Results.TotalCashFlow, classA2CashFlow, 0.01);

            var classBResults = securitizationResult.SecuritizationResultsDictionary["Class B"];
            Assert.AreEqual(classBResults.DollarPrice.Value, 0.999973719470879, 0.00000001);
            Assert.AreEqual(classBResults.PresentValue, 22999395.5478302, 0.01);
            Assert.AreEqual(classBResults.InternalRateOfReturn, 0.05130, 0.00001);
            Assert.AreEqual(classBResults.WeightedAverageLife, 2.51101941891806, 0.000001);
            Assert.AreEqual(classBResults.ForwardWeightedAverageCoupon, 0.0521802166172925, 0.000000001);
            Assert.AreEqual(classBResults.MacaulayDuration, 2.34448438291969, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationAnalytical, 2.28585227213932, 0.0000001);
            Assert.AreEqual(classBResults.ModifiedDurationNumerical, 2.28588948220553, 0.0000001);
            Assert.AreEqual(classBResults.NominalBenchmarkRate.Value, 0.0168085538852692, 0.0000001);
            Assert.AreEqual(classBResults.NominalSpread.Value, 0.0344914461147234, 0.0000001);
            Assert.AreEqual(classBResults.TotalPrincipal, classBPrincipal, 0.01);
            Assert.AreEqual(classBResults.TotalInterest, classBInterest, 0.01);
            Assert.AreEqual(classBResults.TotalCashFlow, classBCashFlow, 0.01);

            var residualResults = securitizationResult.SecuritizationResultsDictionary["Preferred Shares"];
            Assert.AreEqual(residualResults.PresentValue, 5004910.17812584, 0.01);
            Assert.AreEqual(residualResults.InternalRateOfReturn, 0.12000, 0.00001);
            Assert.AreEqual(residualResults.WeightedAverageLife, 9.38341451525603, 0.000001);
            Assert.AreEqual(residualResults.MacaulayDuration, 7.89657471336629, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationAnalytical, 7.05051313693418, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationNumerical, 7.05124041072412, 0.0000001);
            Assert.AreEqual(residualResults.TotalCashFlow, residualCashFlow, 0.01);
            #endregion Results Tie-Out
        }
    }
}
