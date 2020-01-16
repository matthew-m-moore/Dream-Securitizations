using System;

namespace Dream.Core.Converters.Excel.Collateral
{
    /// <summary>
    /// Contains shared functionality for converting loan entity objects into business objects.
    /// </summary>
    public class LoanExcelConverter
    {
        protected readonly DateTime _CollateralCutOffDate;
        protected readonly DateTime _CashFlowStartDate;

        public LoanExcelConverter(DateTime collateralCutOffDate, DateTime? cashFlowStartDate)
        {
            _CollateralCutOffDate = collateralCutOffDate;
            _CashFlowStartDate = cashFlowStartDate ?? collateralCutOffDate;
        }
    }
}
