using Dream.ConsoleApp.Interfaces;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using System.Collections.Generic;
using System;
using Dream.IO.Excel.Entities;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Reporting.Results;

namespace Dream.ConsoleApp.Scripts.WinApp
{
    public class PaceRecordProjectedCashFlowsGenerator : IScript
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
            return "PACE Projected Cash Flows";
        }

        public bool GetVisibilityStatus()
        {
            return true;
        }

        private Dictionary<string, ProjectedCashFlowsSummaryResult> _dictionaryOfResults;

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];

            Console.WriteLine("Loading Data...");
            var loanPoolDataRepository = new LoanPoolExcelDataRepository(inputsFilePath);
            var loanPool = loanPoolDataRepository.GetLoanPoolOfPaceAssessments(out CashFlowPricingInputsRecord cashFlowPricingInputsRecord);
            Console.WriteLine("Data Loaded.");

            var priceTarget = cashFlowPricingInputsRecord.TotalDollarPriceTarget;

            Console.WriteLine("Generating Cash Flows...");
            if (priceTarget.HasValue)
            {
                NumericalSearchUtility.NewtonRaphsonWithBisection(
                    pricingValue => RunCashFlowsReturnTotalPrice(loanPool, pricingValue),
                    priceTarget.Value,
                    1e-10);
            }
            else
            {
                _dictionaryOfResults = loanPool.AnalyzeProjectedCashFlows();
            }
            Console.Write("Cash Flows Generated.");

            Console.WriteLine("Preparing Results...");
            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            CollateralSummaryExcelReport.AddReportTabWithPricing(excelFileWriter.ExcelWorkbook, _dictionaryOfResults);
            Console.WriteLine("Summary Complete.");
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, _dictionaryOfResults);
            Console.WriteLine("Cash Flows Report Complete.");

            Console.WriteLine("Opening Excel...");
            excelFileWriter.ExportWorkbook();
            Console.WriteLine("Process Complete.");
        }

        public double RunCashFlowsReturnTotalPrice(LoanPool loanPool, double pricingValue)
        {
            if (loanPool.Inputs.CashFlowPricingStrategy is NominalSpreadBasedPricingStrategy nominalSpreadBasedPricingStrategy)
            {
                nominalSpreadBasedPricingStrategy.SpreadOverPointOnCurve = pricingValue;
                loanPool.Inputs.CashFlowPricingStrategy = nominalSpreadBasedPricingStrategy;
            }
            else if (loanPool.Inputs.CashFlowPricingStrategy is SpecificMarketValuePricingStrategy specificMarketValuePricingStrategy)
            {
                specificMarketValuePricingStrategy.SpecificMarketValue = pricingValue;
                loanPool.Inputs.CashFlowPricingStrategy = specificMarketValuePricingStrategy;
            }
            else if (loanPool.Inputs.CashFlowPricingStrategy is YieldBasedPricingStrategy yieldBasedPricingStrategy)
            {
                yieldBasedPricingStrategy.YieldToMaturity = pricingValue;
                loanPool.Inputs.CashFlowPricingStrategy = yieldBasedPricingStrategy;
            }
            else if (loanPool.Inputs.CashFlowPricingStrategy is PercentOfBalancePricingStrategy percentOfBalancePricingStrategy)
            {
                percentOfBalancePricingStrategy.PriceAsPercentOfBalance = pricingValue;
                loanPool.Inputs.CashFlowPricingStrategy = percentOfBalancePricingStrategy;
            }

            _dictionaryOfResults = loanPool.AnalyzeProjectedCashFlows();
            return _dictionaryOfResults[AggregationGroupings.TotalAggregationGroupName].DollarPrice.GetValueOrDefault();
        }
    }
}
