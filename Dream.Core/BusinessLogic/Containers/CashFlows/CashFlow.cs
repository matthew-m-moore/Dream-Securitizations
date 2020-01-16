using System;

namespace Dream.Core.BusinessLogic.Containers.CashFlows
{
    public abstract class CashFlow
    {
        public int Period { get; set; }
        public DateTime PeriodDate { get; set; }
        public double Count { get; set; }
        public double BondCount { get; set; }

        public abstract double Payment { get; }

        public CashFlow() { Count = 1.0; }

        public CashFlow(CashFlow cashFlow)
        {
            Period = cashFlow.Period;
            PeriodDate = cashFlow.PeriodDate;
            Count = cashFlow.Count;
        }

        public abstract CashFlow Copy();
        public abstract void Clear();
        public abstract void Scale(double scaleFactor);
        public abstract void Aggregate(CashFlow cashFlow);
    }
}
