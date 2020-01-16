using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using System;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public class SubSequentialProRataDistributionRule : InitialBalanceProRataDistributionRule
    {
        public override void CalculateProportionToDistribute(
            int monthlyPeriod,
            TrancheCashFlowType trancheCashFlowType,
            Tranche currentSecuritizationTranche,
            SecuritizationNodeTree securitizationNode)
        {
            ProportionToDistribute = 1.0;
        }

        public override FundsAvailable ApplyProportionToDistributeToFundsAvailable(
            SecuritizationNodeTree securitizationNode,
            TrancheCashFlowType trancheCashFlowType,
            int monthlyPeriod,
            double fundsAvailable,
            double totalFundsAvailable,
            double? balanceRemaining)
        {
            switch(trancheCashFlowType)
            {
                case TrancheCashFlowType.Payment:
                    return ApplyPrincipalProportionToDistributeToFundsAvailable(
                        securitizationNode,
                        trancheCashFlowType,
                        monthlyPeriod,
                        fundsAvailable,
                        totalFundsAvailable,
                        balanceRemaining);

                case TrancheCashFlowType.Interest:
                    return base.ApplyProportionToDistributeToFundsAvailable(
                        securitizationNode,
                        trancheCashFlowType,
                        monthlyPeriod,
                        fundsAvailable,
                        totalFundsAvailable,
                        balanceRemaining);

                default:
                    // I really have no idea at this point what I would do with the other cash flow types.
                    throw new Exception(string.Format("ERROR: The tranche cash flow type '{0}' is not suppored yet for sub-sequential pro-rata funds distribution",
                        trancheCashFlowType));
            }
        }

        public override DistributionRule Copy()
        {
            return new SubSequentialProRataDistributionRule();
        }

        private FundsAvailable ApplyPrincipalProportionToDistributeToFundsAvailable(
            SecuritizationNodeTree securitizationNode,
            TrancheCashFlowType trancheCashFlowType,
            int monthlyPeriod,
            double fundsAvailable,
            double totalFundsAvailable,
            double? balanceRemaining)
        {
            var parentProportionToDistribute = CalculateParentNodeProportionToDistribute(
                securitizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                fundsAvailable,
                totalFundsAvailable);

            var applicableProportionToDistribute = parentProportionToDistribute * ProportionToDistribute;
            var applicableFundsAvailable = fundsAvailable * applicableProportionToDistribute;
            var proRataFundsEntry = new ProRataFundsEntry(securitizationNode.SecuritizationNodeName, trancheCashFlowType, monthlyPeriod);
            AppliedProportionToDistribute = applicableProportionToDistribute;

            // Note, this logic is a bit tricky, but the idea here is to pass along excess principal once a tranche clears out
            if (balanceRemaining.HasValue && applicableFundsAvailable > balanceRemaining.Value)
            {
                var excessFunds = applicableFundsAvailable - balanceRemaining.Value;
                var grossExcessFunds = excessFunds / applicableProportionToDistribute;
                AddProRataFundsEntry(proRataFundsEntry, grossExcessFunds, grossExcessFunds);
                return new FundsAvailable(balanceRemaining.Value, balanceRemaining.Value);
            }
            else if (ProRataFundsDictionary.ContainsKey(proRataFundsEntry))
            {
                var proRatedFundsAvailable = ProRataFundsDictionary[proRataFundsEntry].SpecificFundsAvailable * applicableProportionToDistribute;
                var proRatedTotalFundsAvailable = ProRataFundsDictionary[proRataFundsEntry].TotalFundsAvailable * applicableProportionToDistribute;

                // Since this is logic is for sequential tranches, there should be nothing left over after each payment
                ProRataFundsDictionary[proRataFundsEntry].SpecificFundsAvailable = 0.0;
                ProRataFundsDictionary[proRataFundsEntry].TotalFundsAvailable = 0.0;

                return new FundsAvailable(proRatedFundsAvailable, proRatedTotalFundsAvailable);
            }
            else
            {
                AddProRataFundsEntry(proRataFundsEntry, 0.0, 0.0);
                var proRatedFundsAvailable = applicableFundsAvailable;
                var proRatedTotalFundsAvailable = totalFundsAvailable * applicableProportionToDistribute;
                return new FundsAvailable(proRatedFundsAvailable, proRatedTotalFundsAvailable);
            }
        }
    }
}
