using System;
using System.Linq;
using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class BondCountBasedFeeTranche : InitialPeriodSpecialAccrualFeeTranche
    {
        public double AnnualFeePerBond { get; }
        public double AnnualMinimumFeeForBonds { get; }
        public double AnnualMaximumFeeForBonds { get; }

        private double _currentBondCountForFeeCalculation;

        public BondCountBasedFeeTranche(
            string trancheName,
            double annualFeePerBond,
            double annualMinimumFeeForBonds,
            double annualMaximumFeeForBonds,
            DateTime initialPeriodEndDate,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            PaymentConvention initialFeePaymentConvention,
            DayCountConvention initialProRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever)
            : base(trancheName, initialPeriodEndDate, feePaymentConvention, proRatingDayCountConvention, 
                   initialFeePaymentConvention, initialProRatingDayCountConvention, availableFundsRetriever)
        {
            AnnualFeePerBond = annualFeePerBond;
            AnnualMinimumFeeForBonds = annualMinimumFeeForBonds;
            AnnualMaximumFeeForBonds = annualMaximumFeeForBonds;
        }

        public override Tranche Copy()
        {
            return new BondCountBasedFeeTranche(
                new string(TrancheName.ToCharArray()),
                AnnualFeePerBond,
                AnnualMinimumFeeForBonds,
                AnnualMaximumFeeForBonds,
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

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            GetCurrentBondCountForFeeCalculation(monthlyPeriod, availableFunds);

            base.AllocatePaymentCashFlows(
                availableFunds,
                distributionRule,
                monthlyPeriod);
        }

        public override double DetermineFee(int monthlyPeriod)
        {
            var totalAnnualBondCountBasedFee = AnnualFeePerBond * _currentBondCountForFeeCalculation;
            totalAnnualBondCountBasedFee = Math.Max(AnnualMinimumFeeForBonds, totalAnnualBondCountBasedFee);
            totalAnnualBondCountBasedFee = Math.Min(AnnualMaximumFeeForBonds, totalAnnualBondCountBasedFee);

            var totalIncreasingFee = totalAnnualBondCountBasedFee * _TimeFactorInYearsForProRating;

            var baseFee = base.DetermineFee(monthlyPeriod);
            var totalFee = totalIncreasingFee + baseFee;

            return totalFee;
        }

        private void GetCurrentBondCountForFeeCalculation(int monthlyPeriod, AvailableFunds availableFunds)
        {
            var currentBondCountCashFlow = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod];

            _currentBondCountForFeeCalculation = currentBondCountCashFlow.BondCount;
        }
    }
}
