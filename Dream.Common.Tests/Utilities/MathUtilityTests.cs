using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Common.Utilities;
using Dream.Common.Enums;

namespace Dream.Common.Tests.Utilities
{
    [TestClass]
    public class MathUtilityTests
    {
        [TestMethod, Owner("Matthew Moore")]
        public void CheckDivisibilityOfIntegers_NumberIsDivisbleByDivisor_ReturnsTrue()
        {
            var number = 16;
            var divisor = 4;
            var isDivisible = MathUtility.CheckDivisibilityOfIntegers(number, divisor);
            Assert.IsTrue(isDivisible);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CheckDivisibilityOfIntegers_NumberIsNotDivisbleByDivisor_ReturnsFalse()
        {
            var number = 8;
            var divisor = 3;
            var isDivisible = MathUtility.CheckDivisibilityOfIntegers(number, divisor);
            Assert.IsFalse(isDivisible);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CheckDivisibilityOfIntegers_DivisorIsZero_ThrowsException()
        {
            var number = 5;
            var divisor = 0;
            MathUtility.CheckDivisibilityOfIntegers(number, divisor);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CheckDivisibilityOfIntegers_DivisorIsUnity_ReturnsTrue()
        {
            var number = 101;
            var divisor = 1;
            var isDivisible = MathUtility.CheckDivisibilityOfIntegers(number, divisor);
            Assert.IsTrue(isDivisible);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertAnnualRateToMonthlyRate_AnnualRateBetweenZeroAndUnity_ResultsAsExpected()
        {
            var annualRate = 0.15;
            var monthlyRate = MathUtility.ConvertAnnualRateToMonthlyRate(annualRate);
            Assert.AreEqual(0.013452, monthlyRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertAnnualRateToMonthlyRate_AnnualRateEqualsUnity_MonthlyRateIsUnity()
        {
            var annualRate = 1.00;
            var monthlyRate = MathUtility.ConvertAnnualRateToMonthlyRate(annualRate);
            Assert.AreEqual(1.000000, monthlyRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertAnnualRateToMonthlyRate_AnnualRateEqualsZero_MonthlyRateIsZero()
        {
            var annualRate = 0.00;
            var monthlyRate = MathUtility.ConvertAnnualRateToMonthlyRate(annualRate);
            Assert.AreEqual(0.000000, monthlyRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertAnnualRateToMonthlyRate_AnnualRateIsNegative_MonthlyRateIsNegative()
        {
            var annualRate = -0.15;
            var monthlyRate = MathUtility.ConvertAnnualRateToMonthlyRate(annualRate);
            Assert.AreEqual(-0.011715, monthlyRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertMonthlyRateToAnnualRate_MonthlyRateBetweenZeroAndUnity_ResultsAsExpected()
        {
            var monthlyRate = 0.05;
            var annualRate = MathUtility.ConvertMonthlyRateToAnnualRate(monthlyRate);
            Assert.AreEqual(0.459640, annualRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertMonthlyRateToAnnualRate_MonthlyRateEqualsUnity_AnnualRateIsUnity()
        {
            var monthlyRate = 1.00;
            var annualRate = MathUtility.ConvertMonthlyRateToAnnualRate(monthlyRate);
            Assert.AreEqual(1.000000, annualRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertMonthlyRateToAnnualRate_MonthlyRateEqualsZero_AnnualRateIsZero()
        {
            var monthlyRate = 0.00;
            var annualRate = MathUtility.ConvertMonthlyRateToAnnualRate(monthlyRate);
            Assert.AreEqual(0.000000, annualRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertMonthlyRateToAnnualRate_MonthlyRateIsNegative_AnnualRateIsNegative()
        {
            var monthlyRate = -0.05;
            var annualRate = MathUtility.ConvertMonthlyRateToAnnualRate(monthlyRate);
            Assert.AreEqual(-0.795856, annualRate, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateFixedAnnuityPayment_Sample30YearMortgage_ResultsAsExpected()
        {
            var presentValue = 408000;
            var interestRatePerPeriod = 0.04375 / 12.0;
            var numberOfPeriods = 360;
            var fixedPayment = MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);

            Assert.AreEqual(2037.08, fixedPayment, 0.01);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateFixedAnnuityPayment_NegativeInterestRate_ResultsAsExpected()
        {
            var presentValue = 408000;
            var interestRatePerPeriod = -0.00125 / 12.0;
            var numberOfPeriods = 360;
            var fixedPayment = MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);

            Assert.AreEqual(1112.16, fixedPayment, 0.01);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateFixedAnnuityPayment_ZeroInterestRate_ReturnsPresentValueDividedByNumberOfPeriods()
        {
            var presentValue = 360000;
            var interestRatePerPeriod = 0.00;
            var numberOfPeriods = 360;
            var fixedPayment = MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);

            var presentValueDividedByPeriods = (double) presentValue / numberOfPeriods;
            Assert.AreEqual(presentValueDividedByPeriods, fixedPayment, 0.01);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateFixedAnnuityPayment_OneHundredPercentInterestRateOnePeriodToMaturity_ReturnsDoublePresentValue()
        {
            var presentValue = 360000;
            var interestRatePerPeriod = 1.00;
            var numberOfPeriods = 1;
            var fixedPayment = MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);

            var doublePresentValue = 2.0 * 360000;
            Assert.AreEqual(doublePresentValue, fixedPayment, 0.01);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateFixedAnnuityPayment_ZeroPeriodsToMaturity_ThrowsException()
        {
            var presentValue = 100000;
            var interestRatePerPeriod = 0.010;
            var numberOfPeriods = 0;
            MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateFixedAnnuityPayment_NegativeOneHundredPercentInterestRate_ThrowsException()
        {
            var presentValue = 100000;
            var interestRatePerPeriod = -1.00;
            var numberOfPeriods = 360;
            MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateFixedAnnuityPayment_NegativeTwoHundredPercentInterestRate_ThrowsException()
        {
            var presentValue = 100000;
            var interestRatePerPeriod = -2.00;
            var numberOfPeriods = 360;
            MathUtility.CalculateFixedAnnuityPayment(presentValue, interestRatePerPeriod, numberOfPeriods);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateSimplyCompoundedInterestAccrualFactor_TimePeriodIsOneYear_ReturnsAnnualInterestRate()
        {
            var timePeriodInYears = 1.00;
            var annualInterestRate = 0.05;
            var simplyCompoundedInterestAccrual = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(timePeriodInYears, annualInterestRate);

            Assert.AreEqual(annualInterestRate, simplyCompoundedInterestAccrual, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateSimplyCompoundedInterestAccrualFactor_TimePeriodIsZero_ReturnsZero()
        {
            var timePeriodInYears = 0.00;
            var annualInterestRate = 0.05;
            var simplyCompoundedInterestAccrual = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(timePeriodInYears, annualInterestRate);

            Assert.AreEqual(0.0, simplyCompoundedInterestAccrual, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateSimplyCompoundedInterestAccrualFactor_TimePeriodIsLessThanOneYear_ResultsAsExpected()
        {
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;
            var simplyCompoundedInterestAccrual = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(timePeriodInYears, annualInterestRate);

            Assert.AreEqual(0.0125, simplyCompoundedInterestAccrual, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateSimplyCompoundedInterestAccrualFactor_TimePeriodIsGreaterThanOneYear_ResultsAsExpected()
        {
            var timePeriodInYears = 1.25;
            var annualInterestRate = 0.05;
            var simplyCompoundedInterestAccrual = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(timePeriodInYears, annualInterestRate);

            Assert.AreEqual(0.0625, simplyCompoundedInterestAccrual, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_SimpleCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Simply;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0125000, accrualFactor, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_ContinuousCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Continuously;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0125784, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_DailyCompoundingThirty360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Thirty360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.012577573, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_DailyCompoundingActual360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.012577573, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_DailyCompoundingActual365DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual365;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.012577585, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_DailyCompoundingActualActualDayCounting_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.ActualActual;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_MonthlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Monthly;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0125522, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_QuarterlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Quarterly;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0125000, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_SemiAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.SemiAnnually;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0124228, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_AnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Annually;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0122722, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateSimpleCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Simply;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0125000, accrualFactor, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateContinuousCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Continuously;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0124222, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateDailyCompoundingThirty360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Thirty360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.012423057, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateDailyCompoundingActual360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.012423057, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateDailyCompoundingActual365DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual365;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.012423045, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateMonthlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Monthly;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0124480, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateQuarterlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Quarterly;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0125000, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateSemiAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.SemiAnnually;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0125791, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Annually;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0127415, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodSimpleCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Simply;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0125000, accrualFactor, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodContinuousCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Continuously;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0125784, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodDailyCompoundingThirty360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Thirty360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.012579331, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodDailyCompoundingActual360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.012579331, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodDailyCompoundingActual365DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual365;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.012579319, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodMonthlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Monthly;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0126049, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodQuarterlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Quarterly;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0126582, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodSemiAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.SemiAnnually;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0127394, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeInterestRateNegativeTimePeriodAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Annually;
            var timePeriodInYears = -0.25;
            var annualInterestRate = -0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(0.0129059, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_ateNegativeTimePeriodSimpleCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Simply;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0125000, accrualFactor, 0.000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodContinuousCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Continuously;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0124222, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodDailyCompoundingThirty360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Thirty360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.012421342, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodDailyCompoundingActual360DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.012421342, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodDailyCompoundingActual365DayCounting_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.Actual365;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.012421354, accrualFactor, 0.000000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodMonthlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Monthly;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0123966, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodQuarterlyCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Quarterly;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0123456, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodSemiAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.SemiAnnually;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0122704, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_NegativeTimePeriodAnnualCompounding_ResultsAsExpected()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Annually;
            var timePeriodInYears = -0.25;
            var annualInterestRate = 0.05;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);

            Assert.AreEqual(-0.0121234, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_DailyCompoundingThirty360DayCounting_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.Thirty360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00 * 360;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_DailyCompoundingActual360DayCounting_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.Actual360;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00 * 360;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_DailyCompoundingActual365DayCounting_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.Actual365;
            var compoundingConvention = CompoundingConvention.Daily;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00 * 365;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_MonthlyCompounding_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Monthly;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00 * 12;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_QuarterlyCompounding_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Quarterly;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00 * 4;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_SemiAnnualCompounding_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.SemiAnnually;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00 * 2;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void CalculateInterestAccrualFactor_AnnualCompounding_ThrowsException()
        {
            var dayCountConvention = DayCountConvention.None;
            var compoundingConvention = CompoundingConvention.Annually;
            var timePeriodInYears = 0.25;
            var annualInterestRate = -1.00;

            var accrualFactor = MathUtility.CalculateInterestAccrualFactor(
                dayCountConvention,
                compoundingConvention,
                timePeriodInYears,
                annualInterestRate);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_TimePeriodInYearsIsZero_ReturnsZero()
        {
            var accrualFactor = 0.0;
            var timePeriodInYears = 0.00;
            var annualInterestRate = 0.05;

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Simply, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Continuously, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.Thirty360, CompoundingConvention.Daily, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.Actual360, CompoundingConvention.Daily, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.Actual365, CompoundingConvention.Daily, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Monthly, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Quarterly, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.SemiAnnually, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Annually, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void CalculateInterestAccrualFactor_AnnualInterestRateIsZero_ReturnsZero()
        {
            var accrualFactor = 0.0;
            var timePeriodInYears = 0.25;
            var annualInterestRate = 0.00;

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Simply, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Continuously, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.Thirty360, CompoundingConvention.Daily, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.Actual360, CompoundingConvention.Daily, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.Actual365, CompoundingConvention.Daily, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Monthly, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Quarterly, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.SemiAnnually, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);

            accrualFactor = MathUtility.CalculateInterestAccrualFactor
                (DayCountConvention.None, CompoundingConvention.Annually, timePeriodInYears, annualInterestRate);
            Assert.AreEqual(0.0, accrualFactor, 0.0000001);
        }
    }
}
