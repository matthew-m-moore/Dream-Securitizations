using System.ComponentModel;

namespace Dream.Common.Enums
{
    public enum MarketDataGrouping
    {
        [Description("N/A")]
        None,

        [Description("LIBOR")]
        Libor,
        [Description("Treasuries (I-Spread)")]
        Treasuries,
        [Description("Swaps (N-Spread)")]
        Swaps
    }
}
