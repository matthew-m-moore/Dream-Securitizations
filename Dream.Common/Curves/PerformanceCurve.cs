using System;
using System.Linq;
using Dream.Common.Enums;
using Dream.Common.Utilities;

namespace Dream.Common.Curves
{
    public class PerformanceCurve
    {
        public PerformanceCurveType Type { get; private set; }
        public Curve<double> Vector { get; private set; }

        public PerformanceCurve(PerformanceCurveType type, Curve<double> vector)
        {
            Type = type;
            Vector = vector;
        }


        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public PerformanceCurve Copy()
        {
            return new PerformanceCurve(Type, new Curve<double>(Vector.ToList()));
        }

        /// <summary>
        /// Converts the underlying vector to an annual rate and changes the curve type accordingly.
        /// </summary>
        public PerformanceCurveType ConvertToAnnualRate()
        {
            var annualizedVector = GetVectorAsAnnualRate();
            var companionType = FindAnnualCompanionType(Type);

            Vector = annualizedVector;
            Type = companionType;

            return Type;
        }

        /// <summary>
        /// Converts the underlying vector to a monthly rate and changes the curve type accordingly.
        /// </summary>
        public PerformanceCurveType ConvertToMonthlyRate()
        {
            var monthlyRateVector = GetVectorAsMonthlyRate();
            var companionType = FindMonthlyCompanionType(Type);

            Vector = monthlyRateVector;
            Type = companionType;

            return Type;
        }

        /// <summary>
        /// Returns the internal vector as an annualized rate.
        /// </summary>
        public Curve<double> GetVectorAsAnnualRate()
        {
            if (GetCompoundingConvention(Type) == CompoundingConvention.Annually)
            {
                return Vector;
            }

            var annualRateVector = Vector.Select(v => MathUtility.ConvertMonthlyRateToAnnualRate(v)).ToList();
            return new Curve<double>(annualRateVector);
        }

        /// <summary>
        /// Returns the internal vector as an monthly rate.
        /// </summary>
        public Curve<double> GetVectorAsMonthlyRate()
        {
            if (GetCompoundingConvention(Type) == CompoundingConvention.Monthly)
            {
                return Vector;
            }

            var monthlyRateVector = Vector.Select(v => MathUtility.ConvertAnnualRateToMonthlyRate(v)).ToList();
            return new Curve<double>(monthlyRateVector);
        }

        /// <summary>
        /// Returns the annualized performance curve type for a given monthly performance curve type.
        /// </summary>
        public static PerformanceCurveType FindAnnualCompanionType(PerformanceCurveType type)
        {
            if (GetCompoundingConvention(type) == CompoundingConvention.Annually)
            {
                return type;
            }

            switch (type)
            {
                case PerformanceCurveType.Smm:
                    return PerformanceCurveType.Cpr;

                case PerformanceCurveType.Mdr:
                    return PerformanceCurveType.Cdr;

                default:
                    throw new Exception(string.Format("ERROR: No companion type found for performance curve type {0}.", type.ToString().ToUpper()));
            }
        }

        /// <summary>
        /// Returns the montly performance curve type for a given annualized performance curve type.
        /// </summary>
        public static PerformanceCurveType FindMonthlyCompanionType(PerformanceCurveType type)
        {
            if (GetCompoundingConvention(type) == CompoundingConvention.Monthly)
            {
                return type;
            }

            switch (type)
            {
                case PerformanceCurveType.Cpr:
                    return PerformanceCurveType.Smm;

                case PerformanceCurveType.Cdr:
                    return PerformanceCurveType.Mdr;

                default:
                    throw new Exception(string.Format("ERROR: No companion type found for performance curve type {0}.", type.ToString().ToUpper()));
            }
        }

        /// <summary>
        /// Returns the rate convention for the underlying vector. Options are typically an annual or monthly rate.
        /// </summary>
        private static CompoundingConvention GetCompoundingConvention(PerformanceCurveType type)
        {
            switch (type)
            {
                case PerformanceCurveType.Cpr:
                case PerformanceCurveType.Cdr:
                    return CompoundingConvention.Annually;

                case PerformanceCurveType.Dq:
                case PerformanceCurveType.Smm:
                case PerformanceCurveType.Mdr:
                case PerformanceCurveType.Lgd:
                    return CompoundingConvention.Monthly;

                default:
                    throw new Exception(string.Format("ERROR: Performance curve type {0} was not found.", type.ToString().ToUpper()));
            }
        }
    }
}
