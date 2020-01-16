using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Excel
{
    public class CompoundingConventionExcelConverter
    {
        private const string _annual = "Annual";
        private const string _semiAnnual = "Semi-Annual";
        private const string _quarterly = "Quarterly";
        private const string _monthly = "Monthly";

        public static CompoundingConvention ConvertString(string compoundingConventionText)
        {
            if (compoundingConventionText == null) return default(CompoundingConvention);

            switch (compoundingConventionText)
            {
                case _annual:
                    return CompoundingConvention.Annually;

                case _semiAnnual:
                    return CompoundingConvention.SemiAnnually;

                case _quarterly:
                    return CompoundingConvention.Quarterly;

                case _monthly:
                    return CompoundingConvention.Monthly;

                default:
                    throw new Exception(string.Format("ERROR: The compounding convention '{0}' is not supported", compoundingConventionText));
            }
        }
    }
}
