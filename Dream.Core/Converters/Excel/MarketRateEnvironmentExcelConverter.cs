using Dream.Core.BusinessLogic.Containers;
using Dream.Common.Enums;
using Dream.IO.Excel.Entities;
using System;
using System.Collections.Generic;

namespace Dream.Core.Converters.Excel
{
    public class MarketRateEnvironmentExcelConverter
    {
        /// <summary>
        /// Converts a collection of MarketDataRecord into a collection MarketDataPoint, which are added to the internal MarketRateEnvironment
        /// </summary>
        public static List<MarketDataPoint> ConvertListOfMarketDataRecords(List<MarketDataRecord> marketDataRecords, DateTime marketDate)
        {
            var listOfMarketDataPoints = new List<MarketDataPoint>();

            foreach (var marketDataRecord in marketDataRecords)
            {
                var marketDataGrouping = (MarketDataGrouping) Enum.Parse(typeof(MarketDataGrouping), marketDataRecord.MarketDataGrouping);
                var marketDataType = (MarketDataType) Enum.Parse(typeof(MarketDataType), marketDataRecord.MarketDataType);

                var marketDataPoint = new MarketDataPoint(marketDataGrouping, marketDataType, marketDataRecord.Value, marketDataRecord.TenorInMonths);
                marketDataPoint.MarketDate = marketDate;

                listOfMarketDataPoints.Add(marketDataPoint);
            }

            return listOfMarketDataPoints;
        }
    }
}
