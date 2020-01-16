using Dream.Core.BusinessLogic.Containers.CashFlows;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.LoanStages
{
    public class NoPaymentLoanStage : InterestOnlyLoanStage
    {
        public override List<ContractualCashFlow> CalculateScheduledPayments()
        {
            throw new NotImplementedException();
        }
    }
}
