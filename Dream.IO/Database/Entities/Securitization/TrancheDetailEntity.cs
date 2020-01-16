using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class TrancheDetailEntity
    {
        public int TrancheDetailId { get; set; }
        public string TrancheName { get; set; }
        public double? TrancheBalance { get; set; }
        public double? TrancheAccruedPayment { get; set; }
        public double? TrancheAccruedInterest { get; set; }
        public int TrancheTypeId { get; set; }
        public int? TrancheCouponId { get; set; }
        public int PaymentAvailableFundsRetrievalDetailId { get; set; }
        public int? InterestAvailableFundsRetrievalDetailId { get; set; }
        public int? ReserveAccountsSetId { get; set; }
        public int? BalanceCapAndFloorSetId { get; set; }
        public int? FeeGroupDetailId { get; set; }
        public int? PaymentConventionId { get; set; }
        public int? AccrualDayCountConventionId { get; set; }
        public int? InitialPaymentConventionId { get; set; }
        public int? InitialDayCountConventionId { get; set; }
        public DateTime? InitialPeriodEnd { get; set; }
        public int MonthsToNextPayment { get; set; }
        public int? MonthsToNextInterestPayment { get; set; }
        public int PaymentFrequencyInMonths { get; set; }
        public int? InterestPaymentFrequencyInMonths { get; set; }
        public bool? IncludePaymentShortfall { get; set; }
        public bool? IncludeInterestShortfall { get; set; }
        public bool? IsShortfallPaidFromReserves { get; set; }
    }
}
