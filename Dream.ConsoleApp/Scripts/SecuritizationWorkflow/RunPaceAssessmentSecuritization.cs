using Dream.ConsoleApp.Interfaces;
using Dream.IO.Excel;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using System.Collections.Generic;
using System;

namespace Dream.ConsoleApp.Scripts.SecuritizationWorkflow
{
    public class RunPaceAssessmentSecuritization : IScript
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
            return "Run HERO Securitization";
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
            Console.WriteLine("Data Loaded.");

            Console.WriteLine("Running Scenarios...");
            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
            var securitizationResultsDictionary = paceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);
            SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Securitization Summary Complete.");
            SecuritizationCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Securitization Waterfall Report Complete.");
            SecuritizationTranchesDecrementTablesExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Securitization Decrement Tables Complete.");
            CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Collateral Summary Complete.");
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, securitizationResultsDictionary);
            Console.WriteLine("Collateral Cash Flows Report Complete.");
            securitizationDataRepository.Dispose();

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
