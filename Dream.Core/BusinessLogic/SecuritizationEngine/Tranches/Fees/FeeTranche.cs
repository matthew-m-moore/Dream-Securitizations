using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Common;


namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees
{
    public abstract class FeeTranche : Tranche
    {     
        public Dictionary<string, double> BaseAnnualFees { get; protected set; }
        public Dictionary<string, DelayedFee> DelayedAnnualFees { get; protected set; }

        public PaymentConvention FeePaymentConvention { get; }
        public DayCountConvention ProRatingDayCountConvention { get; }

        protected double _TimeFactorInYearsForProRating;

        private bool _delayedFeesMonthlyPeriodsAreSet = false;

        private double? _totalBaseFees;
        public double TotalBaseFees
        {
            get
            {
                if (!_totalBaseFees.HasValue)
                {
                    _totalBaseFees = BaseAnnualFees.Values.Sum();
                }

                return _totalBaseFees.Value;
            }
        }

        public FeeTranche(
            string trancheName,
            PaymentConvention feePaymentConvention,
            DayCountConvention proRatingDayCountConvention,
            AvailableFundsRetriever availableFundsRetriever) :
            base(trancheName, new DoNothingPricingStrategy(), availableFundsRetriever)
        {
            FeePaymentConvention = feePaymentConvention;
            ProRatingDayCountConvention = proRatingDayCountConvention;
            BaseAnnualFees = new Dictionary<string, double>();
            DelayedAnnualFees = new Dictionary<string, DelayedFee>();
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            var nextFeePaymentPeriod = GetNextScheduledFeePaymentPeriod(monthlyPeriod);
            GetTimeFactorInYearsForProRating(nextFeePaymentPeriod, availableFunds);
            GetMonthlyPeriodsForDelayedFees(availableFunds);

            var trancheCashFlowType = TrancheCashFlowType.Fees;
            var fundsAvailableToPayFees = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
               SecuritizationNode,
               trancheCashFlowType,
               monthlyPeriod,
               fundsAvailableToPayFees,
               totalFundsAvailable);

            var nextFeeAmount = DetermineFee(nextFeePaymentPeriod);
            var accruedFeeAmount = CalculateAccruedFeeAmount(monthlyPeriod, nextFeePaymentPeriod, nextFeeAmount, availableFunds);
            TrancheCashFlows[monthlyPeriod].AccruedPayment += accruedFeeAmount;

            var feeAmountDue = DetermineFeeAmountDue(monthlyPeriod, nextFeeAmount, IncludePaymentShortfall);
            var feeAmountPayable = DetermineAmountPayable(
                feeAmountDue, 
                fundsAvailable.SpecificFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, feeAmountPayable);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - feeAmountPayable.Amount, 0.0);

            // It's not really principal, just a placeholder until we get to reporting
            TrancheCashFlows[monthlyPeriod].Principal += feeAmountPayable.Amount;
            TrancheCashFlows[monthlyPeriod].PaymentShortfall += feeAmountPayable.Shortfall;

            _PreviouslyAccruedPayments = 0.0;
        }

        public override void AllocatePaymentShortfall(
            AvailableFunds availableFunds,
            DistributionRule distributionRule, 
            int monthlyPeriod)
        {
            var trancheCashFlowType = TrancheCashFlowType.FeesShortfall;
            var fundsAvailableToPayFees = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                fundsAvailableToPayFees,
                totalFundsAvailable);

            var returnOnlyShortfall = true;
            var feeShortfall = DetermineFeeAmountDue(monthlyPeriod, 0.0, IncludePaymentShortfall, returnOnlyShortfall);
            var feeShortfallPayable = DetermineAmountPayable(
                feeShortfall, 
                fundsAvailable.SpecificFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, feeShortfallPayable);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - feeShortfallPayable.Amount, 0.0);

            TrancheCashFlows[monthlyPeriod].Principal += feeShortfallPayable.Amount;
            TrancheCashFlows[monthlyPeriod].PaymentShortfall += feeShortfallPayable.Shortfall;
        }

        public abstract double DetermineFee(int monthlyPeriod);

        public override SecuritizationCashFlowsSummaryResult RunAnalysis(int childToParentOrder)
        {
            var trancheResult = new SecuritizationCashFlowsSummaryResult(TrancheCashFlows);
            trancheResult.TrancheType = GetType();
            trancheResult.ChildToParentOrder = childToParentOrder;
            return trancheResult;
        }

        public void AddBaseFee(string feeName, double feeAnnualValue)
        {
            BaseAnnualFees.Add(feeName, feeAnnualValue);
        }

        public void AddDelayedFee(string feeName, double feeAnnualValue, DateTime delayedUntilDate)
        {
            var delayedFee = new DelayedFee(feeAnnualValue, delayedUntilDate);
            DelayedAnnualFees.Add(feeName, delayedFee);
        }

        public double GetNextScheduledFeePayment(int monthlyPeriod, bool returnOnlyShortfall = false)
        {
            if (returnOnlyShortfall) return TrancheCashFlows[monthlyPeriod].PaymentShortfall;

            var nextFeePaymentPeriod = GetNextScheduledFeePaymentPeriod(monthlyPeriod);
            var nextFeePayment = DetermineFee(monthlyPeriod);

            if (IncludePaymentShortfall) nextFeePayment += TrancheCashFlows[monthlyPeriod].PaymentShortfall;

            return nextFeePayment;
        }

        protected int GetNextScheduledFeePaymentPeriod(int monthlyPeriod)
        {
            if (monthlyPeriod <= MonthsToNextPayment)
            {
                return MonthsToNextPayment;
            }

            var adjustedMonthlyPeriod = monthlyPeriod - MonthsToNextPayment;
            var numberOfPaymentsElapsed = (int) Math.Ceiling((double)adjustedMonthlyPeriod / PaymentFrequencyInMonths);
            var nextPaymentPeriod = (numberOfPaymentsElapsed * PaymentFrequencyInMonths) + MonthsToNextPayment;

            return nextPaymentPeriod;
        }

        protected virtual void GetTimeFactorInYearsForProRating(int monthlyPeriod, AvailableFunds availableFunds)
        {
            if (monthlyPeriod >= availableFunds.ProjectedCashFlowsOnCollateral.Count)
            {
                _TimeFactorInYearsForProRating = 0.0;
                return;
            }

            if (FeePaymentConvention == PaymentConvention.ProRated)
            {
                var currentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[monthlyPeriod].PeriodDate;
                var lookbackMonthlyPeriod = Math.Max(monthlyPeriod - PaymentFrequencyInMonths, 0);
                var lastPaymentPeriodDate = availableFunds.ProjectedCashFlowsOnCollateral[lookbackMonthlyPeriod].PeriodDate;

                var timeFactorInYears = DateUtility.CalculateTimePeriodInYears(
                    ProRatingDayCountConvention,
                    lastPaymentPeriodDate,
                    currentPeriodDate);

                _TimeFactorInYearsForProRating = timeFactorInYears;
            }
            else
            {
                SetTimeFactorFromFeePaymentConvention(FeePaymentConvention);
            }
        }

        protected void SetTimeFactorFromFeePaymentConvention(PaymentConvention feePaymentConvention)
        {
            switch (feePaymentConvention)
            {
                case PaymentConvention.Annual:
                    _TimeFactorInYearsForProRating = 1.00;
                    break;
                case PaymentConvention.SemiAnnual:
                    _TimeFactorInYearsForProRating = 0.50;
                    break;
                case PaymentConvention.Quarterly:
                    _TimeFactorInYearsForProRating = 0.25;
                    break;
                case PaymentConvention.Monthly:
                    _TimeFactorInYearsForProRating = 1.0 / Constants.MonthsInOneYear;
                    break;
                default:
                    throw new Exception(string.Format("ERROR: Fee payment convention {0} is not supported",
                        FeePaymentConvention));
            }
        }

        protected double DetermineFeeAmountDue(int monthlyPeriod, double nextFeeAmount, bool includeShortfall,
            bool returnOnlyShortfall = false)
        {
            if (monthlyPeriod < MonthsToNextPayment)
            {
                return 0.0;
            }

            var isFirstPayment = (monthlyPeriod == MonthsToNextPayment);
            if (isFirstPayment && !returnOnlyShortfall)
            {
                if (AccruedPayment.HasValue) nextFeeAmount += AccruedPayment.Value;
                return nextFeeAmount;
            }
          
            var isPaymentMonth = MathUtility.CheckDivisibilityOfIntegers(
                monthlyPeriod - MonthsToNextPayment,
                PaymentFrequencyInMonths);

            if (isFirstPayment || isPaymentMonth || _IsFinalPeriod)
            {
                var amountDue = 0.0;
                if (returnOnlyShortfall)
                {
                    amountDue = TrancheCashFlows[monthlyPeriod].PaymentShortfall;
                    TrancheCashFlows[monthlyPeriod].PaymentShortfall = 0.0;
                    return amountDue;
                }

                var previouslyAccruedPayments = CalculatePreviouslyAccruedPayments(monthlyPeriod);
                amountDue = previouslyAccruedPayments;

                if (AccruedPayment.HasValue && isFirstPayment) amountDue += AccruedPayment.Value;

                var previouslyDistributedPayments = CalculatePreviouslyDistributedPayments(monthlyPeriod);
                amountDue -= previouslyDistributedPayments;

                if (includeShortfall)
                {
                    amountDue += TrancheCashFlows[monthlyPeriod].PaymentShortfall;
                    TrancheCashFlows[monthlyPeriod].PaymentShortfall = 0.0;
                }

                return amountDue;
            }

            return 0.0;
        }

        private double CalculatePreviouslyDistributedPayments(int monthlyPeriod)
        {
            if (monthlyPeriod <= MonthsToNextPayment || _IsFinalPeriod)
            {
                return 0.0;
            }

            if (monthlyPeriod - MonthsToNextPayment < PaymentFrequencyInMonths)
            {
                return TrancheCashFlows
                    .Where(t => t.Period > MonthsToNextPayment && t.Period < monthlyPeriod)
                    .Sum(c => c.Principal);
            }

            return TrancheCashFlows
                .Where(t => t.Period > monthlyPeriod - PaymentFrequencyInMonths && t.Period < monthlyPeriod)
                .Sum(c => c.Principal);
        }

        protected virtual double CalculateAccruedFeeAmount(int monthlyPeriod, int nextMonthlyPeriod, double feeAmount, AvailableFunds availableFunds)
        {
            if (monthlyPeriod == 0 || 
                nextMonthlyPeriod >= availableFunds.ProjectedCashFlowsOnCollateral.Count) return 0.0;

            var lookbackMonthlyPeriod = nextMonthlyPeriod - PaymentFrequencyInMonths;
            if (monthlyPeriod <= MonthsToNextPayment)
            {
                lookbackMonthlyPeriod = 0;
            }

            var priorPeriodDate = TrancheCashFlows[monthlyPeriod - 1].PeriodDate;
            var currentPeriodDate = TrancheCashFlows[monthlyPeriod].PeriodDate;

            var monthlyTimePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                ProRatingDayCountConvention,
                priorPeriodDate,
                currentPeriodDate);

            var lastPaymentDate = availableFunds.ProjectedCashFlowsOnCollateral[lookbackMonthlyPeriod].PeriodDate;
            var nextPaymentDate = availableFunds.ProjectedCashFlowsOnCollateral[nextMonthlyPeriod].PeriodDate;

            var intraPaymentTimePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                ProRatingDayCountConvention,
                lastPaymentDate,
                nextPaymentDate);

            var grossFeeAmount = feeAmount / intraPaymentTimePeriodInYears;
            var accruedFeeAmount = grossFeeAmount * monthlyTimePeriodInYears;
            return accruedFeeAmount;
        }

        protected void GetMonthlyPeriodsForDelayedFees(AvailableFunds availableFunds)
        {
            if (!_delayedFeesMonthlyPeriodsAreSet)
            {
                foreach (var delayedFeeKeyValuePair in DelayedAnnualFees)
                {
                    var delayedFee = delayedFeeKeyValuePair.Value;
                    var delayedUntilCashFlow = availableFunds.ProjectedCashFlowsOnCollateral
                        .LastOrDefault(c => c.PeriodDate <= delayedFee.DelayedUntilDate);

                    // If the date is beyond the last cash flow, assume that the delayed fee never takes effect
                    if (delayedUntilCashFlow == null)
                    {
                        var maximumMonthlyPeriod = availableFunds.ProjectedCashFlowsOnCollateral.Last().Period;
                        delayedFee.DelayedUntilMonthlyPeriod = maximumMonthlyPeriod + 1;
                    }
                    else
                    {
                        delayedFee.DelayedUntilMonthlyPeriod = delayedUntilCashFlow.Period;
                    }
                }

                // The results of this operation are now cached
                _delayedFeesMonthlyPeriodsAreSet = true;
            }
        }

    }
}
