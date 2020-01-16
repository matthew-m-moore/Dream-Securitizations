using Dream.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Common.Utilities
{
    /// <summary>
    /// A collection of commonly used mathematical operations in finance and otherwise.
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// Checks that a given number is divisible by a specified divisor by checking that the modulus equals zero.
        /// </summary>
        public static bool CheckDivisibilityOfIntegers(int number, int divisor)
        {
            if (divisor == 0)
            {
                throw new Exception("ERROR: Cannot divide by zero when checking divisibility of integers.");
            }

            var isDivisible = (number % divisor) == 0;
            return isDivisible;
        }

        /// <summary>
        /// Takes a weighted average with the provided weights for the list of values given.
        /// </summary>
        public static double WeightedAverage(List<double> weights, List<double> values)
        {
            if (weights.Count != values.Count)
            {
                throw new Exception("ERROR: The number of weights must match the number of values");
            }

            var sumOfWeights = weights.Sum();
            var sumOfWeightedValues = 0.0;

            for (var i = 0; i < values.Count; i++)
            {
                sumOfWeightedValues += (weights[i] * values[i]);
            }

            var weightedAverage = sumOfWeightedValues / sumOfWeights;
            return weightedAverage;
        }

        /// <summary>
        /// De-annualizes an annual rate (i.e. CPR-to-SMM, CDR-to-MDR, etc.)
        /// </summary>
        public static double ConvertAnnualRateToMonthlyRate(double annualRate)
        {
            var oneTwelfth = 1.0 / Constants.MonthsInOneYear;
            var monthlyRate = 1.0 - Math.Pow(1.0 - annualRate, oneTwelfth);

            return monthlyRate;
        }

        /// <summary>
        /// Annualizes a monthly rate (i.e. SMM-to-CPR, MDR-to-CDR, etc.)
        /// </summary>
        public static double ConvertMonthlyRateToAnnualRate(double monthlyRate)
        {
            var annualRate = 1.0 - Math.Pow(1.0 - monthlyRate, Constants.MonthsInOneYear);

            return annualRate;
        }

        /// <summary>
        /// Calculates the fixed payment required for an annuity of N number of periods and target present value.
        /// (Note: This can be used to find the fully amortizing payment of a fixed rate loan with 30/360 day-counting.)
        /// https://en.wikipedia.org/wiki/Amortization_calculator
        /// </summary>
        public static double CalculateFixedAnnuityPayment(double presentValue, double interestRatePerPeriod, int numberOfPeriods)
        {
            if (numberOfPeriods <= 0)
            {
                throw new Exception("ERROR: A fixed annuity payment cannot be calculated for zero or negative periods.");
            }
            if (interestRatePerPeriod <= -1.0)
            {
                throw new Exception("ERROR: A fixed annuity payment cannot be calculated for an interest rate at or below negative 100%.");
            }

            var annuityFactor = CalculateAnnuityFactor(interestRatePerPeriod, numberOfPeriods);
            var annuityPayment = presentValue * annuityFactor;

            return annuityPayment;
        }

        /// <summary>
        /// Calculates the the present value of an annuity with N number of periods and a specific fixed payment.
        /// (Note: This can be used to find the principal balance of a fixed rate loan with 30/360 day-counting.)
        /// https://en.wikipedia.org/wiki/Present_value#Present_value_of_an_annuity
        /// </summary>
        public static double CalculatePresentValueOfAnnuity(double fixedPayment, double interestRatePerPeriod, int numberOfPeriods)
        {
            if (numberOfPeriods <= 0)
            {
                throw new Exception("ERROR: The present value of an annuity cannot be calculated for zero or negative periods.");
            }
            if (interestRatePerPeriod <= -1.0)
            {
                throw new Exception("ERROR: The present value of an annuity cannot be calculated for an interest rate at or below negative 100%.");
            }

            var annuityFactor = CalculateAnnuityFactor(interestRatePerPeriod, numberOfPeriods);
            var presentValue = fixedPayment / annuityFactor;

            return presentValue;
        }

        /// <summary>
        /// Calculates interest accrued over a specified time period in years using simple compounding.
        /// </summary>
        public static double CalculateSimplyCompoundedInterestAccrualFactor(double timePeriodInYears, double annualInterestRate)
        {
            var simplyCompoundedInterestAccrual = CalculateInterestAccrualFactor(
                DayCountConvention.None,
                CompoundingConvention.Simply,
                timePeriodInYears,
                annualInterestRate);

            return simplyCompoundedInterestAccrual;
        }

        /// <summary>
        /// Calculates interest accrued over a specified time period in years using the day-count and compounding conventions
        /// specfied in the inputs.
        /// </summary>
        public static double CalculateInterestAccrualFactor(
            DayCountConvention dayCountConvention, 
            CompoundingConvention compoundingConvention, 
            double timePeriodInYears,
            double annualInterestRate)
        {
            var accrualFactor = 1.0;

            switch (compoundingConvention)
            {
                case CompoundingConvention.Simply:
                    accrualFactor = 1.0 + annualInterestRate * timePeriodInYears;
                    break;

                case CompoundingConvention.Continuously:
                    accrualFactor = Math.Exp(annualInterestRate * timePeriodInYears);
                    break;

                case CompoundingConvention.Daily:
                    accrualFactor = GetDailyAccrualFactor(
                        dayCountConvention,
                        compoundingConvention,
                        timePeriodInYears,
                        annualInterestRate);
                    break;

                default:
                    accrualFactor = GetDefaultAccrualFactor(
                        compoundingConvention,
                        timePeriodInYears,
                        annualInterestRate);
                    break;
            }

            var finalAccuralFactor =  accrualFactor - 1.0;
            return finalAccuralFactor;
        }

        /// <summary>
        /// Calculates the discount rate from a discount factor for a specified time period in years using the day-count and 
        /// compounding conventions specfied in the inputs.
        /// </summary>
        public static double CalculateDiscountRateFromDiscountFactor(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingConvention,
            double timePeriodInYears,
            double discountFactor)
        {
            var discountRate = 0.0;

            switch (compoundingConvention)
            {
                case CompoundingConvention.Simply:
                    discountRate = timePeriodInYears * ((1.0 / discountFactor) - 1.0);
                    break;

                case CompoundingConvention.Continuously:
                    discountRate = (-1.0 * Math.Log(discountFactor)) / timePeriodInYears;
                    break;

                case CompoundingConvention.Daily:
                    discountRate = GetDailyDiscountRate(
                        dayCountConvention,
                        compoundingConvention,
                        timePeriodInYears,
                        discountFactor);
                    break;

                default:
                    discountRate = GetDefaultDiscountRate(
                        compoundingConvention,
                        timePeriodInYears,
                        discountFactor);
                    break;
            }

            return discountRate;
        }

        /// <summary>
        /// Calculates the annuity factor for N number of periods and a specific interest rate.
        /// </summary>
        public static double CalculateAnnuityFactor(double interestRatePerPeriod, int numberOfPeriods)
        {
            if (Math.Abs(interestRatePerPeriod) < 0.0001)
            {
                return 1.0 / numberOfPeriods;
            }

            var numerator = interestRatePerPeriod;
            var denominator = 1.0 - Math.Pow(1.0 + interestRatePerPeriod, -1 * numberOfPeriods);

            var annuityFactor = numerator / denominator;

            return annuityFactor;
        }

        /// <summary>
        /// Returns the assumed number of days in one year according to the specified day counting convention.
        /// </summary>
        public static int GetAssumedNumberOfDaysInOneYear(DayCountConvention dayCountConvention)
        {
            switch (dayCountConvention)
            {
                case DayCountConvention.Thirty360:
                case DayCountConvention.Actual360:
                    return Constants.ThreeHundredSixtyDaysInOneYear;
                case DayCountConvention.Actual365:
                    return Constants.ThreeHundredSixtyFiveDaysInOneYear;
                default:
                    throw new Exception("ERROR: Number of days assumed in one year is not strictly defined for "
                        + dayCountConvention + " day-counting convention.");
            }
        }

        private static double GetDailyAccrualFactor(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingConvention,
            double timePeriodInYears,
            double annualInterestRate)
        {
            var dailyCompoundFactor = GetAssumedNumberOfDaysInOneYear(dayCountConvention);
            var dailyExponentiatedTerm = 1.0 + annualInterestRate / dailyCompoundFactor;

            if (dailyExponentiatedTerm <= 0 && timePeriodInYears < 1.0)
            {
                ThrowUndefinedComputationException(compoundingConvention, timePeriodInYears, annualInterestRate);
            }

            var dailyAccrualFactor = Math.Pow(dailyExponentiatedTerm, dailyCompoundFactor * timePeriodInYears);
            return dailyAccrualFactor;
        }

        private static double GetDailyDiscountRate(
            DayCountConvention dayCountConvention,
            CompoundingConvention compoundingConvention,
            double timePeriodInYears,
            double discountFactor)
        {
            var dailyCompoundingFactor = GetAssumedNumberOfDaysInOneYear(dayCountConvention);
            var dailyExponentialTerm = 1.0 / (dailyCompoundingFactor * timePeriodInYears);

            if (dailyExponentialTerm <= 0)
            {
                ThrowUndefinedComputationException(compoundingConvention, timePeriodInYears, discountFactor);
            }

            var discountFactorTerm = Math.Pow((1.0 / discountFactor), dailyExponentialTerm);
            var discountRate = dailyCompoundingFactor * (discountFactorTerm - 1.0);
            return discountRate;
        }

        private static double GetDefaultAccrualFactor(
            CompoundingConvention compoundingConvention,
            double timePeriodInYears,
            double annualInterestRate)
        {
            var compoundingFactor = (int) compoundingConvention;
            var exponentiatedTerm = 1.0 + annualInterestRate / compoundingFactor;

            if (exponentiatedTerm <= 0 && timePeriodInYears < 1.0)
            {
                ThrowUndefinedComputationException(compoundingConvention, timePeriodInYears, annualInterestRate);
            }

            var accrualFactor = Math.Pow(exponentiatedTerm, compoundingFactor * timePeriodInYears);
            return accrualFactor;
        }

        private static double GetDefaultDiscountRate(
            CompoundingConvention compoundingConvention,
            double timePeriodInYears,
            double discountFactor)
        {
            var compoundingFactor = (int) compoundingConvention;
            var exponentialTerm = 1.0 / (compoundingFactor * timePeriodInYears);

            if (exponentialTerm <= 0)
            {
                ThrowUndefinedComputationException(compoundingConvention, timePeriodInYears, discountFactor);
            }

            var discountFactorTerm = Math.Pow((1.0 / discountFactor), exponentialTerm);
            var discountRate = compoundingFactor * (discountFactorTerm - 1.0);
            return discountRate;
        }

        private static void ThrowUndefinedComputationException(
            CompoundingConvention compoundingConvention,
            double timePeriodInYears,
            double annualInterestRate)
        {
            throw new Exception(string.Format("ERROR: Compounding is undefined for annual rate {0} and time period {1} with compounding convention {2}.",
                annualInterestRate,
                timePeriodInYears,
                compoundingConvention));
        }
    }
}
