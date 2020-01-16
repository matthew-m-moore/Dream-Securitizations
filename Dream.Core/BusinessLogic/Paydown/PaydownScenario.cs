using System;

namespace Dream.Core.BusinessLogic.Paydown
{
    public class PaydownScenario
    {
        public string ScenarioName { get; set; }

        public DateTime BondCallDate { get; set; }
        public bool TryAllFutureBondCallDates { get; set; }

        public double PaydownPercentageAmount { get; set; }
        public double? PaydownDollarAmount { get; set; }
        public bool SolveForMaxPaydownPercentage { get; set; }

        public double PercentageFees { get; set; }
        public double FixedFees { get; set; }
        public double AdditionalCustomerFee { get; set; }

        public bool PaymentAlreadyMadeForPeriod { get; set; }
        public bool? NextBillAmendedForPayment { get; set; }

        public bool SendRefundIfPossible { get; set; }
        
        public PaydownScenario() { }

        public PaydownScenario(PaydownScenario paydownScenario)
        {
            ScenarioName = paydownScenario.ScenarioName;

            BondCallDate = paydownScenario.BondCallDate;
            TryAllFutureBondCallDates = paydownScenario.TryAllFutureBondCallDates;

            PaydownPercentageAmount = paydownScenario.PaydownPercentageAmount;
            SolveForMaxPaydownPercentage = paydownScenario.SolveForMaxPaydownPercentage;

            PercentageFees = paydownScenario.PercentageFees;
            FixedFees = paydownScenario.FixedFees;
            AdditionalCustomerFee = paydownScenario.AdditionalCustomerFee;

            PaymentAlreadyMadeForPeriod = paydownScenario.PaymentAlreadyMadeForPeriod;
            NextBillAmendedForPayment = paydownScenario.NextBillAmendedForPayment;

            SendRefundIfPossible = paydownScenario.SendRefundIfPossible;
        }
    }
}
