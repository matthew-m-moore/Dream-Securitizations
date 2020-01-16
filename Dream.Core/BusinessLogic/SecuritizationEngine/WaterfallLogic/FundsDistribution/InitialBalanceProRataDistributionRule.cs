using System.Linq;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public class InitialBalanceProRataDistributionRule : ProRataDistributionRule
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
                return;
            }

            var sumOfInitialBalancesAtNode = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationNode);
            
            // Assume that cases like this are intentional, and avoid dividing by zero
            if (sumOfInitialBalancesAtNode <= 0.0)
            {
                ProportionToDistribute = 1.0;
                return;
            }

            var initialTrancheBalance = currentSecuritizationTranche.InitialBalance;

            var proportionToDistribute = initialTrancheBalance / sumOfInitialBalancesAtNode;
            ProportionToDistribute = proportionToDistribute;
        }

        public override DistributionRule Copy()
        {
            return new InitialBalanceProRataDistributionRule();
        }

        protected override double CalculateSumOfBalancesAtNode(int monthlyPeriod, TrancheCashFlowType trancheCashFlowType, SecuritizationNodeTree securitizationNode)
        {
            var sumOfInitialBalancesAtNode = 0.0;

            if (securitizationNode.AnyNodes)
            {
                foreach (var securitizationSubNode in securitizationNode.SecuritizationNodes)
                {
                    var sumOfInitialBalancesAtSubNode = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationSubNode);
                    sumOfInitialBalancesAtNode += sumOfInitialBalancesAtSubNode;
                }
            }

            if (securitizationNode.AnyTranches)
            {
                var sumOfTranchesInitialBalances = securitizationNode.SecuritizationTranches.Sum(t => t.InitialBalance);
                sumOfInitialBalancesAtNode += sumOfTranchesInitialBalances;
            }

            return sumOfInitialBalancesAtNode;
        }
    }
}
