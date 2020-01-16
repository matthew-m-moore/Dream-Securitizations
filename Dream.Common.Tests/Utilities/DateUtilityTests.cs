using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Common.Utilities;
using Dream.Common.Enums;

namespace Dream.Common.Tests.Utilities
{
    [TestClass]
    public class DateUtilityTests
    {
        private const double _oneTwelfth = 1.0 / 12.0;

        [TestMethod, Owner("Matthew Moore")]
        public void IsEndOfMonth_DateIsAtEndOfMonth_ReturnsTrue()
        {
            var nonFebruaryEndOfMonth = new DateTime(2016, 12, 31);
            Assert.IsTrue(DateUtility.IsEndOfMonth(nonFebruaryEndOfMonth));

            var februaryEndOfMonth = new DateTime(2016, 2, 29);
            Assert.IsTrue(DateUtility.IsEndOfMonth(februaryEndOfMonth));
        }

        [TestMethod, Owner("Matthew Moore")]
        public void IsEndOfMonth_DateIsNotAtEndOfMonth_ReturnsFalse()
        {
            var nonFebruaryNotEndOfMonth = new DateTime(2016, 12, 30);
            Assert.IsFalse(DateUtility.IsEndOfMonth(nonFebruaryNotEndOfMonth));

            var februaryNotEndOfMonth = new DateTime(2016, 2, 28);
            Assert.IsFalse(DateUtility.IsEndOfMonth(februaryNotEndOfMonth));
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateTimePeriodInYearsForOneMonth_NoDayCountingConvention_ThrowsException()
        {
            var startDate = DateTime.MinValue;
            var dayCountConvention = DayCountConvention.None;
            DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_Thirty360DayCountNonFebruary_ValueIsOneTwelfth()
        {
            var startDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.Thirty360;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.AreEqual(_oneTwelfth, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_Actual360DayCountNonFebruary_ValueIsMoreThanOneTwelfth()
        {
            var startDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.Actual360;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.IsTrue(timePeriod > _oneTwelfth);
            Assert.AreEqual(0.086111, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_Actual365DayCountNonFebruary_ValueIsMoreThanOneTwelfth()
        {
            var startDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.Actual365;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.IsTrue(timePeriod > _oneTwelfth);
            Assert.AreEqual(0.084932, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_ActualActualDayCountNonFebruary_ValueIsMoreThanOneTwelfth()
        {
            var startDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.ActualActual;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.IsTrue(timePeriod > _oneTwelfth);
            Assert.AreEqual(0.084699, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_Thirty360DayCountFebruary_ValueIsOneTwelfth()
        {
            var startDate = new DateTime(2000, 2, 1);
            var dayCountConvention = DayCountConvention.Thirty360;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.AreEqual(_oneTwelfth, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_Actual360DayCountFebruary_ValueIsLessThanOneTwelfth()
        {
            var startDate = new DateTime(2000, 2, 1);
            var dayCountConvention = DayCountConvention.Actual360;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.IsTrue(timePeriod < _oneTwelfth);
            Assert.AreEqual(0.080556, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_Actual365DayCountFebruary_ValueIsLessThanOneTwelfth()
        {
            var startDate = new DateTime(2000, 2, 1);
            var dayCountConvention = DayCountConvention.Actual365;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.IsTrue(timePeriod < _oneTwelfth);
            Assert.AreEqual(0.079452, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYearsForOneMonth_ActualActualDayCountFebruary_ValueIsLessThanOneTwelfth()
        {
            var startDate = new DateTime(2000, 2, 1);
            var dayCountConvention = DayCountConvention.ActualActual;
            var timePeriod = DateUtility.CalculateTimePeriodInYearsForOneMonth(dayCountConvention, startDate);

            Assert.IsTrue(timePeriod < _oneTwelfth);
            Assert.AreEqual(0.079235, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_Thirty360DayCountNonLeapYear_ValueIsUnity()
        {
            var startDate = new DateTime(1999, 1, 1);
            var endDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.Thirty360;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.AreEqual(1.0, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateTimePeriodInYears_NoDayCountingConvention_ThrowsException()
        {
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;
            var dayCountConvention = DayCountConvention.None;
            DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_Actual360DayCountNonLeapYear_ValueIsMoreThanUnity()
        {
            var startDate = new DateTime(1999, 1, 1);
            var endDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.Actual360;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.IsTrue(timePeriod > 1.0);
            Assert.AreEqual(1.013889, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_Actual365DayCountNonLeapYear_ValueIsUnity()
        {
            var startDate = new DateTime(1999, 1, 1);
            var endDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.Actual365;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.AreEqual(1.0, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_ActualActualDayCountNonLeapYear_ValueIsUnity()
        {
            var startDate = new DateTime(1999, 1, 1);
            var endDate = new DateTime(2000, 1, 1);
            var dayCountConvention = DayCountConvention.ActualActual;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.AreEqual(1.0, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_Thirty360DayCountLeapYear_ValueIsUnity()
        {
            var startDate = new DateTime(2000, 1, 1);
            var endDate = new DateTime(2001, 1, 1);
            var dayCountConvention = DayCountConvention.Thirty360;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.AreEqual(1.0, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_Actual360DayCountLeapYear_ValueIsMoreThanUnity()
        {
            var startDate = new DateTime(2000, 1, 1);
            var endDate = new DateTime(2001, 1, 1);
            var dayCountConvention = DayCountConvention.Actual360;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.IsTrue(timePeriod > 1.0);
            Assert.AreEqual(1.016667, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_Actual365DayCountLeapYear_ValueIsMoreThanUnity()
        {
            var startDate = new DateTime(2000, 1, 1);
            var endDate = new DateTime(2001, 1, 1);
            var dayCountConvention = DayCountConvention.Actual365;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.IsTrue(timePeriod > 1.0);
            Assert.AreEqual(1.002740, timePeriod, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateTimePeriodInYears_ActualActualDayCountLeapYear_ValueIsUnity()
        {
            var startDate = new DateTime(2000, 1, 1);
            var endDate = new DateTime(2001, 1, 1);
            var dayCountConvention = DayCountConvention.ActualActual;
            var timePeriod = DateUtility.CalculateTimePeriodInYears(dayCountConvention, startDate, endDate);

            Assert.AreEqual(1.0, timePeriod);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void MonthsBetweenTwoDates_DoNotConsiderDays_TwoDatesTwentyDaysApartAreConsideredZeroMonthsApart()
        {
            var nonFebraryStartDate = new DateTime(2016, 3, 1);
            var oneDayAfterStartDate = nonFebraryStartDate.AddDays(20);
            var numberOfMonthsApart = DateUtility.MonthsBetweenTwoDates(nonFebraryStartDate, oneDayAfterStartDate);

            Assert.AreEqual(0, numberOfMonthsApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void MonthsBetweenTwoDates_DoNotConsiderDays_TwoDatesOneDayApartAreConsideredOneMonthApart()
        {
            var nonFebraryStartDate = new DateTime(2016, 3, 31);
            var oneDayAfterStartDate = nonFebraryStartDate.AddDays(1);
            var numberOfMonthsApart = DateUtility.MonthsBetweenTwoDates(nonFebraryStartDate, oneDayAfterStartDate);

            Assert.AreEqual(1, numberOfMonthsApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void MonthsBetweenTwoDates_ConsiderDays_TwoDatesOneDayApartAreConsideredZeroMonthsApart()
        {
            var considerDays = true;
            var nonFebraryStartDate = new DateTime(2016, 3, 31);
            var oneDayAfterStartDate = nonFebraryStartDate.AddDays(1);
            var numberOfMonthsApart = DateUtility.MonthsBetweenTwoDates(nonFebraryStartDate, oneDayAfterStartDate, considerDays);

            Assert.AreEqual(0, numberOfMonthsApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void MonthsBetweenTwoDates_ConsiderDays_TwoDatesTwentyDaysApartAreConsideredOneMonthApart()
        {
            var considerDays = true;
            var nonFebraryStartDate = new DateTime(2016, 3, 31);
            var oneDayAfterStartDate = nonFebraryStartDate.AddDays(20);
            var numberOfMonthsApart = DateUtility.MonthsBetweenTwoDates(nonFebraryStartDate, oneDayAfterStartDate, considerDays);

            Assert.AreEqual(1, numberOfMonthsApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void MonthsBetweenTwoDates_ConsiderDays_TwoDatesFifteenDaysApartAreConsideredZeroMonthsApart()
        {
            var considerDays = true;
            var nonFebraryStartDate = new DateTime(2016, 3, 31);
            var fifteenDaysAfterStartDate = nonFebraryStartDate.AddDays(15);
            var numberOfMonthsApart = DateUtility.MonthsBetweenTwoDates(nonFebraryStartDate, fifteenDaysAfterStartDate, considerDays);

            Assert.AreEqual(0, numberOfMonthsApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void MonthsBetweenTwoDates_ConsiderDays_TwoDatesFortySixDaysApartAreConsideredTwoMonthsApart()
        {
            var considerDays = true;
            var nonFebraryStartDate = new DateTime(2016, 4, 30);
            var fortySixDaysAfterStartDate = nonFebraryStartDate.AddDays(46);
            var numberOfMonthsApart = DateUtility.MonthsBetweenTwoDates(nonFebraryStartDate, fortySixDaysAfterStartDate, considerDays);

            Assert.AreEqual(2, numberOfMonthsApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryNonLeapYear_OneFullMonthCountsAsTwentyEightDays()
        {
            var startDate = new DateTime(2015, 1, 31);
            var endDate = new DateTime(2015, 2, 28);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(28, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryNonLeapYear_LessThanFullMonthCountsAsTotalDaysPassed()
        {
            var startDate = new DateTime(2015, 1, 31);
            var endDate = new DateTime(2015, 2, 27);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(27, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryNonLeapYear_OneFullMonthCountsAsThirtyDays()
        {
            var startDate = new DateTime(2015, 2, 1);
            var endDate = new DateTime(2015, 3, 1);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(30, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryLeapYear_LessThanFullMonthCountsAsTotalDaysPassed()
        {
            var startDate = new DateTime(2016, 1, 31);
            var endDate = new DateTime(2016, 2, 28);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(28, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryLeapYear_OneFullMonthCountsAsTwentyNineDays()
        {
            var startDate = new DateTime(2016, 1, 31);
            var endDate = new DateTime(2016, 2, 29);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(29, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryLeapYear_OneFullMonthCountsAsThirtyDays()
        {
            var startDate = new DateTime(2016, 2, 1);
            var endDate = new DateTime(2016, 3, 1);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(30, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_ThirtyOneDayMonth_OneFullMonthCountsAsThirtyDays()
        {
            var startDate = new DateTime(2016, 4, 30);
            var endDate = new DateTime(2016, 5, 31);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(30, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_FebruaryMonthEndToFebruaryMonthEnd_OneFullYearCountsAs360Days()
        {
            var startDate = new DateTime(2016, 2, 29);
            var endDate = new DateTime(2017, 2, 28);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(360, numberOfDaysApart);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void DaysThirty360_YearAndOneDay_TotalTimeIs361Days()
        {
            var startDate = new DateTime(2016, 1, 31);
            var endDate = new DateTime(2017, 2, 1);
            var numberOfDaysApart = DateUtility.DaysThirty360(startDate, endDate);

            Assert.AreEqual(361, numberOfDaysApart);
        }
    }
}
