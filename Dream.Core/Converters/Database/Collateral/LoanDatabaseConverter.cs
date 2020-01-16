using System;

namespace Dream.Core.Converters.Database.Collateral
{
    /// <summary>
    /// Contains shared functionality for converting loan entity objects into business objects.
    /// </summary>
    public class LoanDatabaseConverter
    {
        protected readonly DateTime _CollateralCutOffDate;
        protected readonly DateTime _CashFlowStartDate;

        public LoanDatabaseConverter(DateTime collateralCutOffDate, DateTime? cashFlowStartDate)
        {
            _CollateralCutOffDate = collateralCutOffDate;
            _CashFlowStartDate = cashFlowStartDate ?? collateralCutOffDate;
        }
    }
}
