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
    public class Hero_RiskRetention_ResecuritizationCashFlowTieOut
    {
        private const string _fullInputsFile = "Dream.IntegrationTests.Resources.HERO-Resecuritization-Risk-Retention.xlsm";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_FullLoadingFromExcel()
        {
            var precision = 0.01;

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_fullInputsFile);
            var resecuritizationDataRepository = new ResecuritizationExcelDataRepository(inputsFileStream);
            var paceResecuritization = resecuritizationDataRepository.GetPaceResecuritizaion();

            var securitizationScenarios = resecuritizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceResecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            //ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];
            var securitizationResult20Cpr = securitizationResultsDictionary["20% CPR"];

            CheckResults12Cpr(securitizationResult12Cpr, precision);
            CheckResults20Cpr(securitizationResult20Cpr, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_FullLoadingFromDatabase()
        {
            var precision = 0.01;
            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);

            var resecuritizationDataRepository = new ResecuritizationDatabaseRepository(1004, 1);
            var paceResecuritization = resecuritizationDataRepository.GetPaceSecuritization();

            var securitizationScenarios = resecuritizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceResecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];
            var securitizationResult20Cpr = securitizationResultsDictionary["20% CPR"];

            CheckResults12Cpr(securitizationResult12Cpr, precision);
            CheckResults20Cpr(securitizationResult20Cpr, precision);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_SaveToDatabase()
        {
            var precision = 0.01;
            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);

            var resecuritizationDataRepository = new ResecuritizationDatabaseRepository(1004, 1);
            var paceResecuritization = resecuritizationDataRepository.GetPaceSecuritization();
            var securitizationScenarios = resecuritizationDataRepository.ScenariosToAnalyze;

            paceResecuritization.RunSecuritizationAnalysis(securitizationScenarios);

            var resecuritizationDatabaseSaver = new ResecuritizationDatabaseSaver(paceResecuritization, securitizationScenarios);
            var resecuritizationSaveManager = new ResecuritizationSaveManager(resecuritizationDatabaseSaver);

            resecuritizationSaveManager.SaveSecuritization();

            var resecuritizationAnalysisDataSetId = resecuritizationSaveManager.SecuritizationAnalysisDataSetId;
            var resecuritizationAnalysisVersionId = resecuritizationSaveManager.SecuritizationAnalysisVersionId;

            var reloadResecuritizationDataRepository = new ResecuritizationDatabaseRepository(resecuritizationAnalysisDataSetId, resecuritizationAnalysisVersionId);
            var reloadPaceResecuritization = reloadResecuritizationDataRepository.GetPaceSecuritization();

            var reloadSecuritizationScenarios = reloadResecuritizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = reloadPaceResecuritization.RunSecuritizationAnalysis(reloadSecuritizationScenarios);

            // ExportToExcelUtility.ExportSecuritizationResults(securitizationResultsDictionary);

            var securitizationResult12Cpr = securitizationResultsDictionary["12% CPR"];
            var securitizationResult20Cpr = securitizationResultsDictionary["20% CPR"];

            CheckResults12Cpr(securitizationResult12Cpr, precision);
            CheckResults20Cpr(securitizationResult20Cpr, precision);
        }

        public void CheckResults12Cpr(SecuritizationResult securitizationResult, double precision)
        {
            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(1406007752.44, totalEndingBalance, precision);
            Assert.AreEqual(23251178.69, totalScheduledPrincipal, precision);
            Assert.AreEqual(23251178.69, totalPricipal, precision);
            Assert.AreEqual(6719768.82, totalInterest, precision);
            Assert.AreEqual(29970947.51, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(23251178.69, totalScheduledPrincipal, precision);
            Assert.AreEqual(23251178.69, totalPricipal, precision);
            Assert.AreEqual(6719768.82, totalInterest, precision);
            Assert.AreEqual(29970947.51, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var noteEndingBalance = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var notePrincipal = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.Principal);
            var noteInterest = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.Interest);
            var noteCashFlow = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfnoteCashFlow = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(1406007568.51, noteEndingBalance, precision);
            Assert.AreEqual(23251178.00, notePrincipal, precision);
            Assert.AreEqual(4386507.66, noteInterest, precision);
            Assert.AreEqual(27637685.66, noteCashFlow, precision);
            Assert.AreEqual(265, lastPeriodOfnoteCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Risk Retention Equity"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Risk Retention Equity"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Risk Retention Equity"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(1434977.22, residualCashFlow, precision);
            Assert.AreEqual(61, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(265, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2022, 9, 21).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
            Assert.AreEqual(new DateTime(2039, 9, 21).Ticks, lastPeriodOfResidualCashFlow.PeriodDate.Ticks);
            #endregion Notes Tie-Out
            #region Fees Tie-Out
            var caymanGovernmentFee = trancheResultsDictionary["Cayman Govt Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var trusteeFee = trancheResultsDictionary["Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var custodianFee = trancheResultsDictionary["Custodian Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var adminFee = trancheResultsDictionary["Admin Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var regOfficeFee = trancheResultsDictionary["Reg Office Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var fatcaFee = trancheResultsDictionary["FATCA Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var outOfPocketProvision = trancheResultsDictionary["Out of Pocket Provision"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(21341.89, caymanGovernmentFee, precision);
            Assert.AreEqual(250004.55, trusteeFee, precision);
            Assert.AreEqual(250004.55, custodianFee, precision);
            Assert.AreEqual(279660.45, adminFee, precision);
            Assert.AreEqual(48636.60, regOfficeFee, precision);
            Assert.AreEqual(36477.45, fatcaFee, precision);
            Assert.AreEqual(12159.15, outOfPocketProvision, precision);

            caymanGovernmentFee = trancheResultsDictionary["Cayman Govt Fee"].TrancheCashFlows.Average(c => c.Payment);
            trusteeFee = trancheResultsDictionary["Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            custodianFee = trancheResultsDictionary["Custodian Fee"].TrancheCashFlows.Average(c => c.Payment);
            adminFee = trancheResultsDictionary["Admin Fee"].TrancheCashFlows.Average(c => c.Payment);
            regOfficeFee = trancheResultsDictionary["Reg Office Fee"].TrancheCashFlows.Average(c => c.Payment);
            fatcaFee = trancheResultsDictionary["FATCA Fee"].TrancheCashFlows.Average(c => c.Payment);
            outOfPocketProvision = trancheResultsDictionary["Out of Pocket Provision"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(67.54, caymanGovernmentFee, precision);
            Assert.AreEqual(791.15, trusteeFee, precision);
            Assert.AreEqual(791.15, custodianFee, precision);
            Assert.AreEqual(885.00, adminFee, precision);
            Assert.AreEqual(153.91, regOfficeFee, precision);
            Assert.AreEqual(115.43, fatcaFee, precision);
            Assert.AreEqual(38.48, outOfPocketProvision, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(55959317.24, reserveAccountEndingBalance, precision);
            Assert.AreEqual(249668.66, reserveAccountReleases, precision);
            Assert.AreEqual(249668.66, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out

            #region Results Tie-Out
            var noteResults = securitizationResult.SecuritizationResultsDictionary["Risk Retention Note"];
            Assert.AreEqual(noteResults.DollarPrice.Value, 0.936324193949321, 0.00000001);
            Assert.AreEqual(noteResults.PresentValue, 21770640.4992222, 0.01);
            Assert.AreEqual(noteResults.InternalRateOfReturn, 0.05338, 0.00001);
            Assert.AreEqual(noteResults.WeightedAverageLife, 5.03086499784568, 0.000001);
            Assert.AreEqual(noteResults.ForwardWeightedAverageCoupon, 0.0374379861677474, 0.000000001);
            Assert.AreEqual(noteResults.MacaulayDuration, 4.12820728286798, 0.0000001);
            Assert.AreEqual(noteResults.ModifiedDurationAnalytical, 4.02088973581897, 0.0000001);
            Assert.AreEqual(noteResults.ModifiedDurationNumerical, 4.0212448322884713, 0.0000001);
            Assert.AreEqual(noteResults.NominalBenchmarkRate.Value, 0.0185376017971533, 0.0000001);
            Assert.AreEqual(noteResults.NominalSpread.Value, 0.0348423982028472, 0.0000001);
            Assert.AreEqual(noteResults.TotalPrincipal, notePrincipal, 0.01);
            Assert.AreEqual(noteResults.TotalInterest, noteInterest, 0.01);
            Assert.AreEqual(noteResults.TotalCashFlow, noteCashFlow, 0.01);

            var residualResults = securitizationResult.SecuritizationResultsDictionary["Risk Retention Equity"];
            Assert.AreEqual(residualResults.PresentValue, 537732.386826306, 0.01);
            Assert.AreEqual(residualResults.InternalRateOfReturn, 0.12000, 0.00001);
            Assert.AreEqual(residualResults.WeightedAverageLife, 9.45620173958617, 0.000001);
            Assert.AreEqual(residualResults.MacaulayDuration, 8.06774795712969, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationAnalytical, 7.20334639029436, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationNumerical, 7.2040875416735455, 0.0000001);
            Assert.AreEqual(residualResults.TotalCashFlow, residualCashFlow, 0.01);
            #endregion Results Tie-Out
        }

        public void CheckResults20Cpr(SecuritizationResult securitizationResult, double precision)
        {
            #region Collateral Tie-Out
            var cashFlowDictionary = securitizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(1008666698.79, totalEndingBalance, precision);
            Assert.AreEqual(23251178.69, totalScheduledPrincipal, precision);
            Assert.AreEqual(23251178.69, totalPricipal, precision);
            Assert.AreEqual(3910471.74, totalInterest, precision);
            Assert.AreEqual(27161650.44, totalCashFlow, precision);
            #endregion Collateral Tie-Out
            #region Available Funds Tie-Out
            totalScheduledPrincipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal);
            totalPricipal = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailablePrincipal + c.AvailablePrepayments);
            totalInterest = securitizationResult.AvailableFundsCashFlows.Sum(c => c.AvailableInterest + c.AvailablePrepaymentInterest);
            totalCashFlow = securitizationResult.AvailableFundsCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(23251178.69, totalScheduledPrincipal, precision);
            Assert.AreEqual(23251178.69, totalPricipal, precision);
            Assert.AreEqual(3910471.74, totalInterest, precision);
            Assert.AreEqual(27161650.44, totalCashFlow, precision);
            #endregion Available Funds Tie-Out
            #region Notes Tie-Out
            var trancheResultsDictionary = securitizationResult.SecuritizationResultsDictionary;

            var noteEndingBalance = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var notePrincipal = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.Principal);
            var noteInterest = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.Interest);
            var noteCashFlow = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Sum(c => c.Payment);
            var lastPeriodOfnoteCashFlow = trancheResultsDictionary["Risk Retention Note"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(1008666544.01, noteEndingBalance, precision);
            Assert.AreEqual(23251178.00, notePrincipal, precision);
            Assert.AreEqual(3144816.96, noteInterest, precision);
            Assert.AreEqual(26395994.96, noteCashFlow, precision);
            Assert.AreEqual(223, lastPeriodOfnoteCashFlow.Period);

            var residualCashFlow = trancheResultsDictionary["Risk Retention Equity"].TrancheCashFlows.Sum(c => c.Payment);
            var firstPeriodOfResidualCashFlow = trancheResultsDictionary["Risk Retention Equity"].TrancheCashFlows.First(c => c.Payment > 0);
            var lastPeriodOfResidualCashFlow = trancheResultsDictionary["Risk Retention Equity"].TrancheCashFlows.Last(c => c.Payment > 0);

            Assert.AreEqual(36972.98, residualCashFlow, precision);
            Assert.AreEqual(85, firstPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(223, lastPeriodOfResidualCashFlow.Period);
            Assert.AreEqual(new DateTime(2024, 9, 21).Ticks, firstPeriodOfResidualCashFlow.PeriodDate.Ticks);
            Assert.AreEqual(new DateTime(2036, 3, 21).Ticks, lastPeriodOfResidualCashFlow.PeriodDate.Ticks);
            #endregion Notes Tie-Out
            #region Fees Tie-Out
            var caymanGovernmentFee = trancheResultsDictionary["Cayman Govt Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var trusteeFee = trancheResultsDictionary["Trustee Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var custodianFee = trancheResultsDictionary["Custodian Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var adminFee = trancheResultsDictionary["Admin Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var regOfficeFee = trancheResultsDictionary["Reg Office Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var fatcaFee = trancheResultsDictionary["FATCA Fee"].TrancheCashFlows.Sum(c => c.Payment);
            var outOfPocketProvision = trancheResultsDictionary["Out of Pocket Provision"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(17465.56, caymanGovernmentFee, precision);
            Assert.AreEqual(204596.26, trusteeFee, precision);
            Assert.AreEqual(204596.26, custodianFee, precision);
            Assert.AreEqual(224082.64, adminFee, precision);
            Assert.AreEqual(38970.89, regOfficeFee, precision);
            Assert.AreEqual(29228.17, fatcaFee, precision);
            Assert.AreEqual(9742.72, outOfPocketProvision, precision);

            caymanGovernmentFee = trancheResultsDictionary["Cayman Govt Fee"].TrancheCashFlows.Average(c => c.Payment);
            trusteeFee = trancheResultsDictionary["Trustee Fee"].TrancheCashFlows.Average(c => c.Payment);
            custodianFee = trancheResultsDictionary["Custodian Fee"].TrancheCashFlows.Average(c => c.Payment);
            adminFee = trancheResultsDictionary["Admin Fee"].TrancheCashFlows.Average(c => c.Payment);
            regOfficeFee = trancheResultsDictionary["Reg Office Fee"].TrancheCashFlows.Average(c => c.Payment);
            fatcaFee = trancheResultsDictionary["FATCA Fee"].TrancheCashFlows.Average(c => c.Payment);
            outOfPocketProvision = trancheResultsDictionary["Out of Pocket Provision"].TrancheCashFlows.Average(c => c.Payment);

            Assert.AreEqual(55.27, caymanGovernmentFee, precision);
            Assert.AreEqual(647.46, trusteeFee, precision);
            Assert.AreEqual(647.46, custodianFee, precision);
            Assert.AreEqual(709.12, adminFee, precision);
            Assert.AreEqual(123.33, regOfficeFee, precision);
            Assert.AreEqual(92.49, fatcaFee, precision);
            Assert.AreEqual(30.83, outOfPocketProvision, precision);
            #endregion Fees Tie-Out
            #region Reserves Tie-Out
            var reserveAccountEndingBalance = trancheResultsDictionary["Reserve Account"].TrancheCashFlows.Sum(c => c.EndingBalance);
            var reserveAccountReleases = trancheResultsDictionary["Reserve Account"].TrancheCashFlows.Sum(c => c.Interest);
            var reserveAccountContributions = trancheResultsDictionary["Reserve Account"].TrancheCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(38350428.31, reserveAccountEndingBalance, precision);
            Assert.AreEqual(277827.60, reserveAccountReleases, precision);
            Assert.AreEqual(277827.60, reserveAccountContributions, precision);
            #endregion Reserves Tie-Out

            #region Results Tie-Out
            var noteResults = securitizationResult.SecuritizationResultsDictionary["Risk Retention Note"];
            Assert.AreEqual(noteResults.DollarPrice.Value, 0.951919737972921, 0.00000001);
            Assert.AreEqual(noteResults.PresentValue, 22133255.2693218, 0.01);
            Assert.AreEqual(noteResults.InternalRateOfReturn, 0.05338, 0.00001);
            Assert.AreEqual(noteResults.WeightedAverageLife, 3.60677577357013, 0.000001);
            Assert.AreEqual(noteResults.ForwardWeightedAverageCoupon, 0.037413557242463, 0.000000001);
            Assert.AreEqual(noteResults.MacaulayDuration, 3.10873015332743, 0.0000001);
            Assert.AreEqual(noteResults.ModifiedDurationAnalytical, 3.0279150993264, 0.0000001);
            Assert.AreEqual(noteResults.ModifiedDurationNumerical, 3.0280822923747808, 0.0000001);
            Assert.AreEqual(noteResults.NominalBenchmarkRate.Value, 0.0173083681271651, 0.0000001);
            Assert.AreEqual(noteResults.NominalSpread.Value, 0.036071631872835, 0.0000001);
            Assert.AreEqual(noteResults.TotalPrincipal, notePrincipal, 0.01);
            Assert.AreEqual(noteResults.TotalInterest, noteInterest, 0.01);
            Assert.AreEqual(noteResults.TotalCashFlow, noteCashFlow, 0.01);

            var residualResults = securitizationResult.SecuritizationResultsDictionary["Risk Retention Equity"];
            Assert.AreEqual(residualResults.PresentValue, 13789.4940748374, 0.01);
            Assert.AreEqual(residualResults.InternalRateOfReturn, 0.12000, 0.00001);
            Assert.AreEqual(residualResults.WeightedAverageLife, 9.23078250730665, 0.000001);
            Assert.AreEqual(residualResults.MacaulayDuration, 8.31682964231908, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationAnalytical, 7.4257407520706, 0.0000001);
            Assert.AreEqual(residualResults.ModifiedDurationNumerical, 7.4264632365040475, 0.0000001);
            Assert.AreEqual(residualResults.TotalCashFlow, residualCashFlow, 0.01);
            #endregion Results Tie-Out
        }
    }
}
