using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database.Securitization
{
    public class TrancheCashFlowTypeDatabaseConverter
    {
        private const string _payment = "Payment";
        private const string _paymentShortfall = "Payment Shortfall";
        private const string _principal = "Principal";
        private const string _principalShortfall = "Principal Shortfall";
        private const string _interest = "Interest";
        private const string _interestShortfall = "Interest Shortfall";
        private const string _fees = "Fees";
        private const string _feesShortfall = "Fees Shortfall";
        private const string _reserves = "Reserves";

        public static TrancheCashFlowType ConvertString(string trancheCashFlowTypeText)
        {
            if (trancheCashFlowTypeText == null) return default(TrancheCashFlowType);

            switch (trancheCashFlowTypeText)
            {
                case _payment:
                    return TrancheCashFlowType.Payment;

                case _paymentShortfall:
                    return TrancheCashFlowType.PaymentShortfall;

                case _principal:
                    return TrancheCashFlowType.Principal;

                case _principalShortfall:
                    return TrancheCashFlowType.PrincipalShortfall;

                case _interest:
                    return TrancheCashFlowType.Interest;

                case _interestShortfall:
                    return TrancheCashFlowType.InterestShortfall;

                case _fees:
                    return TrancheCashFlowType.Fees;

                case _feesShortfall:
                    return TrancheCashFlowType.FeesShortfall;

                case _reserves:
                    return TrancheCashFlowType.Reserves;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The tranche cash flow type '{0}' is not supported. Please report this error.", 
                        trancheCashFlowTypeText));
            }
        }
    }
}
