using Dream.IO.Excel;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.Repositories.Excel;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration;
using Dream.Core.Reporting.Excel;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Valuation;
using System.Collections.Generic;
using System;
using Dream.IO.Excel.Entities;

namespace Dream.ConsoleApp.Scripts.WinApp
{
    public class PaceRecordContractualCashFlowsGenerator : IScript
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
            return "PACE Contractual Cash Flows";
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
            loanPool.Inputs.UseReplines = false;
            Console.WriteLine("Data Loaded.");

            Console.WriteLine("Generating Cash Flows...");      
            var contractualCashFlowsDictionary = loanPool.GenerateContractualCashFlows();
            var projectedCashFlowsDictionary = ContractualCashFlowGenerator<ContractualCashFlow>.ConvertContractualToProjectedCashFlows(contractualCashFlowsDictionary);

            var projectedCashFlowsAnalysis = new ProjectedCashFlowsAnalysis(new DoNothingPricingStrategy(), projectedCashFlowsDictionary);
            var dictionaryOfResults = projectedCashFlowsAnalysis.RunAnalysis(
                loanPool.Inputs.MarketRateEnvironment,
                loanPool.Inputs.MarketDataGroupingForNominalSpread,
                loanPool.Inputs.CurveTypeForSpreadCalcultion);
            Console.Write("Cash Flows Generated.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, dictionaryOfResults);
            Console.WriteLine("Summary Complete.");
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, dictionaryOfResults);
            Console.WriteLine("Cash Flows Report Complete.");
            ContractualCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, loanPool.ListOfLoans, contractualCashFlowsDictionary);
            Console.WriteLine("Contractual Cash Flows Report Complete");

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }
    }
}
