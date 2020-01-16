using System;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.LoanStages;

namespace Dream.Core.BusinessLogic.ProductTypes
{
    public class BenjiLoan : Loan
    {
        protected override List<LoanStage> GenerateLoanStages()
        {
            throw new NotImplementedException();
        }

        public override List<ContractualCashFlow> GetContractualCashFlows()
        {
            throw new NotImplementedException();
        }

        public override ContractualCashFlow PrepareZerothPeriodCashFlow()
        {
            throw new NotImplementedException();
        }

        public override Loan Copy()
        {
            throw new NotImplementedException();
        }
    }
}
