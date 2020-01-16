using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.LoanStages;
using Dream.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.ProductTypes
{
    public class FixedFloatingHybridLoan : FloatingRateLoan, IGeneratesContractualCashFlows
    {
        public int FixedRateTermInMonths { get; set; }

        public FixedFloatingHybridLoan(FixedFloatingHybridLoan hybridLoan) : base(hybridLoan)
        {
            FixedRateTermInMonths = hybridLoan.FixedRateTermInMonths;
        }

        public override List<ContractualCashFlow> GetContractualCashFlows()
        {
            var loanStages = GenerateLoanStages();

            var contractualCashFlows = new List<ContractualCashFlow>();
            foreach (var loanStage in loanStages)
            {
                var loanStageCashFlows = loanStage.CalculateScheduledPayments();
                contractualCashFlows.AddRange(loanStageCashFlows);
            }

            return contractualCashFlows;
        }

        protected override List<LoanStage> GenerateLoanStages()
        {
            throw new NotImplementedException();
        }

        public override Loan Copy()
        {
            return new FixedFloatingHybridLoan(this);
        }
    }
}
