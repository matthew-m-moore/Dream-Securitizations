using Dream.ConsoleApp.Interfaces;
using Dream.IO.Excel;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using System.Collections.Generic;
using System;

namespace Dream.ConsoleApp.Scripts.ResecuritizationWorkflow
{
    public class RunPaceResecuritization : IScript
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
            return "Run HERO Resecuritization";
        }

        public bool GetVisibilityStatus()
        {
            return false;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];

            Console.WriteLine("Loading Data For Resecuritization...");
            var resecuritizationDataRepository = new ResecuritizationExcelDataRepository(inputsFilePath);
            var resecuritization = resecuritizationDataRepository.GetPaceResecuritizaion();
            Console.WriteLine("Resecuritization Data Loaded.");

            Console.WriteLine("Running Scenarios...");
            var resecuritizationScenarios = resecuritizationDataRepository.ScenariosToAnalyze;
            var resecuritizationResultsDictionary = resecuritization.RunSecuritizationAnalysis(resecuritizationScenarios);
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);
            SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Resecuritization Summary Complete.");
            SecuritizationCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Resecuritization Waterfall Report Complete.");
            SecuritizationTranchesDecrementTablesExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Resecuritization Decrement Tables Complete.");
            CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Collateralized Securitization Summary Complete.");
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Collateralized Securitization Cash Flows Report Complete.");
            resecuritizationDataRepository.Dispose();

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
