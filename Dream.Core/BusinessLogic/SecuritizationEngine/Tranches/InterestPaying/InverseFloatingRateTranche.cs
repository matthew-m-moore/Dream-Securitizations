using System;
using System.Linq;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.Coupons;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying
{
    public class InverseFloatingRateTranche : InterestPayingTranche
    {
        public InverseFloatingRateTranche( 
            string trancheName,
            PricingStrategy pricingStrategy,
            AvailableFundsRetriever principalAvailableFundsRetriever,
            AvailableFundsRetriever interestAvailableFundsRetriever,
            InverseFloatingRateCoupon inverseFloaterCoupon) 
            : base(trancheName, pricingStrategy, 
                   principalAvailableFundsRetriever, 
                   interestAvailableFundsRetriever,
                   inverseFloaterCoupon) { }

        public override Tranche Copy()
        {
            return new InverseFloatingRateTranche(
                new string(TrancheName.ToCharArray()),
                PricingStrategy.Copy(),
                AvailableFundsRetriever.Copy(),
                InterestAvailableFundsRetriever.Copy(),
                Coupon.Copy() as InverseFloatingRateCoupon)
            {
                TrancheDescription = (TrancheDescription == null) ? null : new string(TrancheDescription.ToCharArray()),
                TrancheRating = (TrancheRating == null) ? null : new string(TrancheRating.ToCharArray()),
                TranchePricingScenario = (TranchePricingScenario == null) ? null : new string(TranchePricingScenario.ToCharArray()),
                TrancheDetailId = TrancheDetailId,

                CurrentBalance = InitialBalance,
                InitialBalance = InitialBalance,
                AccruedInterest = AccruedInterest,              

                InterestAccrualDayCountConvention = InterestAccrualDayCountConvention,
                InterestAccrualStartDate = new DateTime(InterestAccrualStartDate.Ticks),
                InitialPeriodInterestAccrualDayCountConvention = InitialPeriodInterestAccrualDayCountConvention,
                InitialPeriodInterestAccrualEndDate = new DateTime(InitialPeriodInterestAccrualEndDate.Ticks),

                MonthsToNextInterestPayment = MonthsToNextInterestPayment,
                InterestPaymentFrequencyInMonths = InterestPaymentFrequencyInMonths,

                MonthsToNextPayment = MonthsToNextPayment,
                PaymentFrequencyInMonths = PaymentFrequencyInMonths,

                AbsorbsRemainingAvailableFunds = AbsorbsRemainingAvailableFunds,
                AbsorbsAssociatedReservesReleased = AbsorbsAssociatedReservesReleased,

                IncludePaymentShortfall = IncludePaymentShortfall,
                IncludeInterestShortfall = IncludeInterestShortfall,

                IsShortfallPaidFromReserves = IsShortfallPaidFromReserves,

                ListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts.ToList(),
                TriggerLogicDictionary = TriggerLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };
        }
    }
}
