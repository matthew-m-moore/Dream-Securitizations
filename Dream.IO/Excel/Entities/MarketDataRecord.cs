using Dream.Common;
using Dream.Common.Enums;

namespace Dream.IO.Excel.Entities
{
    public class MarketDataRecord
    {
        public string MarketDataGrouping { get; set; }
        public string MarketDataType { get; set; }
        public int? TenorInYears { get; set; }
        public int? TenorInMonths => TenorInYears * Constants.MonthsInOneYear;
        public double Value { get; set; }
    }
}
