using System;

namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class ReserveAccountRecord
    {
        public string ReserveAccountName { get; set; }
        public DateTime PrepayIntCollectionStartDate { get; set; }
        public int? DepositOrReleaseFrequencyInMonths { get; set; }
        public double? InitialAccountBalance { get; set; }      

        public double? BalanceCaps { get; set; }
        public DateTime? BalanceCapsEffectiveDates { get; set; }

        public double? BalanceFloors { get; set; }
        public DateTime? BalanceFloorsEffectiveDates { get; set; }
    }
}
