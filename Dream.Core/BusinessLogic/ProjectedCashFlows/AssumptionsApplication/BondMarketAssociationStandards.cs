using Dream.Core.BusinessLogic.Containers.CashFlows;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows.AssumptionsApplication
{
    /// <summary>
    /// The logic below is based off the Bond Market Association "Standard Formulas for the Analysis of 
    /// Mortgage-Backed Securities and Other Related Securities"
    /// http://www.sifma.org/uploadedfiles/services/standard_forms_and_documentation/chsf.pdf?n=65400
    /// </summary>
    public class BondMarketAssociationStandards
    {
        private double _accruedInterest { get; set; }

        public BondMarketAssociationStandards(double accruedInterest)
        {
            _accruedInterest = accruedInterest;
        }

        public ProjectedCashFlow Apply(
            ContractualCashFlow contractualCashFlow,
            double startingProjectedBalanceFactor,
            double singleMonthlyMortality,
            double monthlyDelinquencyRate,
            double monthlyDefaultRate,
            double lossGivenDefault,
            out double endingProjectedBalanceFactor)
        {
            if (contractualCashFlow.StartingBalance <= 0)
            {
                endingProjectedBalanceFactor = 0.0;
                return new ProjectedCashFlow(contractualCashFlow);
            }

            var projectedStartingBalance = contractualCashFlow.StartingBalance * startingProjectedBalanceFactor;           

            // Defaults are assumed to come prior to scheduled payments or prepayments
            var projectedDefault = projectedStartingBalance * monthlyDefaultRate;
            var projectedLoss = projectedDefault * lossGivenDefault;
            var projectedRecovery = projectedDefault - projectedLoss;

            // Deliquencies are assumed to come after defaults, but prior to scheduled payments or prepayments
            var projectedBalanceNetOfDefault = projectedStartingBalance - projectedDefault;
            var projectedDelinquency = projectedBalanceNetOfDefault * monthlyDelinquencyRate;

            // Contractually scheduled principal payments come after all delinquency and default
            var projectedBalanceNetOfDeliquencyAndDefault = projectedBalanceNetOfDefault - projectedDelinquency;
            var contractualBalanceFactor = 1.0 - (contractualCashFlow.EndingBalance / contractualCashFlow.StartingBalance);
            var contractualPrincipal = projectedBalanceNetOfDeliquencyAndDefault * contractualBalanceFactor;

            // Prepayments are assumed to occur after all contractually scheduled principal payments have been made
            var contractualEndingBalance = contractualCashFlow.EndingBalance * startingProjectedBalanceFactor;
            var projectedPrepayment = contractualEndingBalance * singleMonthlyMortality;
            if (projectedPrepayment > projectedBalanceNetOfDeliquencyAndDefault - contractualPrincipal)
                projectedPrepayment = projectedBalanceNetOfDeliquencyAndDefault - contractualPrincipal;

            var defaultedAndDelinquentBalanceFactor = 1 - (monthlyDefaultRate + monthlyDelinquencyRate);

            var decreasesToBalance = contractualPrincipal + projectedDefault + projectedDelinquency + projectedPrepayment;
            var projectedEndingBalance = projectedStartingBalance - decreasesToBalance;

            // If the ending contractual balance is zero, avoid the divide by zero double.NaN creation
            endingProjectedBalanceFactor = contractualCashFlow.EndingBalance > 0
                ? projectedEndingBalance / contractualCashFlow.EndingBalance
                : 0.0;

            // Interest is assumed not to be accrued on defaults or delinquencies here
            // Accrual of delinquent interest on deliquent or defaulted principal would be handled elsewhere
            var projectedAccruedInterest = contractualCashFlow.AccruedInterest * endingProjectedBalanceFactor;

            // In the case where interest does not pay monthly, this logic will capture the appropriate interest to be paid
            var projectedInterest = 0.0;
            if (contractualCashFlow.Interest > 0.0)
            {
                projectedInterest = _accruedInterest;
                _accruedInterest = projectedAccruedInterest;
            }
            else
            {
                _accruedInterest += projectedAccruedInterest;
            }

            return new ProjectedCashFlow(contractualCashFlow)
            {
                StartingBalance = projectedStartingBalance,
                EndingBalance = projectedEndingBalance,

                DelinquentPrincipal = projectedDelinquency,
                Principal = contractualPrincipal,
                Interest = projectedInterest,
                AccruedInterest = projectedAccruedInterest,
                
                Prepayment = projectedPrepayment,

                Default = projectedDefault,
                Loss = projectedLoss,
                Recovery = projectedRecovery
            };
        }
    }
}
