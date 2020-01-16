﻿using System;
using System.Linq;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.Coupons;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying
{
    public class FloatingRateTranche : InterestPayingTranche
    {
        public FloatingRateTranche(
            string trancheName,
            PricingStrategy pricingStrategy, 
            AvailableFundsRetriever principalAvailableFundsRetriever,
            AvailableFundsRetriever interestAvailableFundsRetriever,
            FloatingRateCoupon floaterCoupon) 
            : base(trancheName, pricingStrategy, 
                   principalAvailableFundsRetriever, 
                   interestAvailableFundsRetriever,
                   floaterCoupon) { }

        public override Tranche Copy()
        {
            return new FloatingRateTranche(
                new string(TrancheName.ToCharArray()),
                PricingStrategy.Copy(),
                AvailableFundsRetriever.Copy(),
                InterestAvailableFundsRetriever.Copy(),
                Coupon.Copy() as FloatingRateCoupon)
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
