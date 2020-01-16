using System;
using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class SecuritizationTrancheModelEntry
    {
        public int SecuritizationTrancheId { get; set; }
        public string SecuritizationTrancheName { get; set; }
        public int SecuritizationNodeId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public string SecuritizationTrancheTypeDescription { get; set; }
        public double? SecuritizationTrancheBalance { get; set; }
        public double? SecuritizationTrancheInitialCoupon { get; set; }
        public double? AccruedInterest { get; set; }
        public DateTime FirstPrincipalPaymentDate { get; set; }
        public DateTime? FirstInterestPaymentDate { get; set; }
        public string InterestAccrualDayCountConvention { get; set; }
        public string InitialPeriodInterestAccrualDayCountConvention { get; set; }
        public DateTime? InitialPeriodInterestAccuralEndDate { get; set; }
        public string PrincipalAvailableFundsRetrieverDescription { get; set; }
        public string InterestAvailableFundsRetrieverDescription { get; set; }
        public double? PrincipalAdvanceRate { get; set; }
        public int PrincipalPaymentFrequencyInMonths { get; set; }
        public int? InterestPaymentFrequencyeInMonths { get; set; }
        public bool PaysOutAtRedemption { get; set; }
        public bool IsShortfallRecoverable { get; set; }
        public bool IsShortfallPaidFromReserves { get; set; }
        public List<string> AssociatedReserveAccountNames { get; set; }
        public string PricingMethodologyDescription { get; set; }
        public string PricingDayCountConvention { get; set; }
        public string PricingCompoundingConvention { get; set; }
        public double PricingValue { get; set; }
    }
}