using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Triggers
{
    public abstract class TriggerLogic
    {
        public double TriggerValue { get; }
        public bool IsTriggerSticky { get; }

        protected bool _HasTriggerBeenTripped;

        public TriggerLogic(double triggerValue, bool isTriggerSticky = false)
        {
            TriggerValue = triggerValue;
            IsTriggerSticky = isTriggerSticky;
        }

        public abstract TriggerLogic Copy();
        public abstract void ApplyTriggerLogic(int monthlyPeriod, AvailableFunds availableFunds, AmountPayable paymentAmount);

        // Note that "stick" triggers are triggers that cannot un-trip once tripped
        protected bool CheckTriggerStatus(bool isTriggerTripped)
        {
            if (IsTriggerSticky && _HasTriggerBeenTripped)
            {
                return _HasTriggerBeenTripped;
            }

            return isTriggerTripped;
        }
    }
}
