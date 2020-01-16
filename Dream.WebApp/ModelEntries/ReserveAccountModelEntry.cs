using System;
using System.Collections.Generic;

namespace Dream.WebApp.ModelEntries
{
    public class ReserveAccountModelEntry
    {
        public int? ReserveAccountId { get; set; }
        public string ReserveAccountName { get; set; }
        public int SecuritizationNodeId { get; set; }
        public string SecuritizationNodeName { get; set; }
        public double? InitialAccountBalanceInDollars { get; set; }
        public double? InitialAccountBalanceAsPercentage { get; set; }
        public DateTime FirstReserveAccountDrawOrDepositDate { get; set; }
        public int ReserveAccountDrawOrDepositFrequencyInMonths { get; set; }
        public string AvailableFundsRetrieverDescription { get; set; }
        public List<BalanceCapOrFloorEntry> BalanceCapOrFloorEntries { get; set; }
    }
}