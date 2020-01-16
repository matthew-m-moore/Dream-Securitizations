using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database
{
    public class DayCountConventionDatabaseConverter
    {
        private const string _thirtyThreeSixty = "30/360";
        private const string _actualThreeSixty = "Act/360";
        private const string _actualThreeSixtyFive = "Act/365";
        private const string _actualActualIsda = "Act/Act-ISDA";

        public static DayCountConvention ConvertString(string dayCountConventionText)
        {
            if (dayCountConventionText == null) return default(DayCountConvention);

            switch (dayCountConventionText)
            {
                case _thirtyThreeSixty:
                    return DayCountConvention.Thirty360;

                case _actualThreeSixty:
                    return DayCountConvention.Actual360;

                case _actualThreeSixtyFive:
                    return DayCountConvention.Actual365;

                case _actualActualIsda:
                    return DayCountConvention.ActualActual;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The day-counting convention '{0}' is not supported. Please report this error.",
                        dayCountConventionText));
            }
        }

        public static string ConvertToDescription(DayCountConvention dayCountConvention)
        {
            switch (dayCountConvention)
            {
                case DayCountConvention.Thirty360:
                    return _thirtyThreeSixty;

                case DayCountConvention.Actual360:
                    return _actualThreeSixty;

                case DayCountConvention.Actual365:
                    return _actualThreeSixtyFive;

                case DayCountConvention.ActualActual:
                    return _actualActualIsda;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The day-counting convention '{0}' is not supported. Please report this error.",
                        dayCountConvention));
            }
        }
    }
}
