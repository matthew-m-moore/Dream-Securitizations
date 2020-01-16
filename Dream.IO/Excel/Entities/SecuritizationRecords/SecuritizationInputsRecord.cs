using System;

namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class SecuritizationInputsRecord : CashFlowGenerationInputsRecord
    {
        public string ScenarioDescription { get; set; }
        public bool? RunScenario { get; set; }

        public string AdditionalYieldScenario { get; set; }

        public DateTime SecuritizationStartDate { get; set; }
        public DateTime SecuritizationFirstCashFlowDate { get; set; }
        public DateTime? LastPreFundingDate { get; set; }

        public string RedemptionLogic { get; set; }
        public double? RedemptionLogicTriggerValue { get; set; }
        public double? CleanUpCallPercentage { get; set; }

        public double? PreFundingAmount { get; set; }
        public int PreFundingBondCount { get; set; }
        public bool? UsePreFundingStartDate { get; set; }
    }
}
