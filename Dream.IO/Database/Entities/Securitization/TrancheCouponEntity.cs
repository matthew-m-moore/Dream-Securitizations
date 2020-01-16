using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class TrancheCouponEntity
    {
        public int TrancheCouponId { get; set; }
        public int? TrancheCouponRateIndexId { get; set; }
        public double TrancheCouponValue { get; set; }
        public double? TrancheCouponFactor { get; set; }
        public double? TrancheCouponMargin { get; set; }
        public double? TrancheCouponFloor { get; set; }
        public double? TrancheCouponCeiling { get; set; }
        public double? TrancheCouponInterimCap { get; set; }     
        public int InterestAccrualDayCountConventionId { get; set; }
        public int? InitialPeriodInterestAccrualDayCountConventionId { get; set; }
        public DateTime InitialPeriodInterestAccrualEndDate { get; set; }
        public int? InterestRateResetFrequencyInMonths { get; set; }
        public int? InterestRateResetLookbackMonths { get; set; }
        public int? MonthsToNextInterestRateReset { get; set; }
    }
}
