using System.Linq;
using System.Collections.Generic;
using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.LoanStages;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Common.Utilities;

namespace Dream.Core.BusinessLogic.ProductTypes
{
    public class FixedRateLoan : Loan, IGeneratesContractualCashFlows
    {
        public FixedRateLoan() { }

        public FixedRateLoan(FixedRateLoan fixedRateLoan) : base(fixedRateLoan) { }

        public override List<ContractualCashFlow> GetContractualCashFlows()
        {
            var contractualCashFlows = new List<ContractualCashFlow>();
            var zerothPeriodCashFlow = PrepareZerothPeriodCashFlow();
            contractualCashFlows.Add(zerothPeriodCashFlow);

            var loanStages = GenerateLoanStages();

            foreach (var loanStage in loanStages)
            {
                // Cash flows from subsequent stages may depend on previous stages.
                // Cloning the list is important and necessary due to default pass by reference in C#.
                loanStage.PriorStagesContractualCashFlows =
                    contractualCashFlows.Select(c => new ContractualCashFlow(c)).ToList();

                // Update the starting balance of the stage, if necessary
                if (contractualCashFlows.Any())
                {
                    loanStage.StageStartingBalance = contractualCashFlows.Last().EndingBalance;
                }

                var loanStageCashFlows = loanStage.CalculateScheduledPayments();
                contractualCashFlows.AddRange(loanStageCashFlows);
            }

            return contractualCashFlows;
        }

        protected override List<LoanStage> GenerateLoanStages()
        {
            var loanStages = new List<LoanStage>();

            // Interest accrual for the stage can be bumped one month in this case
            var interestAccrualStartDate = InterestAccrualStartDate;
            if (IsLoanStartSameDateAsInterestAccrualStart)
            {
                interestAccrualStartDate = InterestAccrualStartDate.AddMonths(1);
            }

            var fixedRateFullyAmortizingStage = new FixedRateAmortizingLoanStage
            {
                StageOrderId = 1,
                StageStartMonth = 1,
                IsFinalLoanStage = true,

                StageDurationInMonths = MaturityTermInMonths,
                LoanCoupon = new FixedRateCoupon(InitialCouponRate),

                AmortizationTermInMonths = AmortizationTermInMonths,

                InterestAccrualStartDate = interestAccrualStartDate,
                InterestAccrualDayCountConvention = InterestAccrualDayCountConvention,

                MonthsToNextInterestPayment = MonthsToNextInterestPayment,
                MonthsToNextPrincipalPayment = MonthsToNextPrincipalPayment,
                InterestPaymentFrequencyInMonths = InterestPaymentFrequencyInMonths,
                PrincipalPaymentFrequencyInMonths = PrincipalPaymentFrequencyInMonths, 

                CustomPaymentSchedule = CustomPaymentSchedule,
                ListOfInterestAccrualEndMonths = ListOfInterestAccrualEndMonths,
            };

            loanStages.Add(fixedRateFullyAmortizingStage);

            return loanStages;
        }

        public override ContractualCashFlow PrepareZerothPeriodCashFlow()
        {
            var zerothPeriodAccruedInterest = AccruedInterest;
            if (IsLoanStartSameDateAsInterestAccrualStart)
            {
                var monthlyTimePeriodInYears = DateUtility.CalculateTimePeriodInYearsForOneMonth(
                    InterestAccrualDayCountConvention,
                    InterestAccrualStartDate);

                var accruedInterestFactor = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(
                    monthlyTimePeriodInYears,
                    InitialCouponRate);

                zerothPeriodAccruedInterest += accruedInterestFactor * Balance;
            }

            return new ContractualCashFlow
            {
                StartingBalance = Balance,
                EndingBalance = Balance,

                Period = 0,
                PeriodDate = StartDate,

                AccruedInterest = zerothPeriodAccruedInterest,
            };
        }

        public override Loan Copy()
        {
            return new FixedRateLoan(this);
        }
    }
}
