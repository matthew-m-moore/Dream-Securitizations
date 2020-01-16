using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class RedemptionLogicDataSetEntity
    {
        public int RedemptionLogicDataSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public int RedemptionLogicTypeId { get; set; }
        public int? RedemptionLogicAllowedMonthsSetId { get; set; }
        public int? RedemptionTranchesSetId { get; set; }
        public int? RedemptionPriorityOfPaymentsSetId { get; set; }
        public int? PostRedemptionPriorityOfPaymentsSetId { get; set; }
        public double? RedemptionLogicTriggerValue { get; set; }
    }
}
