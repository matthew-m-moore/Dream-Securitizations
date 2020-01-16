using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities;
using Dream.Core.Converters.Excel;
using Dream.Core.Repositories.Excel;
using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using Dream.Common;
using System.Linq;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Paydown;
using Dream.Core.BusinessLogic.Bonding;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class BondCouponOptimization_TieOut
    {
        private const string _inputsFileTaxesPaid = "Dream.IntegrationTests.Resources.Bond-Coupon-Optimization-Inputs-Taxes-Paid.xlsx";
        private const string _inputsFileTaxesNotPaid = "Dream.IntegrationTests.Resources.Bond-Coupon-Optimization-Inputs-Taxes-Not-Paid.xlsx";
        private const string _paydownCalculatorInputsTabName = "PaydownCalculatorInputs";

        private static DateTime _collateralCutOffDate = new DateTime(2017, 5, 2);
        private static DateTime _bondPaymentStartDate = new DateTime(2017, 5, 2);

        private static bool _lockPrincipalAppliedToAssessment = false;
        private static bool _ignoreFirstPaymentForBondPrincipalPaydown = true;

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_TaxesPaid_NoRefund()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFileTaxesPaid);
            var excelFileReader = new ExcelFileReader(inputFileStream);

            var paydownScenarioResults = RunBondCouponOptimization(excelFileReader);

            var fiveYearAssessmentTwoPercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 5 && Math.Abs(r.PercentageFeeCollection - 2.00) < 0.001);
            Assert.AreEqual(0.0, fiveYearAssessmentTwoPercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(2.65, fiveYearAssessmentTwoPercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.83, fiveYearAssessmentTwoPercentCountyFee.RateDifferential.Value, 0.01);

            var fiveYearAssessmentOneSevenFivePercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 5 && Math.Abs(r.PercentageFeeCollection - 1.75) < 0.001);
            Assert.AreEqual(0.0, fiveYearAssessmentOneSevenFivePercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(2.78, fiveYearAssessmentOneSevenFivePercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.71, fiveYearAssessmentOneSevenFivePercentCountyFee.RateDifferential.Value, 0.01);

            var tenYearAssessmentTwoPercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 10 && Math.Abs(r.PercentageFeeCollection - 2.00) < 0.001);
            Assert.AreEqual(0.0, tenYearAssessmentTwoPercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(6.01, tenYearAssessmentTwoPercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.48, tenYearAssessmentTwoPercentCountyFee.RateDifferential.Value, 0.01);

            var tenYearAssessmentOneSevenFivePercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 10 && Math.Abs(r.PercentageFeeCollection - 1.75) < 0.001);
            Assert.AreEqual(0.0, tenYearAssessmentOneSevenFivePercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(6.08, tenYearAssessmentOneSevenFivePercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.41, tenYearAssessmentOneSevenFivePercentCountyFee.RateDifferential.Value, 0.01);

            var fifteenYearAssessmentTwoPercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 15 && Math.Abs(r.PercentageFeeCollection - 2.00) < 0.001);
            Assert.AreEqual(92.87, fifteenYearAssessmentTwoPercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(8.36, fifteenYearAssessmentTwoPercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.42, fifteenYearAssessmentTwoPercentCountyFee.RateDifferential.Value, 0.01);

            var fifteenYearAssessmentOneSevenFivePercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 15 && Math.Abs(r.PercentageFeeCollection - 1.75) < 0.001);
            Assert.AreEqual(93.60, fifteenYearAssessmentOneSevenFivePercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(8.46, fifteenYearAssessmentOneSevenFivePercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.32, fifteenYearAssessmentOneSevenFivePercentCountyFee.RateDifferential.Value, 0.01);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest_TaxesNotPaid_BillUnAmended()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFileTaxesNotPaid);
            var excelFileReader = new ExcelFileReader(inputFileStream);

            var paydownScenarioResults = RunBondCouponOptimization(excelFileReader);

            var fiveYearAssessmentTwoPercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 5 && Math.Abs(r.PercentageFeeCollection - 2.00) < 0.001);
            Assert.AreEqual(101.74, fiveYearAssessmentTwoPercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(2.60, fiveYearAssessmentTwoPercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.89, fiveYearAssessmentTwoPercentCountyFee.RateDifferential.Value, 0.01);

            var fiveYearAssessmentOneSevenFivePercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 5 && Math.Abs(r.PercentageFeeCollection - 1.75) < 0.001);
            Assert.AreEqual(102.04, fiveYearAssessmentOneSevenFivePercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(2.76, fiveYearAssessmentOneSevenFivePercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.73, fiveYearAssessmentOneSevenFivePercentCountyFee.RateDifferential.Value, 0.01);

            var tenYearAssessmentTwoPercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 10 && Math.Abs(r.PercentageFeeCollection - 2.00) < 0.001);
            Assert.AreEqual(103.24, tenYearAssessmentTwoPercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(5.93, tenYearAssessmentTwoPercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.56, tenYearAssessmentTwoPercentCountyFee.RateDifferential.Value, 0.01);

            var tenYearAssessmentOneSevenFivePercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 10 && Math.Abs(r.PercentageFeeCollection - 1.75) < 0.001);
            Assert.AreEqual(103.78, tenYearAssessmentOneSevenFivePercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(6.07, tenYearAssessmentOneSevenFivePercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.42, tenYearAssessmentOneSevenFivePercentCountyFee.RateDifferential.Value, 0.01);

            var fifteenYearAssessmentTwoPercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 15 && Math.Abs(r.PercentageFeeCollection - 2.00) < 0.001);
            Assert.AreEqual(104.39, fifteenYearAssessmentTwoPercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(8.29, fifteenYearAssessmentTwoPercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.49, fifteenYearAssessmentTwoPercentCountyFee.RateDifferential.Value, 0.01);

            var fifteenYearAssessmentOneSevenFivePercentCountyFee = paydownScenarioResults.Single(r => r.AssessmentTerm == 15 && Math.Abs(r.PercentageFeeCollection - 1.75) < 0.001);
            Assert.AreEqual(105.12, fifteenYearAssessmentOneSevenFivePercentCountyFee.PaydownPercentage, 0.01);
            Assert.AreEqual(8.42, fifteenYearAssessmentOneSevenFivePercentCountyFee.MaxBondCoupon.Value, 0.01);
            Assert.AreEqual(0.37, fifteenYearAssessmentOneSevenFivePercentCountyFee.RateDifferential.Value, 0.01);
        }

        private List<BondCouponOptimizationResult> RunBondCouponOptimization(ExcelFileReader excelFileReader)
        {
            var listOfPaydownCalculatorInputs = excelFileReader
                .GetTransposedDataFromSpecificTab<PaydownCalculatorInputRecord>(_paydownCalculatorInputsTabName);
            var paydownScenarios = PaydownScenarioExcelConverter
                .ConvertListOfPaydownCalculatorInputRecords(listOfPaydownCalculatorInputs);

            var paceExcelDataRepository = new PaceAssessmentExcelDataRepository(excelFileReader, _collateralCutOffDate);

            var paceAssessments = new List<PaceAssessment>();
            paceAssessments.AddRange(paceExcelDataRepository.GetAllPaceAssessments().Select(p => p as PaceAssessment));

            var listOfContractualCashFlowTuples = paceAssessments
                .AsParallel()
                .WithDegreeOfParallelism(Constants.ProcessorCount)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .Select(p => (PaceAssessment: p, ContractualCashFlows: p.GetContractualCashFlows()))
                .ToList();

            var paydownScenarioResults = new List<BondCouponOptimizationResult>();
            foreach (var tuple in listOfContractualCashFlowTuples)
            {
                var paceAssessment = tuple.PaceAssessment;
                var paydownCalculator = new FixedRateLoanPaydownCalculator() { FloorPaydownBalanceAtZero = true };
                var bondCouponOptimizer = new PresentValueTargetSingleFixedRateLoanBondCouponOptimizer
                    <PaceAssessment, FixedRateLoanPaydownCalculator>(
                        paydownScenarios,
                        paceAssessment,
                        paydownCalculator,
                        _collateralCutOffDate,
                        _bondPaymentStartDate,
                        _lockPrincipalAppliedToAssessment,
                        _ignoreFirstPaymentForBondPrincipalPaydown);

                var prePaydownContractualCashFlows = tuple.ContractualCashFlows;
                bondCouponOptimizer.OptimizeBondCouponsForSingleLoan(paceAssessment, prePaydownContractualCashFlows);

                paydownScenarioResults.AddRange(bondCouponOptimizer.OptimizationResults);
            }

            return paydownScenarioResults;
        }
    }
}
