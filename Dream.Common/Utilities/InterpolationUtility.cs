using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Common.Utilities
{
    public static class InterpolationUtility
    {
        /// <summary>
        /// Linearly interpolates between the closest two x-values to find a y-value that is appropriately between them.
        /// </summary>
        public static double LinearlyInterpolate(double interpolationPoint, List<int> xValues, List<int> yValues, bool addOriginPoint)
        {
            var xValuesAsDouble = xValues.Select(x => (double)x).ToList();
            var yValuesAsDouble = yValues.Select(y => (double)y).ToList();

            if (addOriginPoint)
            {
                xValuesAsDouble.Insert(0, 0.0);
                yValuesAsDouble.Insert(0, 0.0);
            }

            return LinearlyInterpolate(interpolationPoint, xValuesAsDouble, yValuesAsDouble);
        }

        /// <summary>
        /// Linearly interpolates between the closest two x-values to find a y-value that is appropriately between them.
        /// </summary>
        public static double LinearlyInterpolate(double interpolationPoint, List<int> xValues, List<double> yValues, bool addOriginPoint)
        {
            var xValuesAsDouble = xValues.Select(x => (double)x).ToList();

            if (addOriginPoint)
            {
                xValuesAsDouble.Insert(0, 0.0);
                yValues.Insert(0, 0.0);
            }

            return LinearlyInterpolate(interpolationPoint, xValuesAsDouble, yValues);
        }

        /// <summary>
        /// Linearly interpolates between the closest two x-values to find a y-value that is appropriately between them.
        /// </summary>
        public static double LinearlyInterpolate(double interpolationPoint, List<double> xValues, List<double> yValues)
        {
            if (double.IsNaN(interpolationPoint)) return double.NaN;

            if (interpolationPoint > xValues.Max() || interpolationPoint < xValues.Min())
            {
                throw new Exception("ERROR: Interpolation point is outside the range of x-values provided.");
            }

            var xValueAboveInterpolationPoint = xValues.First(x => x >= interpolationPoint);
            var indexOfValueAboveInterpolationPoint = xValues.IndexOf(xValueAboveInterpolationPoint);
            var yValueAboveInterpolationPoint = yValues[indexOfValueAboveInterpolationPoint];

            if (xValueAboveInterpolationPoint == interpolationPoint) return yValueAboveInterpolationPoint;

            var xValueBelowInterpolationPoint = xValues[indexOfValueAboveInterpolationPoint - 1];
            var yValueBelowInterpolationPoint = yValues[indexOfValueAboveInterpolationPoint - 1];

            var slope = (yValueAboveInterpolationPoint - yValueBelowInterpolationPoint) / (xValueAboveInterpolationPoint - xValueBelowInterpolationPoint);
            var value = yValueBelowInterpolationPoint + (slope * (interpolationPoint - xValueBelowInterpolationPoint));

            return value;
        }
    }
}
