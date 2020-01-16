namespace Dream.Core.BusinessLogic.SecuritizationEngine.Redemption
{
    public class DoNothingRedemptionLogic : RedemptionLogic
    {
        public DoNothingRedemptionLogic() : base() { }

        private DoNothingRedemptionLogic(DoNothingRedemptionLogic redemptionLogic)
            : base(redemptionLogic) { }

        public override RedemptionLogic Copy()
        {
            return new DoNothingRedemptionLogic(this);
        }

        public override bool IsRedemptionTriggered(int monthlyPeriod)
        {
            return false;
        }

        public override void ProcessRedemption(int monthlyPeriod, SecuritizationInput inputs) { }
    }
}
