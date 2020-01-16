using System;

namespace Dream.IO.Excel.Entities
{
    public class PaydownCalculatorInputRecord
    {
        public string ScenarioName { get; set; }
        public DateTime BondCallDate { get; set; }
        public double? PaydownPercentageAmount { get; set; }

        public double TaxCollectionFeePercentage { get; set; }
        public double OtherFixedFees { get; set; }
        public double AdditionalCustomerFee { get; set; }

        public bool TaxesAlreadyPaidForYear { get; set; }
        public bool? TaxBillAmendedForPayment { get; set; }

        public bool SendRefundIfPossible { get; set; }
        public bool TryAllFutureBondCallDates { get; set; }
    }
}
