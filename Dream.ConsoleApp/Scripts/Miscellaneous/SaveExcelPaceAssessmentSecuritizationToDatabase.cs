using Dream.ConsoleApp.Interfaces;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Database;
using Dream.Core.Repositories.Excel;
using Dream.Core.Savers;
using Dream.Core.Savers.SaveManagers;
using Dream.IO.Excel;
using System;
using System.Collections.Generic;

namespace Dream.ConsoleApp.Scripts.Miscellaneous
{
    public class SaveExcelPaceAssessmentSecuritizationToDatabase : IScript
    {
        public List<string> GetArgumentsList()
        {
            return new List<string>
            {
                "[1] Run securitization after saving",
                "[2] Valid file path to Excel PACE securitization file",
            };
        }

        public string GetFriendlyName()
        {
            return "Save HERO Securitization";
        }

        public bool GetVisibilityStatus()
        {
            return false;
        }

        public void RunScript(string[] args)
        {
            var runAfterSaving = bool.Parse(args[1]);
            var inputsFilePath = args[2];

            Console.WriteLine("Loading From Excel...");
            var securitizationExcelDataRepository = new SecuritizationExcelDataRepository(inputsFilePath);
            var excelPaceSecuritization = securitizationExcelDataRepository.GetPaceSecuritization();
            var scenariosToAnalyze = securitizationExcelDataRepository.ScenariosToAnalyze;
            Console.WriteLine("Loading Complete.");

            Console.WriteLine("Saving to Database...");
            var securitizationDatabaseSaver = new SecuritizationDatabaseSaver(excelPaceSecuritization, scenariosToAnalyze);
            var paceSecuritizationSaveManager = new PaceSecurititizationSaveManager(securitizationDatabaseSaver);
            paceSecuritizationSaveManager.SaveSecuritization();
            Console.WriteLine("Saving Complete.");

            if (runAfterSaving)
            {
                var securitizationAnalysisDataSetId = paceSecuritizationSaveManager.SecuritizationAnalysisDataSetId;
                var securitizationAnalysisVersionId = paceSecuritizationSaveManager.SecuritizationAnalysisVersionId;
                Console.WriteLine("Loading From Database...");
                var securitizationDataRepository = new SecuritizationDatabaseRepository(securitizationAnalysisDataSetId, securitizationAnalysisVersionId);
                var databasePaceSecuritization = securitizationDataRepository.GetPaceSecuritization();
                Console.WriteLine("Loading Complete.");

                Console.WriteLine("Running Scenarios...");
                var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;
                var securitizationResultsDictionary = databasePaceSecuritization.RunSecuritizationAnalysis(securitizationScenarios);
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

                Console.WriteLine("Opening Excel...");
                excelFileWriter.ExportWorkbook();
                Console.WriteLine("Process Complete.");
            }
        }
    }
}
