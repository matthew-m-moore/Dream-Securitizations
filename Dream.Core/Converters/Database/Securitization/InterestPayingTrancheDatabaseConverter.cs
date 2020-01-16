using System;
using System.Linq;
using Dream.IO.Database.Entities.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Repositories.Database;
using Dream.Common.Enums;
using Dream.Common.Curves;

namespace Dream.Core.Converters.Database.Securitization
{
    public class InterestPayingTrancheDatabaseConverter
    {
        public static InterestPayingTranche CreateTranche(
            (Type TrancheType, bool IsResidualTranche) trancheTypeInformation,
             DateTime interestAccrualStartDate,
             PricingStrategy pricingStrategy,       
             TrancheDetailEntity trancheDetailEntity, 
             TrancheCouponEntity trancheCouponEntity,
             AvailableFundsRetriever principalAvailableFundsRetriever,
             AvailableFundsRetriever interestAvailableFundsRetriever,
             MarketRateEnvironment marketRateEnvironment,
             TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository)
        {
            var interestAccrualDayCountConvention = typesAndConventionsDatabaseRepository.DayCountConventions[trancheCouponEntity.InterestAccrualDayCountConventionId];
            var initialInterestAccrualDayCountConvention = default(DayCountConvention);
            var interestRateCurveType = default(InterestRateCurveType);

            if (trancheCouponEntity.InitialPeriodInterestAccrualDayCountConventionId.HasValue)
                initialInterestAccrualDayCountConvention = typesAndConventionsDatabaseRepository.DayCountConventions[trancheCouponEntity.InitialPeriodInterestAccrualDayCountConventionId.Value];

            if (trancheCouponEntity.TrancheCouponRateIndexId.HasValue)
                interestRateCurveType = typesAndConventionsDatabaseRepository.InterestRateCurveTypes[trancheCouponEntity.TrancheCouponRateIndexId.Value];

            InterestPayingTranche interestPayingTranche = null;

            if (trancheTypeInformation.TrancheType == typeof(FixedRateTranche))
                interestPayingTranche = CreateFixedRateTranche(
                    pricingStrategy,
                    trancheDetailEntity,
                    trancheCouponEntity,
                    principalAvailableFundsRetriever,
                    interestAvailableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(FloatingRateTranche))
                interestPayingTranche = CreateFloatingRateTranche(
                    pricingStrategy,
                    trancheDetailEntity,
                    trancheCouponEntity,
                    principalAvailableFundsRetriever,
                    interestAvailableFundsRetriever,
                    marketRateEnvironment,
                    interestRateCurveType);

            else if (trancheTypeInformation.TrancheType == typeof(InverseFloatingRateTranche))
                interestPayingTranche = CreateInverseFloatingRateTranche(
                    pricingStrategy,
                    trancheDetailEntity,
                    trancheCouponEntity,
                    principalAvailableFundsRetriever,
                    interestAvailableFundsRetriever,
                    marketRateEnvironment,
                    interestRateCurveType);

            interestPayingTranche.CurrentBalance = trancheDetailEntity.TrancheBalance.Value;
            interestPayingTranche.InitialBalance = trancheDetailEntity.TrancheBalance.Value;
            interestPayingTranche.AccruedPayment = trancheDetailEntity.TrancheAccruedPayment;
            interestPayingTranche.AccruedInterest = trancheDetailEntity.TrancheAccruedInterest;

            interestPayingTranche.InterestAccrualDayCountConvention = interestAccrualDayCountConvention;
            interestPayingTranche.InterestAccrualStartDate = interestAccrualStartDate;
            interestPayingTranche.InitialPeriodInterestAccrualDayCountConvention = initialInterestAccrualDayCountConvention;
            interestPayingTranche.InitialPeriodInterestAccrualEndDate = trancheCouponEntity.InitialPeriodInterestAccrualEndDate;

            interestPayingTranche.MonthsToNextInterestPayment = trancheDetailEntity.MonthsToNextInterestPayment.Value;
            interestPayingTranche.InterestPaymentFrequencyInMonths = trancheDetailEntity.InterestPaymentFrequencyInMonths.Value;
            interestPayingTranche.MonthsToNextPayment = trancheDetailEntity.MonthsToNextPayment;
            interestPayingTranche.PaymentFrequencyInMonths = trancheDetailEntity.PaymentFrequencyInMonths;

            interestPayingTranche.IncludePaymentShortfall = trancheDetailEntity.IncludePaymentShortfall.GetValueOrDefault();
            interestPayingTranche.IncludeInterestShortfall = trancheDetailEntity.IncludeInterestShortfall.GetValueOrDefault();
            interestPayingTranche.IsShortfallPaidFromReserves = trancheDetailEntity.IsShortfallPaidFromReserves.GetValueOrDefault();

            if (trancheTypeInformation.IsResidualTranche)
            {
                interestPayingTranche.AbsorbsRemainingAvailableFunds = true;
                interestPayingTranche.AbsorbsAssociatedReservesReleased = true;
            }

            return interestPayingTranche;
        }

        private static InterestPayingTranche CreateFixedRateTranche(
            PricingStrategy pricingStrategy,
            TrancheDetailEntity trancheDetailEntity,
            TrancheCouponEntity trancheCouponEntity,
            AvailableFundsRetriever principalAvailableFundsRetriever,
            AvailableFundsRetriever interestAvailableFundsRetriever)
        {
            var fixedRateCoupon = new FixedRateCoupon(trancheCouponEntity.TrancheCouponValue);

            return new FixedRateTranche(
                trancheDetailEntity.TrancheName,
                pricingStrategy,
                principalAvailableFundsRetriever,
                interestAvailableFundsRetriever,
                fixedRateCoupon);
        }

        private static InterestPayingTranche CreateFloatingRateTranche(
            PricingStrategy pricingStrategy,
            TrancheDetailEntity trancheDetailEntity,
            TrancheCouponEntity trancheCouponEntity,
            AvailableFundsRetriever principalAvailableFundsRetriever,
            AvailableFundsRetriever interestAvailableFundsRetriever,
            MarketRateEnvironment marketRateEnvironment,
            InterestRateCurveType interestRateCurveType)
        {
            var interestRateCurve = new InterestRateCurve(
                interestRateCurveType,
                marketRateEnvironment.MarketDate,
                marketRateEnvironment[interestRateCurveType].RateCurve);

            var couponFactor = trancheCouponEntity.TrancheCouponFactor.GetValueOrDefault(1.0);
            var startingCouponRate = (interestRateCurve.RateCurve.First() * couponFactor) + trancheCouponEntity.TrancheCouponMargin.Value;

            var fixedRateCoupon = new FloatingRateCoupon(
               startingCouponRate,
               couponFactor,
               trancheCouponEntity.TrancheCouponCeiling.Value,
               trancheCouponEntity.TrancheCouponFloor.Value,
               trancheCouponEntity.TrancheCouponMargin.Value,
               trancheCouponEntity.TrancheCouponInterimCap.Value,
               trancheCouponEntity.InterestRateResetLookbackMonths.Value,
               trancheCouponEntity.MonthsToNextInterestRateReset.Value,
               trancheCouponEntity.InterestRateResetFrequencyInMonths.Value,
               interestRateCurve);

            return new FloatingRateTranche(
                trancheDetailEntity.TrancheName,
                pricingStrategy,
                principalAvailableFundsRetriever,
                interestAvailableFundsRetriever,
                fixedRateCoupon);
        }

        private static InterestPayingTranche CreateInverseFloatingRateTranche(
            PricingStrategy pricingStrategy,
            TrancheDetailEntity trancheDetailEntity, 
            TrancheCouponEntity trancheCouponEntity, 
            AvailableFundsRetriever principalAvailableFundsRetriever, 
            AvailableFundsRetriever interestAvailableFundsRetriever,
            MarketRateEnvironment marketRateEnvironment,
            InterestRateCurveType interestRateCurveType)
        {
            var interestRateCurve = new InterestRateCurve(
                interestRateCurveType,
                marketRateEnvironment.MarketDate,
                marketRateEnvironment[interestRateCurveType].RateCurve);

            var couponFactor = trancheCouponEntity.TrancheCouponFactor.GetValueOrDefault(1.0);
            var startingCouponRate = (interestRateCurve.RateCurve.First() * couponFactor) + trancheCouponEntity.TrancheCouponMargin.Value;

            var fixedRateCoupon = new InverseFloatingRateCoupon(
               startingCouponRate,
               couponFactor,
               trancheCouponEntity.TrancheCouponCeiling.Value,
               trancheCouponEntity.TrancheCouponFloor.Value,
               trancheCouponEntity.TrancheCouponMargin.Value,
               trancheCouponEntity.TrancheCouponInterimCap.Value,
               trancheCouponEntity.InterestRateResetLookbackMonths.Value,
               trancheCouponEntity.MonthsToNextInterestRateReset.Value,
               trancheCouponEntity.InterestRateResetFrequencyInMonths.Value,
               interestRateCurve);

            return new InverseFloatingRateTranche(
                trancheDetailEntity.TrancheName,
                pricingStrategy,
                principalAvailableFundsRetriever,
                interestAvailableFundsRetriever,
                fixedRateCoupon);
        }
    }
}
