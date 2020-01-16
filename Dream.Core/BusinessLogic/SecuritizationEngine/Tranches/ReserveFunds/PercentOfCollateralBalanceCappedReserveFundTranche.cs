using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds
{
    public class PercentOfCollateralBalanceCappedReserveFundTranche : CappedReserveFundTranche
    {
        // This dictionary represents a floor on the balance cap, not to be too confusing
        public Dictionary<DateTime, double> ReserveFundBalanceFloorDictionary { get; protected set; }

        public PercentOfCollateralBalanceCappedReserveFundTranche(
            string trancheName, 
            double initialDollarAmountOfReserves, 
            AvailableFundsRetriever availableFundsRetriever) 
            : base(trancheName, initialDollarAmountOfReserves, availableFundsRetriever)
        {
            ReserveFundBalanceFloorDictionary = new Dictionary<DateTime, double>();
        }

        public override Tranche Copy()
        {
            return new PercentOfCollateralBalanceCappedReserveFundTranche(
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
                ReserveFundBalanceFloorDictionary = ReserveFundBalanceFloorDictionary.ToDictionary(kvp => new DateTime(kvp.Key.Ticks), kvp => kvp.Value),

                AbsorbsRemainingAvailableFunds = AbsorbsRemainingAvailableFunds,
                AbsorbsAssociatedReservesReleased = AbsorbsAssociatedReservesReleased,
                IncludePaymentShortfall = IncludePaymentShortfall,

                ListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts.ToList(),
                TriggerLogicDictionary = TriggerLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };
        }

        public override void AddReserveFundBalanceCap(DateTime asOfDate, double percentageAmountOfCap)
        {
            if (ReserveFundBalanceCapDictionary.ContainsKey(asOfDate))
            {
                throw new Exception("ERROR: Two balance fund caps cannot take effect on the same date");
            }

            if (percentageAmountOfCap > 1.0)
            {
                throw new Exception("ERROR: A cap as a percentage of collateral balance cannot be greater than 100%");
            }

            ReserveFundBalanceCapDictionary.Add(asOfDate, percentageAmountOfCap);
        }

        public virtual void AddReserveFundBalanceFloor(DateTime asOfDate, double dollarAmountOfFloor)
        {
            if (ReserveFundBalanceFloorDictionary.ContainsKey(asOfDate))
            {
                throw new Exception("ERROR: Two balance fund caps cannot take effect on the same date");
            }

            ReserveFundBalanceFloorDictionary.Add(asOfDate, dollarAmountOfFloor);
        }

        protected override double GetReserveFundBalanceCap(
            int monthlyPeriod, 
            AvailableFunds availableFunds,
            Dictionary<DateTime, double> reserveFundBalanceCapDictionary)
        {
            var reserveFundBalanceCapPercentage = base.GetReserveFundBalanceCap(
                monthlyPeriod, 
                availableFunds,
                reserveFundBalanceCapDictionary);

            var collateralEndingBalance = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].EndingBalance;
            var reserveFundBalanceCap = collateralEndingBalance * reserveFundBalanceCapPercentage;

            var reserveFundBalanceFloor = base.GetReserveFundBalanceCap(
                monthlyPeriod, 
                availableFunds,
                ReserveFundBalanceFloorDictionary);

            reserveFundBalanceCap = Math.Max(reserveFundBalanceCap, reserveFundBalanceFloor);

            return reserveFundBalanceCap;
        }
    }
}
