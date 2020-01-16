using System;
using System.Collections.Generic;
using System.Linq;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine
{
    public class Resecuritization : Securitization
    {
        public Dictionary<string, Securitization> CollateralizedSecuritizationsDictionary { get; set; }
        public Dictionary<string, List<string>> CollateralizedTrancheNamesDictionary { get; private set; }
        public Dictionary<string, Dictionary<string, double>> CollateralizedTranchePercentageDictionary { get; private set; }
        public Dictionary<string, ProjectedCashFlowLogic> ProjectedCashFlowLogicDictionary { get; set; }

        public Resecuritization(SecuritizationInput securitizationInputs) 
            : base(securitizationInputs)
        {
            ProjectedCashFlowLogicDictionary = new Dictionary<string, ProjectedCashFlowLogic>();
            CollateralizedSecuritizationsDictionary = new Dictionary<string, Securitization>();
            CollateralizedTrancheNamesDictionary = new Dictionary<string, List<string>>();
            CollateralizedTranchePercentageDictionary = new Dictionary<string, Dictionary<string, double>>();
        }

        public Resecuritization(
            SecuritizationInput securitizationInputs,           
            PriorityOfPayments priorityOfPayments,         
            Dictionary<string, ProjectedCashFlowLogic> projectedCashFlowLogicDictionary,
            Dictionary<string, Securitization> collateralizedSecuritizationsDictionary,
            Dictionary<string, List<string>> collateralizedTrancheNamesDictionary,
            Dictionary<string, Dictionary<string, double>> collateralizedTranchePercentageDictionary,
            List<RedemptionLogic> redemptionLogicList,
            List<SecuritizationNodeTree> securitizationNodes,
            List<ProjectedCashFlow> projectedCashFlowsOnCollateral = null) 
                : this(securitizationInputs)
        {
            ProjectedCashFlowLogicDictionary = projectedCashFlowLogicDictionary;
            CollateralizedSecuritizationsDictionary = collateralizedSecuritizationsDictionary;
            CollateralizedTrancheNamesDictionary = collateralizedTrancheNamesDictionary;
            CollateralizedTranchePercentageDictionary = collateralizedTranchePercentageDictionary;
   
            PriorityOfPayments = priorityOfPayments;
            RedemptionLogicList = redemptionLogicList;
            SecuritizationNodes = securitizationNodes;

            _ProjectedCashFlowsOnCollateral = projectedCashFlowsOnCollateral;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public override Securitization Copy()
        {
            var resecuritization = new Resecuritization(
                Inputs.Copy(),
                PriorityOfPayments.Copy(),
                ProjectedCashFlowLogicDictionary.ToDictionary(kvp => new string(kvp.Key.ToCharArray()), kvp => kvp.Value.Copy()),
                CollateralizedSecuritizationsDictionary.ToDictionary(kvp => new string(kvp.Key.ToCharArray()), kvp => kvp.Value.Copy()),
                CollateralizedTrancheNamesDictionary.ToDictionary(kvp => new string(kvp.Key.ToCharArray()), kvp => kvp.Value.Select(n => new string(n.ToCharArray())).ToList()),
                CollateralizedTranchePercentageDictionary.ToDictionary(kvp1 => new string(kvp1.Key.ToCharArray()), kvp1 => kvp1.Value.ToDictionary(kvp2 => new string(kvp2.Key.ToCharArray()), kvp2 => kvp2.Value)),
                RedemptionLogicList.Select(r => r.Copy()).ToList(),
                SecuritizationNodes.Where(n => n.ParentSecuritizationNode == null).Select(s => s.Copy()).ToList(),
                _ProjectedCashFlowsOnCollateral);

            foreach (var securitizationNode in resecuritization.SecuritizationNodes)
            {
                securitizationNode.AssignParentNodes();
            }

            SpecialCopyLogicForRedemptionLogic(resecuritization);

            return resecuritization;
        }

        /// <summary>
        /// Returns the sum of all appropriately calculated percentage stakes for all collateralized tranches.
        /// </summary>
        public double GetResecuritizationCollateralBalance()
        {
            var totalCollateralAmount = 0.0;
            foreach (var collateralizedSecuritizationEntry in CollateralizedTranchePercentageDictionary)
            {
                var securitizationName = collateralizedSecuritizationEntry.Key;
                foreach (var collateralizedTrancheEntry in collateralizedSecuritizationEntry.Value)
                {
                    var collateralizedTrancheName = collateralizedTrancheEntry.Key;
                    var collateralizedTranchePercentage = collateralizedTrancheEntry.Value;

                    var collateralizedTrancheBalance = CollateralizedSecuritizationsDictionary[securitizationName]
                        .TranchesDictionary[collateralizedTrancheName]
                        .Tranche.InitialBalance;

                    totalCollateralAmount += (collateralizedTrancheBalance * collateralizedTranchePercentage);
                }
            }

            return totalCollateralAmount;
        }

        public void AddCollaterizedTrancheNameAndPercentage(string securitizationName, string trancheName, double percentageResecuritized)
        {
            if (!CollateralizedSecuritizationsDictionary.ContainsKey(securitizationName))
            {
                throw new Exception(string.Format("ERROR: There is no underlying collateralized securitization named '{0}'.",
                    securitizationName));
            }

            if (!CollateralizedSecuritizationsDictionary[securitizationName].TranchesDictionary.ContainsKey(trancheName))
            {
                throw new Exception(string.Format("ERROR: The underlying collateralized securitization '{0}' does not contain a tranche named '{1}'.",
                    securitizationName,
                    trancheName));
            }

            if (!CollateralizedTrancheNamesDictionary.ContainsKey(securitizationName))
            {
                CollateralizedTrancheNamesDictionary.Add(securitizationName, new List<string>());
            }

            CollateralizedTrancheNamesDictionary[securitizationName].Add(trancheName);

            if (!CollateralizedTranchePercentageDictionary.ContainsKey(securitizationName))
            {
                CollateralizedTranchePercentageDictionary.Add(securitizationName, new Dictionary<string, double>());
            }

            if (!CollateralizedTranchePercentageDictionary[securitizationName].ContainsKey(trancheName))
            {
                CollateralizedTranchePercentageDictionary[securitizationName].Add(trancheName, percentageResecuritized);
            }
            else
            {
                throw new Exception(string.Format("ERROR: A duplicate percentage for resecuritization was provided for underlying collateralized securitization '{0}' and tranche named '{1}'.",
                    securitizationName,
                    trancheName));
            }
        }

        protected override SecuritizationResult GenerateCollateralCashFlows(out int cashFlowPeriods)
        {          
            var securitizedCashFlowsDictionary = new Dictionary<string, List<ProjectedCashFlow>>();
            var aggregatedTotalCashFlows = new List<ProjectedCashFlow>();

            if (_ProjectedCashFlowsOnCollateral == null)
            {
                foreach (var securitizationName in CollateralizedSecuritizationsDictionary.Keys)
                {
                    // Note, it's very important to apply the projected cash flow logic down into the collateralized securitization
                    // Each securization will have different performing assumptions mappings, yuck...
                    CollateralizedSecuritizationsDictionary[securitizationName].Inputs.SelectedPerformanceAssumption = Inputs.SelectedPerformanceAssumption;
                    CollateralizedSecuritizationsDictionary[securitizationName].Inputs.CleanUpCallPercentage = Inputs.CleanUpCallPercentage;
                    CollateralizedSecuritizationsDictionary[securitizationName].ProjectedCashFlowLogic = ProjectedCashFlowLogicDictionary[securitizationName];
                    CollateralizedSecuritizationsDictionary[securitizationName].ProjectedCashFlowLogic.IsPrepaymentInterestSeparate = Inputs.SeparatePrepaymentInterest;
                    CollateralizedSecuritizationsDictionary[securitizationName].RunSecuritizationAnalysis();
                }

                var collateralizedSecuritizationCashFlows = new List<List<ProjectedCashFlow>>();
                foreach (var securitizationName in CollateralizedSecuritizationsDictionary.Keys)
                {
                    var collateralizedTranchesCashFlows = new List<List<ProjectedCashFlow>>();
                    foreach (var collateralizedTrancheName in CollateralizedTrancheNamesDictionary[securitizationName])
                    {
                        var collateralizedTranche = CollateralizedSecuritizationsDictionary[securitizationName].TranchesDictionary[collateralizedTrancheName].Tranche;
                        var collateralizedTranchePercentage = CollateralizedTranchePercentageDictionary[securitizationName][collateralizedTrancheName];
                        var collateralizedTrancheCashFlows = collateralizedTranche.TrancheCashFlows.Select(c => new ProjectedCashFlow(c)).ToList();
                        collateralizedTrancheCashFlows.ForEach(c => c.Scale(collateralizedTranchePercentage));

                        securitizedCashFlowsDictionary.Add(securitizationName + " - " + collateralizedTrancheName, collateralizedTrancheCashFlows);
                        collateralizedTranchesCashFlows.Add(collateralizedTrancheCashFlows);
                    }

                    var aggregatedTrancheCashFlows = CashFlowAggregator.AggregateCashFlows(collateralizedTranchesCashFlows);
                    var dateAggregatedTrancheCashFlows = CashFlowAggregator.AggregateCashFlowsByDate(
                        aggregatedTrancheCashFlows,
                        Inputs.SecuritizationStartDate,
                        Inputs.SecuritizationFirstCashFlowDate);

                    collateralizedSecuritizationCashFlows.Add(dateAggregatedTrancheCashFlows);
                }

                aggregatedTotalCashFlows = CashFlowAggregator.AggregateCashFlows(collateralizedSecuritizationCashFlows);
            }
            else
            {
                aggregatedTotalCashFlows = _ProjectedCashFlowsOnCollateral;
            }

            securitizedCashFlowsDictionary.Add(AggregationGroupings.TotalAggregationGroupName, aggregatedTotalCashFlows);

            // Convert the cash flows dictionary to a list of pricing results
            var projectedCashFlowsAnalysis = new ProjectedCashFlowsAnalysis(Inputs.CashFlowPricingStrategy, securitizedCashFlowsDictionary);
            var dictionaryOfResults = projectedCashFlowsAnalysis.RunAnalysis(
                Inputs.MarketRateEnvironment,
                Inputs.MarketDataGroupingForNominalSpread,
                Inputs.CurveTypeForSpreadCalcultion);

            var dateAggregatedTotalCashFlows = CashFlowAggregator.AggregateCashFlowsByDate(
                aggregatedTotalCashFlows,
                Inputs.SecuritizationStartDate,
                Inputs.SecuritizationFirstCashFlowDate);

            AvailableFunds = new AvailableFunds(dateAggregatedTotalCashFlows);
            cashFlowPeriods = dateAggregatedTotalCashFlows.Count;

            foreach(var redemptionLogic in RedemptionLogicList) redemptionLogic.AvailableFunds = AvailableFunds;

            // Add the aggregated cash flows to the securitization result
            var securitizationResult = new SecuritizationResult();
            securitizationResult.CollateralCashFlowsResultsDictionary = dictionaryOfResults;
            securitizationResult.ProjectedCashFlowsOnCollateral = AvailableFunds.ProjectedCashFlowsOnCollateral;
            return securitizationResult;
        }
    }
}
