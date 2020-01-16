using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Converters.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Repositories.Database
{
    public class MarketRateEnvironmentDatabaseRepository : DatabaseRepository
    {
        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private int _marketRateEnvironmentId;

        private MarketRateEnvironment _marketRateEnvironment;

        private Dictionary<int, (string Description, double? StandardDivisor)> _marketDataTypes;
        public Dictionary<int, (string Description, double? StandardDivisor)> MarketDataTypes
        {
            get
            {
                if (_marketDataTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var marketDataTypeEntities = securitizationEngineContext.MarketDataTypeEntities.ToList();
                        _marketDataTypes = marketDataTypeEntities.ToDictionary(
                            e => e.MarketDataTypeId, 
                            e => (Description: e.MarketDataTypeDescription, StandardDivisor: e.StandardDivisor));
                    }
                }

                return _marketDataTypes;
            }
        }

        private Dictionary<int, (string Description, int? TenorInMonths, int? RateIndexGroupId)> _rateIndexInformation;
        public Dictionary<int, (string Description, int? TenorInMonths, int? RateIndexGroupId)> RateIndexInformation
        {
            get
            {
                if (_rateIndexInformation == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var rateIndexEntities = securitizationEngineContext.RateIndexEntities.ToList();
                        _rateIndexInformation = rateIndexEntities.ToDictionary(
                            e => e.RateIndexId, 
                            e => (Description: e.RateIndexDescription, TenorInMonths: e.TenorInMonths, RateIndexGroupId: e.RateIndexGroupId));
                    }
                }

                return _rateIndexInformation;
            }
        }

        private Dictionary<int, string> _rateIndexGroups;
        public Dictionary<int, string> RateIndexGroups
        {
            get
            {
                if (_rateIndexGroups == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var rateIndexGroupEntities = securitizationEngineContext.RateIndexGroupEntities.ToList();
                        _rateIndexGroups = rateIndexGroupEntities.ToDictionary(e => e.RateIndexGroupId, e => e.RateIndexGroupDescription);
                    }
                }

                return _rateIndexGroups;
            }
        }

        public MarketRateEnvironmentDatabaseRepository()
        {
            _marketRateEnvironment = new MarketRateEnvironment();
        }

        public MarketRateEnvironmentDatabaseRepository(int marketRateEnvironmentId) : this()
        {
            _marketRateEnvironmentId = marketRateEnvironmentId;
        }

        /// <summary>
        /// Retrieves a MarketRateEnvironment object from the relevant database inputs
        /// </summary>
        public MarketRateEnvironment GetMarketRateEnvironment()
        {
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                var marketRateEnvironmentEntity = securitizationEngineContext.MarketRateEnvironmentEntities
                    .Single(e => e.MarketRateEnvironmentId == _marketRateEnvironmentId);

                _marketRateEnvironment.MarketDate = marketRateEnvironmentEntity.CutOffDate;

                var marketRateEnvironmentDatabaseConverter = new MarketRateEnvironmentDatabaseConverter(MarketDataTypes, RateIndexInformation, RateIndexGroups);

                var marketDataSetId = marketRateEnvironmentEntity.MarketDataSetId;
                if (marketDataSetId.HasValue)
                {
                    var marketDataEntities = securitizationEngineContext.MarketDataEntities
                        .Where(e => e.MarketDataSetId == marketDataSetId.Value).ToList();
              
                    var marketDataPoints = marketRateEnvironmentDatabaseConverter.ConvertListOfMarketDataEntities(marketDataEntities);
                    _marketRateEnvironment.AddMarketDataPoints(marketDataPoints);
                }

                var rateCurveDataSetId = marketRateEnvironmentEntity.RateCurveDataSetId;
                if (rateCurveDataSetId.HasValue)
                {
                    var rateCurveDataEntities = securitizationEngineContext.RateCurveDataEntities
                        .Where(e => e.RateCurveDataSetId == rateCurveDataSetId.Value).ToList();

                    var rateCurveDictionary = marketRateEnvironmentDatabaseConverter.ConvertListOfRateCurveDataEntities(rateCurveDataEntities);
                    _marketRateEnvironment.AddRateCurveDictionary(rateCurveDictionary);
                }

                return _marketRateEnvironment;
            }
        }
    }
}
