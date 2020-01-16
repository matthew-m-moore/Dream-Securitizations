using System;
using System.Collections.Generic;
using System.Linq;
using Dream.IO.Database.Entities.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;

namespace Dream.Core.Converters.Database.Securitization
{
    public class ReserveFundTrancheDatabaseConverter
    {
        public const string BalanceCap = "Cap";
        public const string BalanceFloor = "Floor";
        public const string PercentageAmount = "Percentage";
        public const string DollarAmount = "Dollars";

        public static ReserveFundTranche CreateTranche(
            (Type TrancheType, bool IsResidualTranche) trancheTypeInformation, 
             TrancheDetailEntity trancheDetailEntity, 
             List<BalanceCapAndFloorDetailEntity> balanceCapAndFloorDetailEntities,
             AvailableFundsRetriever availableFundsRetriever)
        {
            ReserveFundTranche reserveFundTranche = null;

            if (trancheTypeInformation.TrancheType == typeof(CappedReserveFundTranche))
                reserveFundTranche = CreateCappedReserveFundTranche(
                    trancheDetailEntity,
                    balanceCapAndFloorDetailEntities,
                    availableFundsRetriever);

            else if (trancheTypeInformation.TrancheType == typeof(PercentOfCollateralBalanceCappedReserveFundTranche))
                reserveFundTranche = CreatePercentOfCollateralCappedReserveFundTranche(
                    trancheDetailEntity,
                    balanceCapAndFloorDetailEntities,
                    availableFundsRetriever);

            reserveFundTranche.MonthsToNextPayment = trancheDetailEntity.MonthsToNextPayment;
            reserveFundTranche.PaymentFrequencyInMonths = trancheDetailEntity.PaymentFrequencyInMonths;

            return reserveFundTranche;
        }

        private static ReserveFundTranche CreateCappedReserveFundTranche(
             TrancheDetailEntity trancheDetailEntity, 
             List<BalanceCapAndFloorDetailEntity> balanceCapAndFloorDetailEntities,
             AvailableFundsRetriever availableFundsRetriever)
        {
            var cappedReserveTranche = new CappedReserveFundTranche(
                trancheDetailEntity.TrancheName,
                trancheDetailEntity.TrancheBalance.GetValueOrDefault(0.0),
                availableFundsRetriever);

            foreach (var balanceCapAndFloorDetailEntity in balanceCapAndFloorDetailEntities.Where(e => e.BalanceCapOrFloor == BalanceCap))
            {
                if (balanceCapAndFloorDetailEntity.PercentageOrDollarAmount == PercentageAmount)
                {
                    throw new Exception("INTERNAL ERROR: Percentage amounts are not supported for a simple capped reserve fund tranche. Please report this error.");
                }

                cappedReserveTranche.AddReserveFundBalanceCap(
                    balanceCapAndFloorDetailEntity.EffectiveDate,
                    balanceCapAndFloorDetailEntity.CapOrFloorValue);
            }

            return cappedReserveTranche;
        }

        private static ReserveFundTranche CreatePercentOfCollateralCappedReserveFundTranche(
             TrancheDetailEntity trancheDetailEntity,
             List<BalanceCapAndFloorDetailEntity> balanceCapAndFloorDetailEntities,
             AvailableFundsRetriever availableFundsRetriever)
        {
            var percentOfCollateralCappedReserveTranche = new PercentOfCollateralBalanceCappedReserveFundTranche(
                trancheDetailEntity.TrancheName,
                trancheDetailEntity.TrancheBalance.GetValueOrDefault(0.0),
                availableFundsRetriever);

            foreach (var balanceCapAndFloorDetailEntity in balanceCapAndFloorDetailEntities)
            {
                if (balanceCapAndFloorDetailEntity.BalanceCapOrFloor == BalanceCap)
                {
                    if (balanceCapAndFloorDetailEntity.PercentageOrDollarAmount == DollarAmount)
                    {
                        throw new Exception("INTERNAL ERROR: Dollars amounts are not supported as the balance cap on a percentage of collateral capped reserve fund. Please report this error.");
                    }

                    percentOfCollateralCappedReserveTranche.AddReserveFundBalanceCap(
                        balanceCapAndFloorDetailEntity.EffectiveDate,
                        balanceCapAndFloorDetailEntity.CapOrFloorValue);
                }

                if (balanceCapAndFloorDetailEntity.BalanceCapOrFloor == BalanceFloor)
                {
                    if (balanceCapAndFloorDetailEntity.PercentageOrDollarAmount == PercentageAmount)
                    {
                        throw new Exception("INTERNAL ERROR: Percentage amounts are not supported as the balance floor on a percentage of collateral capped reserve fund. Please report this error.");
                    }

                    percentOfCollateralCappedReserveTranche.AddReserveFundBalanceFloor(
                        balanceCapAndFloorDetailEntity.EffectiveDate,
                        balanceCapAndFloorDetailEntity.CapOrFloorValue);
                }
            }

            return percentOfCollateralCappedReserveTranche;
        }
    }
}
