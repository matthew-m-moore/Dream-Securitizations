using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class FeePaymentConventionExcelConverter
    {
        private const string _annual = "Annual";
        private const string _semiAnnual = "Semi-Annual";
        private const string _quarterly = "Quarterly";
        private const string _monthly = "Monthly";

        public static PaymentConvention ConvertString(string feePaymentConventionText)
        {
            if (feePaymentConventionText == null) return default(PaymentConvention);

            switch (feePaymentConventionText)
            {
                case _annual:
                    return PaymentConvention.Annual;

                case _semiAnnual:
                    return PaymentConvention.SemiAnnual;

                case _quarterly:
                    return PaymentConvention.Quarterly;

                case _monthly:
                    return PaymentConvention.Monthly;

                default:
                    throw new Exception(string.Format("ERROR: The compounding convention '{0}' is not supported", feePaymentConventionText));
            }
        }
    }
}
