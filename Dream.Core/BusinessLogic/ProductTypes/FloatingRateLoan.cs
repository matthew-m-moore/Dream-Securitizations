using Dream.Core.Interfaces;
using System;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.LoanStages;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Common;

namespace Dream.Core.BusinessLogic.ProductTypes
{
    public class FloatingRateLoan : Loan, IGeneratesContractualCashFlows
    {
        public double LifeCap { get; set; }
        public double LifeFloor { get; set; }
        public int LookbackDays { get; set; }
        public int LookbackMonths => (int) Math.Round(((double) LookbackDays) / Constants.ThirtyDaysInOneMonth, 0);
        public int MonthsToNextRateReset { get; set; }
        public int InterestRateResetFrequencyInMonths { get; set; }
        public MarketRateEnvironment RateEnvironment { get; set; }

        public FloatingRateLoan(FloatingRateLoan floatingRateLoan) : base(floatingRateLoan)
        {
            LifeCap = floatingRateLoan.LifeCap;
            LifeFloor = floatingRateLoan.LifeFloor;

            LookbackDays = floatingRateLoan.LookbackDays;
            MonthsToNextRateReset = floatingRateLoan.MonthsToNextRateReset;
            InterestRateResetFrequencyInMonths = floatingRateLoan.InterestRateResetFrequencyInMonths;

            RateEnvironment = floatingRateLoan.RateEnvironment;
        }

        public override List<ContractualCashFlow> GetContractualCashFlows()
        {
            var loanStages = GenerateLoanStages();

            var contractualCashFlows = new List<ContractualCashFlow>();
            foreach(var loanStage in loanStages)
            {
                var loanStageCashFlows = loanStage.CalculateScheduledPayments();
                contractualCashFlows.AddRange(loanStageCashFlows);
            }

            return contractualCashFlows;
        }

        protected override List<LoanStage> GenerateLoanStages()
        {
            var loanStages = new List<LoanStage>();

            var fixedRateLoanStage = new FloatingRateAmortizingLoanStage();
            loanStages.Add(fixedRateLoanStage);

            return loanStages;
        }

        public override ContractualCashFlow PrepareZerothPeriodCashFlow()
        {
            return new ContractualCashFlow
            {
                StartingBalance = Balance,
                EndingBalance = Balance,

                Period = 0,
                PeriodDate = StartDate
            };
        }

        public override Loan Copy()
        {
            return new FloatingRateLoan(this);
        }
    }
}
