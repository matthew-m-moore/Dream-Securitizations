using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class DBRS_StressModel_SingleRepline_TieOut
    {
        private const string _singleReplineInputsFileExample = "Dream.IntegrationTests.Resources.DBRS-Model-Inputs-Single-Repline.xlsx";
        private const string _singleReplineInputsFileBarclays = "Dream.IntegrationTests.Resources.DBRS-Model-Inputs-Single-Repline-Barclays.xlsx";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_SingleRepline()
        {
            var precision = 0.01;

            var dbrsStressModelInput = new DbrsStressModelInput
            {
                StateLevelDefaultRateDictionary = new Dictionary<PropertyState, double> { [default(PropertyState)] = 0.272300001202401 },
                StateLevelLossGivenDefaultDictionary = new Dictionary<PropertyState, double?> { [default(PropertyState)] = null },
                StateLevelForeclosureTermInMonthsDictionary = new Dictionary<PropertyState, int> { [default(PropertyState)] = 30 },
                ReperformanceTermInMonths = new Dictionary<PropertyState, int> { [default(PropertyState)] = 18 },             
                TotalNumberOfDefaultSequences = 4,
            };

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_singleReplineInputsFileExample);
            var dictionaryOfResults = LoadReplineAndRunModel(inputsFileStream, dbrsStressModelInput);

            #region Check Cash Flows
            // Sequence 0 - Cash Flows
            var sequenceZeroResults = dictionaryOfResults["Seq 0: Cash Flow"];
            Assert.AreEqual(2056689424.22972, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(2003260486.63972, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(53821222.2828546, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(4082032.7056536, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(34798205.1143464, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(14548699.77, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(14940984.4628546, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(14940984.4628546, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Sequence 1 - Cash Flows
            var sequenceOneResults = dictionaryOfResults["Seq 1: Aggregated"];
            Assert.AreEqual(789550060.537476, sequenceOneResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(789550060.537476, sequenceOneResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(16267555.1408414, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(1845116.37181718, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(7746752.19096918, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(3961610.96486438, sequenceOneResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(995220.242349293, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(945459.230231828, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(49761.0121174646, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(2718723.79446515, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(3170003.74037687, sequenceOneResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(3011503.55335803, sequenceOneResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(158500.187018843, sequenceOneResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(5888727.53484202, sequenceOneResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Sequence 2 - Cash Flows
            var sequenceTwoResults = dictionaryOfResults["Seq 2: Aggregated"];
            Assert.AreEqual(194254923.634993, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(194254923.634993, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(4267508.78409656, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(692860.281229316, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(1755735.19829972, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(1078746.67049601, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(434268.814839328, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(412555.374097362, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(21713.4407419664, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(599617.139294454, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(849200.832816538, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(806740.791175711, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(42460.0416408269, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(1448817.97211099, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Sequence 3 - Cash Flows
            var sequenceThreeResults = dictionaryOfResults["Seq 3: Aggregated"];
            Assert.AreEqual(44334879.4051385, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(44334879.4051385, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(1095678.44818237, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(240636.019955807, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(338376.908360855, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(292175.488239465, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(207558.253939887, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(197180.341242893, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(10377.9126969944, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(107081.703419839, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(223582.60547682, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(212403.475202979, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(11179.130273841, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(330664.308896659, sequenceThreeResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Sequence 4 - Cash Flows
            var sequenceFourResults = dictionaryOfResults["Seq 4: Aggregated"];
            Assert.AreEqual(9039703.27302556, sequenceFourResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(9039703.27302556, sequenceFourResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(350812.500256027, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(123861.171209599, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(47465.6778273172, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(0.0, sequenceFourResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(120848.639202549, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(114806.207242421, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(6042.43196012744, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(12587.5948854645, sequenceFourResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(54833.5253591846, sequenceFourResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(52091.8490912254, sequenceFourResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(2741.67626795923, sequenceFourResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(67421.1202446492, sequenceFourResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Total - Cash Flows
            var totalResults = dictionaryOfResults["Total"];
            Assert.AreEqual(3093868991.08035, totalResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(3040440053.49035, totalResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(75802777.1562309, totalResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(6984506.5498655, totalResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(44686535.0898035, totalResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(19881232.8935999, totalResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(1757895.95033106, totalResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(1670001.1528145, totalResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(87894.7975165528, totalResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(18378994.6949195, totalResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(4297620.70402941, totalResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(4082739.66882794, totalResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(214881.035201471, totalResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(22676615.3989489, totalResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);
            #endregion #region Check Cash Flows
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_SingleRepline_Barclays()
        {
            var precision = 0.01;

            var dbrsStressModelInput = new DbrsStressModelInput
            {
                StateLevelDefaultRateDictionary = new Dictionary<PropertyState, double> { [default(PropertyState)] = 0.237702051244883 },
                StateLevelLossGivenDefaultDictionary = new Dictionary<PropertyState, double?> { [default(PropertyState)] = null },
                StateLevelForeclosureTermInMonthsDictionary = new Dictionary<PropertyState, int> { [default(PropertyState)] = 18 },
                ReperformanceTermInMonths = new Dictionary<PropertyState, int> { [default(PropertyState)] = 18 },          
                TotalNumberOfDefaultSequences = 4,
            };

            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_singleReplineInputsFileBarclays);
            var dictionaryOfResults = LoadReplineAndRunModel(inputsFileStream, dbrsStressModelInput);

            #region Check Cash Flows
            // Sequence 0 - Cash Flows
            var sequenceZeroResults = dictionaryOfResults["Seq 0: Cash Flow"];
            Assert.AreEqual(33198347.3014365, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(32023743.1714365, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(1244298.8806833, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(417515.324294333, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(581611.358826304, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(175477.446879362, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(245172.197562665, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(0.0, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(233698.65188714, sequenceZeroResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Sequence 1 - Cash Flows
            var sequenceOneResults = dictionaryOfResults["Seq 1: Aggregated"];
            Assert.AreEqual(1793600.50871482, sequenceOneResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(1793600.50871482, sequenceOneResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(178011.916415732, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(41110.9227227118, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(3872.62626440672, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(2739.20129087936, sequenceOneResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(127754.696601365, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(121366.961771296, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(6387.73483006822, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(-3900.82077666644, sequenceOneResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(16381.2909831405, sequenceOneResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(15562.2264339835, sequenceOneResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(819.064549157024, sequenceOneResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(12414.6906165724, sequenceOneResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Sequence 2 - Cash Flows
            var sequenceTwoResults = dictionaryOfResults["Seq 2: Aggregated"];
            Assert.AreEqual(9213.63268382378, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(9213.63268382378, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(2662.2167059716, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(91.9147833402594, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(0.0, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(0.0, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(2647.2865075391, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(2514.92218216214, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(132.364325376955, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(-110.524211689758, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(174.635739114699, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(165.903952158964, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(8.73178695573496, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(64.1115274249411, sequenceTwoResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);

            // Total - Cash Flows
            var totalResults = dictionaryOfResults["Total"];
            Assert.AreEqual(35001161.4428352, totalResults.ProjectedCashFlows.Sum(c => c.StartingBalance), precision);
            Assert.AreEqual(33826557.3128352, totalResults.ProjectedCashFlows.Sum(c => c.EndingBalance), precision);
            Assert.AreEqual(1424973.01380501, totalResults.ProjectedCashFlows.Sum(c => c.Payment), precision);
            Assert.AreEqual(458718.161800385, totalResults.ProjectedCashFlows.Sum(c => c.Principal), precision);
            Assert.AreEqual(585483.985090711, totalResults.ProjectedCashFlows.Sum(c => c.Prepayment), precision);
            Assert.AreEqual(178216.648170242, totalResults.ProjectedCashFlows.Sum(c => c.DelinquentPrincipal), precision);
            Assert.AreEqual(130401.983108904, totalResults.ProjectedCashFlows.Sum(c => c.Default), precision);
            Assert.AreEqual(123881.883953458, totalResults.ProjectedCashFlows.Sum(c => c.Recovery), precision);
            Assert.AreEqual(6520.09915544518, totalResults.ProjectedCashFlows.Sum(c => c.Loss), precision);
            Assert.AreEqual(241160.852574308, totalResults.ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest), precision);
            Assert.AreEqual(16555.9267222552, totalResults.ProjectedCashFlows.Sum(c => c.InterestDefault), precision);
            Assert.AreEqual(15728.1303861424, totalResults.ProjectedCashFlows.Sum(c => c.InterestRecovery), precision);
            Assert.AreEqual(827.796336112761, totalResults.ProjectedCashFlows.Sum(c => c.InterestLoss), precision);
            Assert.AreEqual(246177.454031137, totalResults.ProjectedCashFlows.Sum(c => c.AccruedInterest), precision);
            #endregion #region Check Cash Flows
        }

        private Dictionary<string, ProjectedCashFlowsSummaryResult> LoadReplineAndRunModel(Stream inputsFileStream, DbrsStressModelInput dbrsStressModelInput)
        {
            var loanPoolDataRepository = new LoanPoolExcelDataRepository(inputsFileStream);
            var loanPool = loanPoolDataRepository.GetLoanPoolOfPaceAssessments(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord);

            dbrsStressModelInput.AssumptionsStartDate = loanPool.Inputs.InterestAccrualStartDate;
            var dbrsStressModelProjectedCashFlowLogic = new DbrsStressModelProjectedCashFlowLogic(
                loanPool.ProjectedCashFlowLogic.ProjectedPerformanceAssumptions,
                dbrsStressModelInput,
                string.Empty);

            loanPool.ProjectedCashFlowLogic = dbrsStressModelProjectedCashFlowLogic;
            loanPool.Inputs.SeparatePrepaymentInterest = true;
            var dictionaryOfResults = loanPool.AnalyzeProjectedCashFlows();

            AddSequenceLevelCashFlowReports(dbrsStressModelProjectedCashFlowLogic, dictionaryOfResults);
            return dictionaryOfResults;
        }

        private void AddSequenceLevelCashFlowReports(
            DbrsStressModelProjectedCashFlowLogic dbrsStressModelProjectedCashFlowLogic,
            Dictionary<string, ProjectedCashFlowsSummaryResult> dictionaryOfResults,
            int sequenceLevel = 1)
        {
            if (dbrsStressModelProjectedCashFlowLogic.BaseProjectedCashFlows != null)
            {
                var baseCashFlows = dbrsStressModelProjectedCashFlowLogic.BaseProjectedCashFlows;
                var baseCashFlowSummaryResult = new ProjectedCashFlowsSummaryResult(baseCashFlows);
                dictionaryOfResults.Add("Seq 0: Cash Flow", baseCashFlowSummaryResult);
            }

            var sequenceCashFlowsCount = dbrsStressModelProjectedCashFlowLogic.SequenceLevelProjectedCashFlows.Count;
            if (sequenceCashFlowsCount == 0) return;

            for (var i = 0; i < sequenceCashFlowsCount; i++)
            {
                var sequenceCashFlows = dbrsStressModelProjectedCashFlowLogic.SequenceLevelProjectedCashFlows[i];
                var sequenceCashFlowSummaryResult = new ProjectedCashFlowsSummaryResult(sequenceCashFlows);
                dictionaryOfResults.Add("Seq " + sequenceLevel + ": " + i.ToString("00"), sequenceCashFlowSummaryResult);
            }

            var aggregatedSequenceLevelCashFlows = dbrsStressModelProjectedCashFlowLogic.AggregatedSequenceLevelProjectedCashFlows;
            var aggregatedSequenceCashFlowSummaryResult = new ProjectedCashFlowsSummaryResult(aggregatedSequenceLevelCashFlows);
            dictionaryOfResults.Add("Seq " + sequenceLevel + ": Aggregated", aggregatedSequenceCashFlowSummaryResult);

            if (dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic != null)
            {
                AddSequenceLevelCashFlowReports(dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic, dictionaryOfResults, sequenceLevel + 1);
            }
        }
    }
}
