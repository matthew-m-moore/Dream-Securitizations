using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using System.Collections.Generic;
using System.Linq;
using System;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class PercentOfTrancheBalanceFeeTranche : InitialPeriodSpecialAccrualFeeTranche
    {
        public Securitization Securitization { get; set; }
        public List<string> AssociatedTrancheNames { get; set; }

        public double AnnualMinimumFee { get; }
        public double AnnualPercentageOfTrancheBalance { get; }
        public int RollingAverageMonthsForBalanceCalculation { get; }
        public DateTime DateOfFirstBalanceUpdate { get; }

        protected double _CurrentTotalTrancheBalanceForFeeCalculation;

        public PercentOfTrancheBalanceFeeTranche(
            string trancheName,
            double annualMinimumFee,
            double percentageOfTrancheBalance,
            int rollingAverageMonthsForBalanceCalculation,
            DateTime dateOfFirstBalanceUpdate,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
            : base(trancheName, dateOfFirstBalanceUpdate, feePaymentConvention, proRatingDayCountConvention, 
                   initialFeePaymentConvention, initialProRatingDayCountConvention, availableFundsRetriever)
        {
            AnnualMinimumFee = annualMinimumFee;
            AnnualPercentageOfTrancheBalance = percentageOfTrancheBalance;
            RollingAverageMonthsForBalanceCalculation = rollingAverageMonthsForBalanceCalculation;
            DateOfFirstBalanceUpdate = dateOfFirstBalanceUpdate;

            AssociatedTrancheNames = new List<string>();
        }

        public override Tranche Copy()
        {
            var percentOfBalanceFee =  new PercentOfTrancheBalanceFeeTranche(
                new string(TrancheName.ToCharArray()),
                AnnualMinimumFee,
                AnnualPercentageOfTrancheBalance,
                RollingAverageMonthsForBalanceCalculation,
                new DateTime(DateOfFirstBalanceUpdate.Ticks),
                FeePaymentConvention,
                ProRatingDayCountConvention,
                InitialFeePaymentConvention,
                InitialProRatingDayCountConvention,
                AvailableFundsRetriever.Copy())
            {
                TrancheDescription = (TrancheDescription == null) ? null : new string(TrancheDescription.ToCharArray()),
                TrancheRating = (TrancheRating == null) ? null : new string(TrancheRating.ToCharArray()),
                TranchePricingScenario = (TranchePricingScenario == null) ? null : new string(TranchePricingScenario.ToCharArray()),

                TrancheDetailId = TrancheDetailId,
                AccruedPayment = AccruedPayment,

                MonthsToNextPayment = MonthsToNextPayment,
                PaymentFrequencyInMonths = PaymentFrequencyInMonths,
                IncludePaymentShortfall = IncludePaymentShortfall,

                IsShortfallPaidFromReserves = IsShortfallPaidFromReserves,

                BaseAnnualFees = BaseAnnualFees.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                DelayedAnnualFees = DelayedAnnualFees.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),

                ListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts.ToList(),
                TriggerLogicDictionary = TriggerLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };

            percentOfBalanceFee.AssociatedTrancheNames = AssociatedTrancheNames.Select(t => new string(t.ToCharArray())).ToList();

            return percentOfBalanceFee;
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            var lastFeePaymentPeriod = GetLastScheduledFeePaymentPeriod(monthlyPeriod);

            // Since the tranche cash flows are not known in advance, this logic must work by back-populating the accrued fees
            var isFeePaymentPeriod = MathUtility.CheckDivisibilityOfIntegers(monthlyPeriod - lastFeePaymentPeriod, PaymentFrequencyInMonths);
            var isFirstFeePayment = (monthlyPeriod == MonthsToNextPayment);
            if (isFeePaymentPeriod || isFirstFeePayment)
            {
                GetTrancheBalanceForFeeCalculation(monthlyPeriod);
                GetTimeFactorInYearsForProRating(monthlyPeriod, availableFunds);
                GetMonthlyPeriodsForDelayedFees(availableFunds);

                var trancheCashFlowType = TrancheCashFlowType.Fees;
                var fundsAvailableToPayFees = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

                var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
                var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                   SecuritizationNode,
                   trancheCashFlowType,
                   monthlyPeriod,
                   fundsAvailableToPayFees,
                   totalFundsAvailable);

                var feeAmount = DetermineFee(monthlyPeriod);
                var accruedFeeAmount = CalculateAccruedFeeAmount(monthlyPeriod, monthlyPeriod, feeAmount, availableFunds);

                // Back-populate all the accrued fees, assuming each month is of equal length
                for (var period = lastFeePaymentPeriod + 1; period <= monthlyPeriod; period++)
                {
                    TrancheCashFlows[period].AccruedPayment = accruedFeeAmount;
                }

                var feeAmountDue = DetermineFeeAmountDue(monthlyPeriod, feeAmount, IncludePaymentShortfall);
                var feeAmountPayable = DetermineAmountPayable(
                    feeAmountDue, 
                    fundsAvailable.SpecificFundsAvailable, 
                    distributionRule.AppliedProportionToDistribute, 
                    availableFunds,
                    trancheCashFlowType,
                    monthlyPeriod);

                RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, feeAmountPayable);
                availableFunds[monthlyPeriod].TotalAvailableFunds
                    = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - feeAmountPayable.Amount, 0.0);

                // It's not really principal, just a placeholder until we get to reporting
                TrancheCashFlows[monthlyPeriod].Principal += feeAmountPayable.Amount;
                TrancheCashFlows[monthlyPeriod].PaymentShortfall += feeAmountPayable.Shortfall;

                _PreviouslyAccruedPayments = 0.0;
            }
        }

        public override double DetermineFee(int monthlyPeriod)
        {
            var totalAnnualPercentOfTrancheFee = AnnualPercentageOfTrancheBalance * _CurrentTotalTrancheBalanceForFeeCalculation;
            totalAnnualPercentOfTrancheFee = Math.Max(AnnualMinimumFee, totalAnnualPercentOfTrancheFee);

            var totalPercentOfCollateralFee = totalAnnualPercentOfTrancheFee * _TimeFactorInYearsForProRating;

            var baseFee = base.DetermineFee(monthlyPeriod);
            var totalFee = totalPercentOfCollateralFee + baseFee;

            return totalFee;
        }

        protected int GetLastScheduledFeePaymentPeriod(int monthlyPeriod)
        {
            var lookbackPeriod = monthlyPeriod - PaymentFrequencyInMonths;
            if (lookbackPeriod <= MonthsToNextPayment)
            {
                return MonthsToNextPayment;
            }

            var adjustedMonthlyPeriod = lookbackPeriod - MonthsToNextPayment;
            var numberOfPaymentsElapsed = (int) Math.Ceiling((double) adjustedMonthlyPeriod / PaymentFrequencyInMonths);
            var lastPaymentPeriod = (numberOfPaymentsElapsed * PaymentFrequencyInMonths) + MonthsToNextPayment;

            return lastPaymentPeriod;
        }

        protected void GetTrancheBalanceForFeeCalculation(int monthlyPeriod)
        {
            _CurrentTotalTrancheBalanceForFeeCalculation = 0.0;

            foreach (var associatedTrancheName in AssociatedTrancheNames)
            {
                if (!Securitization.TranchesDictionary.ContainsKey(associatedTrancheName))
                {
                    throw new Exception(string.Format("ERROR: The securitization does not contain a tranche named ''. A fee based on tranche balance cannot be calculated.",
                        associatedTrancheName));
                }

                var associatedTranche = Securitization.TranchesDictionary[associatedTrancheName].Tranche;
                if (monthlyPeriod >= associatedTranche.TrancheCashFlows.Count)
                {
                    continue;
                }

                var currentPeriodDate = associatedTranche.TrancheCashFlows[monthlyPeriod].PeriodDate;
                if (currentPeriodDate.Ticks <= DateOfFirstBalanceUpdate.Ticks)
                {
                    _CurrentTotalTrancheBalanceForFeeCalculation += associatedTranche.TrancheCashFlows.First().StartingBalance;
                }
                else
                {
                    if (monthlyPeriod >= RollingAverageMonthsForBalanceCalculation)
                    {
                        var lookbackPeriod = Math.Max(monthlyPeriod - RollingAverageMonthsForBalanceCalculation, 0);

                        _CurrentTotalTrancheBalanceForFeeCalculation += CalculateAverageTrancheBalanceForLookbackPeriod(
                            associatedTranche.TrancheCashFlows,
                            lookbackPeriod,
                            monthlyPeriod);
                    }
                }
            }
        }

        private double CalculateAverageTrancheBalanceForLookbackPeriod(
            List<SecuritizationCashFlow> trancheCashFlows, 
            int lookbackPeriod,
            int monthlyPeriod)
        {
            var trancheCashFlowPeriods = trancheCashFlows.Where(c => c.Period >= lookbackPeriod && c.Period < monthlyPeriod).ToList();
            var averageTrancheBalance = trancheCashFlowPeriods.Average(c => c.EndingBalance);

            return averageTrancheBalance;
        }
    }
}
