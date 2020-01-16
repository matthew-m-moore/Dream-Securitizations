using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic
{
    public class SecuritizationNodeTree
    {
        public int SecuritizationNodeId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public string SecuritizationNodeType { get; set; }
        public string SecuritizationNodeRating { get; set; }
        public string SecuritizationNodePricingScenario { get; set; }

        public SecuritizationNodeTree ParentSecuritizationNode { get; set; }
        public List<SecuritizationNodeTree> SecuritizationNodes { get; set; }
        public List<Tranche> SecuritizationTranches { get; set; }
        public List<SecuritizationCashFlow> NodeCashFlows { get; set; }
        public PricingStrategy PricingStrategy { get; set; }
        public DistributionRule AvailableFundsDistributionRule { get; }
        
        public bool AnyNodes => SecuritizationNodes != null && SecuritizationNodes.Any();
        public bool AnyTranches => SecuritizationTranches != null && SecuritizationTranches.Any();
        public bool AnyNodesOrTranches => AnyNodes || AnyTranches;

        public SecuritizationNodeTree(DistributionRule availableFundsDistributionRule)
        {
            AvailableFundsDistributionRule = availableFundsDistributionRule;
            NodeCashFlows = new List<SecuritizationCashFlow>();
            PricingStrategy = new DoNothingPricingStrategy();
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public SecuritizationNodeTree Copy()
        {
            var securitizationNodeTree = new SecuritizationNodeTree(AvailableFundsDistributionRule.Copy())
            {
                SecuritizationNodeName = new string (SecuritizationNodeName.ToCharArray()),
                SecuritizationNodeType = (SecuritizationNodeType == null) ? null : new string(SecuritizationNodeType.ToCharArray()),
                SecuritizationNodeRating = (SecuritizationNodeRating == null) ? null : new string(SecuritizationNodeRating.ToCharArray()),
                SecuritizationNodes = (SecuritizationNodes == null) ? null : SecuritizationNodes.Select(s => s.Copy()).ToList(),
                SecuritizationTranches = (SecuritizationTranches == null) ? null : SecuritizationTranches.Select(t => t.Copy()).ToList(),
                PricingStrategy = PricingStrategy.Copy(),
            };

            return securitizationNodeTree;
        }

        public Dictionary<string, SecuritizationNodeTree> RetrieveNodesDictionary()
        {
            var nodesDictionary = new Dictionary<string, SecuritizationNodeTree>();
            if (AnyNodes)
            {
                foreach (var securitizationNode in SecuritizationNodes)
                {
                    var subNodesDictionary = securitizationNode.RetrieveNodesDictionary();
                    nodesDictionary.Combine(subNodesDictionary);
                }
            }

            var keyValuePair = new KeyValuePair<string, SecuritizationNodeTree>(
                SecuritizationNodeName,
                this);

            nodesDictionary.Add(keyValuePair);
            return nodesDictionary;
        }

        public Dictionary<string, TrancheNodeStruct> RetrieveTranchesDictionary()
        {
            var tranchesDictionary = new Dictionary<string, TrancheNodeStruct>();
            if (AnyNodes)
            {
                foreach (var securitizationNode in SecuritizationNodes)
                {
                    var nodeTranchesDictionary = securitizationNode.RetrieveTranchesDictionary();
                    tranchesDictionary.Combine(nodeTranchesDictionary);
                }
            }

            if (AnyTranches)
            {
                foreach (var securitizationTranche in SecuritizationTranches)
                {
                    securitizationTranche.SecuritizationNode = this;
                    var trancheNodeStruct = new TrancheNodeStruct
                    {
                        Tranche = securitizationTranche,
                        SecuritizationNode = this
                    };

                    var keyValuePair = new KeyValuePair<string, TrancheNodeStruct>(
                        securitizationTranche.TrancheName,
                        trancheNodeStruct);

                    tranchesDictionary.Add(keyValuePair);
                }
            }

            return tranchesDictionary;
        }

        public Dictionary<string, SecuritizationCashFlowsSummaryResult> RunAnalysis(
            MarketRateEnvironment rateEnvironment,
            MarketDataGrouping dataGrouping,
            InterestRateCurveType curveType,
            int childToParentOrder = 1)
        {
            var resultsDictionary = new Dictionary<string, SecuritizationCashFlowsSummaryResult>();
            var nodeCashFlowsList = new List<List<SecuritizationCashFlow>>();
            var presentValueOfNode = 0.0;

            if (AnyNodes)
            {
                foreach (var securitizationNode in SecuritizationNodes)
                {
                    var nodeResultsDictionary = securitizationNode.RunAnalysis(rateEnvironment, dataGrouping, curveType, childToParentOrder);
                    resultsDictionary.Combine(nodeResultsDictionary);
                    nodeCashFlowsList.Add(securitizationNode.NodeCashFlows);
                    presentValueOfNode += nodeResultsDictionary.Values.Sum(r => r.PresentValue);
                }
            }

            if (AnyTranches)
            {             
                foreach (var securitizationTranche in SecuritizationTranches)
                {
                    var trancheResult = securitizationTranche.RunAnalysis(rateEnvironment, dataGrouping, curveType, childToParentOrder);
                    var dictionaryKey = securitizationTranche.TrancheName;
                    resultsDictionary.Add(dictionaryKey, trancheResult);
                    nodeCashFlowsList.Add(securitizationTranche.TrancheCashFlows);
                    presentValueOfNode += trancheResult.PresentValue;
                    childToParentOrder++;
                }          
            }

            NodeCashFlows = CashFlowAggregator.AggregateCashFlows(nodeCashFlowsList);
            if (NodeCashFlows.Any())
            {
                SetupNodePricingStrategy(presentValueOfNode);
                var nodeResult = RunNodeAnalysis(rateEnvironment, dataGrouping, curveType, childToParentOrder);
                resultsDictionary.Add(SecuritizationNodeName, nodeResult);
                childToParentOrder++;
            }
            
            return resultsDictionary;
        }

        private void SetupNodePricingStrategy(double presentValueOfNode)
        {
            if (AnyTranches && PricingStrategy is DoNothingPricingStrategy && Math.Abs(presentValueOfNode) > 0.0)
            {
                // Default to BEY (Bond Equivalent Yield) as the yield convention
                var dayCountConvention = DayCountConvention.Thirty360;
                var compoundingConvention = CompoundingConvention.SemiAnnually;

                var yieldConventions = SecuritizationTranches.Select(t => (
                    DayCountConvention: t.PricingStrategy.DayCountConvention,
                    CompoundingConvention: t.PricingStrategy.CompoundingConvention)).Distinct().ToList();

                // If any of the underlying tranches are XIRR convention, use that
                if (yieldConventions.Any(y => y.DayCountConvention == DayCountConvention.Actual365 &&
                                              y.CompoundingConvention == CompoundingConvention.Annually))
                {
                    dayCountConvention = DayCountConvention.Actual365;
                    compoundingConvention = CompoundingConvention.Annually;
                }
                // If there is only one distinct convention, use it
                else if (yieldConventions.Count == 1)
                {
                    dayCountConvention = yieldConventions.First().DayCountConvention;
                    compoundingConvention = yieldConventions.First().CompoundingConvention;
                }

                PricingStrategy = new SpecificMarketValuePricingStrategy(
                    dayCountConvention,
                    compoundingConvention,
                    presentValueOfNode);
            }
        }

        public SecuritizationCashFlowsSummaryResult RunNodeAnalysis(
            MarketRateEnvironment marketRateEnvironment,
            MarketDataGrouping marketDataGrouping,
            InterestRateCurveType interestRateCurveType,
            int childToParentOrder)
        {
            var nodeResult = RunNodeAnalysis(childToParentOrder);

            nodeResult.NominalSpread = PricingStrategy.CalculateNominalSpread(NodeCashFlows, marketRateEnvironment, marketDataGrouping);
            nodeResult.NominalBenchmarkRate = PricingStrategy.InterpolatedRate.GetValueOrDefault(double.NaN);
            nodeResult.MarketDataUseForNominalSpread = PricingStrategy.MarketDataUsedForNominalSpread;

            nodeResult.ZeroVolatilitySpread = PricingStrategy.CalculateSpread(NodeCashFlows, marketRateEnvironment, interestRateCurveType);
            nodeResult.SpreadDuration = PricingStrategy.CalculateSpreadDuration(NodeCashFlows, marketRateEnvironment, interestRateCurveType, Constants.TwentyFiveBpsShock);
            nodeResult.CurveTypeUsedForSpreadCalculation = PricingStrategy.CurveTypeUsedForSpreadCalculation;

            return nodeResult;
        }

        public SecuritizationCashFlowsSummaryResult RunNodeAnalysis(int childToParentOrder)
        {
            var nodeResult = new SecuritizationCashFlowsSummaryResult(NodeCashFlows);

            nodeResult.SecuritizationNodeName = SecuritizationNodeName;
            nodeResult.ChildToParentOrder = childToParentOrder;

            nodeResult.PresentValue = PricingStrategy.CalculatePresentValue(NodeCashFlows);
            nodeResult.DollarPrice = PricingStrategy.CalculatePrice(NodeCashFlows);
            nodeResult.InternalRateOfReturn = PricingStrategy.CalculateInternalRateOfReturn(NodeCashFlows);
            nodeResult.MacaulayDuration = PricingStrategy.CalculateMacaulayDuration(NodeCashFlows);
            nodeResult.ModifiedDurationAnalytical = PricingStrategy.CalculateModifiedDuration(NodeCashFlows);
            nodeResult.ModifiedDurationNumerical = PricingStrategy.CalculateModifiedDuration(NodeCashFlows, Constants.TwentyFiveBpsShock);
            nodeResult.DollarDuration = PricingStrategy.CalculateDollarDuration(NodeCashFlows);

            nodeResult.DayCountConvention = PricingStrategy.DayCountConvention;
            nodeResult.CompoundingConvention = PricingStrategy.CompoundingConvention;

            nodeResult.TotalCashFlow = NodeCashFlows.Sum(c => c.Payment);
            nodeResult.TotalPrincipal = NodeCashFlows.Sum(c => c.Principal);
            nodeResult.TotalInterest = NodeCashFlows.Sum(c => c.Interest);

            nodeResult.ForwardWeightedAverageCoupon = CashFlowMetrics.CalculateForwardWeightedAverageCoupon(NodeCashFlows);

            var usePaymentMetric = (Math.Abs(nodeResult.TotalPrincipal - NodeCashFlows.First().StartingBalance) < 0.01);
            nodeResult.WeightedAverageLife = usePaymentMetric 
                ? CashFlowMetrics.CalculatePaymentWeightedAverageLife(NodeCashFlows)
                : CashFlowMetrics.CalculatePrincipalWeightedAverageLife(NodeCashFlows);
        
            nodeResult.PaymentCorridor = usePaymentMetric
                ? CashFlowMetrics.CalculatePaymentCorridor(NodeCashFlows)
                : CashFlowMetrics.CalculatePrinicipalPaymentCorridor(NodeCashFlows);

            return nodeResult;
        }

        public void AssignParentNodes(SecuritizationNodeTree parentSecuritizationNode = null)
        {
            if (AnyNodes)
            {
                foreach (var securitizationNode in SecuritizationNodes)
                {
                    securitizationNode.AssignParentNodes(this);
                }
            }

            if (parentSecuritizationNode != null)
            {
                ParentSecuritizationNode = parentSecuritizationNode;
            }
        }
    }
}
