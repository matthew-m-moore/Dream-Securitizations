using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Common.Utilities;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class ReserveFundTrancheExcelConverter
    {
        public static ReserveFundTranche ConvertListOfReserveAccountRecords(DateTime securitizationFirstCashFlowDate, List<ReserveAccountRecord> listOfReserveAccountRecords)
        {
            if (listOfReserveAccountRecords.Count(r => r.ReserveAccountName != null) > 1)
            {
                throw new Exception("ERROR: More than one reserve account name was supplied. Please check the inputs file.");
            }

            if (listOfReserveAccountRecords.Count(r => r.InitialAccountBalance.HasValue) > 1)
            {
                throw new Exception("ERROR: More than one value for the initial balance of the reserve account was supplied. Please check the inputs file.");
            }

            var reserveAccountName = listOfReserveAccountRecords.First(r => r.ReserveAccountName != null).ReserveAccountName;
            var reserveAccountRecord = listOfReserveAccountRecords.FirstOrDefault(r => r.InitialAccountBalance.HasValue);

            // Note, assumes the initial account balance for the reserve account is zero, if none is provided
            var initialAccountBalance = (reserveAccountRecord != null)
                ? reserveAccountRecord.InitialAccountBalance.Value
                : 0.0;

            var hasBalanceCaps = listOfReserveAccountRecords.Any(r => r.BalanceCaps.HasValue);
            var hasBalanceFloors = listOfReserveAccountRecords.Any(r => r.BalanceFloors.HasValue);

            ReserveFundTranche reserveFundTranche;
            if (hasBalanceCaps && hasBalanceFloors)
            {
                var prepayInterestCollectionStartDate = reserveAccountRecord.PrepayIntCollectionStartDate;

                reserveFundTranche = CreatePercentOfCollateralCappedReserveFundTranche(
                    listOfReserveAccountRecords, 
                    reserveAccountName,
                    prepayInterestCollectionStartDate,
                    initialAccountBalance);
            }
            else if (hasBalanceCaps)
            {
                reserveFundTranche = CreateCappedReserveFundTranche(
                    listOfReserveAccountRecords,
                    reserveAccountName,
                    initialAccountBalance);
            }
            else
            {
                throw new Exception("ERROR: The information supplied for the reserve account is not sufficient since no balance caps were supplied.");
            }

            // Payments to the reserve account are assumed to occur monthly
            reserveFundTranche.MonthsToNextPayment = 1;
            reserveFundTranche.PaymentFrequencyInMonths = 1;

            if (reserveAccountRecord.DepositOrReleaseFrequencyInMonths.HasValue && 
                reserveAccountRecord.PrepayIntCollectionStartDate > DateTime.MinValue)
            {
                reserveFundTranche.PaymentFrequencyInMonths = reserveAccountRecord.DepositOrReleaseFrequencyInMonths.Value;
                reserveFundTranche.MonthsToNextPayment = DateUtility.MonthsBetweenTwoDates(securitizationFirstCashFlowDate, reserveAccountRecord.PrepayIntCollectionStartDate) + 1;
            }

            return reserveFundTranche;
        }

        private static ReserveFundTranche CreateCappedReserveFundTranche(
            List<ReserveAccountRecord> listOfReserveAccountRecords, 
            string reserveAccountName, 
            double initialAccountBalance)
        {
            // Note that the reserve fund defaults to using all funds remaining from available funds
            var cappedReserveTranche = new CappedReserveFundTranche(
                reserveAccountName, 
                initialAccountBalance, 
                new AllFundsAvailableFundsRetriever());
            
            foreach(var reserveAccountRecord in listOfReserveAccountRecords)
            {
                // If no data was provided, skip this entry
                if (!reserveAccountRecord.BalanceCapsEffectiveDates.HasValue && !reserveAccountRecord.BalanceCaps.HasValue)
                    continue;

                // If only one piece of data was supplied, throw an exception
                if (!reserveAccountRecord.BalanceCapsEffectiveDates.HasValue || !reserveAccountRecord.BalanceCaps.HasValue)
                {
                    throw new Exception("ERROR: A balance cap and/or effective date for the cap was not supplied");
                }

                cappedReserveTranche.AddReserveFundBalanceCap(
                    reserveAccountRecord.BalanceCapsEffectiveDates.Value,
                    reserveAccountRecord.BalanceCaps.Value);
            }

            return cappedReserveTranche;
        }

        private static ReserveFundTranche CreatePercentOfCollateralCappedReserveFundTranche(
            List<ReserveAccountRecord> listOfReserveAccountRecords, 
            string reserveAccountName, 
            DateTime prepayInterestCollectionStartDate,
            double initialAccountBalance)
        {
            // Note that the reserve fund defaults to using all funds remaining from available funds
            var percentOfCollateralCappedReserveTranche = new PercentOfCollateralBalanceCappedReserveFundTranche(
                reserveAccountName,
                initialAccountBalance,
                new IrregularInterestRemittanceAvailableFundsRetriever(prepayInterestCollectionStartDate));

            foreach (var reserveAccountRecord in listOfReserveAccountRecords)
            {
                if (reserveAccountRecord.BalanceCapsEffectiveDates.HasValue || reserveAccountRecord.BalanceCaps.HasValue)
                {
                    // If only one piece of data was supplied, throw an exception
                    if (!reserveAccountRecord.BalanceCapsEffectiveDates.HasValue || !reserveAccountRecord.BalanceCaps.HasValue)
                    {
                        throw new Exception("ERROR: A balance cap and/or effective date for the cap was not supplied");
                    }

                    percentOfCollateralCappedReserveTranche.AddReserveFundBalanceCap(
                        reserveAccountRecord.BalanceCapsEffectiveDates.Value,
                        reserveAccountRecord.BalanceCaps.Value);
                }

                if (reserveAccountRecord.BalanceFloorsEffectiveDates.HasValue || reserveAccountRecord.BalanceFloors.HasValue)
                {
                    // If only one piece of data was supplied, throw an exception
                    if (!reserveAccountRecord.BalanceFloorsEffectiveDates.HasValue || !reserveAccountRecord.BalanceFloors.HasValue)
                    {
                        throw new Exception("ERROR: A balance floor and/or effective date for the floor was not supplied");
                    }

                    percentOfCollateralCappedReserveTranche.AddReserveFundBalanceFloor(
                        reserveAccountRecord.BalanceFloorsEffectiveDates.Value,
                        reserveAccountRecord.BalanceFloors.Value);
                }
            }

            return percentOfCollateralCappedReserveTranche;
        }
    }
}
