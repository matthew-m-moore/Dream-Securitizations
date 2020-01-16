using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Database
{
    public class CompoundingConventionDatabaseConverter
    {
        private const string _simple = "Simple";
        private const string _annual = "Annual";
        private const string _semiannual = "Semi-Annual";
        private const string _quarterly = "Quarterly";
        private const string _monthly = "Monthly";
        private const string _daily = "Daily";
        private const string _continuous = "Continuous";

        public static CompoundingConvention ConvertString(string compoundingConventionText)
        {
            if (compoundingConventionText == null) return default(CompoundingConvention);

            switch (compoundingConventionText)
            {
                case _simple:
                    return CompoundingConvention.Simply;

                case _annual:
                    return CompoundingConvention.Annually;

                case _semiannual:
                    return CompoundingConvention.SemiAnnually;

                case _quarterly:
                    return CompoundingConvention.Quarterly;

                case _monthly:
                    return CompoundingConvention.Monthly;

                case _daily:
                    return CompoundingConvention.Daily;

                case _continuous:
                    return CompoundingConvention.Continuously;

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: The compounding convention '{0}' is not supported. Please report this error.",
                        compoundingConventionText));
            }
        }
    }
}
