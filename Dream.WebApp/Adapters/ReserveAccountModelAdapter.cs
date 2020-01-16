using System.Collections.Generic;
using Dream.WebApp.Models;
using Dream.WebApp.ModelEntries;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using System;

namespace Dream.WebApp.Adapters
{
    public class ReserveAccountModelAdapter
    {
        public static void AddInformationToReserveAccountModel(
            Tranche securitizationTranche, 
            SecuritizationNodeTree securitizationNode,
            ReserveAccountModel reserveAccountModel,
            DateTime securitizationFirstCashFlowDate,
            double totalCollateralBalance)
        {
            var reserveFundTranche = securitizationTranche as ReserveFundTranche;
            var reserveAccountModelEntry = new ReserveAccountModelEntry
            {
                ReserveAccountId = reserveFundTranche.TrancheDetailId,
                ReserveAccountName = reserveFundTranche.TrancheName,
                SecuritizationNodeId = securitizationNode.SecuritizationNodeId,
                SecuritizationNodeName = securitizationNode.SecuritizationNodeName,
                InitialAccountBalanceInDollars = reserveFundTranche.InitialBalance,
                InitialAccountBalanceAsPercentage = reserveFundTranche.InitialBalance / totalCollateralBalance,
                FirstReserveAccountDrawOrDepositDate = securitizationFirstCashFlowDate.AddMonths(reserveFundTranche.MonthsToNextPayment - 1),
                ReserveAccountDrawOrDepositFrequencyInMonths = reserveFundTranche.PaymentFrequencyInMonths,
                AvailableFundsRetrieverDescription = AvailableFundsRetrieverDatabaseConverter.ConvertToDescription(reserveFundTranche.AvailableFundsRetriever.GetType()),
                BalanceCapOrFloorEntries = new List<BalanceCapOrFloorEntry>(),
            };

            if (reserveFundTranche is CappedReserveFundTranche cappedReserveFundTranche)
            {
                foreach (var balanceCapEntry in cappedReserveFundTranche.ReserveFundBalanceCapDictionary)
                {
                    var balanceCapOrFloorEntry = new BalanceCapOrFloorEntry
                    {
                        BalanceCapOrFloor = ReserveFundTrancheDatabaseConverter.BalanceCap,
                        CapOrFloorValue = balanceCapEntry.Value,
                        EffectiveDate = balanceCapEntry.Key,
                        PercentageOrDollarAmount = ReserveFundTrancheDatabaseConverter.DollarAmount,
                    };

                    reserveAccountModelEntry.BalanceCapOrFloorEntries.Add(balanceCapOrFloorEntry);
                }
            }

            else if (reserveFundTranche is PercentOfCollateralBalanceCappedReserveFundTranche percentOfCollateralBalanceCappedReserveFundTranche)
            {
                foreach (var balanceCapEntry in percentOfCollateralBalanceCappedReserveFundTranche.ReserveFundBalanceCapDictionary)
                {
                    var balanceCapOrFloorEntry = new BalanceCapOrFloorEntry
                    {
                        BalanceCapOrFloor = ReserveFundTrancheDatabaseConverter.BalanceCap,
                        CapOrFloorValue = balanceCapEntry.Value,
                        EffectiveDate = balanceCapEntry.Key,
                        PercentageOrDollarAmount = ReserveFundTrancheDatabaseConverter.PercentageAmount,
                    };

                    reserveAccountModelEntry.BalanceCapOrFloorEntries.Add(balanceCapOrFloorEntry);
                }

                foreach (var balanceFloorEntry in percentOfCollateralBalanceCappedReserveFundTranche.ReserveFundBalanceFloorDictionary)
                {
                    var balanceCapOrFloorEntry = new BalanceCapOrFloorEntry
                    {
                        BalanceCapOrFloor = ReserveFundTrancheDatabaseConverter.BalanceFloor,
                        CapOrFloorValue = balanceFloorEntry.Value,
                        EffectiveDate = balanceFloorEntry.Key,
                        PercentageOrDollarAmount = ReserveFundTrancheDatabaseConverter.DollarAmount,
                    };

                    reserveAccountModelEntry.BalanceCapOrFloorEntries.Add(balanceCapOrFloorEntry);
                }
            }

            reserveAccountModel.ReserveAccountModelEntries.Add(reserveAccountModelEntry);
        }
    }
}