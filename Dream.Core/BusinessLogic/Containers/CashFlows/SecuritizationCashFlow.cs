using Dream.Common;

namespace Dream.Core.BusinessLogic.Containers.CashFlows
{
    public class SecuritizationCashFlow : ContractualCashFlow
    {
        public override double Coupon
        {
            get
            {
                if (StartingBalance <= 0.0)
                {
                    return 0.0;
                }

                // Note, dividing by the staring balance here in the overriden getter
                return Constants.MonthsInOneYear * Constants.OneHundredPercentagePoints * (AccruedInterest / StartingBalance);
            }
        }

        public double AccruedPayment { get; set; }
        public double PaymentShortfall { get; set; }
        public double InterestShortfall { get; set; }
        public double InterestAccruedOnInterestShortfall { get; set; }

        public SecuritizationCashFlow() : base() { }

        public SecuritizationCashFlow(CashFlow cashFlow) : base(cashFlow) { }

        public SecuritizationCashFlow(SecuritizationCashFlow cashFlow) : base(cashFlow)
        {
            AccruedPayment = cashFlow.AccruedPayment;

            PaymentShortfall = cashFlow.PaymentShortfall;
            InterestShortfall = cashFlow.InterestShortfall;

            InterestAccruedOnInterestShortfall = cashFlow.InterestAccruedOnInterestShortfall;
        }

        public override CashFlow Copy()
        {
            return new SecuritizationCashFlow(this);
        }

        public override void Clear()
        {
            base.Clear();
            AccruedPayment = 0.0;
            PaymentShortfall = 0.0;
            InterestShortfall = 0.0;
            InterestAccruedOnInterestShortfall = 0.0;
        }

        public override void Scale(double scaleFactor)
        {
            base.Scale(scaleFactor);

            AccruedPayment *= scaleFactor;

            PaymentShortfall *= scaleFactor;
            InterestShortfall *= scaleFactor;

            InterestAccruedOnInterestShortfall *= scaleFactor;
        }

        public override void Aggregate(CashFlow cashFlow)
        {
            var contractualCashFlow = cashFlow as ContractualCashFlow;
            base.Aggregate(contractualCashFlow);

            var securitizationCashFlow = cashFlow as SecuritizationCashFlow;

            AccruedPayment += securitizationCashFlow.AccruedPayment;

            PaymentShortfall += securitizationCashFlow.PaymentShortfall;
            InterestShortfall += securitizationCashFlow.InterestShortfall;

            InterestAccruedOnInterestShortfall += securitizationCashFlow.InterestAccruedOnInterestShortfall;
        }

        public override double GetInterestAccrualBalance()
        {
            return StartingBalance;
        }
    }
}
