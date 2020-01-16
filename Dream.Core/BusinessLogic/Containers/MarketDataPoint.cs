using Dream.Common.Enums;
using System;

namespace Dream.Core.BusinessLogic.Containers
{
    public class MarketDataPoint
    {
        public MarketDataGrouping MarketDataGrouping { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public InterestRateCurveType InterestRateCurveType { get; set; }
        public DateTime MarketDate { get; set; }
        public double Value { get; set; }
        public int? TenorInMonths { get; set; }

        public MarketDataPoint(
            MarketDataGrouping marketDataGrouping,
            MarketDataType marketDataType,
            double value,
            int? tenorInMonths = null)
        {
            MarketDataGrouping = marketDataGrouping;
            MarketDataType = marketDataType;
            Value = value;
            TenorInMonths = tenorInMonths;
        }

        public MarketDataPoint(
            MarketDataGrouping marketDataGrouping,
            MarketDataType marketDataType,
            InterestRateCurveType interestRateCurveType,
            DateTime marketDate,
            double value,
            int? tenorInMonths = null)
        {
            MarketDataGrouping = marketDataGrouping;
            MarketDataType = marketDataType;
            InterestRateCurveType = interestRateCurveType;
            MarketDate = marketDate;
            Value = value;
            TenorInMonths = tenorInMonths;
        }

        public MarketDataPoint Copy()
        {
            return new MarketDataPoint(
                MarketDataGrouping,
                MarketDataType,
                InterestRateCurveType,
                new DateTime(MarketDate.Ticks),
                Value,
                TenorInMonths);
        }
    }
}
