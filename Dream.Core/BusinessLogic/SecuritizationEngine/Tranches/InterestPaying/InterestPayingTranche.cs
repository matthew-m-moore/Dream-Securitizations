using System;
using System.Linq;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Common.Enums;
using Dream.Common.Utilities;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying
{
    public abstract class InterestPayingTranche : Tranche
    {
        public Coupon Coupon { get; }
        public double InitialCoupon => Coupon.GetCouponForSpecificMonthlyPeriod(0, InterestAccrualStartDate);
        public AvailableFundsRetriever InterestAvailableFundsRetriever { get; protected set; }

        public DateTime InterestAccrualStartDate { get; set; }
        public DateTime InitialPeriodInterestAccrualEndDate { get; set; }
        public DayCountConvention InterestAccrualDayCountConvention { get; set; }
        public DayCountConvention InitialPeriodInterestAccrualDayCountConvention { get; set; }
        public bool IncludeInterestShortfall { get; set; }

        public double? AccruedInterest { get;set; }

        public int MonthsToNextInterestPayment { get; set; }
        public int InterestPaymentFrequencyInMonths { get; set; }

        public InterestPayingTranche(
            string trancheName,
            PricingStrategy pricingStrategy, 
            AvailableFundsRetriever principalAvailableFundsRetriever, 
            AvailableFundsRetriever interestAvailableFundsRetriever,
            Coupon coupon) 
            : base(trancheName, pricingStrategy, principalAvailableFundsRetriever)
        {
            Coupon = coupon;
            InterestAvailableFundsRetriever = interestAvailableFundsRetriever;
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            if (CurrentBalance.HasValue && CurrentBalance.Value <= 0.0) return;

            var trancheCashFlowType = TrancheCashFlowType.Principal;
            var principalAvailableToAllocate = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode, 
                trancheCashFlowType, 
                monthlyPeriod, 
                principalAvailableToAllocate,
                totalFundsAvailable,
                CurrentBalance);

            // You cannot pull more than what funds are available at a given time
            var principalAmountDue = DetermineAmountDue(monthlyPeriod, fundsAvailable.SpecificFundsAvailable, IncludePaymentShortfall);
            var principalAmountPayable = DetermineAmountPayable(
                principalAmountDue, 
                fundsAvailable.TotalFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            CapPayableAmountAtTotalPrincipal(monthlyPeriod, availableFunds, principalAmountPayable);
            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, principalAmountPayable);

            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - principalAmountPayable.Amount, 0.0);

            TrancheCashFlows[monthlyPeriod].PaymentShortfall += principalAmountPayable.Shortfall;
            TrancheCashFlows[monthlyPeriod].Principal += principalAmountPayable.Amount;
            TrancheCashFlows[monthlyPeriod].EndingBalance =
                TrancheCashFlows[monthlyPeriod].StartingBalance - principalAmountPayable.Amount;

            // This step is important to deal with accrued principal
            if (CurrentBalance.HasValue) CurrentBalance = TrancheCashFlows[monthlyPeriod].EndingBalance;
            _PreviouslyAccruedPayments = 0.0;
        }

        public override void AllocatePaymentShortfall(
            AvailableFunds availableFunds, 
            DistributionRule distributionRule, 
            int monthlyPeriod)
        {
            var trancheCashFlowType = TrancheCashFlowType.PrincipalShortfall;
            var fundsAvailableToPayShortfall = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                fundsAvailableToPayShortfall,
                totalFundsAvailable);

            var returnOnlyShortfall = true;
            var principalShortfall = DetermineAmountDue(monthlyPeriod, 0.0, IncludePaymentShortfall, returnOnlyShortfall);
            var principalShortfallPayable = DetermineAmountPayable(
                principalShortfall, 
                fundsAvailable.SpecificFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, principalShortfallPayable);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - principalShortfallPayable.Amount, 0.0);

            TrancheCashFlows[monthlyPeriod].PaymentShortfall += principalShortfallPayable.Shortfall;
            TrancheCashFlows[monthlyPeriod].Principal += principalShortfallPayable.Amount;
            TrancheCashFlows[monthlyPeriod].EndingBalance =
                TrancheCashFlows[monthlyPeriod].StartingBalance - principalShortfallPayable.Amount;

            // This step is important to deal with accrued principal
            if (CurrentBalance.HasValue) CurrentBalance = TrancheCashFlows[monthlyPeriod].EndingBalance;
        }

        public virtual void AllocateInterestCashFlows(
            AvailableFunds availableFunds, 
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            var trancheCashFlowType = TrancheCashFlowType.Interest;
            var interestAvailableToAllocate = InterestAvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                interestAvailableToAllocate,
                totalFundsAvailable);

            var accruedInterest = CalculateAccruedInterest(monthlyPeriod);
            TrancheCashFlows[monthlyPeriod].AccruedInterest += accruedInterest;

            var accruedInterestOnInterestShortfall = CalculateAccruedInterestOnInterestShortfall(monthlyPeriod, IncludeInterestShortfall);
            TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall += accruedInterestOnInterestShortfall;

            var interestAmountDue = DetermineInterestAmountDue(monthlyPeriod, IncludeInterestShortfall);
            var interestAmountPayable = DetermineAmountPayable(
                interestAmountDue, 
                fundsAvailable.SpecificFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, interestAmountPayable);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - interestAmountPayable.Amount, 0.0);

            TrancheCashFlows[monthlyPeriod].InterestShortfall += interestAmountPayable.Shortfall;
            TrancheCashFlows[monthlyPeriod].Interest += interestAmountPayable.Amount;
        }

        public virtual void AllocateInterestShortfall(
            AvailableFunds availableFunds, 
            DistributionRule distributionRule, 
            int monthlyPeriod)
        {
            var trancheCashFlowType = TrancheCashFlowType.InterestShortfall;
            var fundsAvailableToPayInterestShortfall = InterestAvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                fundsAvailableToPayInterestShortfall,
                totalFundsAvailable);

            var accruedInterestOnInterestShortfall = CalculateAccruedInterestOnInterestShortfall(monthlyPeriod, true);
            TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall += accruedInterestOnInterestShortfall;

            var returnOnlyShortfall = true;
            var interestShortfall = DetermineInterestAmountDue(monthlyPeriod, IncludeInterestShortfall, returnOnlyShortfall);
            var interestShortfallPayable = DetermineAmountPayable(
                interestShortfall, 
                fundsAvailable.SpecificFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, interestShortfallPayable);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - interestShortfallPayable.Amount, 0.0);

            TrancheCashFlows[monthlyPeriod].InterestShortfall += interestShortfallPayable.Shortfall;
            TrancheCashFlows[monthlyPeriod].Interest += interestShortfallPayable.Amount;
        }

        public override void SwitchToExternalFunds()
        {
            AvailableFundsRetriever = new ExternalPayerAvailableFundsRetriever();
            InterestAvailableFundsRetriever = new ExternalPayerAvailableFundsRetriever();
        }

        public double CalculatePreviouslyAccruedInterest(int monthlyPeriod)
        {
            var minimumMonthlyPeriodForLookback = Math.Max(monthlyPeriod - InterestPaymentFrequencyInMonths, 0);

            // Balloon out all remaining accrued interest
            if (_IsFinalPeriod)
            {
                var lastCashFlowWithAnInterestPayment = TrancheCashFlows.LastOrDefault(c => c.Interest > 0);
                if (lastCashFlowWithAnInterestPayment != null)
                {
                    minimumMonthlyPeriodForLookback = lastCashFlowWithAnInterestPayment.Period;
                }
            }

            var previouslyAccruedInterest = TrancheCashFlows
                .Where(c => c.Period > minimumMonthlyPeriodForLookback
                         && c.Period <= monthlyPeriod)
                .Sum(s => s.AccruedInterest);

            return previouslyAccruedInterest;
        }

        protected virtual double CalculateAccruedInterest(int monthlyPeriod)
        {
            // Assume there is no interest accrual in the zeroth period
            if (monthlyPeriod == 0)
            {
                return 0.0;
            }

            var priorPeriodCashFlow = TrancheCashFlows[monthlyPeriod - 1];
            var accruedInterestFactor = GetInterestAccrualFactor(monthlyPeriod);

            var accruedInterest = accruedInterestFactor * priorPeriodCashFlow.EndingBalance;
            return accruedInterest;
        }

        protected virtual double CalculateAccruedInterestOnInterestShortfall(int monthlyPeriod, bool includeInterestShortfall)
        {
            // Assume there is no interest accrual in the zeroth period, and skip for any time there is an explicit
            // entry on the priority of payments for interest short-fall amounts or no short-fall balance
            if (monthlyPeriod == 0 || !includeInterestShortfall || TrancheCashFlows[monthlyPeriod].InterestShortfall <= 0.0)
            {
                return 0.0;
            }

            var priorPeriodCashFlow = TrancheCashFlows[monthlyPeriod - 1];
            var accruedInterestFactor = GetInterestAccrualFactor(monthlyPeriod);

            var accruedInterestOnInterestShortfall = accruedInterestFactor * priorPeriodCashFlow.InterestShortfall;
            return accruedInterestOnInterestShortfall;
        }

        protected double DetermineInterestAmountDue(int monthlyPeriod, bool includeShortfall, 
            bool returnShortfallOnly = false)
        {
            if (monthlyPeriod < MonthsToNextInterestPayment ||
                TrancheCashFlows[monthlyPeriod].StartingBalance <= 0.0)
            {
                return 0.0;
            }

            var isFirstInterestPayment = monthlyPeriod == MonthsToNextInterestPayment;
            var isInterestPaymentMonth = MathUtility.CheckDivisibilityOfIntegers(
                monthlyPeriod - MonthsToNextInterestPayment,
                InterestPaymentFrequencyInMonths);

            if (isFirstInterestPayment || isInterestPaymentMonth || _IsFinalPeriod)
            {
                var interestAmountDue = 0.0;
                if (returnShortfallOnly)
                {
                    interestAmountDue = TrancheCashFlows[monthlyPeriod].InterestShortfall +
                                        TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall;

                    TrancheCashFlows[monthlyPeriod].InterestShortfall = 0.0;
                    TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall = 0.0;

                    return interestAmountDue;
                }

                var previouslyAccruedInterest = CalculatePreviouslyAccruedInterest(monthlyPeriod);
                interestAmountDue = previouslyAccruedInterest;

                if (AccruedInterest.HasValue && isFirstInterestPayment) interestAmountDue += AccruedInterest.Value;

                if (includeShortfall)
                {
                    interestAmountDue += TrancheCashFlows[monthlyPeriod].InterestShortfall;
                    interestAmountDue += TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall;

                    TrancheCashFlows[monthlyPeriod].InterestShortfall = 0.0;
                    TrancheCashFlows[monthlyPeriod].InterestAccruedOnInterestShortfall = 0.0;
                }
                
                return interestAmountDue;
            }

            return 0.0;
        }

        private double GetInterestAccrualFactor(int monthlyPeriod)
        {
            var interestAccrualDayCountConvention = InterestAccrualDayCountConvention;

            var priorPeriodCashFlow = TrancheCashFlows[monthlyPeriod - 1];
            var priorPeriodDate = priorPeriodCashFlow.PeriodDate;
            var currentPeriodDate = TrancheCashFlows[monthlyPeriod].PeriodDate;

            if (InitialPeriodInterestAccrualEndDate != DateTime.MinValue &&
                InitialPeriodInterestAccrualEndDate.Ticks >= currentPeriodDate.Ticks)
            {
                interestAccrualDayCountConvention = InitialPeriodInterestAccrualDayCountConvention;
            }

            var monthlyTimePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                interestAccrualDayCountConvention,
                priorPeriodDate,
                currentPeriodDate);

            var couponRate = Coupon.GetCouponForSpecificMonthlyPeriod(monthlyPeriod, InterestAccrualStartDate);
            var accruedInterestFactor = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(
                monthlyTimePeriodInYears,
                couponRate);

            return accruedInterestFactor;
        }

        private void CapPayableAmountAtTotalPrincipal(int monthlyPeriod, AvailableFunds availableFunds, AmountPayable principalAmountPayable)
        {
            var trancheStartingBalance = TrancheCashFlows[monthlyPeriod].StartingBalance;
            if (principalAmountPayable.Amount > trancheStartingBalance)
            {
                // If the excess is due (in part) to accrued payments, the accrued payments needs
                // to be returned to the total available funds for any remaining subordinated tranches
                var excessFunds = principalAmountPayable.Amount - trancheStartingBalance;

                if (_PreviouslyAccruedPayments > 0.0
                    && principalAmountPayable.Amount > availableFunds[monthlyPeriod].TotalAvailableFunds)
                {
                    availableFunds[monthlyPeriod].TotalAvailableFunds += _PreviouslyAccruedPayments;
                }

                principalAmountPayable.Amount = Math.Min(trancheStartingBalance, principalAmountPayable.Amount);
            }
        }
    }
}
