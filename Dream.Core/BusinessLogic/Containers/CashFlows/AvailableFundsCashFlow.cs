using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Containers.CashFlows
{
    public class AvailableFundsCashFlow : CashFlow
    {
        public double AvailablePrincipal { get; set; }
        public double AvailableInterest { get; set; }
        public double AvailablePrepaymentInterest { get; set; }
        public double AvailablePrepayments { get; set; }
        public double AvailablePrepaymentPenalties { get; set; }
        public double AvailablePrincipalRecoveries { get; set; }
        public double AvailableInterestRecoveries { get; set; }

        public double TotalAvailableFunds { get; set; }

        // The key of this dictionary is the name of the reserve account
        public Dictionary<string, ReserveFund> AvailableReserveFundsDictionary { get; set; }

        public override double Payment =>
            + AvailablePrincipal
            + AvailableInterest
            + AvailablePrepaymentInterest
            + AvailablePrepayments
            + AvailablePrepaymentPenalties
            + AvailablePrincipalRecoveries
            + AvailableInterestRecoveries;

        public double ReservesReleased =>
            AvailableReserveFundsDictionary.Values.Sum(f => f.ReservesReleased);

        public AvailableFundsCashFlow() : base() { }

        public AvailableFundsCashFlow(CashFlow cashFlow) : base(cashFlow) { }

        public AvailableFundsCashFlow(AvailableFundsCashFlow cashFlow) : base(cashFlow)
        {
            BondCount = cashFlow.BondCount;

            AvailablePrincipal = cashFlow.AvailablePrincipal;
            AvailableInterest = cashFlow.AvailableInterest;
            AvailablePrepaymentInterest = cashFlow.AvailablePrepaymentInterest;
            AvailablePrepayments = cashFlow.AvailablePrepayments;
            AvailablePrepaymentPenalties = cashFlow.AvailablePrepaymentPenalties;
            AvailablePrincipalRecoveries = cashFlow.AvailablePrincipalRecoveries;
            AvailableInterestRecoveries = cashFlow.AvailableInterestRecoveries;

            // The priority rank of each reserve fund will match the seniority rank of the
            // ReserveFundTranche it is pair with, which will be enforced as unique
            AvailableReserveFundsDictionary = cashFlow.AvailableReserveFundsDictionary
                .Select(kvp => new ReserveFund(kvp.Value))
                .ToDictionary(key => key.Name, value => value);
        }

        public override CashFlow Copy()
        {
            return new AvailableFundsCashFlow(this);
        }

        public override void Clear()
        {
            TotalAvailableFunds = Payment;

            AvailablePrincipal = 0.0;
            AvailableInterest = 0.0;
            AvailablePrepaymentInterest = 0.0;
            AvailablePrepayments = 0.0;
            AvailablePrepaymentPenalties = 0.0;
            AvailablePrincipalRecoveries = 0.0;
            AvailableInterestRecoveries = 0.0;

            AvailableReserveFundsDictionary.Clear();
        }


        public override void Scale(double scaleFactor)
        {
            Count *= scaleFactor;
            BondCount *= scaleFactor;

            AvailablePrincipal *= scaleFactor;
            AvailableInterest *= scaleFactor;
            AvailablePrepaymentInterest *= scaleFactor;
            AvailablePrepayments *= scaleFactor;
            AvailablePrepaymentPenalties *= scaleFactor;
            AvailablePrincipalRecoveries *= scaleFactor;
            AvailableInterestRecoveries *= scaleFactor;

            foreach (var reserveFund in AvailableReserveFundsDictionary.Values)
            {
                reserveFund.FundStartingBalance *= scaleFactor;
                reserveFund.FundEndingBalance *= scaleFactor;
                reserveFund.ReservesReleased *= scaleFactor;
                reserveFund.ShortfallAbsorbed *= scaleFactor;
            }
        }

        public override void Aggregate(CashFlow cashFlow)
        {
            var availableFundsCashFlow = cashFlow as AvailableFundsCashFlow;

            Count += availableFundsCashFlow.Count;
            BondCount += availableFundsCashFlow.BondCount;

            AvailablePrincipal += availableFundsCashFlow.AvailablePrincipal;
            AvailableInterest += availableFundsCashFlow.AvailableInterest;
            AvailablePrepaymentInterest += availableFundsCashFlow.AvailablePrepaymentInterest;
            AvailablePrepayments += availableFundsCashFlow.AvailablePrepayments;
            AvailablePrepaymentPenalties += availableFundsCashFlow.AvailablePrepaymentPenalties;
            AvailablePrincipalRecoveries += availableFundsCashFlow.AvailablePrincipalRecoveries;
            AvailableInterestRecoveries += availableFundsCashFlow.AvailableInterestRecoveries;

            // Find all the reserve funds in the cash flow passed in
            foreach (var reserveFund in availableFundsCashFlow.AvailableReserveFundsDictionary.Values)
            {
                // If there is an equivalent priority rank fund, aggregate balances into it
                if (AvailableReserveFundsDictionary.ContainsKey(reserveFund.Name))
                {
                    AvailableReserveFundsDictionary[reserveFund.Name].FundStartingBalance += reserveFund.FundStartingBalance;
                    AvailableReserveFundsDictionary[reserveFund.Name].FundEndingBalance += reserveFund.FundEndingBalance;
                    AvailableReserveFundsDictionary[reserveFund.Name].ReservesReleased += reserveFund.ReservesReleased;
                    AvailableReserveFundsDictionary[reserveFund.Name].ShortfallAbsorbed += reserveFund.ShortfallAbsorbed;
                }
                // Otherwise add a new entry to the dictionary for the missing reserve fund
                else
                {
                    AvailableReserveFundsDictionary.Add(reserveFund.Name, new ReserveFund(reserveFund));
                }
            }
        }
    }
}
