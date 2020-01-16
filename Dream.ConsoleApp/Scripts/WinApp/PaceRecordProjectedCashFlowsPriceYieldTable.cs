using Dream.ConsoleApp.Interfaces;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using System.Collections.Generic;
using System;
using Dream.IO.Excel.Entities;

namespace Dream.ConsoleApp.Scripts.WinApp
{
    public class PaceRecordProjectedCashFlowsPriceYieldTable : IScript
    {
        public bool PrintSummaryTab = false;

        public List<string> GetArgumentsList()
        {
            return new List<string>
            {
                "[1] Valid file path to Excel inputs file",
            };
        }

        public string GetFriendlyName()
        {
            return "PACE Proj. Cash Flows P/Y Table";
        }

        public bool GetVisibilityStatus()
        {
            return true;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];

            Console.WriteLine("Loading Data...");
            var loanPoolDataRepository = new LoanPoolExcelDataRepository(inputsFilePath);
            var loanPool = loanPoolDataRepository.GetLoanPoolOfPaceAssessments(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord);
            Console.WriteLine("Data Loaded.");

            Console.WriteLine("Running Scenarios...");
            var scenarioAnalysisDataRepository = new ScenarioAnalysisExcelDataRepository(inputsFilePath);
            var scenariosToAnalze = scenarioAnalysisDataRepository.GetPriceYieldTableScenarios(loanPool.Inputs);
            var dictionaryOfScenarioResults = loanPool.AnalyzeProjectedCashFlows(scenariosToAnalze);
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            CollateralPriceYieldTableExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, dictionaryOfScenarioResults);
            Console.WriteLine("Price/Yield Table Complete.");

            if (PrintSummaryTab)
            {
                Console.WriteLine("Preparing Summary...");
                CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, dictionaryOfScenarioResults);
                Console.WriteLine("Summary Complete.");
            }

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
