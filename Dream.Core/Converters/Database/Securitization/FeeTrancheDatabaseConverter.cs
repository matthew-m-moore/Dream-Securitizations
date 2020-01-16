using System;
using System.Collections.Generic;
using Dream.IO.Database.Entities.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.Repositories.Database;
using Dream.Common.Enums;

namespace Dream.Core.Converters.Database.Securitization
{
    public class FeeTrancheDatabaseConverter
    {
        public static FeeTranche CreateTranche(
            (Type TrancheType, bool IsResidualTranche) trancheTypeInformation,
            (FeeGroupDetailEntity FeeGroup, List<FeeDetailEntity> FeeDetails) feeGroupInformation,
             TrancheDetailEntity trancheDetailEntity,
             AvailableFundsRetriever availableFundsRetriever,
             Dictionary<int, TrancheDetailEntity> trancheDetailsDictionary,
             TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository)
        {
            var feePaymentConvention = default(PaymentConvention);
            var proRatingDayCountConvention = default(DayCountConvention);
            var initialFeePaymentConvention = default(PaymentConvention);
            var initialProRatingDayCountConvention = default(DayCountConvention);

            if (trancheDetailEntity.PaymentConventionId.HasValue)
                feePaymentConvention = typesAndConventionsDatabaseRepository.PaymentConventions[trancheDetailEntity.PaymentConventionId.Value];

            if (trancheDetailEntity.AccrualDayCountConventionId.HasValue)
                proRatingDayCountConvention = typesAndConventionsDatabaseRepository.DayCountConventions[trancheDetailEntity.AccrualDayCountConventionId.Value];

            if (trancheDetailEntity.InitialPaymentConventionId.HasValue)
                initialFeePaymentConvention = typesAndConventionsDatabaseRepository.PaymentConventions[trancheDetailEntity.InitialPaymentConventionId.Value];

            if (trancheDetailEntity.InitialDayCountConventionId.HasValue)
                initialProRatingDayCountConvention = typesAndConventionsDatabaseRepository.DayCountConventions[trancheDetailEntity.InitialDayCountConventionId.Value];

            FeeTranche feeTranche = null;

            if (trancheTypeInformation.TrancheType == typeof(FlatFeeTranche))
                feeTranche = CreateFlatFeeTranche(
                    feeGroupInformation.FeeGroup,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(PeriodicallyIncreasingFeeTranche))
                feeTranche = CreatePeriodicallyIncreasingFeeTranche(
                    feeGroupInformation.FeeGroup,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(InitialPeriodSpecialAccrualFeeTranche))
                feeTranche = CreateInitialPeriodSpecialAccrualFlatFeeTranche(
                    feeGroupInformation.FeeGroup,
                    trancheDetailEntity,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    initialFeePaymentConvention,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(BondCountBasedFeeTranche))
                feeTranche = CreateBondCountBasedFeeTranche(
                    feeGroupInformation.FeeGroup,
                    trancheDetailEntity,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    initialFeePaymentConvention,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(PercentOfTrancheBalanceFeeTranche))
                feeTranche = CreateBondCountBasedFeeTranche(
                    feeGroupInformation.FeeGroup,
                    trancheDetailEntity,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    initialFeePaymentConvention,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(PercentOfCollateralBalanceFeeTranche))
                feeTranche = CreatePercentOfCollateralBalanceFeeTranche(
                    feeGroupInformation.FeeGroup,
                    trancheDetailEntity,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    initialFeePaymentConvention,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche))
                feeTranche = CreateSpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche(
                    feeGroupInformation.FeeGroup,
                    trancheDetailEntity,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever);

            AddFeeTrancheDetails(feeTranche, feeGroupInformation.FeeDetails, trancheDetailsDictionary);

            feeTranche.AccruedPayment = trancheDetailEntity.TrancheAccruedPayment;
            feeTranche.MonthsToNextPayment = trancheDetailEntity.MonthsToNextPayment;
            feeTranche.PaymentFrequencyInMonths = trancheDetailEntity.PaymentFrequencyInMonths;
            feeTranche.IncludePaymentShortfall = trancheDetailEntity.IncludePaymentShortfall.GetValueOrDefault();
            feeTranche.IsShortfallPaidFromReserves = trancheDetailEntity.IsShortfallPaidFromReserves.GetValueOrDefault();

            return feeTranche;
        }

        private static FeeTranche CreateFlatFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            return new FlatFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                feePaymentConvention,
                proRatingDayCountConvention,
                availableFundsRetriever);
        }

        private static FeeTranche CreatePeriodicallyIncreasingFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            return new PeriodicallyIncreasingFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                feeGroupDetailEntity.FeeIncreaseRate.Value,
                feeGroupDetailEntity.FeeRateUpdateFrequencyInMonths.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                availableFundsRetriever);
        }

        private static FeeTranche CreateInitialPeriodSpecialAccrualFlatFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            TrancheDetailEntity trancheDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            return new InitialPeriodSpecialAccrualFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                trancheDetailEntity.InitialPeriodEnd.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                initialFeePaymentConvention,
                initialProRatingDayCountConvention,
                availableFundsRetriever);
        }

        private static FeeTranche CreateBondCountBasedFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            TrancheDetailEntity trancheDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            return new BondCountBasedFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                feeGroupDetailEntity.FeePerUnit.Value,
                feeGroupDetailEntity.FeeMinimum.Value,
                feeGroupDetailEntity.FeeMaximum.Value,
                trancheDetailEntity.InitialPeriodEnd.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                initialFeePaymentConvention,
                initialProRatingDayCountConvention,
                availableFundsRetriever);
        }

        private static FeeTranche CreatePercentOfTrancheBalanceFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            TrancheDetailEntity trancheDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            return new PercentOfTrancheBalanceFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                feeGroupDetailEntity.FeeMinimum.Value,
                feeGroupDetailEntity.FeeRate.Value,
                feeGroupDetailEntity.FeeRollingAverageInMonths.Value,
                trancheDetailEntity.InitialPeriodEnd.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                initialFeePaymentConvention,
                initialProRatingDayCountConvention,
                availableFundsRetriever);
        }

        private static FeeTranche CreatePercentOfCollateralBalanceFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            TrancheDetailEntity trancheDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            var feeTranche = new PercentOfCollateralBalanceFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                feeGroupDetailEntity.FeeMinimum.Value,
                feeGroupDetailEntity.FeeRate.Value,
                feeGroupDetailEntity.FeeRateUpdateFrequencyInMonths.Value,
                trancheDetailEntity.InitialPeriodEnd.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                initialFeePaymentConvention,
                initialProRatingDayCountConvention,
                availableFundsRetriever);

            feeTranche.UseStartingBalance = feeGroupDetailEntity.UseStartingBalanceToDetermineFee.GetValueOrDefault();
            return feeTranche;
        }

        private static FeeTranche CreateSpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche(
            FeeGroupDetailEntity feeGroupDetailEntity,
            TrancheDetailEntity trancheDetailEntity,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
        {
            var feeTranche = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche(
                feeGroupDetailEntity.FeeGroupName,
                feeGroupDetailEntity.FeeRate.Value,
                feeGroupDetailEntity.FeeRateUpdateFrequencyInMonths.Value,
                trancheDetailEntity.InitialPeriodEnd.Value,
                initialProRatingDayCountConvention,
                proRatingDayCountConvention,
                feePaymentConvention,
                availableFundsRetriever);

            feeTranche.UseStartingBalance = feeGroupDetailEntity.UseStartingBalanceToDetermineFee.GetValueOrDefault();
            return feeTranche;
        }

        private static void AddFeeTrancheDetails(
            FeeTranche feeTranche, 
            List<FeeDetailEntity> feeDetails,
            Dictionary<int, TrancheDetailEntity> trancheDetailsDictionary)
        {
            foreach (var feeDetail in feeDetails)
            {
                // Increasing Fees
                if (feeDetail.IsIncreasingFee.GetValueOrDefault() && feeTranche is PeriodicallyIncreasingFeeTranche)
                {
                    var castedFeeTranche = feeTranche as PeriodicallyIncreasingFeeTranche;

                    castedFeeTranche.AddIncreasingFee(feeDetail.FeeName, feeDetail.FeeAmount);
                    feeTranche = castedFeeTranche;
                }
                // Tranche Associated Fees
                else if (feeDetail.FeeAssociatedTrancheDetailId.HasValue && feeTranche is PercentOfTrancheBalanceFeeTranche)
                {
                    var castedFeeTranche = feeTranche as PercentOfTrancheBalanceFeeTranche;
                    var associatedTrancheName = trancheDetailsDictionary[feeDetail.FeeAssociatedTrancheDetailId.Value].TrancheName;

                    castedFeeTranche.AssociatedTrancheNames.Add(associatedTrancheName);
                    feeTranche = castedFeeTranche;
                }
                // Delayed Fees
                else if (feeDetail.FeeEffectiveDate.HasValue)
                {
                    feeTranche.AddDelayedFee(feeDetail.FeeName, feeDetail.FeeAmount, feeDetail.FeeEffectiveDate.Value);
                }
                // Basic Fees
                else
                {
                    feeTranche.AddBaseFee(feeDetail.FeeName, feeDetail.FeeAmount);
                }
            }
        }
    }
}
