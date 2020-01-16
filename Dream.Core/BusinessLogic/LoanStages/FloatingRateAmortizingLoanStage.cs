using System;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.LoanStages
{
    public class FloatingRateAmortizingLoanStage : LoanStage
    {
        public int MaturityTermInMonths { get; set; }
        public double Margin { get; set; }
        public double InterimCap { get; set; }
        public InterestRateCurveType InterestRateIndex { get; set; }

        public override List<ContractualCashFlow> CalculateScheduledPayments()
        {
            throw new NotImplementedException();
        }
    }
}
