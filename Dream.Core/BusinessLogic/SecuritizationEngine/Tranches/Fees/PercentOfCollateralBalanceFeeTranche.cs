using System;
using System.Linq;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class PercentOfCollateralBalanceFeeTranche : InitialPeriodSpecialAccrualFeeTranche
    {
        public double AnnualMinimumFee { get; }
        public double AnnualPercentageOfCollateralBalance { get; }
        public int BalanceUpdateFrequencyInMonths { get; }
        public DateTime DateOfFirstBalanceUpdate { get; }

        public bool UseStartingBalance = false;

        protected int? _MonthlyPeriodOfFirstBalanceUpdate;
        protected double _CurrentCollateralBalanceForFeeCalculation;

        public PercentOfCollateralBalanceFeeTranche(
            string trancheName,
            double annualMinimumFee,
            double percentageOfCollateralBalance,
            int balanceUpdateFrequencyInMonths,
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
            AnnualPercentageOfCollateralBalance = percentageOfCollateralBalance;
            BalanceUpdateFrequencyInMonths = balanceUpdateFrequencyInMonths;
            DateOfFirstBalanceUpdate = dateOfFirstBalanceUpdate;
        }

        public override Tranche Copy()
        {
            return new PercentOfCollateralBalanceFeeTranche(
                new string(TrancheName.ToCharArray()),
                AnnualMinimumFee,
                AnnualPercentageOfCollateralBalance,
                BalanceUpdateFrequencyInMonths,
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

                UseStartingBalance = UseStartingBalance,
            };
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            var nextFeePaymentPeriod = GetNextScheduledFeePaymentPeriod(monthlyPeriod);
            GetCollateralBalanceForFeeCalculation(nextFeePaymentPeriod, availableFunds);

            base.AllocatePaymentCashFlows(
                availableFunds,
                distributionRule,
                monthlyPeriod);
        }

        public override double DetermineFee(int monthlyPeriod)
        {
            var totalAnnualPercentOfCollateralFee = AnnualPercentageOfCollateralBalance * _CurrentCollateralBalanceForFeeCalculation;
            totalAnnualPercentOfCollateralFee = Math.Max(AnnualMinimumFee, totalAnnualPercentOfCollateralFee);

            var totalPercentOfCollateralFee = totalAnnualPercentOfCollateralFee * _TimeFactorInYearsForProRating;

            var baseFee = base.DetermineFee(monthlyPeriod);
            var totalFee = totalPercentOfCollateralFee + baseFee;

            return totalFee;
        }

        protected void GetCollateralBalanceForFeeCalculation(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (monthlyPeriod >= availableFunds.ProjectedCashFlowsOnCollateral.Count)
            {
                _CurrentCollateralBalanceForFeeCalculation = 0.0;
                return;
            }

            if (!_MonthlyPeriodOfFirstBalanceUpdate.HasValue)
            {
                // TODO: Consider a SingleOrDefault here and a check to see if the object is null, with exception
                var cashFlowOfFirstBalanceUpdate = availableFunds
                    .ProjectedCashFlowsOnCollateral.Single(c => c.PeriodDate.Month == DateOfFirstBalanceUpdate.Month
                                                             && c.PeriodDate.Year == DateOfFirstBalanceUpdate.Year);

                _MonthlyPeriodOfFirstBalanceUpdate = cashFlowOfFirstBalanceUpdate.Period;
            }

            var currentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].PeriodDate;

            if (currentPeriodDate.Ticks <= DateOfFirstBalanceUpdate.Ticks)
            {
                // Note, there's a really important reason why the first part of this conditional only uses the
                // starting balance. It has to do with pro-rate accrual of these fees.
                if (currentPeriodDate.Ticks < DateOfFirstBalanceUpdate.Ticks)
                {
                    _CurrentCollateralBalanceForFeeCalculation =
                        availableFunds.ProjectedCashFlowsOnCollateral.First().StartingBalance;
                }
                else
                {
                    _CurrentCollateralBalanceForFeeCalculation = UseStartingBalance
                        ? availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].StartingBalance
                        : availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].EndingBalance;
                }
            }
            else
            {
                var monthlyPeriodsSinceFirstUpdate = monthlyPeriod - _MonthlyPeriodOfFirstBalanceUpdate.Value;

                var totalNumberOfBalanceUpdates = (double) monthlyPeriodsSinceFirstUpdate / BalanceUpdateFrequencyInMonths;
                var integerNumberOfBalanceUpdates = (int) Math.Floor(totalNumberOfBalanceUpdates);

                var balanceLookupMonthlyPeriod = (BalanceUpdateFrequencyInMonths * integerNumberOfBalanceUpdates);
                var adjustedBalanceLookupPeriod = balanceLookupMonthlyPeriod + _MonthlyPeriodOfFirstBalanceUpdate.Value;

                if (availableFunds.ProjectedCashFlowsOnCollateral.Count <= adjustedBalanceLookupPeriod)
                {
                    _CurrentCollateralBalanceForFeeCalculation = 0.0;
                }
                else
                {
                    var updatedBalanceCashFlow = availableFunds.ProjectedCashFlowsOnCollateral[adjustedBalanceLookupPeriod];

                    _CurrentCollateralBalanceForFeeCalculation = UseStartingBalance
                        ? updatedBalanceCashFlow.StartingBalance
                        : updatedBalanceCashFlow.EndingBalance;
                }
            }
        }
    }
}
