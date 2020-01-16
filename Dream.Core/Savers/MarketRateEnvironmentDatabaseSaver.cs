using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.InterestRates;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Dream.Core.BusinessLogic.Containers;

namespace Dream.Core.Savers
{
    public class MarketRateEnvironmentDatabaseSaver : DatabaseSaver
    {
        private MarketRateEnvironment _marketRateEnvironment;
        private MarketRateEnvironmentDatabaseRepository _marketRateEnvironmentDatabaseRepository;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public MarketRateEnvironmentDatabaseSaver(
            MarketRateEnvironment marketRateEnvironment,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository,
            DateTime cutOffDate,
            string description)
        : base(cutOffDate, description)
        {
            _marketRateEnvironment = marketRateEnvironment;
            _typesAndConventionsDatabaseRepository = typesAndConventionsDatabaseRepository;

            _marketRateEnvironmentDatabaseRepository = new MarketRateEnvironmentDatabaseRepository();
        }

        public int SaveMarketRateEnvironment()
        {
            var marketDataSetId = SaveMarketDataSet();
            var rateCurveDataSetId = SaveRateCurveDataSet();

            var marketRateEnvironmentEntity = new MarketRateEnvironmentEntity
            {
                CutOffDate = _CutOffDate,
                MarketRateEnvironmentDescription = _Description,
                MarketDataSetId = marketDataSetId,
                RateCurveDataSetId = rateCurveDataSetId
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.MarketRateEnvironmentEntities.Add(marketRateEnvironmentEntity);
                securitizationEngineContext.SaveChanges();
            }

            return marketRateEnvironmentEntity.MarketRateEnvironmentId;
        }

        private int? SaveMarketDataSet()
        {
            if (_marketRateEnvironment.MarketDataPointsDictionary == null || !_marketRateEnvironment.MarketDataPointsDictionary.Any()) return null;

            var marketDataSet = new MarketDataSetEntity
            {
                CutOffDate = _CutOffDate,
                MarketDataSetDescription = _Description
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.MarketDataSetEntities.Add(marketDataSet);
                securitizationEngineContext.SaveChanges();
            }

            SaveMarketData(marketDataSet.MarketDataSetId);
            return marketDataSet.MarketDataSetId;
        }

        private int? SaveRateCurveDataSet()
        {
            if (_marketRateEnvironment.RateCurveDictionary == null || !_marketRateEnvironment.RateCurveDictionary.Any()) return null;

            var rateCurveDataSet = new RateCurveDataSetEntity
            {
                CutOffDate = _CutOffDate,
                RateCurveDataSetDescription = _Description
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.RateCurveDataSetEntities.Add(rateCurveDataSet);
                securitizationEngineContext.SaveChanges();
            }

            SaveRateCurveData(rateCurveDataSet.RateCurveDataSetId);
            return rateCurveDataSet.RateCurveDataSetId;
        }

        private void SaveMarketData(int marketDataSetId)
        {
            var listOfMarketDataEntities = new List<MarketDataEntity>();
            foreach (var marketDataPointEntry in _marketRateEnvironment.MarketDataPointsDictionary)
            {
                var marketDataGrouping = marketDataPointEntry.Key;
                var marketDataPoints = marketDataPointEntry.Value;

                foreach (var marketDataPoint in marketDataPoints)
                {
                    var marketDataTypeId = _typesAndConventionsDatabaseRepository.MarketDataTypesReversed[marketDataPoint.MarketDataType];
                    var marketDataValueDivisor = _marketRateEnvironmentDatabaseRepository.MarketDataTypes[marketDataTypeId].StandardDivisor.GetValueOrDefault(1.0);
                    var rateIndexId = GetInterestRateCurveTypeId(marketDataPoint);

                    var marketDataEntity = new MarketDataEntity
                    {
                        MarketDataSetId = marketDataSetId,
                        MarketDataTypeId = marketDataTypeId,
                        RateIndexId = rateIndexId,
                        MarketDateTime = marketDataPoint.MarketDate,
                        DataValue = marketDataPoint.Value * marketDataValueDivisor
                    };

                    listOfMarketDataEntities.Add(marketDataEntity);
                }
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.MarketDataEntities.AddRange(listOfMarketDataEntities);
                securitizationEngineContext.SaveChanges();
            }
        }

        private int GetInterestRateCurveTypeId(MarketDataPoint marketDataPoint)
        {
            if (marketDataPoint.InterestRateCurveType != default(InterestRateCurveType))
            {
                return _typesAndConventionsDatabaseRepository.InterestRateCurveTypesReversed[marketDataPoint.InterestRateCurveType];
            }

            var marketDataGrouping = marketDataPoint.MarketDataGrouping;
            var tenorInMonths = marketDataPoint.TenorInMonths;

            if (marketDataGrouping == default(MarketDataGrouping) || !tenorInMonths.HasValue || tenorInMonths.Value == 0)
            {
                return _typesAndConventionsDatabaseRepository.InterestRateCurveTypesReversed[default(InterestRateCurveType)];
            }

            var interestRateCurveType = _typesAndConventionsDatabaseRepository.InterestRateCurveTypeMapping[marketDataGrouping][tenorInMonths.Value];
            return _typesAndConventionsDatabaseRepository.InterestRateCurveTypesReversed[interestRateCurveType];
        }

        private void SaveRateCurveData(int rateCurveDataSetId)
        {
            var listOfRateCurveDataEntities = new List<RateCurveDataEntity>();
            foreach (var rateCurveDataPointEntry in _marketRateEnvironment.RateCurveDictionary)
            {
                var marketDataType = default(MarketDataType);
                var interestRateCurve = rateCurveDataPointEntry.Value;
                if (interestRateCurve.IsForwardCurve) marketDataType = MarketDataType.Rate;
                if (interestRateCurve.IsDiscountFactorCurve) marketDataType = MarketDataType.DiscountFactor;

                var rateIndexId = _typesAndConventionsDatabaseRepository.InterestRateCurveTypesReversed[rateCurveDataPointEntry.Key];
                var marketDataTypeId = _typesAndConventionsDatabaseRepository.MarketDataTypesReversed[marketDataType];

                var rateCurveValuesCount = interestRateCurve.RateCurve.Count();
                for (var i = 0; i < rateCurveValuesCount; i++)
                {
                    var rateCurveValue = interestRateCurve.RateCurve[i];

                    // Note, this assumes rate curve data is provided with monthly resolution, which is most likely the case
                    var marketDataEntity = new RateCurveDataEntity
                    {
                        RateCurveDataSetId = rateCurveDataSetId,
                        MarketDataTypeId = marketDataTypeId,
                        RateIndexId = rateIndexId,
                        MarketDateTime = interestRateCurve.MarketDate,
                        ForwardDateTime = interestRateCurve.MarketDate.AddMonths(i),
                        RateCurveValue = rateCurveValue
                    };

                    listOfRateCurveDataEntities.Add(marketDataEntity);
                }
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.RateCurveDataEntities.AddRange(listOfRateCurveDataEntities);
                securitizationEngineContext.SaveChanges();
            }
        }
    }
}
