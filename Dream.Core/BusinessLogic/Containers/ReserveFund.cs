namespace Dream.Core.BusinessLogic.Containers
{
    public class ReserveFund
    {
        public string Name { get; }
        public int PriorityRank { get; }
        public bool IsReserveTranche { get; }

        public double FundStartingBalance { get; set; }
        public double FundEndingBalance { get; set; }
        public double NetChangeInReserveFundBalance => FundEndingBalance - FundStartingBalance;

        public double ReservesReleased { get; set; }
        public double ShortfallAbsorbed { get; set; }
        public bool IsFundBalanceClearedOutForRedemption { get; set; }

        public ReserveFund(ReserveFund reserveFund)
        {
            Name = reserveFund.Name;
            PriorityRank = reserveFund.PriorityRank;
            IsReserveTranche = reserveFund.IsReserveTranche;

            FundStartingBalance = reserveFund.FundStartingBalance;
            FundEndingBalance = reserveFund.FundEndingBalance;

            ReservesReleased = reserveFund.ReservesReleased;
            ShortfallAbsorbed = reserveFund.ShortfallAbsorbed;
            IsFundBalanceClearedOutForRedemption = reserveFund.IsFundBalanceClearedOutForRedemption;
        }

        public ReserveFund(string name, int priorityRank, bool isReserveTranche, double startingBalance, double endingBalance)
        {
            Name = name;
            PriorityRank = priorityRank;
            IsReserveTranche = isReserveTranche;

            FundStartingBalance = startingBalance;
            FundEndingBalance = endingBalance;

            ReservesReleased = 0.0;
            ShortfallAbsorbed = 0.0;
            IsFundBalanceClearedOutForRedemption = false;
        }
    }
}
