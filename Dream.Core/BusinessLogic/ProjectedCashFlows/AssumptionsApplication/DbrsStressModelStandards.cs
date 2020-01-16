using Dream.Core.BusinessLogic.Containers.CashFlows;
using System;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows.AssumptionsApplication
{
    public class DbrsStressModelStandards
    {
        private double _accruedInterest { get; set; }

        private double _deliquencyCurveScalingFactor;
        private double _originalStartingBalance;

        public DbrsStressModelStandards(
            double accruedInterest,  
            double originalStartingBalance,
            double delinquencyCurveScalingFactor)
        {
            _accruedInterest = accruedInterest;       
            _originalStartingBalance = originalStartingBalance;
            _deliquencyCurveScalingFactor = delinquencyCurveScalingFactor;
        }

        public ProjectedCashFlow Apply(
            ContractualCashFlow contractualCashFlow,
            double startingProjectedBalanceFactor,
            double singleMonthlyMortality,
            double monthlyDelinquencyRate,
            out double endingProjectedBalanceFactor)
        {
            if (contractualCashFlow.StartingBalance <= 0)
            {
                endingProjectedBalanceFactor = 0.0;
                return new ProjectedCashFlow(contractualCashFlow);
            }

            var projectedStartingBalance = contractualCashFlow.StartingBalance * startingProjectedBalanceFactor;

            // Deliquencies are assumed to come prior to scheduled payments or prepayments, defaulted payments are handled separately
            var projectedDelinquency = _originalStartingBalance * monthlyDelinquencyRate * _deliquencyCurveScalingFactor;
            if (projectedDelinquency > projectedStartingBalance) projectedDelinquency = projectedStartingBalance;

            // Contractually scheduled principal payments come after all delinquency and default
            var projectedBalanceNetOfDeliquency = projectedStartingBalance - projectedDelinquency;
            var contractualBalanceFactor = 1.0 - (contractualCashFlow.EndingBalance / contractualCashFlow.StartingBalance);
            var contractualPrincipal = projectedBalanceNetOfDeliquency * contractualBalanceFactor;

            // Prepayments are assumed to occur after all contractually scheduled principal payments have been made
            var contractualEndingBalance = contractualCashFlow.EndingBalance * startingProjectedBalanceFactor;
            var projectedPrepayment = contractualEndingBalance * singleMonthlyMortality;
            if (projectedPrepayment > projectedBalanceNetOfDeliquency - contractualPrincipal)
                projectedPrepayment = projectedBalanceNetOfDeliquency - contractualPrincipal;

            // If the starting balance is zero, there is nothing to be delinquent
            var delinquentBalanceFactor = 0.0;
            if (projectedStartingBalance > 0.0) delinquentBalanceFactor = 1 - (projectedDelinquency / projectedStartingBalance);

            var decreasesToBalance = contractualPrincipal + projectedDelinquency + projectedPrepayment;
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
            };
        }
    }
}
