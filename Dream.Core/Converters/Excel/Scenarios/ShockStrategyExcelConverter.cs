using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Excel.Scenarios
{
    public class ShockStrategyExcelConverter
    {
        private const string _replacement = "Replace Value";
        private const string _additive = "Add To Value";
        private const string _multiplicative = "Multiply By Value";

        public static ShockStrategy ConvertString(string shockStrategyText)
        {
            if (shockStrategyText == null) return default(ShockStrategy);

            switch (shockStrategyText)
            {
                case _replacement:
                    return ShockStrategy.Replacement;

                case _additive:
                    return ShockStrategy.Additive;

                case _multiplicative:
                    return ShockStrategy.Multiplicative;

                default:
                    throw new Exception(string.Format("ERROR: The shock strategy '{0}' is not supported", shockStrategyText));
            }
        }
    }
}
