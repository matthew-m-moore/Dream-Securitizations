using Dream.Common;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.BusinessLogic.Bonding;
using Dream.Core.BusinessLogic.Paydown;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Converters.Excel;
using Dream.Core.Reporting.Results;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.ConsoleApp.Scripts.Miscellaneous
{
    public class BondCouponOptimizationScript : IScript
    {
        private const string _paydownCalculatorInputsTabName = "PaydownCalculatorInputs";

        public bool OutputCashFlows = false;

        public List<string> GetArgumentsList()
        {
            return new List<string>
            {
                "[1] Collateral cut-off date",
                "[2] Bond payment start date",
                "[3] Lock principal applied to assessment",
                "[4] Ignore collected payments for bond principal applied",
                "[5] Valid file path to Excel inputs file",
            };
        }

        public string GetFriendlyName()
        {
            return "PACE Bond Coupon Optimizer";
        }

        public bool GetVisibilityStatus()
        {
            return false;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[5];
            var excelFileReader = new ExcelFileReader(inputsFilePath);

            var listOfPaydownCalculatorInputs = excelFileReader
                .GetTransposedDataFromSpecificTab<PaydownCalculatorInputRecord>(_paydownCalculatorInputsTabName);
            var paydownScenarios = PaydownScenarioExcelConverter
                .ConvertListOfPaydownCalculatorInputRecords(listOfPaydownCalculatorInputs);

            var collateralCutOffDate = DateTime.Parse(args[1]);
            var paceExcelDataRepository = new PaceAssessmentExcelDataRepository(excelFileReader, collateralCutOffDate);

            var paceAssessments = new List<PaceAssessment>();
            paceAssessments.AddRange(paceExcelDataRepository.GetAllPaceAssessments().Select(p => p as PaceAssessment));

            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            var listOfContractualCashFlowTuples = paceAssessments
                .AsParallel()
                .WithDegreeOfParallelism(Constants.ProcessorCount)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .Select(p => (PaceAssesement: p, ContractualCashFlows: p.GetContractualCashFlows()))
                .ToList();

            var bondPaymentStartDate = DateTime.Parse(args[2]);
            var lockPrincipalAppliedToAssessment = bool.Parse(args[3]);
            var ignoreFirstPaymentForBondPrincipalPaydown = bool.Parse(args[4]);

            var paydownScenarioResults = new List<BondCouponOptimizationResult>();
            foreach (var tuple in listOfContractualCashFlowTuples)
            {
                var tabName = tuple.PaceAssesement.IntegerId + " - Contractual";
                var prePaydownContractualCashFlows = tuple.ContractualCashFlows;
                if (OutputCashFlows) excelFileWriter.AddWorksheetForListOfData(prePaydownContractualCashFlows, tabName);
                Console.WriteLine(tabName);

                var paceAssessment = tuple.PaceAssesement;
                var paydownCalculator = new FixedRateLoanPaydownCalculator() { FloorPaydownBalanceAtZero = true };
                var bondCouponOptimizer = new PresentValueTargetSingleFixedRateLoanBondCouponOptimizer
                    <PaceAssessment, FixedRateLoanPaydownCalculator>(
                        paydownScenarios,
                        paceAssessment,
                        paydownCalculator,
                        collateralCutOffDate,
                        bondPaymentStartDate,
                        lockPrincipalAppliedToAssessment,
                        ignoreFirstPaymentForBondPrincipalPaydown);

                bondCouponOptimizer.OptimizeBondCouponsForSingleLoan(paceAssessment, prePaydownContractualCashFlows);

                paydownScenarioResults.AddRange(bondCouponOptimizer.OptimizationResults);
                foreach (var result in bondCouponOptimizer.OptimizationResults)
                {
                    tabName = paceAssessment.IntegerId + " - Scen " + result.ScenarioName;
                    if (OutputCashFlows) excelFileWriter.AddWorksheetForListOfData(result.PostPaydownContractualCashFlows, tabName);
                    Console.WriteLine(tabName);

                    if (result.MaxBondCoupon.HasValue)
                    {
                        tabName = paceAssessment.IntegerId + " - Bond Pre " + result.ScenarioName;
                        if (OutputCashFlows) excelFileWriter.AddWorksheetForListOfData(result.BondPrePaydownContractualCashFlows, tabName);
                        Console.WriteLine(tabName);

                        tabName = paceAssessment.IntegerId + " - Bond Post " + result.ScenarioName;
                        if (OutputCashFlows) excelFileWriter.AddWorksheetForListOfData(result.BondPostPaydownContractualCashFlows, tabName);
                        Console.WriteLine(tabName);
                    }
                }
            }

            excelFileWriter.AddWorksheetForListOfData(paydownScenarioResults, "Results");
            excelFileWriter.ExportWorkbook();
        }
    }
}
