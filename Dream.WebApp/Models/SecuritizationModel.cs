using System;

namespace Dream.WebApp.Models
{
    public class SecuritizationModel
    {
        public string SecuritizationDescription { get; set; }
        public int SecuritizationDataSetId { get; set; }
        public int SecuritizationVersionId { get; set; }
        public SecuritizationSummaryModel SecuritizationSummaryModel { get; set; }
        public SecuritizationNodeModel SecuritizationNodeModel { get; set; }
        public SecuritizationTrancheModel SecuritizationTrancheModel { get; set; }
        public FeeTrancheModel FeeTrancheModel { get; set; }
        public ReserveAccountModel ReserveAccountModel { get; set; }
        public PriorityOfPaymentsModel PriorityOfPaymentsModel { get; set; }
        public MarketRateEnvironmentModel MarketRateEnvironmentModel { get; set; }
        public PerformanceAssumptionsModel PerformanceAssumptionsModel { get; set; }
        public ScenarioOptionsModel ScenarioOptionsModel { get; set; }
        public PaceAssessmentRecordModel PaceAssessmentRecordModel { get; set; }
        public CollateralizedTranchesModel CollateralizedTranchesModel { get; set; }
        public DateTime CollateralCutOffDate { get; set; }
        public DateTime CashFlowStartDate { get; set; }
        public DateTime InterestAccrualStartDate { get; set; }
        public DateTime SecuritizationClosingDate { get; set; }
        public DateTime SecuritizationFirstCashFlowDate { get; set; }
        public string RedemptionLogicDescription { get; set; }
        public string NominalSpreadRateIndexGroup { get; set; }
        public string CurveSpreadRateIndexGroup { get; set; }
        public bool IsResecuritization { get; set; }
        public bool UseReplines { get; set; }
        public bool SeparatePrepaymentInterest { get; set; }
        public double? PreFundingAmount { get; set; }
        public double? PreFundingBondCount { get; set; }
        public double? CleanUpCallPercentage { get; set; }
    }
}