using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class CalendarDimMapping : EntityTypeConfiguration<CalendarDimEntity>
    {
        public CalendarDimMapping()
        {
            HasKey(t => t.DateDWId);

            ToTable("CalendarDim", Constants.FinanceManagementSchemaName);

            Property(t => t.DateDWId)
                .HasColumnName("DateDWId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.StartOfQuarterDate).HasColumnName("StartOfQuarterDate");
            Property(t => t.StartOfMonthDate).HasColumnName("StartOfMonthDate");
            Property(t => t.EndOfMonthDate).HasColumnName("EndOfMonthDate");
            Property(t => t.AltStartOfWeekDate).HasColumnName("AltStartOfWeekDate");
            Property(t => t.AltEndOfWeekDate).HasColumnName("AltEndOfWeekDate");

            Property(t => t.DateMMDDYYYY).HasColumnName("DateMMDDYYYY");
            Property(t => t.DateDDMMYYYY).HasColumnName("DateDDMMYYYY");
            Property(t => t.MonthNameAbbreviation).HasColumnName("MonthNameAbbreviation");
            Property(t => t.HolidayName).HasColumnName("HolidayName");
            Property(t => t.DayOfWeekName).HasColumnName("DayOfWeekName");
            Property(t => t.DayOfWeekNameAbbreviation).HasColumnName("DayOfWeekNameAbbreviation");
            Property(t => t.QuarterName).HasColumnName("QuarterName");

            Property(t => t.QuarterNumber).HasColumnName("QuarterNumber");
            Property(t => t.YearNumber).HasColumnName("YearNumber");
            Property(t => t.MonthNumber).HasColumnName("MonthNumber");
            Property(t => t.DayNumber).HasColumnName("DayNumber");
            Property(t => t.DayOfWeekNumber).HasColumnName("DayOfWeekNumber");
            Property(t => t.AltDayOfWeekNumber).HasColumnName("AltDayOfWeekNumber");

            Property(t => t.WeekOfMonth).HasColumnName("WeekOfMonth");
            Property(t => t.AltWeekOfMonth).HasColumnName("AltWeekOfMonth");
            Property(t => t.WeekOfYear).HasColumnName("WeekOfYear");
            Property(t => t.AltWeekOfYear).HasColumnName("AltWeekOfYear");
            
            Property(t => t.IsBankHoliday).HasColumnName("IsBankHoliday");
            Property(t => t.IsSalesHoliday).HasColumnName("IsSalesHoliday");
            Property(t => t.WeekDayFlag).HasColumnName("WeekDayFlag");
            Property(t => t.AltWeekOfYear).HasColumnName("AltWeekOfYear");
        }
    }
}
