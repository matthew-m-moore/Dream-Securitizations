using System;

namespace Dream.Core.BusinessLogic.Coupons
{
    public class FixedRateCoupon : Coupon
    {
        public double FixedCouponRate { get; }

        public FixedRateCoupon(double fixedCouponRate)
        {
            FixedCouponRate = fixedCouponRate;
        }

        public override double GetCouponForSpecificMonthlyPeriod(int monthlyPeriod, DateTime startDate)
        {
            return FixedCouponRate;
        }

        public override Coupon Copy()
        {
            return new FixedRateCoupon(FixedCouponRate);
        }
    }
}
