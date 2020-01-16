using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database
{
    public class PaymentConventionDatabaseConverter
    {
        private const string _proRated = "Pro-Rated";
        private const string _annual = "Annual";
        private const string _semiannual = "Semi-Annual";
        private const string _quarterly = "Quarterly";
        private const string _monthly = "Monthly";
        private const string _none = "None";

        public static PaymentConvention ConvertString(string paymentConventionText)
        {
            if (paymentConventionText == null) return default(PaymentConvention);

            switch (paymentConventionText)
            {
                case _proRated:
                    return PaymentConvention.ProRated;

                case _annual:
                    return PaymentConvention.Annual;

                case _semiannual:
                    return PaymentConvention.SemiAnnual;

                case _quarterly:
                    return PaymentConvention.Quarterly;

                case _monthly:
                    return PaymentConvention.Monthly;

                case _none:
                    return PaymentConvention.None;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The payment convention '{0}' is not supported. Please report this error.",
                        paymentConventionText));
            }
        }

        public static string ConvertToDescription(PaymentConvention paymentConvention)
        {
            switch (paymentConvention)
            {
                case PaymentConvention.ProRated:
                    return _proRated;

                case PaymentConvention.Annual:
                    return _annual;

                case PaymentConvention.SemiAnnual:
                    return _semiannual;

                case PaymentConvention.Quarterly:
                    return _quarterly;

                case PaymentConvention.Monthly:
                    return _monthly;

                case PaymentConvention.None:
                    return _none;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The payment convention '{0}' is not supported. Please report this error.",
                        paymentConvention));
            }
        }
    }
}
