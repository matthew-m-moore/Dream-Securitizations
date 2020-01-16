using Dream.Common.Enums;
using System;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class TrancheCashFlowTypeExcelConverter
    {
        public static TrancheCashFlowType ConvertString(string trancheCashFlowTypeText)
        {
            if (trancheCashFlowTypeText == null) return default(TrancheCashFlowType);

            return TryCasteTrancheCashFlowType(trancheCashFlowTypeText);
        }

        private static TrancheCashFlowType TryCasteTrancheCashFlowType(string trancheCashFlowTypeText)
        {
            try
            {
                var trancheCashFlowType = (TrancheCashFlowType)Enum.Parse(typeof(TrancheCashFlowType), trancheCashFlowTypeText);
                return trancheCashFlowType;
            }
            catch
            {
                throw new Exception(string.Format("ERROR: The tranche cash flow type '{0}' is not supported", trancheCashFlowTypeText));
            }
        }
    }
}
