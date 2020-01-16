using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds
{
    public class CappedReserveFundTranche : ReserveFundTranche
    {
        public Dictionary<DateTime, double> ReserveFundBalanceCapDictionary { get; protected set; }

        public CappedReserveFundTranche(
            string trancheName,
            double initialDollarAmountOfReserves,
            AvailableFundsRetriever availableFundsRetriever) 
            : base(trancheName, initialDollarAmountOfReserves, availableFundsRetriever)
        {
            ReserveFundBalanceCapDictionary = new Dictionary<DateTime, double>();
        }

        public override Tranche Copy()
        {
            return new CappedReserveFundTranche(
                new string(TrancheName.ToCharArray()),
                InitialBalance,
                AvailableFundsRetriever.Copy())
            {
                TrancheDescription = (TrancheDescription == null) ? null : new string(TrancheDescription.ToCharArray()),
                TrancheRating = (TrancheRating == null) ? null : new string(TrancheRating.ToCharArray()),
                TranchePricingScenario = (TranchePricingScenario == null) ? null : new string(TranchePricingScenario.ToCharArray()),
                TrancheDetailId = TrancheDetailId,

                MonthsToNextPayment = MonthsToNextPayment,
                PaymentFrequencyInMonths = PaymentFrequencyInMonths,

                ReserveFundBalanceCapDictionary = ReserveFundBalanceCapDictionary.ToDictionary(kvp => new DateTime(kvp.Key.Ticks), kvp => kvp.Value),

                AbsorbsRemainingAvailableFunds = AbsorbsRemainingAvailableFunds,
                AbsorbsAssociatedReservesReleased = AbsorbsAssociatedReservesReleased,
                IncludePaymentShortfall = IncludePaymentShortfall,

                ListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts.ToList(),
                TriggerLogicDictionary = TriggerLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };
        }

        public override double DetermineReserveAmountsToContributeAndRelease(
            double reserveAmountPayable,
            int monthlyPeriod,
            AvailableFunds availableFunds, 
            out double reservesReleased)
        {
            if (!_IsReservesReleaseMonth)
            {
                reservesReleased = 0.0;
                return 0.0;
            }

            var reserveFundBalanceCap = GetReserveFundBalanceCap(
                monthlyPeriod, 
                availableFunds, 
                ReserveFundBalanceCapDictionary);

            var startingReserveFundBalance = TrancheCashFlows[monthlyPeriod].StartingBalance;
            var endingReserveFundBalance = startingReserveFundBalance + reserveAmountPayable;

            if (startingReserveFundBalance > reserveFundBalanceCap)
            {
                reservesReleased = startingReserveFundBalance - reserveFundBalanceCap;

                var reservesContributed = reserveFundBalanceCap - startingReserveFundBalance;
                return reservesContributed;
            }
            else if (endingReserveFundBalance > reserveFundBalanceCap)
            {
                reservesReleased = 0.0;

                var reservesContributed = reserveFundBalanceCap - startingReserveFundBalance;
                return reservesContributed;
            }
            else
            {
                reservesReleased = 0.0;
                return reserveAmountPayable;
            }
        }

        public virtual void AddReserveFundBalanceCap(DateTime asOfDate, double dollarAmountOfCap)
        {
            if (ReserveFundBalanceCapDictionary.ContainsKey(asOfDate))
            {
                throw new Exception("ERROR: Two balance fund caps cannot take effect on the same date");
            }

            ReserveFundBalanceCapDictionary.Add(asOfDate, dollarAmountOfCap);
        }

        protected virtual double GetReserveFundBalanceCap(
            int monthlyPeriod, 
            AvailableFunds availableFunds,
            Dictionary<DateTime, double> reserveFundBalanceCapDictionary)
        {
            var monthlyPeriodDate = availableFunds[monthlyPeriod].PeriodDate;
            var reserveFundBalanceCapKeyValuePair = reserveFundBalanceCapDictionary
                .OrderBy(r => r.Key.Ticks)
                .LastOrDefault(r => r.Key <= monthlyPeriodDate);

            // Note that the default value of a DateTime is the minimum value
            if (reserveFundBalanceCapKeyValuePair.Key == DateTime.MinValue)
            {
                throw new Exception(string.Format("ERROR: There is no balance cap/floor for reserve fund {0} with an as-of date less than {1}.",
                    TrancheName,
                    monthlyPeriodDate.ToString()));
            }

            var reserveFundBalanceCap = reserveFundBalanceCapKeyValuePair.Value;
            return reserveFundBalanceCap;
        }
    }
}
