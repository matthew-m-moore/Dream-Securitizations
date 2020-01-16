using System.Collections.Generic;
using Dream.ConsoleApp.Interfaces;
using Dream.IO.Excel;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using System;

namespace Dream.ConsoleApp.Scripts.SecuritizationWorkflow
{
    public class RunPaceSecuritizationPriceYieldTable : IScript
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
            return "Run HERO Securitization P/Y Table";
        }

        public bool GetVisibilityStatus()
        {
            return false;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];

            Console.WriteLine("Loading Data...");
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFilePath);
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritization();
            paceSecuritization.Inputs.ScenarioDescription = string.Empty;
            Console.WriteLine("Data Loaded.");

            Console.WriteLine("Running Scenarios...");
            var scenarioAnalysisDataRepository = new ScenarioAnalysisExcelDataRepository(inputsFilePath);
            var securitizationScenarios = scenarioAnalysisDataRepository.GetPriceYieldTableScenarios(paceSecuritization.Inputs);
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);
            SecuritizationTranchesPriceYieldTablesExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Tranche Price/Yield Table Complete.");
            SecuritizationNodesPriceYieldTablesExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Node Price/Yield Table Complete.");
            CollateralPriceYieldTableExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Collateral Price/Yield Table Complete.");

            if (PrintSummaryTab)
            {
                Console.WriteLine("Preparing Securitization Summary...");
                SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
                Console.WriteLine("Securitization Summary Complete.");
            }

            securitizationDataRepository.Dispose();
            scenarioAnalysisDataRepository.Dispose();

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
