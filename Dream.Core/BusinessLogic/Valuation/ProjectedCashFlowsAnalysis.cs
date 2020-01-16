using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Valuation
{
    public class ProjectedCashFlowsAnalysis
    {
        public string ScenarioDescription { get; set; }
        public PricingStrategy PricingStrategy { get; private set; }

        private Dictionary<string, List<ProjectedCashFlow>> _dictionaryOfProjectedCashFlows;

        public ProjectedCashFlowsAnalysis(PricingStrategy pricingStrategy, Dictionary<string, List<ProjectedCashFlow>> dictionaryOfProjectedCashFlows)
        {
            PricingStrategy = pricingStrategy;
            _dictionaryOfProjectedCashFlows = dictionaryOfProjectedCashFlows;
        }

        public Dictionary<string, ProjectedCashFlowsSummaryResult> RunAnalysis()
        {
            var dictionaryOfResults = new Dictionary<string, ProjectedCashFlowsSummaryResult>();

            foreach (var entry in _dictionaryOfProjectedCashFlows)
            {
                var projectedCashFlows = entry.Value;
                var projectedCashFlowsSummaryResult = new ProjectedCashFlowsSummaryResult(projectedCashFlows);

                projectedCashFlowsSummaryResult.Balance = projectedCashFlows.First().StartingBalance;

                projectedCashFlowsSummaryResult.PresentValue = PricingStrategy.CalculatePresentValue(projectedCashFlows);
                projectedCashFlowsSummaryResult.DollarPrice = PricingStrategy.CalculatePrice(projectedCashFlows);
                projectedCashFlowsSummaryResult.InternalRateOfReturn = PricingStrategy.CalculateInternalRateOfReturn(projectedCashFlows);
                projectedCashFlowsSummaryResult.MacaulayDuration = PricingStrategy.CalculateMacaulayDuration(projectedCashFlows);
                projectedCashFlowsSummaryResult.ModifiedDurationAnalytical = PricingStrategy.CalculateModifiedDuration(projectedCashFlows);
                projectedCashFlowsSummaryResult.ModifiedDurationNumerical = PricingStrategy.CalculateModifiedDuration(projectedCashFlows, Constants.TwentyFiveBpsShock);
                projectedCashFlowsSummaryResult.DollarDuration = PricingStrategy.CalculateDollarDuration(projectedCashFlows);

                projectedCashFlowsSummaryResult.DayCountConvention = PricingStrategy.DayCountConvention;
                projectedCashFlowsSummaryResult.CompoundingConvention = PricingStrategy.CompoundingConvention;

                projectedCashFlowsSummaryResult.WeightedAverageLife = CashFlowMetrics.CalculatePrincipalWeightedAverageLife(projectedCashFlows);
                projectedCashFlowsSummaryResult.ForwardWeightedAverageCoupon = CashFlowMetrics.CalculateForwardWeightedAverageCoupon(projectedCashFlows);

                projectedCashFlowsSummaryResult.TotalCashFlow = projectedCashFlows.Sum(c => c.Payment);
                projectedCashFlowsSummaryResult.TotalPrincipal = projectedCashFlows.Sum(c => c.Principal);
                projectedCashFlowsSummaryResult.TotalPrepayment = projectedCashFlows.Sum(c => c.Prepayment);
                projectedCashFlowsSummaryResult.TotalRecovery = projectedCashFlows.Sum(c => c.Recovery);
                projectedCashFlowsSummaryResult.TotalDefault = projectedCashFlows.Sum(c => c.Default);
                projectedCashFlowsSummaryResult.TotalLoss = projectedCashFlows.Sum(c => c.Loss);
                projectedCashFlowsSummaryResult.TotalInterest = projectedCashFlows.Sum(c => c.Interest);

                dictionaryOfResults.Add(entry.Key, projectedCashFlowsSummaryResult);
            }

            return dictionaryOfResults;
        }

        public Dictionary<string, ProjectedCashFlowsSummaryResult> RunAnalysis(
            MarketRateEnvironment marketRateEnvironment, 
            MarketDataGrouping marketDataGrouping,
            InterestRateCurveType interestRateCurveType)
        {
            var dictionaryOfResults = RunAnalysis();

            foreach(var groupingIdentifier in dictionaryOfResults.Keys)
            {
                PricingStrategy.ClearCachedValues();
                var projectedCashFlows = _dictionaryOfProjectedCashFlows[groupingIdentifier];

                dictionaryOfResults[groupingIdentifier].NominalSpread = PricingStrategy.CalculateNominalSpread(projectedCashFlows, marketRateEnvironment, marketDataGrouping);
                dictionaryOfResults[groupingIdentifier].NominalBenchmarkRate = PricingStrategy.InterpolatedRate.GetValueOrDefault(double.NaN);
                dictionaryOfResults[groupingIdentifier].MarketDataUseForNominalSpread = PricingStrategy.MarketDataUsedForNominalSpread;

                dictionaryOfResults[groupingIdentifier].ZeroVolatilitySpread = PricingStrategy.CalculateSpread(projectedCashFlows, marketRateEnvironment, interestRateCurveType);
                dictionaryOfResults[groupingIdentifier].SpreadDuration = PricingStrategy.CalculateSpreadDuration(projectedCashFlows, marketRateEnvironment, interestRateCurveType, Constants.TwentyFiveBpsShock);
                dictionaryOfResults[groupingIdentifier].CurveTypeUsedForSpreadCalculation = PricingStrategy.CurveTypeUsedForSpreadCalculation;
            }

            return dictionaryOfResults;
        }
    }
}
