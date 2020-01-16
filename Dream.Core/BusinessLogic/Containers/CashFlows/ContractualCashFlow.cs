using System;
using Dream.Common;

namespace Dream.Core.BusinessLogic.Containers.CashFlows
{
    public class ContractualCashFlow : CashFlow
    {
        public double StartingBalance { get; set; }
        public double EndingBalance { get; set; }
        public double Principal { get; set; }
        public double Interest { get; set; }
        public double AccruedInterest { get; set; }

        public override double Payment => Principal + Interest;
        public virtual double Coupon
        {
            get
            {
                // Avoid the divide-by-zero error
                if (EndingBalance <= 0.0)
                {
                    return 0.0;
                }

                return Constants.MonthsInOneYear * Constants.OneHundredPercentagePoints * (AccruedInterest / EndingBalance);
            }
        }

        public ContractualCashFlow() : base() { }

        public ContractualCashFlow(CashFlow cashFlow) : base(cashFlow) { }

        public ContractualCashFlow(ContractualCashFlow cashFlow) : base(cashFlow)
        {
            BondCount = cashFlow.BondCount;

            StartingBalance = cashFlow.StartingBalance;
            EndingBalance = cashFlow.EndingBalance;

            Principal = cashFlow.Principal;
            Interest = cashFlow.Interest;

            AccruedInterest = cashFlow.AccruedInterest;
        }

        public override CashFlow Copy() 
        {
            return new ContractualCashFlow(this);
        }

        public override void Clear()
        {
            EndingBalance = StartingBalance;
            Principal = 0.0;
            Interest = 0.0;
            AccruedInterest = 0.0;
        }

        public override void Scale(double scaleFactor)
        {
            Count *= scaleFactor;
            BondCount *= scaleFactor;

            StartingBalance *= scaleFactor;
            EndingBalance *= scaleFactor;

            Principal *= scaleFactor;
            Interest *= scaleFactor;

            AccruedInterest *= scaleFactor;
        }

        public override void Aggregate(CashFlow cashFlow)
        {
            var contractualCashFlow = cashFlow as ContractualCashFlow;

            Count += contractualCashFlow.Count;
            BondCount += contractualCashFlow.BondCount;

            StartingBalance += contractualCashFlow.StartingBalance;
            EndingBalance += contractualCashFlow.EndingBalance;

            Principal += contractualCashFlow.Principal;
            Interest += contractualCashFlow.Interest;

            AccruedInterest += contractualCashFlow.AccruedInterest;
        }

        public virtual double GetInterestAccrualBalance()
        {
            return EndingBalance;
        }
    }
}
