using Dream.Common;

namespace Dream.Core.BusinessLogic.Containers
{
    public class PaceRatePlan
    {
        public string Description { get; set; }
        public int TermInYears { get; set; }
        public int TermInMonths => TermInYears * Constants.MonthsInOneYear;
        public double InterestRate { get; set; }
        public double BuyDownRate { get; set; }

        public PaceRatePlan() { }

        public PaceRatePlan(PaceRatePlan paceRatePlan)
        {
            Description = paceRatePlan.Description;
            TermInYears = paceRatePlan.TermInYears;
            InterestRate = paceRatePlan.InterestRate;
            BuyDownRate = paceRatePlan.BuyDownRate;
        }

        public PaceRatePlan Copy()
        {
            return new PaceRatePlan(this);
        }
    }
}
