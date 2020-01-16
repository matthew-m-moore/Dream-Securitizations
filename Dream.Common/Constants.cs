using Dream.Common.Enums;
using System;
using System.Collections.Generic;

namespace Dream.Common
{
    public static class Constants
    {
        public const int MonthsInOneYear = 12;

        public const int ThreeHundredSixtyDaysInOneYear = 360;
        public const int ThreeHundredSixtyFiveDaysInOneYear = 365;
        public const int ThreeHundredSixtySixDaysInOneYear = 366;

        public const int ThirtyDaysInOneMonth = 30;
        public const int ThirtyOneDaysInOneMonth = 31;

        public const int OneHundredPercentagePoints = 100;
        public const int BpsPerOneHundredPercentagePoints = 10000;

        public const double TwentyFiveBpsShock = 0.0025;

        public const string NominalSpreadBasedPricing = "N-Spread (bps)";
        public const string YieldBasedPricing = "Yield (%)";
        public const string MarketValueBasedPricing = "Market Value ($)";
        public const string PercentOfBalanceBasedPricing = "Price (%)";

        public const string Automatic = "Automatic";
        public const string LoanLevel = "Loan-Level";

        public const string DatabaseOwnerSchemaName = "dbo";
        public const string DreamSchemaName = "dream";
        public const string FinanceManagementSchemaName = "FinanceManagement";

        public static DateTime SqlMinDate = new DateTime(1900, 1, 1);

        public static int ProcessorCount = Environment.ProcessorCount;

        public static List<InterestRateCurveType> DiscountFactorCurves = new List<InterestRateCurveType>
        {
            InterestRateCurveType.LiborDiscount,
            InterestRateCurveType.TreasuryDiscount
        };

        public static List<InterestRateCurveType> ZeroVolatilitySpotRateCurves = new List<InterestRateCurveType>
        {
            InterestRateCurveType.LiborSpot,
            InterestRateCurveType.TreasurySpot
        };
    }
}
