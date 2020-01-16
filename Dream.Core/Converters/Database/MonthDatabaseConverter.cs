using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database
{
    public class MonthDatabaseConverter
    {
        private const string _january = "January";
        private const string _february = "February";
        private const string _march = "March";
        private const string _april = "April";
        private const string _may = "May";
        private const string _june = "June";
        private const string _july = "July";
        private const string _august = "August";
        private const string _september = "September";
        private const string _october = "October";
        private const string _november = "November";
        private const string _december = "December";

        public static Month ConvertString(string monthText)
        {
            if (monthText == null) return default(Month);

            switch (monthText)
            {
                case _january:
                    return Month.January;

                case _february:
                    return Month.February;

                case _march:
                    return Month.March;

                case _april:
                    return Month.April;

                case _may:
                    return Month.May;

                case _june:
                    return Month.June;

                case _july:
                    return Month.July;

                case _august:
                    return Month.August;

                case _september:
                    return Month.September;

                case _october:
                    return Month.October;

                case _november:
                    return Month.November;

                case _december:
                    return Month.December;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The month '{0}' is not supported. Please report this error.",
                        monthText));
            }
        }
    }
}
