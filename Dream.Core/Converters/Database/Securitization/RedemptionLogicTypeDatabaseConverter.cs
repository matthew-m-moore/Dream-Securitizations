using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using System;

namespace Dream.Core.Converters.Database.Securitization
{
    public class RedemptionLogicTypeDatabaseConverter
    {
        private const string _doNothing = "Do Nothing";

        private const string _tranchesPaidOutInFull = "All Tranches Can Be Paid Out in Full";
        private const string _lessThanPercentOfInitialBalance = "Less Than Percentage of Initial Balance";

        public static Type ConvertString(string redemptionLogicTypeText)
        {
            switch (redemptionLogicTypeText)
            {
                case _doNothing:
                    return typeof(DoNothingRedemptionLogic);

                case _tranchesPaidOutInFull:
                    return typeof(TranchesCanBePaidOutFromAvailableFundsRedemptionLogic);

                case _lessThanPercentOfInitialBalance:
                    return typeof(LessThanPercentOfInitalCollateralBalanceRedemptionLogic);

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The redemption logic type '{0}' is not supported. Please report this error.",
                        redemptionLogicTypeText));
            }
        }

        public static string ConvertTypeToDescription(Type redemptionLogicType)
        {
            if (redemptionLogicType == typeof(DoNothingRedemptionLogic))
                return _doNothing;

            if (redemptionLogicType == typeof(TranchesCanBePaidOutFromAvailableFundsRedemptionLogic))
                return _tranchesPaidOutInFull;

            if (redemptionLogicType == typeof(LessThanPercentOfInitalCollateralBalanceRedemptionLogic))
                return _lessThanPercentOfInitialBalance;

            throw new Exception(string.Format("INTERNAL ERROR: The redemption logic type '{0}' is not supported. Please report this error.",
                redemptionLogicType));
        }
    }
}
