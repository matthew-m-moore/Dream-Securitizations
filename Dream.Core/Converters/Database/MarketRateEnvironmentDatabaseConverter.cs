using Dream.Common.Curves;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using Dream.IO.Database.Entities.InterestRates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Database
{
    public class MarketRateEnvironmentDatabaseConverter
    {
        private Dictionary<int, (string Description, double? StandardDivisor)> _marketDataTypesDictionary;
        private Dictionary<int, (string Description, int? TenorInMonths, int? RateIndexGroupId)> _rateIndexInformationDictionary;
        private Dictionary<int, string> _rateIndexGroupsDictionary;

        public MarketRateEnvironmentDatabaseConverter(
            Dictionary<int, (string Description, double? StandardDivisor)> marketDataTypesDictionary,
            Dictionary<int, (string Description, int? TenorInMonths, int? RateIndexGroupId)> rateIndexInformationDictionary,
            Dictionary<int, string> rateIndexGroupsDictionary)
        {
            _marketDataTypesDictionary = marketDataTypesDictionary;
            _rateIndexInformationDictionary = rateIndexInformationDictionary;
            _rateIndexGroupsDictionary = rateIndexGroupsDictionary;
        }

        /// <summary>
        /// Converts a collection of MarketDataEntity into a collection MarketDataPoint, which are added to the internal MarketRateEnvironment
        /// </summary>
        public List<MarketDataPoint> ConvertListOfMarketDataEntities(List<MarketDataEntity> marketDataEntities)
        {
            var listOfMarketDataPoints = new List<MarketDataPoint>();

            foreach (var marketDataEntity in marketDataEntities)
            {
                var marketDataValue = marketDataEntity.DataValue;
                var marketDate = marketDataEntity.MarketDateTime;

                var marketDataTypeEntry = _marketDataTypesDictionary[marketDataEntity.MarketDataTypeId];
                if (marketDataTypeEntry.StandardDivisor.HasValue)
                {
                    marketDataValue /= marketDataTypeEntry.StandardDivisor.Value;
                }

                var rateIndexEntry = _rateIndexInformationDictionary[marketDataEntity.RateIndexId];
                var tenorInMonths = rateIndexEntry.TenorInMonths;
                var rateIndexGroupId = rateIndexEntry.RateIndexGroupId;

                var marketDataGrouping = default(MarketDataGrouping);
                if (rateIndexGroupId.HasValue)
                {
                    var rateIndexGroup = _rateIndexGroupsDictionary[rateIndexGroupId.Value];
                    marketDataGrouping = MarketDataGroupingDatabaseConverter.ConvertDescription(rateIndexGroup);
                }

                var marketDataType = MarketDataTypeDatabaseConverter.ConvertDescription(marketDataTypeEntry.Description);
                var interestRateCurveType = InterestRateCurveTypeDatabaseConverter.ConvertDescription(rateIndexEntry.Description);

                var marketDataPoint = new MarketDataPoint(
                    marketDataGrouping, 
                    marketDataType, 
                    interestRateCurveType, 
                    marketDate, 
                    marketDataValue, 
                    tenorInMonths);

                listOfMarketDataPoints.Add(marketDataPoint);
            }

            return listOfMarketDataPoints;
        }

        /// <summary>
        /// Converts a collection of MarketDataEntity into a collection MarketDataPoint, which are added to the internal MarketRateEnvironment
        /// </summary>
        public Dictionary<InterestRateCurveType, InterestRateCurve> ConvertListOfRateCurveDataEntities(List<RateCurveDataEntity> rateCurveDataEntities)
        {
            var rateCurveDictionary = new Dictionary<InterestRateCurveType, InterestRateCurve>();
            var rateCurveDataEntityGroups = rateCurveDataEntities.GroupBy(e => e.RateIndexId);

            foreach (var rateCurveDataEntityGroup in rateCurveDataEntityGroups)
            {
                var rateIndexEntry = _rateIndexInformationDictionary[rateCurveDataEntityGroup.Key];
                var interestRateCurveType = InterestRateCurveTypeDatabaseConverter.ConvertDescription(rateIndexEntry.Description);
                var tenorInMonths = rateIndexEntry.TenorInMonths;
                
                // We are just assuming the data comes in here with one data point for each month, all nice and clean
                var listOfRateCurveDataEntities = rateCurveDataEntityGroup.ToList();            
                var orderedListOfRateCurveDataEntities = listOfRateCurveDataEntities.OrderBy(e => e.ForwardDateTime.Ticks);

                var listOfRateCurveValues = new List<double>();
                foreach (var rateCurveDataEntity in orderedListOfRateCurveDataEntities)
                {
                    var rateCurveValue = rateCurveDataEntity.RateCurveValue;

                    var marketDataTypeEntry = _marketDataTypesDictionary[rateCurveDataEntity.MarketDataTypeId];
                    if (marketDataTypeEntry.StandardDivisor.HasValue)
                    {
                        rateCurveValue /= marketDataTypeEntry.StandardDivisor.Value;
                    }

                    listOfRateCurveValues.Add(rateCurveValue);
                }

                var interestRateCurve = new Curve<double>(listOfRateCurveValues);
                if (!rateCurveDictionary.ContainsKey(interestRateCurveType))
                {
                    var marketDate = orderedListOfRateCurveDataEntities.First().MarketDateTime;
                    rateCurveDictionary.Add(interestRateCurveType, new InterestRateCurve(interestRateCurveType, marketDate, interestRateCurve, tenorInMonths));
                }
                else
                {
                    throw new Exception(string.Format("ERROR: Cannot add two rate curves for the same rate: '{0}'. Please check the market data inputs.",
                        rateIndexEntry.Description));
                }
            }

            return rateCurveDictionary;
        }
    }
}
