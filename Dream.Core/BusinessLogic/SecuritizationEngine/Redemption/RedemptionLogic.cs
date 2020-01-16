using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Redemption
{
    /// <summary>
    /// A class that encapsulates the behavior or a mandatory or optional redemption. Colloquially, this can be known as a "Clean-Up Call".
    /// When the logic indicates a redemption is triggered, a function will return true, otherwise false.
    /// </summary>
    public abstract class RedemptionLogic
    {
        public AvailableFunds AvailableFunds { get; set; }
        public PriorityOfPayments PriorityOfPayments { get; set; }
        public Dictionary<string, TrancheNodeStruct> TranchesDictionary { get; set; }
        public bool TreatAsCleanUpCall { get; set; }

        // This would be the value that triggers a redemption event, such as less than 10% of intial balance, for example
        public double? RedemptionTriggeredThreshold { get; private set; }
        public List<Month> ListOfAllowedMonthsForRedemption { get; private set; }

        // The normal redemption priority of payments is the default if no
        // specific post-redemption priority of payments is provided.
        protected PriorityOfPayments _PostRedemptionPriorityOfPayments;
        public PriorityOfPayments PostRedemptionPriorityOfPayments
        {
            get
            {
                if (_PostRedemptionPriorityOfPayments == null)
                {
                    return PriorityOfPayments;
                }

                return _PostRedemptionPriorityOfPayments;
            }

            set
            {
                _PostRedemptionPriorityOfPayments = value;
            }
        }       

        public RedemptionLogic()
        {
            ListOfAllowedMonthsForRedemption = new List<Month>();
        }

        public RedemptionLogic(double redemptionTriggeredThresholdValue)
        {
            RedemptionTriggeredThreshold = redemptionTriggeredThresholdValue;
            ListOfAllowedMonthsForRedemption = new List<Month>();
        }

        protected RedemptionLogic(RedemptionLogic redemptionLogic)
        {
            RedemptionTriggeredThreshold = redemptionLogic.RedemptionTriggeredThreshold;
            ListOfAllowedMonthsForRedemption = redemptionLogic.ListOfAllowedMonthsForRedemption.ToList();
        }

        public abstract bool IsRedemptionTriggered(int monthlyPeriod);
        public abstract RedemptionLogic Copy();

        /// <summary>
        /// Add an allowed month for redemption. By default, all months are allowed.
        /// </summary>
        public void AddAllowedMonthForRedemption(Month allowedMonth)
        {
            ListOfAllowedMonthsForRedemption.Add(allowedMonth);
        }

        /// <summary>
        /// Add a list of allowed months for redemption. By default, all months are allowed.
        /// </summary>
        public void AddAllowedMonthsForRedemption(List<Month> allowedMonths)
        {
            ListOfAllowedMonthsForRedemption.AddRange(allowedMonths);
        }

        public virtual void ProcessRedemption(int monthlyPeriod, SecuritizationInput inputs)
        {
            if (TreatAsCleanUpCall)
            {
                PriorityOfPayments = new PriorityOfPayments(
                    PriorityOfPayments.OrderedListOfEntries
                    .Where(e => e.TrancheCashFlowType != TrancheCashFlowType.Reserves).ToList());

                AvailableFunds.PopulateStartingAvailableFundsForCleanUpCall(
                    monthlyPeriod,
                    inputs.SecuritizationFirstCashFlowDate);
            }
            else
            {
                AvailableFunds.PopulateStartingAvailableFunds(
                    monthlyPeriod,
                    inputs.SecuritizationStartDate,
                    inputs.SecuritizationFirstCashFlowDate);
            }

            var shortfallReservesAllocator = new ShortfallReservesAllocator();

            // Note that redemption logic could have it's own, different, priority of payments
            foreach (var priorityOfPaymentEntry in PriorityOfPayments.OrderedListOfEntries)
            {
                var trancheName = priorityOfPaymentEntry.TrancheName;
                if (!TranchesDictionary.ContainsKey(trancheName)) continue;
                var trancheNodeStruct = TranchesDictionary[trancheName];

                var securitizationTranche = trancheNodeStruct.Tranche;
                var securitizationNode = trancheNodeStruct.SecuritizationNode;
                var trancheCashFlowType = priorityOfPaymentEntry.TrancheCashFlowType;
                securitizationTranche.ShortfallReservesAllocator = shortfallReservesAllocator;
                securitizationTranche.ShortfallReservesAllocator.TrancheCashFlowType = trancheCashFlowType;

                if (TreatAsCleanUpCall) securitizationTranche.SwitchToExternalFunds();

                // It is important to specify now that this will be the final period payment for all tranches
                AdjustFinalPeriodStatusOfTranche(securitizationTranche);
                ResetPaymentShortfallRecovery(securitizationTranche, monthlyPeriod, trancheCashFlowType);
                ClearOutReserveFunds(securitizationTranche, monthlyPeriod);

                var availableFundsDistributionRule = securitizationNode.AvailableFundsDistributionRule;               
                availableFundsDistributionRule.CalculateProportionToDistribute(
                    monthlyPeriod,
                    trancheCashFlowType,
                    securitizationTranche,
                    securitizationNode);

                // Note that reserve accounts are going to skipped outright by default here, even if they
                // are included in the redemption priority of payments
                switch (trancheCashFlowType)
                {
                    case TrancheCashFlowType.Interest:
                    case TrancheCashFlowType.AccruedInterest:
                        securitizationTranche.TrancheCashFlows[monthlyPeriod].Interest = 0.0;
                        securitizationTranche.TrancheCashFlows[monthlyPeriod].AccruedInterest = 0.0;
                        var interestPayingTranche = securitizationTranche as InterestPayingTranche;
                        interestPayingTranche.AllocateInterestCashFlows(
                            AvailableFunds,
                            availableFundsDistributionRule,
                            monthlyPeriod);
                        break;

                    case TrancheCashFlowType.InterestShortfall:
                        var interestShortfallTranche = securitizationTranche as InterestPayingTranche;
                        interestShortfallTranche.AllocateInterestShortfall(
                            AvailableFunds, 
                            availableFundsDistributionRule,
                            monthlyPeriod);
                        break;

                    case TrancheCashFlowType.Fees:
                        // Note that fees would generally not be subject to special logic for redemption
                        securitizationTranche.TrancheCashFlows[monthlyPeriod].Principal = 0.0;
                        securitizationTranche.TrancheCashFlows[monthlyPeriod].AccruedPayment = 0.0;
                        securitizationTranche.AllocatePaymentCashFlows(
                            AvailableFunds,
                            availableFundsDistributionRule,
                            monthlyPeriod);
                        break;

                    case TrancheCashFlowType.Reserves:
                        break;

                    case TrancheCashFlowType.PaymentShortfall:
                    case TrancheCashFlowType.FeesShortfall:
                        securitizationTranche.AllocatePaymentShortfall(
                            AvailableFunds,
                            availableFundsDistributionRule, 
                            monthlyPeriod);
                        break;

                    default:
                        // Note, the starting balance of the tranche must be reset to ensure a full pay out
                        securitizationTranche.TrancheCashFlows[monthlyPeriod].Principal = 0.0;
                        securitizationTranche.AccrueAnyAssociatedReservesReleased(monthlyPeriod, AvailableFunds);
                        AdjustTrancheCurrentBalance(securitizationTranche, monthlyPeriod);
                        securitizationTranche.AllocatePaymentCashFlows(
                            AvailableFunds,
                            availableFundsDistributionRule,
                            monthlyPeriod);
                        securitizationTranche.AccrueRemainingAvailableFunds(monthlyPeriod, AvailableFunds);
                        break;
                }

                // Payments will continue afterward for anything left in the redemption priority of payments
                securitizationTranche.SetIsFinalPeriod(false);
            }
        }

        protected virtual void AdjustFinalPeriodStatusOfTranche(Tranche securitizationTranche)
        {
            securitizationTranche.SetIsFinalPeriod(true);
        }

        private void ResetPaymentShortfallRecovery(Tranche securitizationTranche, int monthlyPeriod, TrancheCashFlowType trancheCashFlowType)
        {
            // Need to reset any cleared out payment and interest shortfalls
            var previousMonthlyPeriod = Math.Max(monthlyPeriod - 1, 0);

            if (trancheCashFlowType == TrancheCashFlowType.Interest 
                || trancheCashFlowType == TrancheCashFlowType.InterestShortfall
                || trancheCashFlowType == TrancheCashFlowType.AccruedInterest)
            {
                var previousPeriodInterestShortfall = securitizationTranche.TrancheCashFlows[previousMonthlyPeriod].InterestShortfall;
                securitizationTranche.TrancheCashFlows[monthlyPeriod].InterestShortfall = previousPeriodInterestShortfall;

                var previousPeriodInterestAccruedOnInterestShortfall = securitizationTranche.TrancheCashFlows[previousMonthlyPeriod].InterestAccruedOnInterestShortfall;
                securitizationTranche.TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall = previousPeriodInterestAccruedOnInterestShortfall;
            }
            else
            {
                var previousPeriodPaymentShortfall = securitizationTranche.TrancheCashFlows[previousMonthlyPeriod].PaymentShortfall;
                securitizationTranche.TrancheCashFlows[monthlyPeriod].PaymentShortfall = previousPeriodPaymentShortfall;
            }
        }

        protected bool CheckAllowedIfMonthIsNotAllowed(int monthlyPeriod)
        {
            if (!ListOfAllowedMonthsForRedemption.Any()) return false;

            var periodDate = AvailableFunds[monthlyPeriod].PeriodDate;
            var monthOfPeriodDate = periodDate.Month;

            var isMonthAllowed = ListOfAllowedMonthsForRedemption.Any(m => monthOfPeriodDate == (int) m);
            return !isMonthAllowed;
        }

        private void ClearOutReserveFunds(Tranche securitizationTranche, int monthlyPeriod)
        {
            var reserveFunds = AvailableFunds[monthlyPeriod].AvailableReserveFundsDictionary;

            // This basic logic assumes that all reserve funds are cleared out and available in order
            // of the priority of payments, which could be defined differently than the securitization
            foreach (var reserveFund in securitizationTranche.ListOfAssociatedReserveAccounts)
            {
                if (reserveFunds[reserveFund.AccountName].IsFundBalanceClearedOutForRedemption) continue;

                var startingReserveFundBalance = reserveFunds[reserveFund.AccountName].FundStartingBalance;
                AvailableFunds[monthlyPeriod].TotalAvailableFunds += startingReserveFundBalance;
                reserveFunds[reserveFund.AccountName].FundEndingBalance = 0.0;

                reserveFunds[reserveFund.AccountName].IsFundBalanceClearedOutForRedemption = true;

                if (TranchesDictionary[reserveFund.AccountName].Tranche is ReserveFundTranche)
                {
                    var reserveTranche = TranchesDictionary[reserveFund.AccountName].Tranche;
                    reserveTranche.TrancheCashFlows[monthlyPeriod].Principal = -1 * startingReserveFundBalance;
                    reserveTranche.TrancheCashFlows[monthlyPeriod].Interest = startingReserveFundBalance;
                    reserveTranche.TrancheCashFlows[monthlyPeriod].EndingBalance = 0.0;
                    return;       
                }

                TranchesDictionary[reserveFund.AccountName].Tranche
                    .TrancheCashFlows[monthlyPeriod].AccruedPayment = -1 * startingReserveFundBalance;
            }
        }

        private void AdjustTrancheCurrentBalance(Tranche securitizationTranche, int monthlyPeriod)
        {
            // A null current balance is indicative of certain specialized logic for payment accruals
            if (securitizationTranche.CurrentBalance.HasValue)
            {
                securitizationTranche.CurrentBalance
                    = securitizationTranche.TrancheCashFlows[monthlyPeriod].StartingBalance;
            }
        }
    }
}
