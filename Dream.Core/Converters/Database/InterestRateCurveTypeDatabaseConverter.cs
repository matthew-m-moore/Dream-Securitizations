using Dream.Common.Enums;
using Dream.Core.Repositories.Database;
using System;

namespace Dream.Core.Converters.Database
{
    public class InterestRateCurveTypeDatabaseConverter
    {
        public static InterestRateCurveType ConvertDescription(string rateIndexDescription)
        {
            if (string.IsNullOrEmpty(rateIndexDescription)) return default(InterestRateCurveType);

            switch (rateIndexDescription)
            {
                case "LIBOR-O/N":
                    return InterestRateCurveType.LiborOvernight;
                case "LIBOR-1W":
                    return InterestRateCurveType.Libor1Wk;
                case "LIBOR-1M":
                    return InterestRateCurveType.Libor1Mo;
                case "LIBOR-2M":
                    return InterestRateCurveType.Libor2Mo;
                case "LIBOR-3M":
                    return InterestRateCurveType.Libor3Mo;
                case "LIBOR-6M":
                    return InterestRateCurveType.Libor6Mo;
                case "LIBOR-1Y":
                    return InterestRateCurveType.Libor1Yr;
                case "LIBOR-Based Zero Rates":
                    return InterestRateCurveType.LiborSpot;
                case "LIBOR-Based Discount Factors":
                    return InterestRateCurveType.LiborDiscount;
                case "CMT-1M":
                    return InterestRateCurveType.Treasury1Mo;
                case "CMT-3M":
                    return InterestRateCurveType.Treasury3Mo;
                case "CMT-6M":
                    return InterestRateCurveType.Treasury6Mo;
                case "CMT-1Y":
                    return InterestRateCurveType.Treasury1Yr;
                case "CMT-2Y":
                    return InterestRateCurveType.Treasury2Yr;
                case "CMT-3Y":
                    return InterestRateCurveType.Treasury3Yr;
                case "CMT-5Y":
                    return InterestRateCurveType.Treasury5Yr;
                case "CMT-7Y":
                    return InterestRateCurveType.Treasury7Yr;
                case "CMT-10Y":
                    return InterestRateCurveType.Treasury10Yr;
                case "CMT-20Y":
                    return InterestRateCurveType.Treasury20Yr;
                case "CMT-30Y":
                    return InterestRateCurveType.Treasury30Yr;
                case "Treasury Zero Rates":
                    return InterestRateCurveType.TreasurySpot;
                case "Treasury Discount Factors":
                    return InterestRateCurveType.TreasuryDiscount;
                case "Swap-4M":
                    return InterestRateCurveType.Swap4Mo;
                case "Swap-5M":
                    return InterestRateCurveType.Swap5Mo;
                case "Swap-6M":
                    return InterestRateCurveType.Swap6Mo;
                case "Swap-7M":
                    return InterestRateCurveType.Swap7Mo;
                case "Swap-8M":
                    return InterestRateCurveType.Swap8Mo;
                case "Swap-9M":
                    return InterestRateCurveType.Swap9Mo;
                case "Swap-10M":
                    return InterestRateCurveType.Swap10Mo;
                case "Swap-11M":
                    return InterestRateCurveType.Swap11Mo;
                case "Swap-18M":
                    return InterestRateCurveType.Swap18Mo;
                case "Swap-1Y":
                    return InterestRateCurveType.Swap1Yr;
                case "Swap-2Y":
                    return InterestRateCurveType.Swap2Yr;
                case "Swap-3Y":
                    return InterestRateCurveType.Swap3Yr;
                case "Swap-4Y":
                    return InterestRateCurveType.Swap4Yr;
                case "Swap-5Y":
                    return InterestRateCurveType.Swap5Yr;
                case "Swap-6Y":
                    return InterestRateCurveType.Swap6Yr;
                case "Swap-7Y":
                    return InterestRateCurveType.Swap7Yr;
                case "Swap-8Y":
                    return InterestRateCurveType.Swap8Yr;
                case "Swap-9Y":
                    return InterestRateCurveType.Swap9Yr;
                case "Swap-10Y":
                    return InterestRateCurveType.Swap10Yr;
                case "Swap-11Y":
                    return InterestRateCurveType.Swap11Yr;
                case "Swap-12Y":
                    return InterestRateCurveType.Swap12Yr;
                case "Swap-15Y":
                    return InterestRateCurveType.Swap15Yr;
                case "Swap-20Y":
                    return InterestRateCurveType.Swap20Yr;
                case "Swap-25Y":
                    return InterestRateCurveType.Swap25Yr;
                case "Swap-30Y":
                    return InterestRateCurveType.Swap30Yr;
                case "Swap-40Y":
                    return InterestRateCurveType.Swap40Yr;
                case "Swap-50Y":
                    return InterestRateCurveType.Swap50Yr;
                case "None":
                    return InterestRateCurveType.None;

                default:
                    throw new Exception(string.Format("ERROR: The market data type '{0}' is not supported", rateIndexDescription));
            }
        }
    }
}
