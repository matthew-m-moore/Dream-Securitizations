using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database
{
    public class MarketDataTypeDatabaseConverter
    {
        private const string _price = "Price (%)";
        private const string _spread = "Spread (bps)";
        private const string _rate = "Rate (%)";
        private const string _marketValue = "Market Value ($)";
        private const string _discountFactor = "Discount Factor";
        private const string _none = "None";

        public static MarketDataType ConvertDescription(string marketDataTypeDescription)
        {
            if (string.IsNullOrEmpty(marketDataTypeDescription)) return default(MarketDataType);

            switch (marketDataTypeDescription)
            {
                case _price:
                    return MarketDataType.Price;

                case _spread:
                    return MarketDataType.Spread;

                case _rate:
                    return MarketDataType.Rate;

                case _marketValue:
                    return MarketDataType.MarketValue;

                case _discountFactor:
                    return MarketDataType.DiscountFactor;

                case _none:
                    return MarketDataType.None;

                default:
                    throw new Exception(string.Format("ERROR: The market data type '{0}' is not supported", marketDataTypeDescription));
            }
        }
    }
}
