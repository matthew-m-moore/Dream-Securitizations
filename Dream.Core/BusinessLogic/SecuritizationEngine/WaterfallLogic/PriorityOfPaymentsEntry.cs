using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic
{
    public class PriorityOfPaymentsEntry
    {
        public int SeniorityRanking { get; }
        public string TrancheName { get; }
        public TrancheCashFlowType TrancheCashFlowType { get; }

        public PriorityOfPaymentsEntry(int seniorityRanking, string trancheName, TrancheCashFlowType trancheCashFlowType)
        {
            SeniorityRanking = seniorityRanking;
            TrancheName = trancheName;
            TrancheCashFlowType = trancheCashFlowType;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public PriorityOfPaymentsEntry Copy()
        {
            return new PriorityOfPaymentsEntry(
                SeniorityRanking,
                TrancheName,
                TrancheCashFlowType);
        }
    }
}
