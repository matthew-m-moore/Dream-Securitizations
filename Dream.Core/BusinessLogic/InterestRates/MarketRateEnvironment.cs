using Dream.Common.Curves;
using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.Containers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.InterestRates
{
    public class MarketRateEnvironment
    {
        public DateTime MarketDate { get; set; }

        public Dictionary<MarketDataGrouping, List<MarketDataPoint>> MarketDataPointsDictionary { get; private set; }
        public Dictionary<InterestRateCurveType, InterestRateCurve> RateCurveDictionary { get; private set; }
        public Dictionary<InterestRateCurveType, RateCurveCalculationLogic> CurveCalculationLogicDictionary { get; private set; }

        public MarketRateEnvironment(DateTime marketDate) : base()
        {
            MarketDate = marketDate;
        }

        public MarketRateEnvironment()
        {
            MarketDataPointsDictionary = new Dictionary<MarketDataGrouping, List<MarketDataPoint>>();
            RateCurveDictionary = new Dictionary<InterestRateCurveType, InterestRateCurve>();
            CurveCalculationLogicDictionary = new Dictionary<InterestRateCurveType, RateCurveCalculationLogic>();
        }

        public InterestRateCurve this[InterestRateCurveType interestRateIndex]
        {
            get
            {
                if (RateCurveDictionary.ContainsKey(interestRateIndex))
                {
                    return RateCurveDictionary[interestRateIndex];
                }

                if (CurveCalculationLogicDictionary.ContainsKey(interestRateIndex))
                {
                    var curveCalculationLogic = CurveCalculationLogicDictionary[interestRateIndex];
                    var rateCurve = curveCalculationLogic.CalculateRateCurve();
                    return rateCurve;
                }

                throw new Exception(
                    string.Format("ERROR: No rate curve or calculation found for index {0}",
                    interestRateIndex.ToString()));
            }
        }

        /// <summary>
        /// Adds a collection of market data points to the list of market data for the MarketRateEnvironment object.
        /// </summary>
        public void AddMarketDataPoints(List<MarketDataPoint> listOfMarketDataPoints)
        {
            foreach(var marketDataPoint in listOfMarketDataPoints)
            {
                AddMarketDataPoint(marketDataPoint.MarketDataGrouping, marketDataPoint);
            }
        }

        /// <summary>
        /// Adds a single market data point to the list of market data for the MarketRateEnvironment object.
        /// </summary>
        public void AddMarketDataPoint(MarketDataGrouping marketDataGrouping, MarketDataPoint marketDataPoint)
        {
            if (!MarketDataPointsDictionary.ContainsKey(marketDataGrouping))
            {
                MarketDataPointsDictionary.Add(marketDataGrouping, new List<MarketDataPoint>());
            }

            // Note, there is no checking for redundant data points, as this will be assumed to be handled up stream, if needed
            MarketDataPointsDictionary[marketDataGrouping].Add(marketDataPoint);
        }

        /// <summary>
        /// Combines a given rate curve dictionary with whatever internal rate curve dictionary exists.
        /// </summary>
        public void AddRateCurveDictionary(Dictionary<InterestRateCurveType, InterestRateCurve> rateCurveDictionary)
        {
            RateCurveDictionary.Combine(rateCurveDictionary);
        }

        /// <summary>
        /// Adds a rate curve to the internal rate curve dictionary that exists.
        /// </summary>
        public void AddRateCurve(KeyValuePair<InterestRateCurveType, InterestRateCurve> rateCurveKeyValuePair)
        {
            RateCurveDictionary.Add(rateCurveKeyValuePair);
        }


        public MarketRateEnvironment Copy()
        {
            var marketRateEnvironment = new MarketRateEnvironment
            {
                MarketDataPointsDictionary = MarketDataPointsDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(m => m.Copy()).ToList()),
                RateCurveDictionary = RateCurveDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
                CurveCalculationLogicDictionary = CurveCalculationLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };

            return marketRateEnvironment;
        }
    }
}
