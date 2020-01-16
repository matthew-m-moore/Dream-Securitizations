using Dream.Common;

namespace Dream.Core.BusinessLogic.Containers
{
    public class PrepaymentPenalty
    {
        public int PenaltyEndYear { get; set; }
        public int PenaltyEndMonth => PenaltyEndYear * Constants.MonthsInOneYear;
        public double PenaltyPercentageAmount { get; set; }
        public double PenaltyDollarAmount { get; set; }

        public PrepaymentPenalty(int penaltyEndYear, double penaltyPercentageAmount, double penaltyDollarAmount)
        {
            PenaltyEndYear = penaltyEndYear;
            PenaltyPercentageAmount = penaltyPercentageAmount;
            PenaltyDollarAmount = penaltyDollarAmount;
        }
    }
}
