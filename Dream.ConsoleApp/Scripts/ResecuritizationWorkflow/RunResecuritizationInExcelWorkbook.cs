using System;
using System.Collections.Generic;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.Repositories.Excel;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using ClosedXML.Excel;

namespace Dream.ConsoleApp.Scripts.ResecuritizationWorkflow
{
    public class RunResecuritizationInExcelWorkbook : IScript
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
            return "Populate Excel Resecuritization Summary";
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
            resecuritizationDataRepository.Dispose();
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var excelWorkbook = new XLWorkbook();

            var interactionObject = Interaction.GetObject(inputsFilePath);
            var activeExcelWorkbook = interactionObject as Workbook;
            if (activeExcelWorkbook != null)
            {
                Console.WriteLine("Updating Securitization Summary Tab...");
                ExcelWorkbookUpdateHelper.UpdateSecuritizationSummary(activeExcelWorkbook, excelWorkbook, resecuritizationResultsDictionary);
                Console.WriteLine("Update Completed.");

                Console.WriteLine("Updating Securitization Waterfall Tab...");
                var additionalYieldScenario = resecuritization.Inputs.AdditionalYieldScenarioDescription;
                ExcelWorkbookUpdateHelper.UpdateSecuritizationWaterfall(additionalYieldScenario, activeExcelWorkbook, excelWorkbook, resecuritizationResultsDictionary);
                Console.WriteLine("Update Completed.");
            }

            Console.WriteLine("Process Complete.");
        }
    }
}
