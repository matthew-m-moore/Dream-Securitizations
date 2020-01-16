using System;
using System.Collections.Generic;
using System.Linq;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public class PeriodicallyIncreasingFeeTranche : FlatFeeTranche
    {
        public double RateOfIncrease { get; }
        public int FeeIncreaseFrequencyInMonths { get; }
        public Dictionary<string, double> IncreasingFees { get; protected set; }

        private double? _totalIncreasingFees;
        public double TotalIncreasingAnnualFees
        {
            get
            {
                if (!_totalIncreasingFees.HasValue)
                {
                    _totalIncreasingFees = IncreasingFees.Values.Sum();
                }
              
                return _totalIncreasingFees.Value;
            }
        }

        public PeriodicallyIncreasingFeeTranche(
            string trancheName, 
            double rateOfIncrease,
            int feeIncreaseFrequencyInMonths,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever) 
            : base(trancheName, feePaymentConvention, proRatingDayCountConvention, availableFundsRetriever)
        {
            RateOfIncrease = rateOfIncrease;
            FeeIncreaseFrequencyInMonths = feeIncreaseFrequencyInMonths;
            IncreasingFees = new Dictionary<string, double>();
        }

        public override Tranche Copy()
        {
            return new PeriodicallyIncreasingFeeTranche(
                new string(TrancheName.ToCharArray()),
                RateOfIncrease,
                FeeIncreaseFrequencyInMonths,
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
                IncreasingFees = IncreasingFees.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                ListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts.ToList(),
                TriggerLogicDictionary = TriggerLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };
        }

        public override double DetermineFee(int monthlyPeriod)
        { 
            var totalNumberOfFeeIncreases = (double)(monthlyPeriod - MonthsToNextPayment) / FeeIncreaseFrequencyInMonths;
            var integerNumberOfFeeIncreases = (int) Math.Floor(totalNumberOfFeeIncreases);

            var totalIncreasingAnnualFee = TotalIncreasingAnnualFees * Math.Pow(1.0 + RateOfIncrease, integerNumberOfFeeIncreases);
            var totalIncreasingFee = totalIncreasingAnnualFee * _TimeFactorInYearsForProRating;

            var baseFee = base.DetermineFee(monthlyPeriod);
            var totalFee = totalIncreasingFee + baseFee;

            return totalFee;
        }

        public void AddIncreasingFee(string increasingFeeName, double increasingFeeValue)
        {
            IncreasingFees.Add(increasingFeeName, increasingFeeValue);
        }
    }
}
