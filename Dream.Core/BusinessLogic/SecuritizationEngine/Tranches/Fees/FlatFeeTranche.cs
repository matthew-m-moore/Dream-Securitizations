using System.Linq;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class FlatFeeTranche : FeeTranche
    {
        public FlatFeeTranche(
            string trancheName,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever) 
            : base(trancheName, feePaymentConvention, proRatingDayCountConvention, availableFundsRetriever)
        {
        }

        public override Tranche Copy()
        {
            return new FlatFeeTranche(
                new string(TrancheName.ToCharArray()),
                FeePaymentConvention,
                ProRatingDayCountConvention,
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

        public override double DetermineFee(int monthlyPeriod)
        {
            var relevantDelayedFees = DelayedAnnualFees.Values
                 .Where(f => monthlyPeriod >= f.DelayedUntilMonthlyPeriod)
                 .Sum(d => d.DelayedFeeValue);

            var totalAnnualFee = relevantDelayedFees + TotalBaseFees;
            var totalFee = totalAnnualFee * _TimeFactorInYearsForProRating;

            return totalFee;
        }
    }
}
