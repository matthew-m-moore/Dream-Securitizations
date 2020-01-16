using Dream.Common;

namespace Dream.IO.Excel.Entities.CollateralTapeRecords
{
    public class RatePlanMappingRecord
    {
        public string RatePlanName { get; set; }
        public int TermInYears { get; set; }
        public int TermInMonths => TermInYears * Constants.MonthsInOneYear;
        public double InterestRate { get; set; }
        public double BuyDownRate { get; set; }
    }
}
