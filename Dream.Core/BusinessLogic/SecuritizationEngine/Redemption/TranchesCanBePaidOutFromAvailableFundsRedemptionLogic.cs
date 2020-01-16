using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Redemption
{
    public class TranchesCanBePaidOutFromAvailableFundsRedemptionLogic : RedemptionLogic
    {
        public List<Tranche> ListOfTranchesToBePaidOut { get; set; }

        public TranchesCanBePaidOutFromAvailableFundsRedemptionLogic() : base()
        {
            ListOfTranchesToBePaidOut = new List<Tranche>();
        }

        private TranchesCanBePaidOutFromAvailableFundsRedemptionLogic(TranchesCanBePaidOutFromAvailableFundsRedemptionLogic redemptionLogic)
            : base(redemptionLogic)
        {
            ListOfTranchesToBePaidOut = new List<Tranche>();
        }

        public override RedemptionLogic Copy()
        {       
            return new TranchesCanBePaidOutFromAvailableFundsRedemptionLogic(this)
            {
                PriorityOfPayments = PriorityOfPayments.Copy(),
                PostRedemptionPriorityOfPayments = PostRedemptionPriorityOfPayments.Copy(),
                TreatAsCleanUpCall = TreatAsCleanUpCall,

                // Note, there is special copy logic for the ListOfTranchesToBePaidOut that will be called later, but this line is important for that to work
                ListOfTranchesToBePaidOut = ListOfTranchesToBePaidOut.ToList()
            };
        }

        public override bool IsRedemptionTriggered(int monthlyPeriod)
        {
            if (CheckAllowedIfMonthIsNotAllowed(monthlyPeriod)) return false;

            // Need to loop through and get the accrued interest or accrued payment for all of these tranches
            var totalPayOutAmount = 0.0;
            foreach (var tranche in ListOfTranchesToBePaidOut)
            {
                tranche.SetIsFinalPeriod(true);
                var balanceDue = tranche.TrancheCashFlows[monthlyPeriod].StartingBalance;
                var interestDue = tranche.TrancheCashFlows[monthlyPeriod].Interest;
                totalPayOutAmount += (balanceDue + interestDue);
                
                // If interest was not due, make sure to get all the previously accrued interest
                var interestPayingTranche = tranche as InterestPayingTranche;
                if (interestPayingTranche != null && interestDue <= 0.0)
                {
                    var accruedInterest = interestPayingTranche.CalculatePreviouslyAccruedInterest(monthlyPeriod);
                    totalPayOutAmount += accruedInterest;
                }

                var feeTranche = tranche as FeeTranche;
                if (feeTranche != null)
                {
                    // If principal was not due, make sure to get all the previously accrued payments
                    // Note, this also handles payments due for tranches with zero balance
                    var feePayment = feeTranche.TrancheCashFlows[monthlyPeriod].Principal;
                    if (feePayment <= 0.0)
                    {
                        var accruedPayments = tranche.CalculatePreviouslyAccruedPayments(monthlyPeriod);
                        totalPayOutAmount += accruedPayments;
                    }
                    totalPayOutAmount += feePayment;
                }

                tranche.SetIsFinalPeriod(false);
            }

            var reserveFunds = AvailableFunds[monthlyPeriod].AvailableReserveFundsDictionary;
            var totalReserveFunds = reserveFunds.Sum(r => r.Value.FundStartingBalance);
            var netCollectionFromCollateral = AvailableFunds[monthlyPeriod].Payment;

            var totalFundsAvailable = netCollectionFromCollateral + totalReserveFunds;
            var isRedemptionTriggered = totalFundsAvailable > totalPayOutAmount;

            return isRedemptionTriggered;
        }

        protected override void AdjustFinalPeriodStatusOfTranche(Tranche securitizationTranche)
        {
            if (ListOfTranchesToBePaidOut.Any(t => t.TrancheName == securitizationTranche.TrancheName))
            {
                securitizationTranche.SetIsFinalPeriod(true); 
            }
        }
    }
}
