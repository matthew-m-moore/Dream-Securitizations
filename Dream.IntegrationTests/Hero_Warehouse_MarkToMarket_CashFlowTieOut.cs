using Dream.Core.BusinessLogic.Containers;
using Dream.Core.Repositories.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Common.Utilities;

namespace Dream.IntegrationTests
{
    [TestClass]
    public class Hero_Warehouse_MarkToMarket_CashFlowTieOut
    {
        private const string _inputsFile = "Dream.IntegrationTests.Resources.HERO-Warehouse-MTM-Funding-Pricer-Inputs.xlsx";

        [TestMethod, Owner("Matthew Moore")]
        public void RunTest()
        {
            var inputsFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_inputsFile);
            var warehouseMarkToMarketDataRepository = new WarehouseMarkToMarketExcelDataRepository(inputsFileStream);

            WarehouseMarkToMarketInput warehouseMarkToMarketInput;
            var warehouseMarkToMarketMockSecuritization = warehouseMarkToMarketDataRepository.GetMockPaceSecuritizationForMarkToMarket(out warehouseMarkToMarketInput);
            var warehouseMarkToMarketSecuritizationResult = warehouseMarkToMarketMockSecuritization.RunSecuritizationAnalysis();

            var pricingStrategy = new NominalSpreadBasedPricingStrategy(
                warehouseMarkToMarketInput.DayCountConvention,
                warehouseMarkToMarketInput.CompoundingConvention,
                warehouseMarkToMarketSecuritizationResult.MarketRateEnvironment,
                warehouseMarkToMarketInput.MarketDataForNominalSpread,
                warehouseMarkToMarketInput.NominalSpread);

            var paceAssessments = warehouseMarkToMarketMockSecuritization.Collateral.OfType<PaceAssessment>().ToList();

            CheckResults(warehouseMarkToMarketSecuritizationResult, pricingStrategy, paceAssessments);
        }

        private void CheckResults(
            SecuritizationResult warehouseMarkToMarketSecuritizationResult,
            NominalSpreadBasedPricingStrategy pricingStrategy,
            List<PaceAssessment> paceAssessments)
        {
            var cashFlowPrecision = 0.01;
            var cashFlowDictionary = warehouseMarkToMarketSecuritizationResult.CollateralCashFlowsResultsDictionary;

            var totalEndingBalance = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.EndingBalance);
            var totalScheduledPrincipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal);
            var totalPrepayment = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Prepayment);
            var totalPricipal = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Principal + c.Prepayment);
            var totalInterest = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Interest + c.PrepaymentInterest);
            var totalCashFlow = cashFlowDictionary["Total"].ProjectedCashFlows.Sum(c => c.Payment);

            Assert.AreEqual(874520688.042, totalEndingBalance, cashFlowPrecision);
            Assert.AreEqual(5137333.48, totalScheduledPrincipal, cashFlowPrecision);
            Assert.AreEqual(6019943.00, totalPrepayment, cashFlowPrecision);
            Assert.AreEqual(11157276.48, totalPricipal, cashFlowPrecision);
            Assert.AreEqual(5156397.23, totalInterest, cashFlowPrecision);
            Assert.AreEqual(16313673.71, totalCashFlow, cashFlowPrecision);

            var metricsPrecistion = 1e-12;
            var totalCashFlows = cashFlowDictionary["Total"].ProjectedCashFlows;
            Assert.AreEqual(6.5317664336956, CashFlowMetrics.CalculatePrincipalWeightedAverageLife(totalCashFlows), metricsPrecistion);
            Assert.AreEqual(0.0707550635024, CashFlowMetrics.CalculateForwardWeightedAverageCoupon(totalCashFlows), metricsPrecistion);
            Assert.AreEqual(0.0795476929837, CashFlowMetrics.CalculateLifetimeConstantPrepaymentRate(totalCashFlows), metricsPrecistion);

            var buyDownRates = paceAssessments.Select(p => p.RatePlan.BuyDownRate).ToList();
            var assessmentBalances = paceAssessments.Select(p => p.Balance).ToList();
            var weightedAverageBuyDown = MathUtility.WeightedAverage(assessmentBalances, buyDownRates);
            Assert.AreEqual(0.030690713184, weightedAverageBuyDown, metricsPrecistion);

            var cashOutlayRate = 1.0 - weightedAverageBuyDown;
            Assert.AreEqual(0.969309286816, cashOutlayRate, metricsPrecistion);

            var advanceRate = cashOutlayRate * 0.94;
            Assert.AreEqual(0.911150729607, advanceRate, metricsPrecistion);

            var presentValue = pricingStrategy.CalculatePresentValue(totalCashFlows);
            var price = pricingStrategy.CalculatePrice(totalCashFlows);
            var internalRateOfReturn = pricingStrategy.CalculateInternalRateOfReturn(totalCashFlows);
            var nominalSpread = pricingStrategy.CalculateNominalSpread(totalCashFlows, 
                pricingStrategy.MarketRateEnvironment, 
                pricingStrategy.MarketDataGrouping);

            Assert.AreEqual(12236782.09, presentValue, 0.01);
            Assert.AreEqual(1.09675, price, 0.00001);
            Assert.AreEqual(0.05137, internalRateOfReturn, 0.00001);
            Assert.AreEqual(0.03150, nominalSpread, 0.00001);

            var markToMarketCushion = price - advanceRate;
            Assert.AreEqual(0.185602775376, markToMarketCushion, metricsPrecistion);
        }
    }
}
