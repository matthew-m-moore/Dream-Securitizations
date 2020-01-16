using Dream.IO.Excel;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.ConsoleApp.Scripts.Miscellaneous
{
    public class GetPaceSecuritizationContractualCashFlows : IScript
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
            return "Get HERO Contractual CFs";
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
            var securitizationInput = securitizationDataRepository.GetSecuritizationInputs(out SecuritizationInputsRecord baseSecuritizationInputsRecord).First();

            securitizationInput.PreFundingPercentageAmount = 0.0;
            securitizationInput.UseReplines = false;
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritizationWithoutNodesDefined(securitizationInput);
            Console.WriteLine("Data Loaded.");

            Console.WriteLine("Generating Cash Flows...");
            var contractualCashFlowGenerator = new ContractualCashFlowGenerator<ContractualCashFlow>(securitizationInput);
            var contractualCashFlowsDictionary = contractualCashFlowGenerator.GenerateContractualCashFlowsDictionaryOnListOfLoans(paceSecuritization.Collateral);
            Console.Write("Cash Flows Generated.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);
            ContractualCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, paceSecuritization.Collateral, contractualCashFlowsDictionary);
            Console.WriteLine("Contractual Cash Flows Report Complete");
            securitizationDataRepository.Dispose();

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
