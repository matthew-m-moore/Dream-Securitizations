using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Excel
{
    public class DayCountConventionExcelConverter
    {
        private const string _thirty360 = "30/360";
        private const string _actual360 = "Act/360";
        private const string _actual365 = "Act/365";
        private const string _actualActual = "Act/Act-ISDA";

        public static DayCountConvention ConvertString(string dayCountConventionText)
        {
            if (dayCountConventionText == null) return default(DayCountConvention);

            switch (dayCountConventionText)
            {
                case _thirty360:
                    return DayCountConvention.Thirty360;

                case _actual360:
                    return DayCountConvention.Actual360;

                case _actual365:
                    return DayCountConvention.Actual365;

                case _actualActual:
                    return DayCountConvention.ActualActual;

                default:
                    throw new Exception(string.Format("ERROR: The day-counting convention '{0}' is not supported", dayCountConventionText));
            }
        }
    }
}
