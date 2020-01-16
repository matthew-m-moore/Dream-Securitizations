using System;
using System.Collections.Generic;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.Repositories.Excel;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using ClosedXML.Excel;

namespace Dream.ConsoleApp.Scripts.SecuritizationWorkflow
{
    public class RunSecuritizationInExcelWorkbook : IScript
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
            return "Populate Excel Securitization Summary";
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
            securitizationDataRepository.Dispose();
            Console.WriteLine("Scenarios Completed.");

            Console.WriteLine("Preparing Results...");
            var excelWorkbook = new XLWorkbook();

            var interactionObject = Interaction.GetObject(inputsFilePath);
            var activeExcelWorkbook = interactionObject as Workbook;
            if (activeExcelWorkbook != null)
            {
                Console.WriteLine("Updating Securitization Summary Tab...");
                ExcelWorkbookUpdateHelper.UpdateSecuritizationSummary(activeExcelWorkbook, excelWorkbook, securitizationResultsDictionary);
                Console.WriteLine("Update Completed.");

                Console.WriteLine("Updating Securitization Waterfall Tab...");
                var additionalYieldScenario = paceSecuritization.Inputs.AdditionalYieldScenarioDescription;
                ExcelWorkbookUpdateHelper.UpdateSecuritizationWaterfall(additionalYieldScenario, activeExcelWorkbook, excelWorkbook, securitizationResultsDictionary);
                Console.WriteLine("Update Completed.");
            }

            Console.WriteLine("Process Complete.");
        }
    }
}
