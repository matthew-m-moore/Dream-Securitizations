using System;

namespace Dream.Core.Reporting.Results
{
    public class PaymentCorridor
    {
        private const char _enDash = (char)0x2013;

        public int FirstPaymentPeriod { get; set; }
        public DateTime FirstPaymentPeriodDate { get; set; }

        public int LastPaymentPeriod { get; set; }
        public DateTime LastPaymentPeriodDate { get; set; }

        public override string ToString()
        {
            return FirstPaymentPeriod + " " + _enDash + " " + LastPaymentPeriod;
        }
    }
}