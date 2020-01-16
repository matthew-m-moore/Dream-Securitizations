using System.Linq;
using Dream.Common.Curves;
using Dream.Common.Utilities;
using System;

namespace Dream.Core.BusinessLogic.Coupons
{
    public class FloatingRateCoupon : Coupon
    {
        public double StartingCouponRate { get; }
        public double LastCouponRate { get; private set; }

        public double Factor { get; }
        public double LifeCap { get; }
        public double LifeFloor { get; }
        public double Margin { get; }
        public double InterimCap { get; }

        public int LookbackMonths { get; }
        public int MonthsToNextRateReset { get; }
        public int InterestRateResetFrequencyInMonths { get; }

        public InterestRateCurve InterestRateCurve { get; }

        public FloatingRateCoupon(
            double startingCouponRate,
            double factor,
            double lifeCap,
            double lifeFloor,
            double margin,
            double interimCap,
            int lookbackMonths,
            int monthsToNextRateReset,
            int interestRateResetFrequencyInMonths,
            InterestRateCurve interestRateCurve)
        {
            StartingCouponRate = startingCouponRate;
            LastCouponRate = startingCouponRate;

            Factor = factor;
            LifeCap = lifeCap;
            LifeFloor = lifeFloor;
            Margin = margin;
            InterimCap = interimCap;

            LookbackMonths = lookbackMonths;
            MonthsToNextRateReset = monthsToNextRateReset;
            InterestRateResetFrequencyInMonths = interestRateResetFrequencyInMonths;

            InterestRateCurve = interestRateCurve;
        }

        public override Coupon Copy()
        {
            return new FloatingRateCoupon(
                StartingCouponRate,
                Factor,
                LifeCap,
                LifeFloor,
                Margin,
                InterimCap,
                LookbackMonths,
                MonthsToNextRateReset,
                InterestRateResetFrequencyInMonths,
                InterestRateCurve.Copy());
        }

        public override double GetCouponForSpecificMonthlyPeriod(int monthlyPeriod, DateTime startDate)
        {
            if (monthlyPeriod < MonthsToNextRateReset)
            {
                return StartingCouponRate;
            }

            var isRateResetMonth = MathUtility.CheckDivisibilityOfIntegers(
                monthlyPeriod - MonthsToNextRateReset,
                InterestRateResetFrequencyInMonths);

            if (isRateResetMonth || monthlyPeriod == MonthsToNextRateReset)
            {
                var dateDifferenceInMonths = DateUtility.MonthsBetweenTwoDates(InterestRateCurve.MarketDate, startDate);
                var rateIndexValue = InterestRateCurve.RateCurve[monthlyPeriod + dateDifferenceInMonths - LookbackMonths];
                var rateIndexPlusMargin = (rateIndexValue * Factor) + Margin;

                var periodicCap = LastCouponRate + InterimCap;
                var periodicFloor = LastCouponRate - InterimCap;

                var newCouponRate = CheckCapAndFloor(rateIndexPlusMargin, periodicCap, periodicFloor);
                var finalCouponRate = CheckCapAndFloor(newCouponRate, LifeCap, LifeFloor);

                LastCouponRate = finalCouponRate;
                return finalCouponRate;
            }

            return LastCouponRate;
        }

        private double CheckCapAndFloor(double targetCouponRate, double rateCap, double rateFloor)
        {
            if (targetCouponRate > rateCap)
            {
                return rateCap;
            }

            if (targetCouponRate < rateFloor)
            {
                return rateFloor;
            }

            return targetCouponRate;
        }
    }
}
