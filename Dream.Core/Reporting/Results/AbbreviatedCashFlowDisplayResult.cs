using System;

namespace Dream.Core.Reporting.Results
{
    public class AbbreviatedCashFlowDisplayResult
    {
        public string Identifier { get; set; }
        public DateTime Date { get; set; }

        public double BeginningBalance { get; set; }
        public double Payment { get; set; }
        public double InterestPayment { get; set; }
        public double PrincipalPayment { get; set; }
        public double Prepayment { get; set; }
        public double EndingBalance { get; set; }
        public double Coupon { get; set; }
        public double BuyDownRate { get; set; }

        public int TermCount { get; set; }
        public int TermInYears { get; set; }
    }
}
