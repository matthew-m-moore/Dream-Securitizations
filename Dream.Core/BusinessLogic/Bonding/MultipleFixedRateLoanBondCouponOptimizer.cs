using Dream.Core.BusinessLogic.Paydown;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;
using System;

namespace Dream.Core.BusinessLogic.Bonding
{
    public class MultipleFixedRateLoanBondCouponOptimizer<T, U> : BondCouponOptimizer<T, U>
        where T : FixedRateLoan
        where U : FixedRateLoanPaydownCalculator
    {
        public MultipleFixedRateLoanBondCouponOptimizer(
            List<PaydownScenario> paydownScenarios,
            List<T> fixedRateLoans, 
            U paydownCalculator, 
            DateTime collateralCutOffDate,
            DateTime bondPaymentStartDate, 
            bool lockBondPrincipalPaydownToLoan,
            bool ignoreFirstPaymentForBondPrincipalPaydown) 
            : base(paydownScenarios, paydownCalculator, collateralCutOffDate, bondPaymentStartDate, 
                   lockBondPrincipalPaydownToLoan, ignoreFirstPaymentForBondPrincipalPaydown)
        { }

        public override double FindOptimalBondCoupon(PaydownScenario paydownScenario, List<ContractualCashFlow> loanContractualCashFlows)
        {
            throw new NotImplementedException();
        }
    }
}
