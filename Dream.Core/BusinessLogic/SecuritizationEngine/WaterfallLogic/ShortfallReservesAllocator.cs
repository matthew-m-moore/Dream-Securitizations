using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Containers.CompoundKeys;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic
{
    public class ShortfallReservesAllocator
    {
        public Dictionary<ShortfallReservesAllocationEntry, double> ShortfallReservesAllocationDictionary { get; private set; }
        public List<(string AccountName, TrancheCashFlowType CashFlowType)> ListOfAssociatedReserveAccounts { get; private set; }

        public TrancheCashFlowType TrancheCashFlowType { get; set; }   
        public string SecuritizationNodeName { get; private set; }
        public bool IsProRataDistributionRule { get; private set; }
        public bool IsShortfallPaidFromReserves { get; private set; }
        
        public ShortfallReservesAllocator()
        {
            ShortfallReservesAllocationDictionary = new Dictionary<ShortfallReservesAllocationEntry, double>();
        }

        public void SetTrancheSpecificInformation(Tranche tranche)
        {
            ListOfAssociatedReserveAccounts = tranche.ListOfAssociatedReserveAccounts;
            SecuritizationNodeName = tranche.SecuritizationNode.SecuritizationNodeName;
            IsProRataDistributionRule = tranche.SecuritizationNode.AvailableFundsDistributionRule is ProRataDistributionRule;
            IsShortfallPaidFromReserves = tranche.IsShortfallPaidFromReserves;
        }

        public AmountPayable AllocateReservesToCoverShortfall(
            TrancheCashFlowType cashFlowType,
            Dictionary<string, ReserveFund> reserveFunds,
            AvailableFunds availableFunds,
            double appliedProportionToDistribute,
            double amountDue,
            double amountPayable)
        {
            var amountOfShortfall = amountDue - amountPayable;
            if (amountPayable < amountDue &&
                reserveFunds.Any(r => ListOfAssociatedReserveAccounts.Any(a => a.AccountName == r.Key && a.CashFlowType == cashFlowType)))
            {
                // Note, there is also an assumption here that reserves released are pulled before the remaining balance of
                // any reserve funds is touched
                var totalReservesReleased = availableFunds.GetAllAvailableReservesReleased();
                var copiedListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts
                    .Where(a => a.CashFlowType == cashFlowType).Select(r => new string(r.AccountName.ToCharArray())).ToList();

                while (totalReservesReleased > 0.0 && amountOfShortfall > 0.0 && copiedListOfAssociatedReserveAccounts.Any())
                {
                    var firstReserveFundWithReservesReleased =
                        availableFunds.GetFirstAvailableReservesReleased(copiedListOfAssociatedReserveAccounts);

                    if (firstReserveFundWithReservesReleased == null) break;

                    var shortfallReservesAllocationEntry = new ShortfallReservesAllocationEntry
                        (SecuritizationNodeName, firstReserveFundWithReservesReleased.Name, TrancheCashFlowType.ReservesReleased, TrancheCashFlowType);

                    var remainingReservesReleased = firstReserveFundWithReservesReleased.ReservesReleased;
                    if (!ShortfallReservesAllocationDictionary.ContainsKey(shortfallReservesAllocationEntry))
                    {
                        ShortfallReservesAllocationDictionary.Add(shortfallReservesAllocationEntry, remainingReservesReleased);
                    }

                    var reservesReleasedPayable = IsProRataDistributionRule 
                        ? ShortfallReservesAllocationDictionary[shortfallReservesAllocationEntry]
                        : remainingReservesReleased;

                    amountPayable += Math.Min((reservesReleasedPayable * appliedProportionToDistribute), remainingReservesReleased);
                    totalReservesReleased -= reservesReleasedPayable;

                    firstReserveFundWithReservesReleased.ReservesReleased -= amountPayable;
                    amountOfShortfall = amountDue - amountPayable;

                    copiedListOfAssociatedReserveAccounts =
                        copiedListOfAssociatedReserveAccounts.Where(r => r != firstReserveFundWithReservesReleased.Name).ToList();
                }

                // Note, this logic assumes that the lowest priority reserve fund (if there are multiple), is the one that funds
                // are drawn from first, but that may not always be the case. The dictionary of reserve funds or the reserve
                // funds object could be better fleshed out to handle that possibility.
                copiedListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts
                    .Where(a => a.CashFlowType == cashFlowType).Select(r => new string(r.AccountName.ToCharArray())).ToList();

                while (reserveFunds.Where(AvailableForShortfall)
                    .Any(r => r.Value.FundEndingBalance > 0.0 && copiedListOfAssociatedReserveAccounts.Contains(r.Key))
                       && amountOfShortfall > 0.0)
                {
                    var lastReserveFundWithBalance = reserveFunds
                        .OrderBy(f => f.Value.PriorityRank)
                        .Where(AvailableForShortfall)
                        .Where(k => copiedListOfAssociatedReserveAccounts.Contains(k.Key))
                        .Last(r => r.Value.FundEndingBalance > 0.0).Value;

                    var shortfallReservesAllocationEntry = new ShortfallReservesAllocationEntry
                        (SecuritizationNodeName, lastReserveFundWithBalance.Name, TrancheCashFlowType.Reserves, TrancheCashFlowType);

                    if (!ShortfallReservesAllocationDictionary.ContainsKey(shortfallReservesAllocationEntry))
                    {
                        ShortfallReservesAllocationDictionary.Add(shortfallReservesAllocationEntry, lastReserveFundWithBalance.FundEndingBalance);
                    }

                    var endingReserveFundBalance = IsProRataDistributionRule
                        ? ShortfallReservesAllocationDictionary[shortfallReservesAllocationEntry]
                        : lastReserveFundWithBalance.FundEndingBalance;
                    
                    var reserveFundAmountPayable = ((endingReserveFundBalance * appliedProportionToDistribute) < amountOfShortfall)
                        ? endingReserveFundBalance * appliedProportionToDistribute
                        : amountOfShortfall;

                    amountPayable += reserveFundAmountPayable;
                    reserveFunds[lastReserveFundWithBalance.Name].FundEndingBalance -= reserveFundAmountPayable;
                    reserveFunds[lastReserveFundWithBalance.Name].ShortfallAbsorbed = reserveFundAmountPayable;

                    amountOfShortfall = amountDue - amountPayable;

                    copiedListOfAssociatedReserveAccounts =
                        copiedListOfAssociatedReserveAccounts.Where(r => r != lastReserveFundWithBalance.Name).ToList();
                }
            }

            return new AmountPayable(amountPayable, amountOfShortfall);
        }

        private bool AvailableForShortfall(KeyValuePair<string, ReserveFund> reserveFundKeyValuePair)
        {
            if (IsShortfallPaidFromReserves && reserveFundKeyValuePair.Value.IsReserveTranche) return true;
            if (!reserveFundKeyValuePair.Value.IsReserveTranche) return true;
            return false;
        }
    }
}
