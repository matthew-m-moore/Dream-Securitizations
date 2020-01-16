using System;
using System.Linq;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Common.Utilities;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche : PercentOfCollateralBalanceFeeTranche
    {
        public SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche(
            string trancheName, 
            double percentageOfCollateralBalance, 
            int balanceUpdateFrequencyInMonths,
            DateTime initialAccrualEndDate,
            DayCountConvention initialProRatingDayCountConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention feePaymentConvention,
            AvailableFundsRetriever availableFundsRetriever) 
            : base(trancheName, 
                   default(double),
                   percentageOfCollateralBalance, 
                   balanceUpdateFrequencyInMonths, 
                   initialAccrualEndDate,
                   feePaymentConvention, 
                   proRatingDayCountConvention, 
                   PaymentConvention.None, 
                   initialProRatingDayCountConvention,
                   availableFundsRetriever)
        { }

        public override Tranche Copy()
        {
            return new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche(
                new string(TrancheName.ToCharArray()),
                AnnualPercentageOfCollateralBalance,
                BalanceUpdateFrequencyInMonths,
                new DateTime(DateOfFirstBalanceUpdate.Ticks),
                InitialProRatingDayCountConvention,
                ProRatingDayCountConvention,
                FeePaymentConvention,
                AvailableFundsRetriever.Copy())
            {
                TrancheDescription = (TrancheDescription == null) ? null : new string(TrancheDescription.ToCharArray()),
                TrancheRating = (TrancheRating == null) ? null : new string(TrancheRating.ToCharArray()),
                TranchePricingScenario = (TranchePricingScenario == null) ? null : new string(TranchePricingScenario.ToCharArray()),

                AccruedPayment = AccruedPayment,
                TrancheDetailId = TrancheDetailId,

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

        protected override void GetTimeFactorInYearsForProRating(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (monthlyPeriod >= availableFunds.ProjectedCashFlowsOnCollateral.Count)
            {
                _TimeFactorInYearsForProRating = 0.0;
                return;
            }

            var currentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].PeriodDate;
            
            // This is some bizarre logic, but it's correct so don't mess with it
            if (currentPeriodDate.Ticks <= InitialPeriodEndDate.Ticks)
            {
                var initialPeriodDate = availableFunds[0].PeriodDate;
                var lookbackMonthlyPeriod = Math.Max(monthlyPeriod - PaymentFrequencyInMonths, 0);
                var lastPaymentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[lookbackMonthlyPeriod].PeriodDate;

                // First, calculate the time factor for the whole special initial accrual period
                var initialPaymentTimeFactorInYears = DateUtility.CalculateTimePeriodInYears(
                    InitialProRatingDayCountConvention,
                    initialPeriodDate,
                    InitialPeriodEndDate);

                // Second, find the number of payments expected
                var numberOfMonthsLeftInAccrualPeriod = DateUtility.MonthsBetweenTwoDates(initialPeriodDate, InitialPeriodEndDate);
                var numberOfPaymentsLeftInAccrualPeriod = (double) numberOfMonthsLeftInAccrualPeriod / PaymentFrequencyInMonths;
                var integerNumberOfPaymentsLeftInAccrualPeriod = (int) Math.Round(numberOfPaymentsLeftInAccrualPeriod);

                // Last, divide the time factor up evenly
                _TimeFactorInYearsForProRating = initialPaymentTimeFactorInYears / integerNumberOfPaymentsLeftInAccrualPeriod;
            }
            else
            {
                base.GetTimeFactorInYearsForProRating(monthlyPeriod, availableFunds);
            }

            // There is a corner case where the initial accrual period ends before the first payment date projected
            // In that case, and extra amount of pro-rata fee is added to the overall fee for that period
            if (_MonthlyPeriodOfFirstBalanceUpdate.HasValue
                && _MonthlyPeriodOfFirstBalanceUpdate.Value < MonthsToNextPayment
                && monthlyPeriod <= MonthsToNextPayment)
            {
                var initialPeriodDate = availableFunds[0].PeriodDate;
                var initialPaymentTimeFactorInYears = DateUtility.CalculateTimePeriodInYears(
                    InitialProRatingDayCountConvention,
                    initialPeriodDate,
                    InitialPeriodEndDate);

                var initialCollateralBalanceForFeeCalculation = UseStartingBalance
                    ? availableFunds.ProjectedCashFlowsOnCollateral.First().StartingBalance
                    : availableFunds.ProjectedCashFlowsOnCollateral.First().EndingBalance;

                var scalingFactor = initialCollateralBalanceForFeeCalculation / _CurrentCollateralBalanceForFeeCalculation;

                initialPaymentTimeFactorInYears *= scalingFactor;
                _TimeFactorInYearsForProRating += initialPaymentTimeFactorInYears;
            }
        }
    }
}
