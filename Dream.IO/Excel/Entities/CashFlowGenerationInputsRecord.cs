using System;

namespace Dream.IO.Excel.Entities
{
    public class CashFlowGenerationInputsRecord
    {
        public DateTime CollateralCutOffDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime InterestStartDate { get; set; }

        public string AggregationGrouping { get; set; }
        public string PerformanceAssumption { get; set; }
        public string PerformanceAssumptionGrouping { get; set; }

        public bool? TurnOnReplining { get; set; }
        public bool? SeparatePrepaymentInterest { get; set; }
    }
}
