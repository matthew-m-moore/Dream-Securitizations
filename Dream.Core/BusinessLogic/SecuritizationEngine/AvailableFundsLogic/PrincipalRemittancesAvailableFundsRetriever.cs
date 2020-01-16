using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using System;
using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic
{
    public class PrincipalRemittancesAvailableFundsRetriever : AvailableFundsRetriever
    {
        public double PrincipalAdvanceRate { get; }

        public PrincipalRemittancesAvailableFundsRetriever() : base()
        {
            PrincipalAdvanceRate = 1.0;
        }

        public PrincipalRemittancesAvailableFundsRetriever(double principalAdvanceRate) : base()
        {
            PrincipalAdvanceRate = principalAdvanceRate;
        }

        public override AvailableFundsRetriever Copy()
        {
            return new PrincipalRemittancesAvailableFundsRetriever(PrincipalAdvanceRate);
        }



        public override double RetrieveAvailableFundsForTranche(int monthlyPeriod, AvailableFunds availableFunds, SecuritizationNodeTree securitizationNode)
        {
            // Calculate the total available principal remittances
            var scheduledPrincipalRemittances = availableFunds[monthlyPeriod].AvailablePrincipal;
            var unscheduledPrincipalRemittances = availableFunds[monthlyPeriod].AvailablePrepayments;
            var unscheduledPrincipalRecoveries = availableFunds[monthlyPeriod].AvailablePrincipalRecoveries;

            var scheduledPrincipalRemittancesAvailable = scheduledPrincipalRemittances * PrincipalAdvanceRate;
            var unscheduledPrincipalRemittancesAvailable = unscheduledPrincipalRemittances * PrincipalAdvanceRate;
            var unscheduledPrincipalRecoveriesAvailable = unscheduledPrincipalRecoveries * PrincipalAdvanceRate;

            var totalPrincipalRemittancesAvailable = scheduledPrincipalRemittancesAvailable 
                                                   + unscheduledPrincipalRemittancesAvailable 
                                                   + unscheduledPrincipalRecoveriesAvailable;

            // Calculate the excess dollars of over-collateralization
            var lastMonthlyPeriod = Math.Max(monthlyPeriod - 1, 0);
            var tranchesDictionary = securitizationNode.RetrieveTranchesDictionary();
            var securitizationNodeStartingBalance = tranchesDictionary.Sum(t => t.Value.Tranche.TrancheCashFlows[lastMonthlyPeriod].EndingBalance);
            var securitizationNodeEndingBalnce = securitizationNodeStartingBalance - totalPrincipalRemittancesAvailable;

            var endingCollateralBalance = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].EndingBalance;
            var targetDollarsOfOvercollateralization = (1 - PrincipalAdvanceRate) * endingCollateralBalance;
            var existingDollarsOfOvercollateralization = endingCollateralBalance - securitizationNodeEndingBalnce;

            var excessDollarsOfOvercollateralization = existingDollarsOfOvercollateralization - targetDollarsOfOvercollateralization;

            // If there is no excess, then supplment the available funds target
            if (excessDollarsOfOvercollateralization < 0.0)
            {
                return totalPrincipalRemittancesAvailable - excessDollarsOfOvercollateralization;
            }

            return totalPrincipalRemittancesAvailable;
        }
    }
}
