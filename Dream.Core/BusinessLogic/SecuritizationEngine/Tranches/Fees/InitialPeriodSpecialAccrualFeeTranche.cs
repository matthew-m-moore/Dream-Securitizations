using System;
using System.Linq;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Common.Utilities;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class InitialPeriodSpecialAccrualFeeTranche : FlatFeeTranche
    {
        public DateTime InitialPeriodEndDate { get; }
        public PaymentConvention InitialFeePaymentConvention { get; }
        public DayCountConvention InitialProRatingDayCountConvention { get; }       

        public InitialPeriodSpecialAccrualFeeTranche(
            string trancheName, 
            DateTime initialPeriodEndDate,
            PaymentConvention feePaymentConvention, 
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever) 
            : base(trancheName, feePaymentConvention, proRatingDayCountConvention, availableFundsRetriever)
        {
            InitialPeriodEndDate = initialPeriodEndDate;
            InitialFeePaymentConvention = initialFeePaymentConvention;
            InitialProRatingDayCountConvention = initialProRatingDayCountConvention;
        }

        public override Tranche Copy()
        {
            return new InitialPeriodSpecialAccrualFeeTranche(
                new string(TrancheName.ToCharArray()),
                new DateTime(InitialPeriodEndDate.Ticks),
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
        }

        protected override void GetTimeFactorInYearsForProRating(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (monthlyPeriod >= availableFunds.ProjectedCashFlowsOnCollateral.Count)
            {
                _TimeFactorInYearsForProRating = 0.0;
                return;
            }

            var currentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].PeriodDate;

            if (InitialPeriodEndDate != DateTime.MinValue &&
                InitialPeriodEndDate >= currentPeriodDate)
            {
                if (InitialFeePaymentConvention == PaymentConvention.ProRated)
                {
                    var lookbackMonthlyPeriod = Math.Max(monthlyPeriod - PaymentFrequencyInMonths, 0);
                    var lastPaymentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[lookbackMonthlyPeriod].PeriodDate;

                    var timeFactorInYears = DateUtility.CalculateTimePeriodInYears(
                        InitialProRatingDayCountConvention,
                        lastPaymentPeriodDate,
                        currentPeriodDate);

                    _TimeFactorInYearsForProRating = timeFactorInYears;
                }
                else
                {
                    SetTimeFactorFromFeePaymentConvention(InitialFeePaymentConvention);
                }
            }
            else
            {
                base.GetTimeFactorInYearsForProRating(monthlyPeriod, availableFunds);
            }
        }

        protected override double CalculateAccruedFeeAmount(int monthlyPeriod, int nextMonthlyPeriod, double feeAmount, AvailableFunds availableFunds)
        {
            if (nextMonthlyPeriod >= availableFunds.ProjectedCashFlowsOnCollateral.Count) return 0.0;

            // Do not bother with accrual through the first payment
            if (monthlyPeriod <= MonthsToNextPayment)
            {
                return base.CalculateAccruedFeeAmount(monthlyPeriod, nextMonthlyPeriod, feeAmount, availableFunds);
            }

            var currentPeriodDate = TrancheCashFlows[monthlyPeriod].PeriodDate;
            var priorPeriodDate = TrancheCashFlows[monthlyPeriod - 1].PeriodDate;
            var lastPaymentDate = availableFunds.ProjectedCashFlowsOnCollateral[nextMonthlyPeriod - PaymentFrequencyInMonths].PeriodDate;
            var nextPaymentDate = availableFunds.ProjectedCashFlowsOnCollateral[nextMonthlyPeriod].PeriodDate;

            if (currentPeriodDate.Ticks <= InitialPeriodEndDate.Ticks ||
                lastPaymentDate.Ticks < InitialPeriodEndDate.Ticks)
            {
                var intraPaymentTimePeriodInYears = 0.0;
                if (nextPaymentDate.Ticks > InitialPeriodEndDate.Ticks)
                {
                    intraPaymentTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        InitialProRatingDayCountConvention,
                        lastPaymentDate,
                        InitialPeriodEndDate);

                    intraPaymentTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        ProRatingDayCountConvention,
                        InitialPeriodEndDate,
                        nextPaymentDate);
                }
                else
                {
                    intraPaymentTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        InitialProRatingDayCountConvention,
                        lastPaymentDate,
                        nextPaymentDate);
                }

                var monthlyTimePeriodInYears = 0.0;
                if (priorPeriodDate.Ticks < InitialPeriodEndDate.Ticks
                 && currentPeriodDate.Ticks > InitialPeriodEndDate.Ticks)
                {
                    monthlyTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        InitialProRatingDayCountConvention,
                        priorPeriodDate,
                        InitialPeriodEndDate);

                    monthlyTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        ProRatingDayCountConvention,
                        InitialPeriodEndDate,
                        currentPeriodDate);
                }
                else if (priorPeriodDate.Ticks >= InitialPeriodEndDate.Ticks)
                {
                    monthlyTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        ProRatingDayCountConvention,
                        priorPeriodDate,
                        currentPeriodDate);
                }
                else
                {
                    monthlyTimePeriodInYears += DateUtility.CalculateTimePeriodInYears(
                        InitialProRatingDayCountConvention,
                        priorPeriodDate,
                        currentPeriodDate);
                }

                var grossFeeAmount = feeAmount / intraPaymentTimePeriodInYears;
                var accruedFeeAmount = grossFeeAmount * monthlyTimePeriodInYears;
                return accruedFeeAmount;
            }

            return base.CalculateAccruedFeeAmount(monthlyPeriod, nextMonthlyPeriod, feeAmount, availableFunds);
        }
    }
}
