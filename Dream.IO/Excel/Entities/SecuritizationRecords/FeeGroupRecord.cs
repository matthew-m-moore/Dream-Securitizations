using System;

namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class FeeGroupRecord
    {
        public string NodeDescription { get; set; }
        public string FundsDistribution { get; set; }
        public string FeeGroupName { get; set; }

        public string FeeType { get; set; }
        public bool IsShortfallRecoverable { get; set; }
        public bool IsShortfallPaidFromReserves { get; set; }
        public bool PaysOutAtRedemption { get; set; }

        public DateTime FirstFeePaymentDate { get; set; }
        public string PaymentConvention { get; set; }

        public double? AccruedPayment { get; set; }
        public string Accrual { get; set; }
        public string InitialFeeAccrual { get; set; }
        public DateTime? InitialAccrualEndDate { get; set; }

        public string FundsSource { get; set; }

        public double? AnnualFeePerUnit { get; set; }
        public double? AnnualMinFeeAmount { get; set; }
        public double? AnnualMaxFeeAmount { get; set; }

        public int? RollingAverageMonths { get; set; }
        public double? PercentOfTrancheBalance { get; set; }
        public string AssociatedTrancheName1 { get; set; }
        public string AssociatedTrancheName2 { get; set; }
        public string AssociatedTrancheName3 { get; set; }

        public double? IncreaseRate { get; set; }
        public int? IncreaseFrequency { get; set; }

        public double? PercentOfCollateral { get; set; }
        public string BeginningOrEndingBalance { get; set; }
        public int? BalanceUpdateFreq { get; set; }
    }
}
