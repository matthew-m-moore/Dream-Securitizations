using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.LoanStages
{
    /// <summary>
    /// Typically, an introductory stage of a loan wherein no payment is due, but interest might be accrued.
    /// If the borrower pays off prior to the end of this stage, any interest accrued is forgiven.
    /// When the borrower makes it to the end of this stage and starts fully amortizing, the accrued interest
    /// is capitalized into the balance of the loan.
    /// </summary>
    public class NoPaymentPayoffOptionLoanStage : InterestOnlyLoanStage
    {
        public override List<ContractualCashFlow> CalculateScheduledPayments()
        {
            throw new NotImplementedException();
        }
    }
}
