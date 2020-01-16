using System.Collections.Generic;
using Dream.ConsoleApp.Interfaces;
using Dream.IO.Excel;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using System;

namespace Dream.ConsoleApp.Scripts.ResecuritizationWorkflow
{
    public class RunPaceResecuritizationPriceYieldTable : IScript
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
            return "Run HERO Resecuritization P/Y Table";
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
            resecuritization.Inputs.ScenarioDescription = string.Empty;
            Console.WriteLine("Resecuritization Data Loaded.");

            Console.WriteLine("Running Scenarios...");
            var scenarioAnalysisDataRepository = new ScenarioAnalysisExcelDataRepository(inputsFilePath);
            var resecuritizationScenarios = scenarioAnalysisDataRepository.GetPriceYieldTableScenarios(resecuritization.Inputs);
            var resecuritizationResultsDictionary = resecuritization.RunSecuritizationAnalysis(resecuritizationScenarios);
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);
            SecuritizationTranchesPriceYieldTablesExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Resecuritization Price/Yield Table Complete.");
            CollateralPriceYieldTableExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
            Console.WriteLine("Collateralized Securitization Price/Yield Table Complete.");

            if (PrintSummaryTab)
            {
                Console.WriteLine("Preparing Resecuritization Summary...");
                SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, resecuritizationResultsDictionary);
                Console.WriteLine("Resecuritization Summary Complete.");
            }

            resecuritizationDataRepository.Dispose();
            scenarioAnalysisDataRepository.Dispose();

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
