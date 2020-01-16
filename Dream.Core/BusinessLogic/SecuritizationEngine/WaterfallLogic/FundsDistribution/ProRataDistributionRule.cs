using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public abstract class ProRataDistributionRule : DistributionRule
    {
        public Dictionary<ProRataFundsEntry, FundsAvailable> ProRataFundsDictionary { get; private set; }

        public ProRataDistributionRule() : base()
        {
            ProRataFundsDictionary = new Dictionary<ProRataFundsEntry, FundsAvailable>();
        }

        protected abstract double CalculateSumOfBalancesAtNode(int monthlyPeriod, TrancheCashFlowType trancheCashFlowType, SecuritizationNodeTree securitizationNode);

        public override void ClearData(int monthlyPeriod)
        {
            ProRataFundsDictionary = ProRataFundsDictionary
                .Where(entry => entry.Key.MonthlyPeriod != monthlyPeriod)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            ProportionToDistribute = 0.0;
        }

        public override FundsAvailable ApplyProportionToDistributeToFundsAvailable(
            SecuritizationNodeTree securitizationNode,
            TrancheCashFlowType trancheCashFlowType,
            int monthlyPeriod,
            double fundsAvailable,
            double totalFundsAvailable,
            double? balanceRemaining)
        {
            // Now, if a sub-node is encountered, it will first adjust its proportion based on any parent nodes, prior to allocating funds
            var parentProportionToDistribute = CalculateParentNodeProportionToDistribute(
                securitizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                fundsAvailable,
                totalFundsAvailable);

            var proRataFundsEntry = new ProRataFundsEntry(securitizationNode.SecuritizationNodeName, trancheCashFlowType, monthlyPeriod);
            AddProRataFundsEntry(proRataFundsEntry, fundsAvailable, totalFundsAvailable);

            var applicableProportionToDistribute = parentProportionToDistribute * ProportionToDistribute;
            var proRatedFundsAvailable = ProRataFundsDictionary[proRataFundsEntry].SpecificFundsAvailable * applicableProportionToDistribute;
            var proRatedTotalFundsAvailable = ProRataFundsDictionary[proRataFundsEntry].TotalFundsAvailable * applicableProportionToDistribute;

            AppliedProportionToDistribute = applicableProportionToDistribute;
            return new FundsAvailable(proRatedFundsAvailable, proRatedTotalFundsAvailable);
        }

        public void AddProRataFundsEntry(ProRataFundsEntry proRataFundsEntry, double fundsAvailable, double totalFundsAvailable)
        {
            // Note, the idea here is to never override the first dictionary entry.
            // That would store the original funds available, such that a pro-rata amount
            // can be properly applied.
            if (ProRataFundsDictionary.ContainsKey(proRataFundsEntry)) return;
            else
            {
                var fundsAvailableContainer = new FundsAvailable(fundsAvailable, totalFundsAvailable);
                ProRataFundsDictionary.Add(proRataFundsEntry, fundsAvailableContainer);
            }
        }

        protected double CalculateParentNodeProportionToDistribute(
            SecuritizationNodeTree securitizationNode,
            TrancheCashFlowType trancheCashFlowType,
            int monthlyPeriod,
            double fundsAvailable,
            double totalFundsAvailable)
        {
            var parentProportionToDistribute = 1.0;
            if (securitizationNode.ParentSecuritizationNode != null)
            {
                parentProportionToDistribute = CalculateParentProportionToDistribute(monthlyPeriod, trancheCashFlowType, securitizationNode);

                var parentSecuritizationNode = securitizationNode.ParentSecuritizationNode;
                var parentSecuritizationNodeName = parentSecuritizationNode.SecuritizationNodeName;
                var parentProRataFundsEntry = new ProRataFundsEntry(parentSecuritizationNodeName, trancheCashFlowType, monthlyPeriod);

                if (parentSecuritizationNode.AvailableFundsDistributionRule is ProRataDistributionRule parentNodeDistributionRule)
                {
                    parentNodeDistributionRule.AddProRataFundsEntry(parentProRataFundsEntry, fundsAvailable, totalFundsAvailable);
                }
            }

            return parentProportionToDistribute;
        }

        protected double CalculateParentProportionToDistribute(
            int monthlyPeriod,
            TrancheCashFlowType trancheCashFlowType,
            SecuritizationNodeTree securitizationNode)
        {
            var parentSecuritizationNode = securitizationNode.ParentSecuritizationNode;
            if (parentSecuritizationNode.AvailableFundsDistributionRule is SequentialDistributionRule) return 1.0;

            var parentNodeBalance = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, parentSecuritizationNode);
            var currentNodeBalance = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationNode);

            var parentProportionToDistribute = currentNodeBalance / parentNodeBalance;
            return parentProportionToDistribute;
        }
    }
}
