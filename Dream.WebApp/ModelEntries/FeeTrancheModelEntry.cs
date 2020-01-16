using System;
using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class FeeTrancheModelEntry
    {
        public int? FeeTrancheId { get; set; }
        public string FeeTrancheName { get; set; }
        public string FeeTrancheTypeDescription { get; set; }
        public double? AccruedFeePayment { get; set; }
        public string FeePaymentConvention { get; set; }
        public string FeeAccrualDayCountConvention { get; set; }
        public string InitialPeriodFeeAccrualDayCountConvention { get; set; }
        public DateTime FirstFeePaymentDate { get; set; }
        public DateTime? InitialPeriodFeeAccuralEndDate { get; set; }
        public string AvailableFundsRetrieverDescription { get; set; }
        public double? AnnualMinimumFeeAmount { get; set; }
        public double? AnnualMaximumFeeAmount { get; set; }
        public double? AnnualFeePerUnit { get; set; }
        public int? RollingAverageBalanceMonths { get; set; }
        public int? BalanceUpdateFrequenceInMonths { get; set; }
        public int? FeeIncreaseFrequncyInMonths { get; set; }
        public double? FeeIncreasePercentage { get; set; }
        public double? PercentOfTrancheBalance { get; set; }
        public double? PercentOfCollateralBalance { get; set; }
        public List<string> AssociatedTrancheNames { get; set; }
        public List<string> AssociatedReserveAccountNames { get; set; }
        public List<FeeDetailEntry> FeeDetailEntries { get; set; }
        public int SecuritizationNodeId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public bool UseStartingBalance { get; set; }
        public bool PaysOutAtRedemption { get; set; }
        public bool IsShortfallRecoverable { get; set; }
        public bool IsShortfallPaidFromReserves { get; set; }
    }
}