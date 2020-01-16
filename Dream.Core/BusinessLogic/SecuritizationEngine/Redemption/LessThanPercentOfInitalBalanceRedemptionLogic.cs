using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Redemption
{
    public class LessThanPercentOfInitalCollateralBalanceRedemptionLogic : RedemptionLogic
    {
        public LessThanPercentOfInitalCollateralBalanceRedemptionLogic(double redemptionTriggeredThresholdValue) 
            : base(redemptionTriggeredThresholdValue) { }

        private LessThanPercentOfInitalCollateralBalanceRedemptionLogic(
            LessThanPercentOfInitalCollateralBalanceRedemptionLogic redemptionLogic)
            : base(redemptionLogic) { }

        public override RedemptionLogic Copy()
        {
            return new LessThanPercentOfInitalCollateralBalanceRedemptionLogic(this)
            {
                PriorityOfPayments = PriorityOfPayments.Copy(),
                PostRedemptionPriorityOfPayments = PostRedemptionPriorityOfPayments.Copy(),
                TreatAsCleanUpCall = TreatAsCleanUpCall
            };
        }

        public override bool IsRedemptionTriggered(int monthlyPeriod)
        {
            if (CheckAllowedIfMonthIsNotAllowed(monthlyPeriod)) return false;

            var initialCollateralCashFlow = AvailableFunds.ProjectedCashFlowsOnCollateral.First();
            var initialCollateralBalance = initialCollateralCashFlow.EndingBalance;

            var currentCollateralCashFlow = AvailableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod];
            var currentCollateralBalance = currentCollateralCashFlow.EndingBalance;

            var percentageOfInitialBalance = currentCollateralBalance / initialCollateralBalance;
            var isRedemptionTriggered = percentageOfInitialBalance <= RedemptionTriggeredThreshold.Value;
            return isRedemptionTriggered;
        }
    }
}
