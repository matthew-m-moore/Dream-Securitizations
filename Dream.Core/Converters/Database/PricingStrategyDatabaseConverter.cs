using Dream.Common;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.Repositories.Database;
using Dream.IO.Database.Entities.Securitization;
using System;

namespace Dream.Core.Converters.Database
{
    public class PricingStrategyDatabaseConverter
    {
        private MarketRateEnvironment _marketRateEnivironment;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        public PricingStrategyDatabaseConverter(
            MarketRateEnvironment marketRateEnvironment, 
            TypesAndConventionsDatabaseRepository pricingConventionsDatabaseRepository)
        {
            _marketRateEnivironment = marketRateEnvironment;
            _typesAndConventionsDatabaseRepository = pricingConventionsDatabaseRepository;
        }

        public PricingStrategy ExtractPricingStrategyFromSecuritizationNode(SecuritizationNodeEntity securitizationNodeEntity)
        {
            if (!securitizationNodeEntity.TranchePricingTypeId.HasValue)
            {
                return new DoNothingPricingStrategy();
            }

            if (!securitizationNodeEntity.TranchePricingDayCountConventionId.HasValue
             || !securitizationNodeEntity.TranchePricingCompoundingConventionId.HasValue
             || !securitizationNodeEntity.TranchePricingValue.HasValue)
            {
                throw new Exception("INTERNAL ERROR: Pricing type is missing key information such as day-count convention, compounding convention, or pricing value. Please report this error");
            }

            var pricingTypeId = securitizationNodeEntity.TranchePricingTypeId.Value;
            var pricingMethodology = _typesAndConventionsDatabaseRepository.PricingTypes[pricingTypeId];

            var pricingValue = securitizationNodeEntity.TranchePricingValue.Value;

            var dayCountConventionId = securitizationNodeEntity.TranchePricingDayCountConventionId.Value;
            var compoundingConventionId = securitizationNodeEntity.TranchePricingCompoundingConventionId.Value;

            var dayCountConvention = _typesAndConventionsDatabaseRepository.DayCountConventions[dayCountConventionId];
            var compoundingConvention = _typesAndConventionsDatabaseRepository.CompoundingConventions[compoundingConventionId];

            return DeterminePricingStrategy(
                pricingMethodology,
                pricingValue,
                dayCountConvention,
                compoundingConvention);
        }

        private PricingStrategy DeterminePricingStrategy(
            string pricingMethodology,
            double pricingValue,
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingConvention)
        {
            switch (pricingMethodology)
            {
                case Constants.NominalSpreadBasedPricing:
                    return new NominalSpreadBasedPricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        _marketRateEnivironment,
                        MarketDataGrouping.Swaps,
                        pricingValue / Constants.BpsPerOneHundredPercentagePoints);

                case Constants.YieldBasedPricing:
                    return new YieldBasedPricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        pricingValue);

                case Constants.MarketValueBasedPricing:
                    return new SpecificMarketValuePricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        pricingValue);

                case Constants.PercentOfBalanceBasedPricing:
                    return new PercentOfBalancePricingStrategy(
                        dayCountConvention,
                        compoundingConvention,
                        pricingValue / Constants.OneHundredPercentagePoints);

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The pricing type provided named '{0}' is not supported. Please report this error.",
                        pricingMethodology));
            }
        }

        public static string DeterminePricingStrategyDescription(Type pricingStrategyType)
        {
            if (pricingStrategyType == typeof(NominalSpreadBasedPricingStrategy))
                return Constants.NominalSpreadBasedPricing;

            if (pricingStrategyType == typeof(YieldBasedPricingStrategy))
                return Constants.YieldBasedPricing;

            if (pricingStrategyType == typeof(SpecificMarketValuePricingStrategy))
                return Constants.MarketValueBasedPricing;

            if (pricingStrategyType == typeof(PercentOfBalancePricingStrategy))
                return Constants.PercentOfBalanceBasedPricing;

            throw new Exception(string.Format("INTERNAL ERROR: The pricing type provided named '{0}' is not supported. Please report this error.",
                pricingStrategyType));
        }

        // Note that if a spread over a curve is added, then the RateIndexId will need to be populated as well
        public static void PopulateSecuritizationNodeEntity(
            string pricingStrategyDescription, 
            SecuritizationNodeEntity securitizationNodeEntity, 
            PricingStrategy pricingStrategy)
        {
            switch (pricingStrategyDescription)
            {
                case Constants.NominalSpreadBasedPricing:
                    securitizationNodeEntity.TranchePricingValue = pricingStrategy.NominalSpread * pricingStrategy.PricingValueMultiplier;
                    break;

                case Constants.YieldBasedPricing:
                    securitizationNodeEntity.TranchePricingValue = pricingStrategy.InternalRateOfReturn * pricingStrategy.PricingValueMultiplier;
                    break;

                case Constants.MarketValueBasedPricing:
                    securitizationNodeEntity.TranchePricingValue = pricingStrategy.PresentValue * pricingStrategy.PricingValueMultiplier;
                    break;

                case Constants.PercentOfBalanceBasedPricing:
                    securitizationNodeEntity.TranchePricingValue = pricingStrategy.Price * pricingStrategy.PricingValueMultiplier;
                    break;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The pricing type provided named '{0}' is not supported. Please report this error.",
                        pricingStrategyDescription));
            }
        }
    }
}
