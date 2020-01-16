using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class SecuritizationAnalysisInputEntity
    {
        public int SecuritizationAnalysisInputId { get; set; }
        public string SecuritizationAnalysisScenarioDescription { get; set; }
        public string AdditionalAnalysisScenarioDescription { get; set; }
        public DateTime CollateralCutOffDate { get; set; }
        public DateTime CashFlowStartDate { get; set; }
        public DateTime InterestAccrualStartDate { get; set; }
        public DateTime SecuritizationClosingDate { get; set; }
        public DateTime SecuritizationFirstCashFlowDate { get; set; }
        public string SelectedAggregationGrouping { get; set; }
        public string SelectedPerformanceAssumption { get; set; }
        public string SelectedPerformanceAssumptionGrouping { get; set; }
        public int? NominalSpreadRateIndexGroupId { get; set; }
        public int? CurveSpreadRateIndexId { get; set; }
        public bool UseReplines { get; set; }
        public bool SeparatePrepaymentInterest { get; set; }
        public double? PreFundingAmount { get; set; }
        public double? PreFundingBondCount { get; set; }
        public double? CleanUpCallPercentage { get; set; }
        public bool? UsePreFundingStartDate { get; set; }
    }
}
