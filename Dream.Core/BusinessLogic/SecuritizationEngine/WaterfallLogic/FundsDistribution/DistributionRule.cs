using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public abstract class DistributionRule
    {
        public double ProportionToDistribute { get; protected set; }
        public double AppliedProportionToDistribute { get; protected set; }

        public abstract DistributionRule Copy();
        public abstract void CalculateProportionToDistribute(
            int monthlyPeriod, 
            TrancheCashFlowType trancheCashFlowType, 
            Tranche currentSecuritizationTranche, 
            SecuritizationNodeTree securitizationNode);

        public virtual void ClearData(int monthlyPeriod) { ProportionToDistribute = 0.0; }

        public virtual FundsAvailable ApplyProportionToDistributeToFundsAvailable(
            SecuritizationNodeTree securitizationNode,
            TrancheCashFlowType trancheCashFlowType,
            int monthlyPeriod,
            double fundsAvailable,
            double totalFundsAvailable,
            double? balanceRemaining = null)
        {
            var proportionOfFundsAvailable = ProportionToDistribute * fundsAvailable;
            var proportionOfTotalFundsAvailable = ProportionToDistribute * totalFundsAvailable;

            AppliedProportionToDistribute = ProportionToDistribute;
            return new FundsAvailable(proportionOfFundsAvailable, proportionOfTotalFundsAvailable);
        }
    }
}
