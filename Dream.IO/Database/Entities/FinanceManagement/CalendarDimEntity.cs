using System;

namespace Dream.IO.Database.Entities.FinanceManagement
{
    public class CalendarDimEntity
    {
        public int DateDWId { get; set; }

        public DateTime Date { get; set; }
        public DateTime? StartOfQuarterDate { get; set; }
        public DateTime StartOfMonthDate { get; set; }
        public DateTime EndOfMonthDate { get; set; }
        public DateTime AltStartOfWeekDate { get; set; }
        public DateTime AltEndOfWeekDate { get; set; }

        public string DateMMDDYYYY { get; set; }
        public string DateDDMMYYYY { get; set; }
        public string MonthNameAbbreviation { get; set; }
        public string HolidayName { get; set; }
        public string DayOfWeekName { get; set; }
        public string DayOfWeekNameAbbreviation { get; set; }
        public string QuarterName { get; set; }

        public int QuarterNumber { get; set; }
        public int YearNumber { get; set; }
        public int MonthNumber { get; set; }
        public int DayNumber { get; set; }
        public int DayOfWeekNumber { get; set; }
        public int AltDayOfWeekNumber { get; set; }      

        public int? WeekOfMonth { get; set; }
        public int? AltWeekOfMonth { get; set; }
        public int WeekOfYear { get; set; }
        public int AltWeekOfYear { get; set; }

        public bool IsBankHoliday { get; set; }
        public bool IsSalesHoliday { get; set; }
        public bool WeekDayFlag { get; set; }
        public bool ActiveFlag { get; set; }
    }
}
