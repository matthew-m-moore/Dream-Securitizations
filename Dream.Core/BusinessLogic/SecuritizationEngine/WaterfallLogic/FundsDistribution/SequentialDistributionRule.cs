using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public class SequentialDistributionRule : DistributionRule
    {
        public override void CalculateProportionToDistribute(
            int monthlyPeriod, 
            TrancheCashFlowType trancheCashFlowType, 
            Tranche currentSecuritizationTranche,
            SecuritizationNodeTree securitizationNode)
        {
            ProportionToDistribute = 1.0;
        }

        public override DistributionRule Copy()
        {
            return new SequentialDistributionRule();
        }
    }
}
