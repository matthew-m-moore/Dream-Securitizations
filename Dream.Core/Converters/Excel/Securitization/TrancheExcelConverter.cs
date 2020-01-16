using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class TrancheExcelConverter
    {
        private const string _fixedRateTranche = "Fixed Rate";
        private const string _fixedRateResidualTranche = "Fixed Rate NIM";
        private const string _residualTranche = "NIM-let";

        private DateTime _interestAccrualStartDate;
        private DateTime _securitizationFirstCashFlowDate;
        private PriorityOfPayments _priorityOfPayments;
        private MarketRateEnvironment _marketRateEnivironment;
        private ReserveFundTranche _reserveAccount;

        public TrancheExcelConverter(
            DateTime interestAccrualStartDate, 
            DateTime securitizationFirstCashFlowDate,
            MarketRateEnvironment marketRateEnvironment, 
            PriorityOfPayments priorityOfPayments,
            ReserveFundTranche reserveAccount)
        {
            _interestAccrualStartDate = interestAccrualStartDate;
            _securitizationFirstCashFlowDate = securitizationFirstCashFlowDate;
            _marketRateEnivironment = marketRateEnvironment;
            _priorityOfPayments = priorityOfPayments;
            _reserveAccount = reserveAccount;
        }

        /// <summary>
        /// Converts a list of Excel tranche structure records into securitization tranche business objects.
        /// </summary>
        public List<Tranche> ConvertListOfTrancheStructureRecords(List<TrancheStructureRecord> listOfTrancheStructureRecords)
        {
            var listOfSecuritiztionTranches = listOfTrancheStructureRecords
                .Select(ConvertTrancheStructureRecord)
                .Where(t => t != null).ToList();

            return listOfSecuritiztionTranches;
        }

        private Tranche ConvertTrancheStructureRecord(TrancheStructureRecord trancheStructureRecord)
        {
            var trancheType = trancheStructureRecord.TrancheType;
            var isResidualTranche = false;

            Tranche tranche;
            switch (trancheType)
            {
                case _fixedRateTranche:
                    if (trancheStructureRecord.Balance.GetValueOrDefault() <= 0.0) return null;
                    tranche = ConvertTrancheStructureRecordToFixedRateTranche(trancheStructureRecord, isResidualTranche);
                    break;

                case _fixedRateResidualTranche:
                    isResidualTranche = true;
                    if (trancheStructureRecord.Balance.GetValueOrDefault() <= 0.0) return null;
                    tranche = ConvertTrancheStructureRecordToFixedRateTranche(trancheStructureRecord, isResidualTranche);
                    break;

                case _residualTranche:
                    tranche = ConvertTrancheStructureRecordToResidualTranche(trancheStructureRecord);
                    break;

                default:
                    throw new Exception((trancheType == null)
                        ? string.Format("ERROR: Tranche type was not provided for tranche named '{0}'.", trancheStructureRecord.TrancheName)
                        : string.Format("ERROR: Tranche type of '{0}' is not yet supported.", trancheStructureRecord.TrancheType));
            }

            // The shortfall may be handled in the priority of payments directly, instead of implicitely
            var isPaymentShortfallListedInPriorityOfPayments = _priorityOfPayments.OrderedListOfEntries
                .Any(e => e.TrancheName == trancheStructureRecord.TrancheName && e.TrancheCashFlowType == TrancheCashFlowType.PaymentShortfall);
            var isInterestShortfallListedInPriorityOfPayments = _priorityOfPayments.OrderedListOfEntries
                .Any(e => e.TrancheName == trancheStructureRecord.TrancheName && e.TrancheCashFlowType == TrancheCashFlowType.InterestShortfall);

            if (!trancheStructureRecord.IsShortfallRecoverable && (isPaymentShortfallListedInPriorityOfPayments || isInterestShortfallListedInPriorityOfPayments))
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' has short-fall that is set to be not recoverable," +
                    " but the priority of payments waterfall includes an entry for the interest and/or principal shortfall of this tranche.",
                    trancheStructureRecord.TrancheName));
            }

            if (!trancheStructureRecord.IsShortfallRecoverable && trancheStructureRecord.IsShortfallPaidFromReserves)
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' has short-fall that is set to be not recoverable in the tranche structure," +
                    " but on the same tab it is indicated that short-fall for that tranche can be paid from reserves.",
                    trancheStructureRecord.TrancheName));
            }

            if (trancheStructureRecord.IsShortfallRecoverable && !isPaymentShortfallListedInPriorityOfPayments && !isInterestShortfallListedInPriorityOfPayments)
            {
                tranche.IncludePaymentShortfall = true;

                if (tranche is InterestPayingTranche interestPayingTranche)
                {
                    interestPayingTranche.IncludeInterestShortfall = true;
                    tranche = interestPayingTranche;
                }
            }

            if (trancheStructureRecord.IsPrincipalPaidFromLiquidityReserve.HasValue)
            {
                tranche.IncludePaymentShortfall = trancheStructureRecord.IsPrincipalPaidFromLiquidityReserve.Value;
            }

            tranche.IsShortfallPaidFromReserves = trancheStructureRecord.IsShortfallPaidFromReserves;
            tranche.TrancheDescription = trancheStructureRecord.TrancheDescription;
            tranche.TrancheRating = trancheStructureRecord.TrancheRating;
            tranche.TranchePricingScenario = trancheStructureRecord.TranchePricingScenario;

            if (_reserveAccount != null) AddReserveAccountToTranche(tranche, trancheStructureRecord.IsPrincipalPaidFromLiquidityReserve);

            return tranche;
        }

        private InterestPayingTranche ConvertTrancheStructureRecordToFixedRateTranche(TrancheStructureRecord trancheStructureRecord, bool isResidualTranche)
        {
            if (!_priorityOfPayments.OrderedListOfEntries.Any(e => e.TrancheName == trancheStructureRecord.TrancheName))
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' was not listed in the standard priority of payments.",
                    trancheStructureRecord.TrancheName));
            }

            if (!trancheStructureRecord.Balance.HasValue || !trancheStructureRecord.Coupon.HasValue)
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' is designated as an interest-paying tranche, but no principal balance and/or no coupon amount was provided.", 
                    trancheStructureRecord.TrancheName));
            }

            if (!trancheStructureRecord.FirstIntPaymentDate.HasValue || !trancheStructureRecord.IntPaymentFreq.HasValue)
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' is designated as an interest-paying tranche, but no first interest payment date and/or no interest payment frequency was provided.",
                    trancheStructureRecord.TrancheName));
            }

            var pricingStrategyExcelConverter = new PricingStrategyExcelConverter(_marketRateEnivironment);
            var pricingStrategy = pricingStrategyExcelConverter.ExtractPricingStrategyFromTrancheStructureRecord(trancheStructureRecord);

            var principalAvailableFundsRetriever = 
                AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromTrancheStructureRecord(trancheStructureRecord, false);
            var interestAvailableFundsRetriever =
                AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromTrancheStructureRecord(trancheStructureRecord, true);

            var fixedRateCoupon = new FixedRateCoupon(trancheStructureRecord.Coupon.Value);

            var interestAccrualDayCountConvention = DayCountConventionExcelConverter.ConvertString(trancheStructureRecord.InterestAccrual);
            var initialPeriodInterestAccrualDayCountConvention = DayCountConventionExcelConverter.ConvertString(trancheStructureRecord.InitialInterestAccrual);

            // Both of these fields must be populated or neither can be populated
            if (!(initialPeriodInterestAccrualDayCountConvention != default(DayCountConvention) && trancheStructureRecord.InitialAccrualEndDate.HasValue))
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' has only part of its initial interest accrual period set up. Please double-check the inputs file.",
                    trancheStructureRecord.TrancheName));
            }

            var fixedRateTranche = new FixedRateTranche(
                trancheStructureRecord.TrancheName,
                pricingStrategy,
                principalAvailableFundsRetriever,
                interestAvailableFundsRetriever,
                fixedRateCoupon)
            {
                CurrentBalance = trancheStructureRecord.Balance.Value,
                InitialBalance = trancheStructureRecord.Balance.Value,
                AccruedInterest = trancheStructureRecord.AccruedInterest,

                InterestAccrualDayCountConvention = interestAccrualDayCountConvention,
                InterestAccrualStartDate = _interestAccrualStartDate,
                InitialPeriodInterestAccrualDayCountConvention = initialPeriodInterestAccrualDayCountConvention,
                InitialPeriodInterestAccrualEndDate = trancheStructureRecord.InitialAccrualEndDate.GetValueOrDefault(),

                MonthsToNextInterestPayment = DateUtility.MonthsBetweenTwoDates(_securitizationFirstCashFlowDate, trancheStructureRecord.FirstIntPaymentDate.Value) + 1,
                InterestPaymentFrequencyInMonths = trancheStructureRecord.IntPaymentFreq.Value,

                MonthsToNextPayment = DateUtility.MonthsBetweenTwoDates(_securitizationFirstCashFlowDate, trancheStructureRecord.FirstPrinPaymentDate) + 1,
                PaymentFrequencyInMonths = trancheStructureRecord.PrinPaymentFreq,
            };

            // This is for Class B tranches that are based on the excess spread of a structure
            if (isResidualTranche)
            {
                fixedRateTranche.AbsorbsRemainingAvailableFunds = true;
                fixedRateTranche.AbsorbsAssociatedReservesReleased = true;
            }

            return fixedRateTranche;
        }

        private ResidualTranche ConvertTrancheStructureRecordToResidualTranche(TrancheStructureRecord trancheStructureRecord)
        {
            if (!_priorityOfPayments.OrderedListOfEntries.Any(e => e.TrancheName == trancheStructureRecord.TrancheName))
            {
                throw new Exception(string.Format("ERROR: The tranche named '{0}' was not listed in the standard priority of payments.",
                    trancheStructureRecord.TrancheName));
            }

            var pricingStrategyExcelConverter = new PricingStrategyExcelConverter(_marketRateEnivironment);
            var pricingStrategy = pricingStrategyExcelConverter.ExtractPricingStrategyFromTrancheStructureRecord(trancheStructureRecord);

            var availableFundsRetriever =
                AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromTrancheStructureRecord(trancheStructureRecord, false);

            var residualTranche = new ResidualTranche(
                trancheStructureRecord.TrancheName,
                pricingStrategy,
                availableFundsRetriever)
            {
                MonthsToNextPayment = DateUtility.MonthsBetweenTwoDates(_securitizationFirstCashFlowDate, trancheStructureRecord.FirstPrinPaymentDate) + 1,
                PaymentFrequencyInMonths = trancheStructureRecord.PrinPaymentFreq,
            };

            return residualTranche;
        }

        private void AddReserveAccountToTranche(Tranche securitizationTranche, bool? isPrincipalPaidFromLiquidityReserve)
        {
            if (!securitizationTranche.IsShortfallPaidFromReserves &&
                !securitizationTranche.AbsorbsAssociatedReservesReleased) return;

            if (securitizationTranche is InterestPayingTranche)
            {
                securitizationTranche.AddAssociatedReserveAccount(_reserveAccount, TrancheCashFlowType.Interest);
                securitizationTranche.AddAssociatedReserveAccount(_reserveAccount, TrancheCashFlowType.InterestShortfall);

                if (!isPrincipalPaidFromLiquidityReserve.HasValue || isPrincipalPaidFromLiquidityReserve.Value)
                {
                    securitizationTranche.AddAssociatedReserveAccount(_reserveAccount, TrancheCashFlowType.Principal);
                    securitizationTranche.AddAssociatedReserveAccount(_reserveAccount, TrancheCashFlowType.PrincipalShortfall);
                }
            }

            if (securitizationTranche is ResidualTranche)
            {
                securitizationTranche.AddAssociatedReserveAccount(_reserveAccount, TrancheCashFlowType.Payment);
                securitizationTranche.AddAssociatedReserveAccount(_reserveAccount, TrancheCashFlowType.PaymentShortfall);
            }
        }
    }
}
