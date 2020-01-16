using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.SecuritizationEngine.Triggers;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.Reporting.Results;
using Dream.Common;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches
{
    public abstract class Tranche
    {
        public string TrancheName { get; }       
        public string TrancheDescription { get; set; }
        public string TrancheRating { get; set; }
        public string TranchePricingScenario { get; set; }
        public int? TrancheDetailId { get; set; }
        public int SeniorityRanking { get; private set; }

        public SecuritizationNodeTree SecuritizationNode { get; set; }
        public List<SecuritizationCashFlow> TrancheCashFlows { get; }
        public PricingStrategy PricingStrategy { get; set; }
        public AvailableFundsRetriever AvailableFundsRetriever { get; protected set; }
        public ShortfallReservesAllocator ShortfallReservesAllocator { get; set; }
        public Dictionary<TrancheCashFlowType, TriggerLogic> TriggerLogicDictionary { get; protected set; }
        public List<(string AccountName, TrancheCashFlowType CashFlowType)> ListOfAssociatedReserveAccounts { get; protected set; }

        // Note that balance can be set externally, if solving for a balance
        // Also, some tranches may have a null balance
        public double? CurrentBalance { get; set; }
        public double InitialBalance { get; set; }
        public double? AccruedPayment { get; set; }

        public int MonthsToNextPayment { get; set; }
        public int PaymentFrequencyInMonths { get; set; }
        public bool AbsorbsRemainingAvailableFunds { get; set; }  
        public bool AbsorbsAssociatedReservesReleased { get; set; }     
        public bool IncludePaymentShortfall { get; set; }
        public bool IsShortfallPaidFromReserves { get; set; }

        protected double _PreviouslyAccruedPayments;

        protected bool _IsFinalPeriod;
        protected bool _IsFirstPayment;
        protected bool _IsPaymentMonth;

        public Tranche(
            string trancheName, 
            PricingStrategy pricingStrategy, 
            AvailableFundsRetriever availableFundsRetriever)
        {
            TrancheName = trancheName;
            PricingStrategy = pricingStrategy;
            AvailableFundsRetriever = availableFundsRetriever;

            TrancheCashFlows = new List<SecuritizationCashFlow>();
            TriggerLogicDictionary = new Dictionary<TrancheCashFlowType, TriggerLogic>();
            ListOfAssociatedReserveAccounts = new List<(string AccountName, TrancheCashFlowType CashFlowType)>();
        }

        public abstract Tranche Copy();

        public abstract void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod);

        public virtual void AllocatePaymentShortfall(
            AvailableFunds availableFunds, 
            DistributionRule distributionRule, 
            int monthlyPeriod) { }

        public virtual SecuritizationCashFlowsSummaryResult RunAnalysis(int childToParentOrder)
        {
            var trancheResult = new SecuritizationCashFlowsSummaryResult(TrancheCashFlows);

            trancheResult.ChildToParentOrder = childToParentOrder;
            trancheResult.SecuritizationNodeName = SecuritizationNode.SecuritizationNodeName;
            trancheResult.TrancheName = TrancheName;
            trancheResult.TrancheType = GetType();
            trancheResult.Balance = InitialBalance;

            trancheResult.PresentValue = PricingStrategy.CalculatePresentValue(TrancheCashFlows);
            trancheResult.DollarPrice = PricingStrategy.CalculatePrice(TrancheCashFlows);
            trancheResult.InternalRateOfReturn = PricingStrategy.CalculateInternalRateOfReturn(TrancheCashFlows);
            trancheResult.MacaulayDuration = PricingStrategy.CalculateMacaulayDuration(TrancheCashFlows);
            trancheResult.ModifiedDurationAnalytical = PricingStrategy.CalculateModifiedDuration(TrancheCashFlows);
            trancheResult.ModifiedDurationNumerical = PricingStrategy.CalculateModifiedDuration(TrancheCashFlows, Constants.TwentyFiveBpsShock);
            trancheResult.DollarDuration = PricingStrategy.CalculateDollarDuration(TrancheCashFlows);

            trancheResult.DayCountConvention = PricingStrategy.DayCountConvention;
            trancheResult.CompoundingConvention = PricingStrategy.CompoundingConvention;

            trancheResult.WeightedAverageLife = CashFlowMetrics.CalculatePrincipalWeightedAverageLife(TrancheCashFlows);
            trancheResult.ForwardWeightedAverageCoupon = CashFlowMetrics.CalculateForwardWeightedAverageCoupon(TrancheCashFlows);
            trancheResult.PaymentCorridor = CashFlowMetrics.CalculatePrinicipalPaymentCorridor(TrancheCashFlows);
            trancheResult.InterestShortfallRecord = CashFlowMetrics.CalculateInterestShortfallRecord(TrancheCashFlows);

            trancheResult.TotalCashFlow = TrancheCashFlows.Sum(c => c.Payment);
            trancheResult.TotalPrincipal = TrancheCashFlows.Sum(c => c.Principal);
            trancheResult.TotalInterest = TrancheCashFlows.Sum(c => c.Interest);

            return trancheResult;
        }

        public virtual SecuritizationCashFlowsSummaryResult RunAnalysis(
            MarketRateEnvironment marketRateEnvironment, 
            MarketDataGrouping marketDataGrouping, 
            InterestRateCurveType interestRateCurveType,
            int childToParentOrder)
        {
            var trancheResult = RunAnalysis(childToParentOrder);

            trancheResult.NominalSpread = PricingStrategy.CalculateNominalSpread(TrancheCashFlows, marketRateEnvironment, marketDataGrouping);
            trancheResult.NominalBenchmarkRate = PricingStrategy.InterpolatedRate.GetValueOrDefault(double.NaN);
            trancheResult.MarketDataUseForNominalSpread = PricingStrategy.MarketDataUsedForNominalSpread;

            trancheResult.ZeroVolatilitySpread = PricingStrategy.CalculateSpread(TrancheCashFlows, marketRateEnvironment, interestRateCurveType);
            trancheResult.SpreadDuration = PricingStrategy.CalculateSpreadDuration(TrancheCashFlows, marketRateEnvironment, interestRateCurveType, Constants.TwentyFiveBpsShock);
            trancheResult.CurveTypeUsedForSpreadCalculation = PricingStrategy.CurveTypeUsedForSpreadCalculation;

            return trancheResult;
        }

        public void SetIsFinalPeriod(bool isFinalPeriod)
        {
            _IsFinalPeriod = isFinalPeriod;
        }

        /// <summary>
        /// Sets the seniority ranking of a tranche, which can only be set if it is new.
        /// </summary>
        public void SetSeniorityRanking(int seniorityRanking)
        {
            if (SeniorityRanking == seniorityRanking) return;
            SeniorityRanking = seniorityRanking;
        }

        public void AddTriggerLogic(TrancheCashFlowType trancheCashFlowType, TriggerLogic triggerLogic)
        {
            if (!TriggerLogicDictionary.ContainsKey(trancheCashFlowType))
            {
                TriggerLogicDictionary.Add(trancheCashFlowType, triggerLogic);
            }
            else
            {
                TriggerLogicDictionary[trancheCashFlowType] = triggerLogic;
            }
        }

        public void AddAssociatedReserveAccount(Tranche reserveAccount, TrancheCashFlowType trancheCashFlowType)
        {
            var reserveAccountName = reserveAccount.TrancheName;
            if (!ListOfAssociatedReserveAccounts.Any(a => a.AccountName == reserveAccountName && a.CashFlowType == trancheCashFlowType))
            {
                ListOfAssociatedReserveAccounts.Add((AccountName: reserveAccountName, CashFlowType: trancheCashFlowType));
            }
        }

        public void AccrueRemainingAvailableFunds(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (!AbsorbsRemainingAvailableFunds) return;

            var paymentToAccrue = 0.0;

            // Do not allow accured principal to occur in excess of the balance of the tranche, if any is designated
            if (CurrentBalance.HasValue)
            {
                paymentToAccrue = Math.Min(CurrentBalance.Value, availableFunds[monthlyPeriod].TotalAvailableFunds);
                TrancheCashFlows[monthlyPeriod].AccruedPayment += paymentToAccrue;
                availableFunds[monthlyPeriod].TotalAvailableFunds -= paymentToAccrue;
            }
            else
            {
                paymentToAccrue = availableFunds[monthlyPeriod].TotalAvailableFunds;
                TrancheCashFlows[monthlyPeriod].AccruedPayment += paymentToAccrue;
                availableFunds[monthlyPeriod].TotalAvailableFunds = 0.0;
            }

            var reserveFunds = availableFunds[monthlyPeriod].AvailableReserveFundsDictionary;
            AddReserveAccountForAccruedPayments(reserveFunds, paymentToAccrue);
        }

        public void AccrueAnyAssociatedReservesReleased(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (!AbsorbsAssociatedReservesReleased) return;

            var reserveFunds = availableFunds[monthlyPeriod].AvailableReserveFundsDictionary;
            if (ListOfAssociatedReserveAccounts.Any(r => reserveFunds.ContainsKey(r.AccountName)))
            {
                foreach (var reserveAccount in ListOfAssociatedReserveAccounts)
                {
                    if (reserveFunds.ContainsKey(reserveAccount.AccountName))
                    {
                        var paymentToAccrue = 0.0;

                        // Do not allow accured principal to occur in excess of the balance of the tranche, if any is designated
                        if (CurrentBalance.HasValue)
                        {
                            paymentToAccrue = Math.Min(CurrentBalance.Value, reserveFunds[reserveAccount.AccountName].ReservesReleased);
                            TrancheCashFlows[monthlyPeriod].AccruedPayment += paymentToAccrue;
                            reserveFunds[reserveAccount.AccountName].ReservesReleased -= paymentToAccrue;
                        }
                        else
                        {
                            TrancheCashFlows[monthlyPeriod].AccruedPayment += reserveFunds[reserveAccount.AccountName].ReservesReleased;
                            reserveFunds[reserveAccount.AccountName].ReservesReleased = 0.0;
                        }

                        AddReserveAccountForAccruedPayments(reserveFunds, paymentToAccrue);
                    }
                }
            }
        }

        // TODO: This logic is faulty in certain cases where a fee payment is not intended to be able to
        // collect on its short fall during the "IsFinalPeriod" flag set to true phase of run-time.
        // It's probably looking more into why a change to this logic breaks the 2017-1 Prelim test.
        public virtual double CalculatePreviouslyAccruedPayments(int monthlyPeriod)
        {
            var minimumMonthlyPeriodForLookback = Math.Max(monthlyPeriod - PaymentFrequencyInMonths, 0);

            // Balloon out all remaining accrued funds, even if we happen to land on a scheduled payment
            if (_IsFinalPeriod)
            {
                var lastCashFlowWithAnyPayment = TrancheCashFlows
                    .Where(c => c.Period != monthlyPeriod)
                    .LastOrDefault(c => c.Payment > 0);

                if (lastCashFlowWithAnyPayment != null)
                {
                    minimumMonthlyPeriodForLookback = 
                        Math.Max(lastCashFlowWithAnyPayment.Period, minimumMonthlyPeriodForLookback);
                }
            }

            var previouslyAccruedPayments = TrancheCashFlows
                .Where(c => c.Period > minimumMonthlyPeriodForLookback
                         && c.Period <= monthlyPeriod)
                .Sum(s => s.AccruedPayment);

            return previouslyAccruedPayments;
        }

        public virtual void SwitchToExternalFunds()
        {
            AvailableFundsRetriever = new ExternalPayerAvailableFundsRetriever();
        }

        public void AddNewTrancheCashFlow(int monthlyPeriod, DateTime securitizationStartDate, DateTime securitizationFirstCashFlowDate)
        {
            if (TrancheCashFlows.ElementAtOrDefault(monthlyPeriod) != null)
            {
                return;
            }

            SecuritizationCashFlow newTrancheCashFlow;
            if (monthlyPeriod == 0)
            {
                newTrancheCashFlow = new SecuritizationCashFlow
                {
                    Period = monthlyPeriod,
                    PeriodDate = securitizationStartDate,
                    StartingBalance = CurrentBalance.GetValueOrDefault(),
                    EndingBalance = CurrentBalance.GetValueOrDefault()
                };
            }
            else
            {
                // Note the offset by one since the start date may be a different, ad hoc date
                var periodDate = securitizationFirstCashFlowDate.AddMonths(monthlyPeriod - 1);
                var priorPeriodCashFlow = TrancheCashFlows[monthlyPeriod - 1];

                newTrancheCashFlow = new SecuritizationCashFlow
                {
                    Period = monthlyPeriod,
                    PeriodDate = periodDate,
                    StartingBalance = priorPeriodCashFlow.EndingBalance,
                    EndingBalance = priorPeriodCashFlow.EndingBalance,

                    PaymentShortfall = priorPeriodCashFlow.PaymentShortfall,
                    InterestShortfall = priorPeriodCashFlow.InterestShortfall,

                    InterestAccruedOnInterestShortfall = priorPeriodCashFlow.InterestAccruedOnInterestShortfall
                };
            }

            TrancheCashFlows.Add(newTrancheCashFlow);
        }

        protected double DetermineAmountDue(int monthlyPeriod, double amountToPay, bool includeShortfall, 
            bool returnOnlyShortfall = false)
        {
            if (monthlyPeriod < MonthsToNextPayment)
            {
                return 0.0;
            }

            // Balloon out all remaining principal at the final period
            if (_IsFinalPeriod && CurrentBalance.HasValue && CurrentBalance.Value > 0.0)
            {
                var amountDue = CurrentBalance.Value;
                return amountDue;
            }

            _IsFirstPayment = monthlyPeriod == MonthsToNextPayment;
            _IsPaymentMonth = MathUtility.CheckDivisibilityOfIntegers(
                monthlyPeriod - MonthsToNextPayment,
                PaymentFrequencyInMonths);

            if (_IsFirstPayment || _IsPaymentMonth || _IsFinalPeriod)
            {
                var amountDue = 0.0;
                if (returnOnlyShortfall)
                {
                    amountDue = TrancheCashFlows[monthlyPeriod].PaymentShortfall;
                    TrancheCashFlows[monthlyPeriod].PaymentShortfall = 0.0;
                    return amountDue;
                }

                var previouslyAccruedPayments = CalculatePreviouslyAccruedPayments(monthlyPeriod);
                _PreviouslyAccruedPayments = previouslyAccruedPayments;

                amountDue = amountToPay + previouslyAccruedPayments;
                if (includeShortfall)
                {
                    amountDue += TrancheCashFlows[monthlyPeriod].PaymentShortfall;
                    TrancheCashFlows[monthlyPeriod].PaymentShortfall = 0.0;
                }

                return amountDue;
            }

            return 0.0;
        }

        protected AmountPayable DetermineAmountPayable(
            double amountDue, 
            double fundsAvailable, 
            double appliedProportionToDistribute,
            AvailableFunds availableFunds,
            TrancheCashFlowType cashFlowType,
            int monthlyPeriod)
        {
            fundsAvailable += _PreviouslyAccruedPayments;

            var reserveFunds = availableFunds[monthlyPeriod].AvailableReserveFundsDictionary;
            if (reserveFunds.ContainsKey(TrancheName))
            {
                reserveFunds[TrancheName].FundEndingBalance = 
                    Math.Max(reserveFunds[TrancheName].FundEndingBalance - _PreviouslyAccruedPayments, 0.0);
            }

            var amountPayable = 0.0;
            if (amountDue > 0.0 && fundsAvailable > 0.0)
            {             
                if (!_IsFinalPeriod)
                {
                    amountPayable = Math.Min(amountDue, fundsAvailable);
                }
                // On the final period (such as redemption), the only limit will be on the total funds available
                else
                {
                    var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
                    totalFundsAvailable += _PreviouslyAccruedPayments;
                    amountPayable = Math.Min(amountDue, totalFundsAvailable);
                }
            }

            // If there is no shortfall, there's no need to look at the reserve funds
            // However, some funds will pay out all reserves released, which is controlled for here
            if (amountPayable >= amountDue) return new AmountPayable(amountPayable);

            ShortfallReservesAllocator.SetTrancheSpecificInformation(this);

            return ShortfallReservesAllocator.AllocateReservesToCoverShortfall(
                cashFlowType,
                reserveFunds,
                availableFunds,
                appliedProportionToDistribute,
                amountDue,
                amountPayable);
        }

        protected virtual void RunTriggerLogic(
            int monthlyPeriod, 
            TrancheCashFlowType trancheCashFlowType, 
            AvailableFunds availableFunds, 
            AmountPayable amountPayable)
        {
            if (TriggerLogicDictionary.ContainsKey(trancheCashFlowType))
            {
                var triggerLogic = TriggerLogicDictionary[trancheCashFlowType];
                triggerLogic.ApplyTriggerLogic(monthlyPeriod, availableFunds, amountPayable);
            }
        }

        private void AddReserveAccountForAccruedPayments(Dictionary<string, ReserveFund> reserveFunds, double paymentToAccrue)
        {
            // Any accrued payment is treated like a reserve fund that can be pulled from in the event a more senior
            // tranche or fee runs out of money higher in the priority of payments waterfall
            if (reserveFunds.ContainsKey(TrancheName))
            {
                reserveFunds[TrancheName].FundEndingBalance += paymentToAccrue;
            }
            else
            {
                var isReserveTranche = false;
                var startingBalance = 0.0;
                var reserveFund = new ReserveFund(TrancheName, SeniorityRanking, isReserveTranche, startingBalance, paymentToAccrue);
                reserveFunds.Add(TrancheName, reserveFund);
            }
        }
    }
}
