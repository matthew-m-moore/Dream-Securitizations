using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.WebApp.Models;
using Dream.WebApp.ModelEntries;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.Converters.Database;

namespace Dream.WebApp.Adapters
{
    public class FeeTrancheModelAdapter
    {
        public static void AddInformationToFeeTrancheModel(
            Tranche securitizationTranche, 
            SecuritizationNodeTree securitizationNode,
            FeeTrancheModel feeTrancheModel,
            DateTime securitizationFirstCashFlowDate,
            bool isFeeShortFallInPriorityOfPayments,
            bool paysOutAtRedemption)
        {
            var feeTranche = securitizationTranche as FeeTranche;
            var feeTrancheModelEntry = new FeeTrancheModelEntry
            {
                FeeTrancheId = feeTranche.TrancheDetailId,
                FeeTrancheName = feeTranche.TrancheName,
                FeeTrancheTypeDescription = feeTranche.TrancheDescription,
                FeePaymentConvention = PaymentConventionDatabaseConverter.ConvertToDescription(feeTranche.FeePaymentConvention),
                FeeAccrualDayCountConvention = DayCountConventionDatabaseConverter.ConvertToDescription(feeTranche.ProRatingDayCountConvention),
                AccruedFeePayment = feeTranche.AccruedPayment,
                FirstFeePaymentDate = securitizationFirstCashFlowDate.AddMonths(feeTranche.MonthsToNextPayment - 1),
                AvailableFundsRetrieverDescription = AvailableFundsRetrieverDatabaseConverter.ConvertToDescription(feeTranche.AvailableFundsRetriever.GetType()),
                SecuritizationNodeId = securitizationNode.SecuritizationNodeId,
                SecuritizationNodeName = securitizationNode.SecuritizationNodeName,
                // AssociatedReserveAccountNames = feeTranche.ListOfAssociatedReserveAccounts,
                FeeDetailEntries = new List<FeeDetailEntry>(),
                PaysOutAtRedemption = paysOutAtRedemption,
                IsShortfallRecoverable = isFeeShortFallInPriorityOfPayments || feeTranche.IncludePaymentShortfall,
                IsShortfallPaidFromReserves = feeTranche.IsShortfallPaidFromReserves,
            };

            foreach (var baseFeeDetailEntry in feeTranche.BaseAnnualFees)
            {
                var feeDetailEntry = new FeeDetailEntry
                {
                    FeeTrancheName = feeTranche.TrancheName,
                    FeeDetailName = baseFeeDetailEntry.Key,
                    AnnualFeeAmount = baseFeeDetailEntry.Value,
                };

                feeTrancheModelEntry.FeeDetailEntries.Add(feeDetailEntry);
            }

            foreach (var delayedFeeDetailEntry in feeTranche.DelayedAnnualFees)
            {
                var feeDetailEntry = new FeeDetailEntry
                {
                    FeeTrancheName = feeTranche.TrancheName,
                    FeeDetailName = delayedFeeDetailEntry.Key,
                    AnnualFeeAmount = delayedFeeDetailEntry.Value.DelayedFeeValue,
                    FeeEffectiveDate = delayedFeeDetailEntry.Value.DelayedUntilDate,
                };

                feeTrancheModelEntry.FeeDetailEntries.Add(feeDetailEntry);
            }

            // if () -- Now, need to add fee detial information for each different kind of fee type

            feeTrancheModel.FeeTrancheModelEntries.Add(feeTrancheModelEntry);
        }
    }
}