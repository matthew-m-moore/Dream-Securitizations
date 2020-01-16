using System;
using System.Linq;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.Stratifications;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;
using Dream.Common;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.Tranches
{
    public class ResidualTranche : Tranche
    {
        public ResidualTranche(
            string trancheName,
            PricingStrategy pricingStrategy,
            AvailableFundsRetriever availableFundsRetriever) :
            base(trancheName, pricingStrategy, availableFundsRetriever)
        {
            AbsorbsRemainingAvailableFunds = true;
            AbsorbsAssociatedReservesReleased = true;
            IncludePaymentShortfall = false;
        }

        public override Tranche Copy()
        {
            return new ResidualTranche(
                new string(TrancheName.ToCharArray()),
                PricingStrategy.Copy(),
                AvailableFundsRetriever.Copy())
            {
                TrancheDescription = (TrancheDescription == null) ? null : new string(TrancheDescription.ToCharArray()),
                TrancheRating = (TrancheRating == null) ? null : new string(TrancheRating.ToCharArray()),
                TranchePricingScenario = (TranchePricingScenario == null) ? null : new string(TranchePricingScenario.ToCharArray()),
                TrancheDetailId = TrancheDetailId,

                MonthsToNextPayment = MonthsToNextPayment,
                PaymentFrequencyInMonths = PaymentFrequencyInMonths,

                IsShortfallPaidFromReserves = IsShortfallPaidFromReserves,

                ListOfAssociatedReserveAccounts = ListOfAssociatedReserveAccounts.ToList(),
                TriggerLogicDictionary = TriggerLogicDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy()),
            };
        }

        public override void AllocatePaymentCashFlows(
            AvailableFunds availableFunds,
            DistributionRule distributionRule,
            int monthlyPeriod)
        {
            var trancheCashFlowType = TrancheCashFlowType.Payment;
            var fundsAvailableToAllocate = AvailableFundsRetriever.RetrieveAvailableFundsForTranche(monthlyPeriod, availableFunds, SecuritizationNode);

            var totalFundsAvailable = availableFunds[monthlyPeriod].TotalAvailableFunds;
            var fundsAvailable = distributionRule.ApplyProportionToDistributeToFundsAvailable(
                SecuritizationNode,
                trancheCashFlowType,
                monthlyPeriod,
                fundsAvailableToAllocate,
                totalFundsAvailable);

            var paymentAmountDue = DetermineAmountDue(monthlyPeriod, fundsAvailable.SpecificFundsAvailable, IncludePaymentShortfall);
            var paymentAmountPayable = DetermineAmountPayable(
                paymentAmountDue, 
                fundsAvailable.TotalFundsAvailable, 
                distributionRule.AppliedProportionToDistribute, 
                availableFunds,
                trancheCashFlowType,
                monthlyPeriod);

            RunTriggerLogic(monthlyPeriod, trancheCashFlowType, availableFunds, paymentAmountPayable);
            availableFunds[monthlyPeriod].TotalAvailableFunds
                = Math.Max(availableFunds[monthlyPeriod].TotalAvailableFunds - paymentAmountPayable.Amount, 0.0);

            TrancheCashFlows[monthlyPeriod].PaymentShortfall = paymentAmountPayable.Shortfall;
            TrancheCashFlows[monthlyPeriod].Interest = paymentAmountPayable.Amount;

            _PreviouslyAccruedPayments = 0.0;
        }

        public override SecuritizationCashFlowsSummaryResult RunAnalysis(int childToParentOrder)
        {
            var trancheResult = new SecuritizationCashFlowsSummaryResult(TrancheCashFlows);

            trancheResult.SecuritizationNodeName = SecuritizationNode.SecuritizationNodeName;
            trancheResult.ChildToParentOrder = childToParentOrder;
            trancheResult.TrancheName = TrancheName;
            trancheResult.TrancheType = GetType();

            trancheResult.PresentValue = PricingStrategy.CalculatePresentValue(TrancheCashFlows);
            trancheResult.InternalRateOfReturn = PricingStrategy.CalculateInternalRateOfReturn(TrancheCashFlows);
            trancheResult.MacaulayDuration = PricingStrategy.CalculateMacaulayDuration(TrancheCashFlows);
            trancheResult.ModifiedDurationAnalytical = PricingStrategy.CalculateModifiedDuration(TrancheCashFlows);
            trancheResult.ModifiedDurationNumerical = PricingStrategy.CalculateModifiedDuration(TrancheCashFlows, Constants.TwentyFiveBpsShock);
            trancheResult.DollarDuration = PricingStrategy.CalculateDollarDuration(TrancheCashFlows);

            trancheResult.DayCountConvention = PricingStrategy.DayCountConvention;
            trancheResult.CompoundingConvention = PricingStrategy.CompoundingConvention;

            trancheResult.WeightedAverageLife = CashFlowMetrics.CalculatePaymentWeightedAverageLife(TrancheCashFlows);
            trancheResult.PaymentCorridor = CashFlowMetrics.CalculatePaymentCorridor(TrancheCashFlows);

            trancheResult.TotalCashFlow = TrancheCashFlows.Sum(c => c.Payment);

            return trancheResult;
        }
    }
}
