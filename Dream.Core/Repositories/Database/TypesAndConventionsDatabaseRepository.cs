using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.Converters.Database;
using Dream.Core.Converters.Database.Securitization;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.InterestRates;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Repositories.Database
{
    public class TypesAndConventionsDatabaseRepository : DatabaseRepository
    {
        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private Dictionary<int, string> _pricingTypes;
        public Dictionary<string, int> PricingTypesReversed => PricingTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, string> PricingTypes
        {
            get
            {
                if (_pricingTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var pricingTypes = securitizationEngineContext.PricingTypeEntities.ToList();
                        _pricingTypes = pricingTypes.ToDictionary(
                            e => e.PricingTypeId,
                            e => e.PricingTypeDescription);
                    }
                }

                return _pricingTypes;
            }
        }


        private Dictionary<int, MarketDataType> _marketDataTypes;
        public Dictionary<MarketDataType, int> MarketDataTypesReversed => MarketDataTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, MarketDataType> MarketDataTypes
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
                            e => MarketDataTypeDatabaseConverter.ConvertDescription(e.MarketDataTypeDescription));
                    }
                }

                return _marketDataTypes;
            }
        }

        private Dictionary<int, InterestRateCurveType> _interestRateCurveTypes;
        public Dictionary<InterestRateCurveType, int> InterestRateCurveTypesReversed => InterestRateCurveTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, InterestRateCurveType> InterestRateCurveTypes
        {
            get
            {
                if (_interestRateCurveTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var rateIndexEntities = securitizationEngineContext.RateIndexEntities.ToList();
                        _interestRateCurveTypes = rateIndexEntities.ToDictionary(
                            e => e.RateIndexId,
                            e => InterestRateCurveTypeDatabaseConverter.ConvertDescription(e.RateIndexDescription));
                    }
                }

                return _interestRateCurveTypes;
            }
        }

        private Dictionary<MarketDataGrouping, Dictionary<int, InterestRateCurveType>> _interestRateCurveTypeMapping;
        public Dictionary<MarketDataGrouping, Dictionary<int, InterestRateCurveType>> InterestRateCurveTypeMapping
        {
            get
            {
                if (_interestRateCurveTypeMapping == null)
                {
                    var mappableRateIndexEntities = new List<RateIndexEntity>();
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        mappableRateIndexEntities = securitizationEngineContext.RateIndexEntities
                            .Where(e => e.RateIndexGroupId.HasValue && e.TenorInMonths.HasValue).ToList();
                    }

                    var groupedRateIndexEntities = mappableRateIndexEntities.GroupBy(e => e.RateIndexGroupId);

                    _interestRateCurveTypeMapping = groupedRateIndexEntities.ToDictionary(
                        rateIndexGroup => MarketDataGroupings[rateIndexGroup.Key.Value],
                        rateIndexGroup => rateIndexGroup.ToDictionary(e => e.TenorInMonths.Value, e => InterestRateCurveTypes[e.RateIndexId]));
                }

                return _interestRateCurveTypeMapping;
            }
        }


        private Dictionary<int, PerformanceCurveType> _performanceCurveTypes;
        public Dictionary<PerformanceCurveType, int> PerformanceCurveTypesReversed => PerformanceCurveTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, PerformanceCurveType> PerformanceCurveTypes
        {
            get
            {
                if (_performanceCurveTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var performanceCurveTypeEntities = securitizationEngineContext.PerformanceAssumptionTypeEntities.ToList();
                        _performanceCurveTypes = performanceCurveTypeEntities.ToDictionary(
                            e => e.PerformanceAssumptionTypeId,
                            e =>
                            {
                                var performanceCurveTypeAbbreviation = e.PerformanceAssumptionTypeAbbreviation.ToTitleCase();
                                var performanceCurveType = (PerformanceCurveType)Enum.Parse(typeof(PerformanceCurveType), performanceCurveTypeAbbreviation);
                                return performanceCurveType;
                            });
                    }
                }

                return _performanceCurveTypes;
            }
        }

        private Dictionary<int, MarketDataGrouping> _marketDataGroupings;
        public Dictionary<MarketDataGrouping, int> MarketDataGroupingsReversed => MarketDataGroupings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, MarketDataGrouping> MarketDataGroupings
        {
            get
            {
                if (_marketDataGroupings == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var rateIndexEntities = securitizationEngineContext.RateIndexGroupEntities.ToList();
                        _marketDataGroupings = rateIndexEntities.ToDictionary(
                            e => e.RateIndexGroupId,
                            e => MarketDataGroupingDatabaseConverter.ConvertDescription(e.RateIndexGroupDescription));
                    }
                }

                return _marketDataGroupings;
            }
        }

        private Dictionary<int, Month> _allowedMonths;
        public Dictionary<Month, int> AllowedMonthsReversed => AllowedMonths.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, Month> AllowedMonths
        {
            get
            {
                if (_allowedMonths == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var allowedMonths = securitizationEngineContext.AllowedMonthEntities.ToList();
                        _allowedMonths = allowedMonths.ToDictionary(
                            e => e.AllowedMonthId,
                            e => MonthDatabaseConverter.ConvertString(e.AllowedMonthLongDescription));
                    }
                }

                return _allowedMonths;
            }
        }


        private Dictionary<int, DayCountConvention> _dayCountConventions;
        public Dictionary<DayCountConvention, int> DayCountConventionsReversed => DayCountConventions.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, DayCountConvention> DayCountConventions
        {
            get
            {
                if (_dayCountConventions == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var dayCountConventions = securitizationEngineContext.DayCountConventionEntities.ToList();
                        _dayCountConventions = dayCountConventions.ToDictionary(
                            e => e.DayCountConventionId,
                            e => DayCountConventionDatabaseConverter.ConvertString(e.DayCountConventionDescription));
                    }
                }

                return _dayCountConventions;
            }
        }

        private Dictionary<int, CompoundingConvention> _compoundingConventions;
        public Dictionary<CompoundingConvention, int> CompoundingConventionsReversed => CompoundingConventions.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, CompoundingConvention> CompoundingConventions
        {
            get
            {
                if (_compoundingConventions == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var compoundingConventions = securitizationEngineContext.CompoundingConventionEntities.ToList();
                        _compoundingConventions = compoundingConventions.ToDictionary(
                            e => e.CompoundingConventionId,
                            e => CompoundingConventionDatabaseConverter.ConvertString(e.CompoundingConventionDescription));
                    }
                }

                return _compoundingConventions;
            }
        }

        private Dictionary<int, PaymentConvention> _paymentConventions;
        public Dictionary<PaymentConvention, int> PaymentConventionsReversed => PaymentConventions.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, PaymentConvention> PaymentConventions
        {
            get
            {
                if (_paymentConventions == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var paymentConventions = securitizationEngineContext.PaymentConventionEntities.ToList();
                        _paymentConventions = paymentConventions.ToDictionary(
                            e => e.PaymentConventionId,
                            e => PaymentConventionDatabaseConverter.ConvertString(e.PaymentConventionDescription));
                    }
                }

                return _paymentConventions;
            }
        }

        private Dictionary<int, TrancheCashFlowType> _trancheCashFlowTypes;
        public Dictionary<TrancheCashFlowType, int> TrancheCashFlowTypesReversed => TrancheCashFlowTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        public Dictionary<int, TrancheCashFlowType> TrancheCashFlowTypes
        {
            get
            {
                if (_trancheCashFlowTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var trancheCashFlowTypeEntities = securitizationEngineContext.TrancheCashFlowTypeEntities.ToList();
                        _trancheCashFlowTypes = trancheCashFlowTypeEntities.ToDictionary(
                            e => e.TrancheCashFlowTypeId,
                            e => TrancheCashFlowTypeDatabaseConverter.ConvertString(e.TrancheCashFlowTypeDescription));
                    }
                }

                return _trancheCashFlowTypes;
            }
        }
    }
}
