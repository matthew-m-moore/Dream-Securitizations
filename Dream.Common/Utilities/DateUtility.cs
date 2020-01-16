using Dream.Common.Enums;
using System;

namespace Dream.Common.Utilities
{
    /// <summary>
    /// A collection of commonly used date manipulations and date math operations.
    /// </summary>
    public static class DateUtility
    {
        /// <summary>
        /// Checks if a date is at the end of a month.
        /// </summary>
        public static bool IsEndOfMonth(DateTime date)
        {
            // After incrementing the date by one day, is it a new month?
            var isEndOfMonth = (date.Month != date.AddDays(1).Month);

            return isEndOfMonth;
        }

        /// <summary>
        /// Checks if a date is at the end of a year.
        /// </summary>
        public static bool IsEndOfYear(DateTime date)
        {
            // After incrementing the date by one day, is it a new year?
            var IsEndOfYear = (date.Year != date.AddDays(1).Year);

            return IsEndOfYear;
        }

        /// <summary>
        /// Adds an integer number of months to the given date, but maintains the end-of-month status of the output date.
        /// </summary>
        public static DateTime AddMonthsForCashFlowPeriods(DateTime date, int monthsToAdd)
        {
            if (IsEndOfMonth(date))
            {
                return new DateTime(date.Year, date.Month, 1).AddMonths(1 + monthsToAdd).AddDays(-1);
            }

            return date.AddMonths(monthsToAdd);
        }

        /// <summary>
        /// Calculates time period in years for one month according to the day count convention chosen.
        /// </summary>
        public static double CalculateTimePeriodInYearsForOneMonth(DayCountConvention dayCountConvention, DateTime startDate)
        {
            var startingDate = new DateTime(startDate.Ticks);
            var endingDate = AddMonthsForCashFlowPeriods(startDate, 1);

            // Note, there is some bizarre logic here to ensure we always get one full month of interest, when using 30/360 day-counting
            if (IsEndOfMonth(endingDate) && IsMonthFebruary(endingDate) && dayCountConvention == DayCountConvention.Thirty360)
            {
                startingDate = endingDate.AddMonths(-1);
            }

            var timePeriod = CalculateTimePeriodInYears(dayCountConvention, startingDate, endingDate);
            return timePeriod;
        }

        /// <summary>
        /// Calculates time period in years between two dates according to the day count convention chosen.
        /// </summary>
        public static double CalculateTimePeriodInYears(DayCountConvention dayCountConvention, DateTime startDate, DateTime endDate)
        {
            switch (dayCountConvention)
            {
                case DayCountConvention.Thirty360:
                    return Thirty360TimePeriodInYears(startDate, endDate);

                case DayCountConvention.Actual360:
                    return Actual360TimePeriodInYears(startDate, endDate);

                case DayCountConvention.Actual365:
                    return Actual365TimePeriodInYears(startDate, endDate);

                case DayCountConvention.ActualActual:
                    return ActualActualIsdaTimePeriodInYears(startDate, endDate);
            }

            throw new Exception(string.Format("ERROR: Day counting convention {0} is not supported", dayCountConvention.ToString()));
        }

        /// <summary>
        /// Returns the integer number of months between two dates, with an option to consider the number of days.
        /// </summary>
        public static int MonthsBetweenTwoDates(DateTime startDate, DateTime endDate, bool considerDays = false)
        {
            var numberOfMonths = 0;

            // If the number of days is not to be considered, return at this point
            if (!considerDays)
            {
                numberOfMonths = Constants.MonthsInOneYear * (endDate.Year - startDate.Year)
                                 + (endDate.Month - startDate.Month);

                return numberOfMonths;
            }

            // Count the difference in days using 30/360 day-counting convention, then
            // convert both numbers to doubles (for precision) and divide them. 
            var numeratorAsDouble = (double) DaysThirty360(startDate, endDate);
            var denominatorAsDouble = (double) Constants.ThirtyDaysInOneMonth;

            // Fractions of a month greater than 0.5 will round up, less then will round down.
            // Fractions of 0.5 exactly will round to even (i.e. Banker's rounding).
            numberOfMonths = (int) Math.Round(numeratorAsDouble / denominatorAsDouble, 0);

            return numberOfMonths;
        }

        /// <summary>
        /// Counts the number of days between two dates according to 30/360 day-counting convention.
        /// http://en.wikipedia.org/wiki/360-day_calendar
        /// </summary>
        public static int DaysThirty360(DateTime startDate, DateTime endDate)
        {
            var considerDays = false;
            var numberOfMonths = MonthsBetweenTwoDates(startDate, endDate, considerDays);

            int startingDay; int endingDay;
            var numberOfDays = DifferenceInDaysThirty360(startDate, endDate, out startingDay, out endingDay);

            // If there is no difference in months, just count the difference in days
            if (numberOfMonths == 0)
            {
                return numberOfDays;
            }

            // Need to account for any partial month completed
            var daysUntilEndOfMonth = Constants.ThirtyDaysInOneMonth - startingDay;
            var daysFromStartOfMonth = endingDay;

            var numberOfDaysInMonthsPast = Constants.ThirtyDaysInOneMonth * (numberOfMonths - 1);

            numberOfDays = daysUntilEndOfMonth + daysFromStartOfMonth + numberOfDaysInMonthsPast;
            return numberOfDays;
        }

        private static int DifferenceInDaysThirty360(DateTime startDate, DateTime endDate, out int startingDay, out int endingDay)
        {
            var isStartDateEndOfMonth = IsEndOfMonth(startDate);
            var isStartDateEndOfMonthFebruary = isStartDateEndOfMonth && IsMonthFebruary(startDate);

            var isEndDateEndOfMonthFebruary = IsEndOfMonth(endDate) && IsMonthFebruary(endDate);

            startingDay = startDate.Day;
            endingDay = endDate.Day;

            // If both dates end on February month-end, the ending day is altered to 30.
            if (isStartDateEndOfMonthFebruary && isEndDateEndOfMonthFebruary)
            {
                endingDay = Constants.ThirtyDaysInOneMonth;
            }

            // If the start date is on month-end, alter it to be day 30.
            if (isStartDateEndOfMonth)
            {
                startingDay = Constants.ThirtyDaysInOneMonth;
            }

            // After applying the above criteria, set ending day to 30 if it is 31 and starting day is 30.
            // The logic here is to prevent ending up with a negative day being counted.
            if (startingDay == Constants.ThirtyDaysInOneMonth &&
                  endingDay == Constants.ThirtyOneDaysInOneMonth)
            {
                endingDay = Constants.ThirtyDaysInOneMonth;
            }

            var numberOfDays = endingDay - startingDay;
            return numberOfDays;
        }

        private static bool IsMonthFebruary(DateTime date)
        {
            var isMonthFebruary = (date.Month == (int) Month.February);

            return isMonthFebruary;
        }

        private static double Thirty360TimePeriodInYears(DateTime startDate, DateTime endDate)
        {
            var numberOfDays = DaysThirty360(startDate, endDate);
            var numberOfYears = (double) numberOfDays / Constants.ThreeHundredSixtyDaysInOneYear;

            return numberOfYears;
        }

        private static double Actual360TimePeriodInYears(DateTime startDate, DateTime endDate)
        {
            var numberOfDays = endDate.Subtract(startDate).Days;
            var numberOfYears = (double) numberOfDays / Constants.ThreeHundredSixtyDaysInOneYear;

            return numberOfYears;
        }

        private static double Actual365TimePeriodInYears(DateTime startDate, DateTime endDate)
        {
            var numberOfDays = endDate.Subtract(startDate).Days;
            var numberOfYears = (double) numberOfDays / Constants.ThreeHundredSixtyFiveDaysInOneYear;

            return numberOfYears;
        }

        /// <summary>
        /// There are a few conventions for Actual/Actual day-counting. The ISDA convetion is implemented below.
        /// http://en.wikipedia.org/wiki/Day_count_convention#Actual.2FActual_ISDA
        /// </summary>
        private static double ActualActualIsdaTimePeriodInYears(DateTime startDate, DateTime endDate)
        {
            var isStartDateLeapYear = DateTime.IsLeapYear(startDate.Year);
            var isEndDateLeapYear = DateTime.IsLeapYear(endDate.Year);
            var nextYearAfterStartDate = new DateTime(startDate.Year, Constants.MonthsInOneYear, Constants.ThirtyOneDaysInOneMonth).AddDays(1);

            var numberOfYears = Math.Max(0.0, endDate.Year - startDate.Year - 1.0);

            // Adjust start date if it's not a full year
            var adjustedNextYearAfterStartDate = nextYearAfterStartDate;
            if (endDate < nextYearAfterStartDate)
            {
                adjustedNextYearAfterStartDate = endDate;
            }

            // Calculate the residual time in the staring year.
            var excessDaysInStartYear = adjustedNextYearAfterStartDate.Subtract(startDate).Days;
            if (isStartDateLeapYear)
            {
                numberOfYears += (double) excessDaysInStartYear / Constants.ThreeHundredSixtySixDaysInOneYear;
            }
            else
            {
                numberOfYears += (double) excessDaysInStartYear / Constants.ThreeHundredSixtyFiveDaysInOneYear;
            }

            // Calculate the residual time in the ending year.
            if (endDate > nextYearAfterStartDate)
            {
                var startYearOfEndDate = new DateTime(endDate.Year, 1, 1);
                var excessDaysInEndYear = endDate.Subtract(startYearOfEndDate).Days;
                if (isStartDateLeapYear)
                {
                    numberOfYears += (double) excessDaysInEndYear / Constants.ThreeHundredSixtySixDaysInOneYear;
                }
                else
                {
                    numberOfYears += (double) excessDaysInEndYear / Constants.ThreeHundredSixtyFiveDaysInOneYear;
                }
            }

            return numberOfYears;
        }
    }
}
