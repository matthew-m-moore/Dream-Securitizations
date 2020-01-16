using System;
using System.Linq;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Triggers
{
    public class CumulativeLossesTriggerLogic : TriggerLogic
    {
        public double CumulativeLossesTriggerPercentage { get; }

        public CumulativeLossesTriggerLogic(double triggerValue) : base(triggerValue)
        {
            CumulativeLossesTriggerPercentage = triggerValue;
        }

        public override TriggerLogic Copy()
        {
            return new CumulativeLossesTriggerLogic(CumulativeLossesTriggerPercentage);
        }

        public override void ApplyTriggerLogic(int monthlyPeriod, AvailableFunds availableFunds, AmountPayable paymentAmount)
        {
            var cumulativeLosses = GetCumulativeLossesPercentageForGivenPeriod(monthlyPeriod, availableFunds);
            var isTriggerTripped = cumulativeLosses > CumulativeLossesTriggerPercentage;

            if (CheckTriggerStatus(isTriggerTripped))
            {
                var amountToBePaid = paymentAmount.Amount;

                paymentAmount.Amount -= amountToBePaid;
                paymentAmount.Shortfall += amountToBePaid;

                _HasTriggerBeenTripped = true;
            }
        }

        private double GetCumulativeLossesPercentageForGivenPeriod(int monthlyPeriod, AvailableFunds availableFunds)
        {
            var cumulativeLosses = availableFunds.ProjectedCashFlowsOnCollateral
                .Where(p => p.Period <= monthlyPeriod)
                .Sum(c => c.Loss);

            var startingCollateralBalance = availableFunds.ProjectedCashFlowsOnCollateral.First().StartingBalance;

            if (startingCollateralBalance <= 0)
            {
                throw new Exception("ERROR: A cumulative loss percentage is not defined for collateral with a zero starting balance.");
            }

            var cumulativeLossesPercentage = cumulativeLosses / startingCollateralBalance;
            return cumulativeLossesPercentage;
        }
    }
}
