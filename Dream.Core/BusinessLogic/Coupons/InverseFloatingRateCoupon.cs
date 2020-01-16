using System;
using System.Linq;
using Dream.Common.Curves;

namespace Dream.Core.BusinessLogic.Coupons
{
    public class InverseFloatingRateCoupon : FloatingRateCoupon
    {
        public InverseFloatingRateCoupon(
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
            : base(startingCouponRate, 
                   factor,
                   lifeCap, 
                   lifeFloor, 
                   margin, 
                   interimCap, 
                   lookbackMonths, 
                   monthsToNextRateReset, 
                   interestRateResetFrequencyInMonths, 
                   interestRateCurve) { }

        public override double GetCouponForSpecificMonthlyPeriod(int monthlyPeriod, DateTime startDate)
        {
            throw new NotImplementedException();
        }

        public override Coupon Copy()
        {
            return new InverseFloatingRateCoupon(
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
    }
}
