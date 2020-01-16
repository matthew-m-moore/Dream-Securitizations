using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public class CurrentBalanceProRataDistributionRule : ProRataDistributionRule
    {
        public override void CalculateProportionToDistribute(
            int monthlyPeriod, 
            TrancheCashFlowType trancheCashFlowType, 
            Tranche currentSecuritizationTranche,
            SecuritizationNodeTree securitizationNode)
        {
            if (!securitizationNode.AnyNodesOrTranches)
            {
                ProportionToDistribute = 1.0;
            }

            // In some cases the "last" balance may refer to different periods depending on which of the tranches
            // in the node are calculated first, so it is better to do this by a specified integer monthly period
            var sumOfCurrentBalancesAtNode = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationNode);

            // Assume that cases like this are intentional, and avoid dividing by zero
            if (sumOfCurrentBalancesAtNode <= 0.0)
            {
                ProportionToDistribute = 1.0;
                return;
            }

            var currentTrancheBalance = currentSecuritizationTranche.TrancheCashFlows[monthlyPeriod].StartingBalance;

            var proportionToDistribute = currentTrancheBalance / sumOfCurrentBalancesAtNode;
            ProportionToDistribute = proportionToDistribute;
        }

        public override DistributionRule Copy()
        {
            return new CurrentBalanceProRataDistributionRule();
        }

        protected override double CalculateSumOfBalancesAtNode(int monthlyPeriod, TrancheCashFlowType trancheCashFlowType, SecuritizationNodeTree securitizationNode)
        {
            var sumOfStartingBalancesAtNode = 0.0;

            if (securitizationNode.AnyNodes)
            {
                foreach (var securitizationSubNode in securitizationNode.SecuritizationNodes)
                {
                    var sumOfStartingBalancesAtSubNode = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationSubNode);
                    sumOfStartingBalancesAtNode += sumOfStartingBalancesAtSubNode;
                }
            }

            if (securitizationNode.AnyTranches)
            {
                var sumOfTranchesStartingBalances = securitizationNode.SecuritizationTranches
                    .Sum(t => t.TrancheCashFlows[monthlyPeriod].StartingBalance);

                sumOfStartingBalancesAtNode += sumOfTranchesStartingBalances;
            }

            return sumOfStartingBalancesAtNode;
        }
    }
}
