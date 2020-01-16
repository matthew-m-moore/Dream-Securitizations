using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database
{
    public class MarketDataGroupingDatabaseConverter
    {
        private const string _libor = "London Inter-Bank Offer Rates (LIBOR)";
        private const string _swap = "Interest Rate Swaps, Fixed Leg Rate (Swap)";
        private const string _treasury = "Constant Maturity Treasury (CMT)";

        public static MarketDataGrouping ConvertDescription(string marketDataGroupingDescription)
        {
            if (marketDataGroupingDescription == null) return default(MarketDataGrouping);

            switch (marketDataGroupingDescription)
            {
                case _libor:
                    return MarketDataGrouping.Libor;

                case _swap:
                    return MarketDataGrouping.Swaps;

                case _treasury:
                    return MarketDataGrouping.Treasuries;

                default:
                    throw new Exception(string.Format("ERROR: The market data type '{0}' is not supported", marketDataGroupingDescription));
            }
        }
    }
}
