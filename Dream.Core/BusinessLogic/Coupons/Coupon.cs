using System;

namespace Dream.Core.BusinessLogic.Coupons
{
    public abstract class Coupon
    {
        public abstract double GetCouponForSpecificMonthlyPeriod(int monthlyPeriod, DateTime startDate);
        public abstract Coupon Copy();
    }
}
