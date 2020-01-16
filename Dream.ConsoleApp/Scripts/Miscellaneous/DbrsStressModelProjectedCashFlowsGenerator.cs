using Dream.Common.Enums;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.Reporting.Excel;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities;
using System;
using System.Collections.Generic;

namespace Dream.ConsoleApp.Scripts.Miscellaneous
{
    public class DbrsStressModelProjectedCashFlowsGenerator : IScript
    {
        public List<string> GetArgumentsList()
        {
            return new List<string>
            {
                "[1] Valid file path to Excel inputs file",
            };
        }

        public string GetFriendlyName()
        {
            return "DBRS Stress Model";
        }

        public bool GetVisibilityStatus()
        {
            return false;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];

            Console.WriteLine("Loading Data...");
            var loanPoolDataRepository = new LoanPoolExcelDataRepository(inputsFilePath);
            var loanPool = loanPoolDataRepository.GetLoanPoolOfPaceAssessments(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord);
            Console.WriteLine("Data Loaded.");

            Console.WriteLine("Swapping Out Cash Flow Logic...");
            // Barclay's Sample Parameters
            // ===================================================================================
            var dbrsStressModelInput = new DbrsStressModelInput
            {
                StateLevelDefaultRateDictionary = new Dictionary<PropertyState, double> { [default(PropertyState)] = 0.237702051244883 },
                StateLevelLossGivenDefaultDictionary = new Dictionary<PropertyState, double?> { [default(PropertyState)] = null },
                StateLevelForeclosureTermInMonthsDictionary = new Dictionary<PropertyState, int> { [default(PropertyState)] = 18 },
                ReperformanceTermInMonths = new Dictionary<PropertyState, int> { [default(PropertyState)] = 18 },
                TotalNumberOfDefaultSequences = 4
            };

            // Sample Parameters
            // ===================================================================================
            //var dbrsStressModelInput = new DbrsStressModelInput
            //{
            //    DefaultRate = new Dictionary<PropertyState, double> { [default(PropertyState)] = 0.272300001202401 },
            //    ForeclosureTermInMonths = new Dictionary<PropertyState, int> { [default(PropertyState)] = 30 },
            //    ReperformanceTermInMonths = new Dictionary<PropertyState, int> { [default(PropertyState)] = 18 },
            //    TotalNumberOfDefaultSequences = 4
            //};

            dbrsStressModelInput.AssumptionsStartDate = loanPool.Inputs.InterestAccrualStartDate;

            var selectedAssumptionsGrouping = loanPool.Inputs.SelectedPerformanceAssumptionGrouping ?? string.Empty;
            var dbrsStressModelProjectedCashFlowLogic = new DbrsStressModelProjectedCashFlowLogic(
                loanPool.ProjectedCashFlowLogic.ProjectedPerformanceAssumptions,
                dbrsStressModelInput,
                selectedAssumptionsGrouping);

            loanPool.ProjectedCashFlowLogic = dbrsStressModelProjectedCashFlowLogic;
            loanPool.Inputs.SeparatePrepaymentInterest = true;

            Console.WriteLine("Generating Cash Flows...");
            var dictionaryOfResults = loanPool.AnalyzeProjectedCashFlows();
            Console.Write("Cash Flows Generated.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            CollateralSummaryExcelReport.AddReportTabWithPricing(excelFileWriter.ExcelWorkbook, dictionaryOfResults);
            Console.WriteLine("Summary Complete.");
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, dictionaryOfResults);
            Console.WriteLine("Cash Flows Report Complete.");

            AddSequenceLevelCashFlowReports(dbrsStressModelProjectedCashFlowLogic, excelFileWriter);
            Console.WriteLine("Sequence Cash Flows Report Complete.");

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }

        private void AddSequenceLevelCashFlowReports(
            DbrsStressModelProjectedCashFlowLogic dbrsStressModelProjectedCashFlowLogic, 
            ExcelFileWriter excelFileWriter,
            int sequenceLevel = 1)
        {
            if (dbrsStressModelProjectedCashFlowLogic.BaseProjectedCashFlows != null)
            {
                var baseDictionaryOfResults = new Dictionary<string, ProjectedCashFlowsSummaryResult>();
                var baseCashFlows = dbrsStressModelProjectedCashFlowLogic.BaseProjectedCashFlows;
                var baseCashFlowSummaryResult = new ProjectedCashFlowsSummaryResult(baseCashFlows);
                baseDictionaryOfResults.Add("Seq 0: Cash Flow", baseCashFlowSummaryResult);

                CollateralCashFlowsExcelReport.ReportTabName = "Seq 0 Cash Flows";
                CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, baseDictionaryOfResults);
            }

            var sequenceDictionaryOfResults = new Dictionary<string, ProjectedCashFlowsSummaryResult>();
            var sequenceCashFlowsCount = dbrsStressModelProjectedCashFlowLogic.SequenceLevelProjectedCashFlows.Count;
            if (sequenceCashFlowsCount == 0) return;

            for (var i = 0; i < sequenceCashFlowsCount; i++)
            {
                var sequenceCashFlows = dbrsStressModelProjectedCashFlowLogic.SequenceLevelProjectedCashFlows[i];
                var sequenceCashFlowSummaryResult = new ProjectedCashFlowsSummaryResult(sequenceCashFlows);
                sequenceDictionaryOfResults.Add("Seq " + sequenceLevel + ": " + i.ToString("00"), sequenceCashFlowSummaryResult);
            }

            var aggregatedSequenceLevelCashFlows = dbrsStressModelProjectedCashFlowLogic.AggregatedSequenceLevelProjectedCashFlows;
            var aggregatedSequenceCashFlowSummaryResult = new ProjectedCashFlowsSummaryResult(aggregatedSequenceLevelCashFlows);
            sequenceDictionaryOfResults.Add("Seq " + sequenceLevel + ": Aggregated", aggregatedSequenceCashFlowSummaryResult);

            CollateralCashFlowsExcelReport.ReportTabName = "Seq " + sequenceLevel + " Cash Flows";
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, sequenceDictionaryOfResults);

            if (dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic != null)
            {
                AddSequenceLevelCashFlowReports(dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic, excelFileWriter, sequenceLevel + 1);
            }
        }
    }
}
