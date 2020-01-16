using Dream.Common;

namespace Dream.Core.BusinessLogic.Containers.CashFlows
{
    public class ProjectedCashFlow : ContractualCashFlow
    {
        public double DelinquentPrincipal { get; set; }

        public double Prepayment { get; set; }
        public double Default { get; set; }
        public double Recovery { get; set; }
        public double Loss { get; set; }

        public double InterestDefault { get; set; }
        public double InterestRecovery { get; set; }
        public double InterestLoss { get; set; }

        public double PrepaymentInterest { get; set; }
        public double PrepaymentPenalty { get; set; }

        public override double Payment => base.Payment + Prepayment + Recovery + InterestRecovery + PrepaymentInterest + PrepaymentPenalty;
        public override double Coupon
        {
            get
            {
                // Avoid the divide-by-zero error
                if (EndingBalance <= 0.0)
                {
                    return 0.0;
                }

                return Constants.MonthsInOneYear * Constants.OneHundredPercentagePoints * ((AccruedInterest + PrepaymentInterest) / EndingBalance);
            }
        }

        public ProjectedCashFlow() : base() { }

        public ProjectedCashFlow(CashFlow cashFlow) : base(cashFlow) { }

        public ProjectedCashFlow(ContractualCashFlow cashFlow) : base(cashFlow) { }

        public ProjectedCashFlow(ProjectedCashFlow cashFlow) : base(cashFlow)
        {
            DelinquentPrincipal = cashFlow.DelinquentPrincipal;

            Prepayment = cashFlow.Prepayment;           
            Default = cashFlow.Default;
            Recovery = cashFlow.Recovery;
            Loss = cashFlow.Loss;

            InterestDefault = cashFlow.InterestDefault;
            InterestRecovery = cashFlow.InterestRecovery;
            InterestLoss = cashFlow.InterestLoss;

            PrepaymentInterest = cashFlow.PrepaymentInterest;
            PrepaymentPenalty = cashFlow.PrepaymentPenalty;
        }

        public override CashFlow Copy()
        {
            return new ProjectedCashFlow(this);
        }

        public override void Clear()
        {
            base.Clear();

            DelinquentPrincipal = 0.0;

            Prepayment = 0.0;
            Default = 0.0;
            Recovery = 0.0;
            Loss = 0.0;

            InterestDefault = 0.0;
            InterestRecovery = 0.0;
            InterestLoss = 0.0;

            PrepaymentInterest = 0.0;
            PrepaymentPenalty = 0.0;
        }

        public override void Scale(double scaleFactor)
        {
            base.Scale(scaleFactor);

            DelinquentPrincipal *= scaleFactor;

            Prepayment *= scaleFactor;
            Default *= scaleFactor;
            Recovery *= scaleFactor;
            Loss *= scaleFactor;

            InterestDefault *= scaleFactor;
            InterestRecovery *= scaleFactor;
            InterestLoss *= scaleFactor;

            PrepaymentInterest *= scaleFactor;
            PrepaymentPenalty *= scaleFactor;
        }

        public override void Aggregate(CashFlow cashFlow)
        {
            var contractualCashFlow = cashFlow as ContractualCashFlow;
            base.Aggregate(contractualCashFlow);

            var projectedCashFlow = cashFlow as ProjectedCashFlow;

            DelinquentPrincipal += projectedCashFlow.DelinquentPrincipal;

            Prepayment += projectedCashFlow.Prepayment;
            Default += projectedCashFlow.Default;
            Recovery += projectedCashFlow.Recovery;
            Loss += projectedCashFlow.Loss;

            InterestDefault += projectedCashFlow.InterestDefault;
            InterestRecovery += projectedCashFlow.InterestRecovery;
            InterestLoss += projectedCashFlow.InterestLoss;

            PrepaymentInterest += projectedCashFlow.PrepaymentInterest;
            PrepaymentPenalty += projectedCashFlow.PrepaymentPenalty;
        }
    }
}
