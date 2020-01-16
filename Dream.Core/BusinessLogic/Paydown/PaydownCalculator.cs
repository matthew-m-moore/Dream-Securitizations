using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Paydown
{
    public abstract class PaydownCalculator
    {
        public bool FloorPaydownBalanceAtZero = true;

        public double PrincipalToApply { get; protected set; }
        public double YearEndPrincipalToApply { get; protected set; }
        public double TotalPaymentApplied { get; protected set; }
        public double BalanceToCalculatePaydown { get; set; }
        public double AccruedInterestPeriod { get; protected set; }
        public double? FirstPaymentIfAlreadyCollectedOrUnamended { get; protected set; }

        public ContractualCashFlow FirstCashFlowAfterPaydown { get; protected set; }

        protected int _RemainingTermInMonths;
        protected List<ContractualCashFlow> _ContractualCashFlows;

        protected bool _IsInInterestOnlyPeriod;
        protected bool _IsCalculationPrepared;
        protected bool _IsPrincipalAppliedCalculated;
      
        private DayCountConvention _interestAccrualDayCountConvention;

        public PaydownCalculator() { }

        protected PaydownCalculator(
            List<ContractualCashFlow> contractualCashFlows,
            DayCountConvention interestAccrualDayCountConvention)
        {
            _ContractualCashFlows = contractualCashFlows;           
            _IsCalculationPrepared = false;
            _IsPrincipalAppliedCalculated = false;

            _interestAccrualDayCountConvention = interestAccrualDayCountConvention;
        }

        public abstract PaydownCalculator GetNewPaydownCalculator(
            List<ContractualCashFlow> contractualCashFlows,
            DayCountConvention interestAccrualDayCountConvention);

        /// <summary>
        /// This method calculates the principaly to apply to a given fixed rate loan, under various scenarios.
        /// </summary>
        public abstract void CalculatePrincipalToApply(PaydownScenario paydownScenario, Loan loan);

        /// <summary>
        /// This method prepares the calculator to find the appropriate amount of principal to apply for a given paydown scenario.
        /// This method must be run before CalculatePrincipalToApply can be called.
        /// </summary>
        public virtual void PrepareForCalculation(DateTime paydownEffectiveDate)
        {
            _RemainingTermInMonths = _ContractualCashFlows
                .Where(c => c.PeriodDate > paydownEffectiveDate)
                .Count();

            FirstCashFlowAfterPaydown = _ContractualCashFlows
                .Where(f => f.Payment > 0.0)
                .First(c => c.PeriodDate >= paydownEffectiveDate);
            BalanceToCalculatePaydown = FirstCashFlowAfterPaydown.StartingBalance;

            AccruedInterestPeriod = GetInterestAccrualPeriod(
                _ContractualCashFlows,
                _interestAccrualDayCountConvention,
                paydownEffectiveDate);

            var firstPrincipalPayment = _ContractualCashFlows.First(c => c.Principal > 0.0);
            var lastInterestOnlyPayment = _ContractualCashFlows
                .Where(c => c.PeriodDate.Ticks < firstPrincipalPayment.PeriodDate.Ticks)
                .LastOrDefault(c => c.Interest > 0.0);

            _IsInInterestOnlyPeriod = false;
            if (lastInterestOnlyPayment != null)
            {
                var lastInterestOnlyPaymentDate = lastInterestOnlyPayment.PeriodDate;
                _IsInInterestOnlyPeriod = (paydownEffectiveDate.Ticks < lastInterestOnlyPaymentDate.Ticks);
            }

            _IsCalculationPrepared = true;
        }

        /// <summary>
        /// If a payment was already made within the period under consideration, determines what payment was already collected.
        /// </summary>
        public void DetermineFirstPaymentIfAlreadyCollectedOrUnamended(
            PaydownScenario paydownScenario,
            List<ContractualCashFlow> prePaydownContractualCashFlows,
            List<ContractualCashFlow> postPaydownContractualCashFlows)
        {
            if (paydownScenario.PaymentAlreadyMadeForPeriod || !paydownScenario.NextBillAmendedForPayment.GetValueOrDefault(true))
            {
                var firstPaymentIfAlreadyCollected = 0.0;
                var firstPaymentCashFlow = postPaydownContractualCashFlows.FirstOrDefault(c => c.Payment > 0.0);
                if (firstPaymentCashFlow != null)
                {
                    firstPaymentIfAlreadyCollected = prePaydownContractualCashFlows
                        .Single(c => c.PeriodDate.Ticks == firstPaymentCashFlow.PeriodDate.Ticks).Payment;
                };

                FirstPaymentIfAlreadyCollectedOrUnamended = firstPaymentIfAlreadyCollected;
            }
        }

        /// <summary>
        /// Determines the total cash available when a scheduled payment has already been collected prior to paydown.
        /// </summary>
        public double DetermineCashAvailableWhenPaymentAlreadyCollectedOrUnamended(PaydownScenario paydownScenario)
        {         
            var firstPaymentIfAlreadyCollectedOrUnamended = FirstPaymentIfAlreadyCollectedOrUnamended.GetValueOrDefault();

            var percentageFeeDollars = paydownScenario.PercentageFees * (firstPaymentIfAlreadyCollectedOrUnamended + paydownScenario.AdditionalCustomerFee);
            firstPaymentIfAlreadyCollectedOrUnamended -= percentageFeeDollars;
            firstPaymentIfAlreadyCollectedOrUnamended -= paydownScenario.FixedFees;

            return firstPaymentIfAlreadyCollectedOrUnamended;
        }

        /// <summary>
        /// Determines the proper cash inflow when a scheduled payment has already been collected prior to paydown.
        /// </summary>
        public double DetermineCashInFlowWhenPaymentAlreadyCollected(
            PaydownScenario paydownScenario,
            double? principalToApply,
            double couponRate)
        {
            var paydownAmount = paydownScenario.PaydownPercentageAmount * BalanceToCalculatePaydown;
            var cashAlreadyCollected = DetermineCashAvailableWhenPaymentAlreadyCollectedOrUnamended(paydownScenario);
            var totalCashAvailable = paydownAmount + cashAlreadyCollected;

            var appliedPrincipal = principalToApply ?? PrincipalToApply;
            var deltaInterestAccrued = appliedPrincipal * couponRate * AccruedInterestPeriod;
            var totalCashUsed = deltaInterestAccrued + appliedPrincipal;

            var assessmentCashInFlow = totalCashAvailable - totalCashUsed;
            return assessmentCashInFlow;
        }

        /// <summary>
        /// Determines the proper cash inflow when the tax bill was not amended after a paydown.
        /// </summary>
        public double DetermineCashInFlowWhenPaymentIsUnamended(PaydownScenario paydownScenario)
        {
            var assessmentCashInFlow = DetermineCashAvailableWhenPaymentAlreadyCollectedOrUnamended(paydownScenario);
            return assessmentCashInFlow;
        }

        /// <summary>
        /// Adjusts the cash flows of the specified fixed rate loan for the paydown amount and appropriate paydown date.
        /// The necessary amount of principal is applied and the cash flows are shortened.
        /// </summary>
        public void AdjustLoanForPaydown(DateTime collateralCutOffDate, Loan loan)
        {
            if (!_IsPrincipalAppliedCalculated)
            {
                throw new Exception("ERROR: The principal to apply at paydown has not been calculated. Cannot proceed");
            }

            // TODO: Now, How am I going ot deal with applying this additional principal to the bond level
            loan.Balance = BalanceToCalculatePaydown - PrincipalToApply;
            loan.AddCustomPayment(FirstCashFlowAfterPaydown.PeriodDate, YearEndPrincipalToApply, 0.0);

            if (FloorPaydownBalanceAtZero && loan.Balance < 0.0)
            {
                loan.Balance = double.NaN;
            }

            loan.StartDate = FirstCashFlowAfterPaydown.PeriodDate
                .AddMonths(-1 * loan.PrincipalPaymentFrequencyInMonths);

            loan.AdjustLoanToCashFlowStartDate(collateralCutOffDate);
        }

        /// <summary>
        /// Adjusts the cash flows of the specified fixed rate loan for the paydown amount and appropriate paydown date.
        /// The necessary amount of principal is applied and the cash flows are shortened.
        /// </summary>
        public void AdjustLoanForSpecificPaydown(DateTime collateralCutOffDate, Loan loan, double principalToApply)
        {
            loan.Balance = BalanceToCalculatePaydown - principalToApply;

            if (FloorPaydownBalanceAtZero && loan.Balance < 0.0)
            {
                loan.Balance = double.NaN;
            }

            loan.StartDate = FirstCashFlowAfterPaydown.PeriodDate
                .AddMonths(-1 * loan.PrincipalPaymentFrequencyInMonths);

            loan.AdjustLoanToCashFlowStartDate(collateralCutOffDate);
        }

        private double GetInterestAccrualPeriod(
            List<ContractualCashFlow> contractualCashFlows,
            DayCountConvention interestAccrualDayCountConvention,
            DateTime bondCallDate)
        {
            var lastPaymentPeriodDate = contractualCashFlows.First().PeriodDate;
            var lastPaymentCashFlow = contractualCashFlows
                .Where(t => t.PeriodDate < bondCallDate)
                .LastOrDefault(c => c.Payment > 0.0);

            if (lastPaymentCashFlow != null) lastPaymentPeriodDate = lastPaymentCashFlow.PeriodDate;

            var accruedInterestPeriod = DateUtility.CalculateTimePeriodInYears(
                interestAccrualDayCountConvention,
                lastPaymentPeriodDate,
                bondCallDate);

            return accruedInterestPeriod;
        }
    }
}
