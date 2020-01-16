using System.ComponentModel;

namespace Dream.Common.Enums
{
    public enum InterestRateCurveType
    {
        [Description("None")]
        None,

        [Description("LIBOR-O/N")]
        LiborOvernight,
        [Description("LIBOR-1W")]
        Libor1Wk,
        [Description("LIBOR-1M")]
        Libor1Mo,
        [Description("LIBOR-2M")]
        Libor2Mo,
        [Description("LIBOR-3M")]
        Libor3Mo,
        [Description("LIBOR-6M")]
        Libor6Mo,
        [Description("LIBOR-1Y")]
        Libor1Yr,

        [Description("LIBOR-Based Zero Rates (E-Spread)")]
        LiborSpot,
        [Description("LIBOR-Based Discount Factors (E-Spread)")]
        LiborDiscount,

        [Description("CMT-1M")]
        Treasury1Mo,
        [Description("CMT-3M")]
        Treasury3Mo,
        [Description("CMT-6M")]
        Treasury6Mo,
        [Description("CMT-1Y")]
        Treasury1Yr,
        [Description("CMT-2Y")]
        Treasury2Yr,
        [Description("CMT-3Y")]
        Treasury3Yr,
        [Description("CMT-5Y")]
        Treasury5Yr,
        [Description("CMT-7Y")]
        Treasury7Yr,
        [Description("CMT-10Y")]
        Treasury10Yr,
        [Description("CMT-20Y")]
        Treasury20Yr,
        [Description("CMT-30Y")]
        Treasury30Yr,

        [Description("Treasury Zero Rates (Z-Spread)")]
        TreasurySpot,
        [Description("Treasury Discount Factors (Z-Spread)")]
        TreasuryDiscount,

        [Description("Swap-4M")]
        Swap4Mo,
        [Description("Swap-5M")]
        Swap5Mo,
        [Description("Swap-6M")]
        Swap6Mo,
        [Description("Swap-7M")]
        Swap7Mo,
        [Description("Swap-8M")]
        Swap8Mo,
        [Description("Swap-9M")]
        Swap9Mo,
        [Description("Swap-10M")]
        Swap10Mo,
        [Description("Swap-11M")]
        Swap11Mo,
        [Description("Swap-18M")]
        Swap18Mo,
        [Description("Swap-1Y")]
        Swap1Yr,
        [Description("Swap-2Y")]
        Swap2Yr,
        [Description("Swap-3Y")]
        Swap3Yr,
        [Description("Swap-4Y")]
        Swap4Yr,
        [Description("Swap-5Y")]
        Swap5Yr,
        [Description("Swap-6Y")]
        Swap6Yr,
        [Description("Swap-7Y")]
        Swap7Yr,
        [Description("Swap-8Y")]
        Swap8Yr,
        [Description("Swap-9Y")]
        Swap9Yr,
        [Description("Swap-10Y")]
        Swap10Yr,
        [Description("Swap-11Y")]
        Swap11Yr,
        [Description("Swap-12Y")]
        Swap12Yr,
        [Description("Swap-15Y")]
        Swap15Yr,
        [Description("Swap-20Y")]
        Swap20Yr,
        [Description("Swap-25Y")]
        Swap25Yr,
        [Description("Swap-30Y")]
        Swap30Yr,
        [Description("Swap-40Y")]
        Swap40Yr,
        [Description("Swap-50Y")]
        Swap50Yr
    }
}
