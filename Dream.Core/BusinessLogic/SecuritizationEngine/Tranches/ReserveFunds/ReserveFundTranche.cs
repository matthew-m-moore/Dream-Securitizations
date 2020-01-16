using System;
using System.Collections.Generic;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds
{
    public abstract class ReserveFundTranche : Tranche
    {
        protected double _ReservesReleasedForMonthlyPeriod;
        protected bool _IsReservesReleaseMonth => _IsFirstPayment || _IsPaymentMonth;

        public ReserveFundTranche(
            string trancheName,
            double initialDollarAmountOfReserves,
            AvailableFundsRetriever availableFundsRetriever) :
            base(trancheName, new DoNothingPricingStrategy(), availableFundsRetriever)
        {
            InitialBalance = initialDollarAmountOfReserves;
            CurrentBalance = initialDollarAmountOfReserves;
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            var trancheCashFlowType = TrancheCashFlowType.Reserves;
            var reserveFundsAvailableToAllocate = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                reserveFundsAvailableToAllocate,
                totalFundsAvailable);

            double reservesReleased;
            var reserveAmountDue = DetermineAmountDue(monthlyPeriod, fundsAvailable.SpecificFundsAvailable, IncludePaymentShortfall);
            var reserveAmountPayable = DetermineAmountPayable(
                reserveAmountDue, 
                fundsAvailable.TotalFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, reserveAmountPayable);
            var reserveAmountToContribute = DetermineReserveAmountsToContributeAndRelease(
                reserveAmountPayable.Amount,
                monthlyPeriod, 
                availableFunds, 
                out reservesReleased);

            // The usage of reserve funds to cover other shortfalls could cause some existing principal adjustments to exist
            var principalAdjustmentsToReserves = TrancheCashFlows[monthlyPeriod].Principal;
            _ReservesReleasedForMonthlyPeriod = reservesReleased;

            // These are not really principal and interest, just placeholders until we get to the reporting
            TrancheCashFlows[monthlyPeriod].Principal += reserveAmountToContribute;
            TrancheCashFlows[monthlyPeriod].Interest += reservesReleased;

            TrancheCashFlows[monthlyPeriod].PaymentShortfall += reserveAmountPayable.Shortfall;
            TrancheCashFlows[monthlyPeriod].EndingBalance =
                    TrancheCashFlows[monthlyPeriod].StartingBalance + reserveAmountToContribute + principalAdjustmentsToReserves;

            // The any negative reserve contribution amount (i.e. funds released) will be floored at zero before hitting this 
            // available funds line, such that it is not double-counted later when pulled from the "reserves released" buckets 
            reserveAmountToContribute = Math.Max(reserveAmountToContribute, 0.0);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - reserveAmountToContribute, 0.0);

            if (CurrentBalance.HasValue) CurrentBalance = TrancheCashFlows[monthlyPeriod].EndingBalance;
            _PreviouslyAccruedPayments = 0.0;
        }

        public override SecuritizationCashFlowsSummaryResult RunAnalysis(int childToParentOrder)
        {
            var trancheResult = new SecuritizationCashFlowsSummaryResult(TrancheCashFlows);
            trancheResult.TrancheType = GetType();
            trancheResult.ChildToParentOrder = childToParentOrder;
            return trancheResult;
        }

        public abstract double DetermineReserveAmountsToContributeAndRelease(
            double reserveAmountPayable,
            int monthlyPeriod, 
            AvailableFunds availableFunds, 
            out double reservesReleased);

        public KeyValuePair<string, ReserveFund> PopulateReserveFundFromTranche(int monthlyPeriod)
        {
            var startingBalance = TrancheCashFlows[monthlyPeriod].StartingBalance;
            var endingBalance = TrancheCashFlows[monthlyPeriod].EndingBalance;

            var isReserveTranche = true;
            var reserveFund = new ReserveFund(TrancheName, SeniorityRanking, isReserveTranche, startingBalance, endingBalance);
            reserveFund.ReservesReleased = _ReservesReleasedForMonthlyPeriod;

            var reserveFundKeyValuePair = new KeyValuePair<string, ReserveFund>(TrancheName, reserveFund);
            return reserveFundKeyValuePair;
        }
    }
}
