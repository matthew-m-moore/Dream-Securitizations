using System;

namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class TrancheStructureRecord
    {
        public string NodeDescription { get; set; }
        public string NodeType { get; set; }
        public string NodeRating { get; set; }
        public string NodePricingScenario { get; set; }
        public string FundsDistribution { get; set; }

        public string ChildNodeDescription { get; set; }
        public string ChildNodeType { get; set; }
        public string ChildNodeRating { get; set; }
        public string ChildNodePricingScenario { get; set; }
        public string ChildFundsDistribution { get; set; }

        public string TrancheName { get; set; }
        public string TrancheType { get; set; }
        public string TrancheDescription { get; set; }  
        public string TrancheRating { get; set; }
        public string TranchePricingScenario { get; set; }
        public double? Balance { get; set; }
        public double? Coupon { get; set; }

        public bool PaysOutAtRedemption { get; set; }
        public bool IsShortfallRecoverable { get; set; }
        public bool IsShortfallPaidFromReserves { get; set; }
        public bool? IsPrincipalPaidFromLiquidityReserve { get; set; }

        public string PricingMethodology { get; set; }
        public double PricingValue { get; set; }
        public string DayCounting { get; set; }
        public string Compounding { get; set; }

        public DateTime FirstPrinPaymentDate { get; set; }
        public DateTime? FirstIntPaymentDate { get; set; }

        public double? AccruedInterest { get; set; }
        public string InterestAccrual { get; set; }
        public string InitialInterestAccrual { get; set; }
        public DateTime? InitialAccrualEndDate { get; set; }

        public string PrincipalFundsSource { get; set; }
        public string InterestFundsSource { get; set; }
        public double? PrinAdvanceRate { get; set; }

        public int PrinPaymentFreq { get; set; }
        public int? IntPaymentFreq { get; set; }
    }
}
