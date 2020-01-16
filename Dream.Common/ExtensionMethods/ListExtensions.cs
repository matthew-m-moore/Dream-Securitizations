using System;
using System.Collections.Generic;

namespace Dream.Common.ExtensionMethods
{
    public static class ListExtensions
    {
        /// <summary>
        /// Finds the middle value of a list of doubles. Note, this is not the median, since the values are not ordered beforehand.
        /// </summary>
        public static double Middle(this List<double> listOfElements)
        {
            var numberOfElements = listOfElements.Count;
            if (numberOfElements % 2 == 0)
            {
                var lowerMiddleIndex = numberOfElements / 2;
                var upperMiddleIndex = lowerMiddleIndex + 1;

                var lowerMiddleValue = listOfElements[lowerMiddleIndex];
                var upperMiddleValue = listOfElements[upperMiddleIndex];
                var averageValue = (lowerMiddleValue + upperMiddleValue) / 2.0;
                return averageValue;
            }
            else
            {
                var middleIndex = (int) Math.Ceiling(numberOfElements / 2.0);
                return listOfElements[middleIndex];
            }
        }

        /// <summary>
        /// Finds the middle value of a list of integers. Note, this is not the median, since the values are not ordered beforehand.
        /// </summary>
        public static double Middle(this List<int> listOfElements)
        {
            var numberOfElements = listOfElements.Count;
            if (numberOfElements % 2 == 0)
            {
                var lowerMiddleIndex = numberOfElements / 2;
                var upperMiddleIndex = lowerMiddleIndex + 1;

                var lowerMiddleValue = listOfElements[lowerMiddleIndex];
                var upperMiddleValue = listOfElements[upperMiddleIndex];
                var averageValue = (lowerMiddleValue + upperMiddleValue) / 2.0;
                return averageValue;
            }
            else
            {
                var middleIndex = (int) Math.Ceiling(numberOfElements / 2.0);
                return listOfElements[middleIndex];
            }
        }
    }
}
