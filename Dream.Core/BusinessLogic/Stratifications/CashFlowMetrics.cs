using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Stratifications
{
    public class CashFlowMetrics
    {
        /// <summary>
        /// Calculates the weighted average life (WAL) for a given list of cash flow. Determines whether to use
        /// the total cash-flow or the principal payment based on the cash flows given.
        /// </summary>
        public static double CalculateWeightedAverageLife<T>(List<T> cashFlows) where T : CashFlow
        {
            var firstCashFlow = cashFlows.FirstOrDefault();
            var firstCashFlowAsContractualCashFlow = firstCashFlow as ContractualCashFlow;

            if (firstCashFlowAsContractualCashFlow != null && firstCashFlowAsContractualCashFlow.StartingBalance > 0.0)
            {
                var contractualCashFlows = cashFlows.Select(c => c as ContractualCashFlow).ToList();
                return CalculatePrincipalWeightedAverageLife(contractualCashFlows);
            }
            // Use the cash-flow weighted average life for cash-flow streams without a principal balance
            else
            {
                return CalculatePaymentWeightedAverageLife(cashFlows);
            }
        }

        /// <summary>
        /// Calculates the cash-flow weighted average life (WAL) for a given list of cash flow.
        /// </summary>
        public static double CalculatePaymentWeightedAverageLife<T>(List<T> cashFlows) where T : CashFlow
        {
            var firstCashFlow = cashFlows.First();
            var startingPeriod = firstCashFlow.PeriodDate;

            var totalPayment = 0.0;
            var totalWeightedPayment = 0.0;
            var cashFlowCount = cashFlows.Count;

            for (var monthlyPeriod = 0; monthlyPeriod < cashFlowCount; monthlyPeriod++)
            {
                var cashFlow = cashFlows[monthlyPeriod];

                var timePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                    DayCountConvention.Thirty360,
                    startingPeriod,
                    cashFlow.PeriodDate);

                var timeWeightedPayment = timePeriodInYears * cashFlow.Payment;

                totalPayment += cashFlow.Payment;
                totalWeightedPayment += timeWeightedPayment;
            }

            var weightedAverageLife = totalWeightedPayment / totalPayment;
            return weightedAverageLife;
        }

        /// <summary>
        /// Calculates the principal weighted average life (WAL) for a given list of cash flow.
        /// </summary>
        public static double CalculatePrincipalWeightedAverageLife<T>(List<T> cashFlows) where T : ContractualCashFlow
        {
            var firstCashFlow = cashFlows.First();
            var totalPrincipal = firstCashFlow.EndingBalance;
            var startingPeriod = firstCashFlow.PeriodDate;

            var totalWeightedPrincipalChange = 0.0;
            var cashFlowCount = cashFlows.Count;

            for (var monthlyPeriod = 1; monthlyPeriod < cashFlowCount; monthlyPeriod++)
            {
                var priorCashFlow = cashFlows[monthlyPeriod - 1];
                var currentCashFlow = cashFlows[monthlyPeriod];

                var timePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                    DayCountConvention.Thirty360,
                    startingPeriod,
                    currentCashFlow.PeriodDate);
                
                var principalChange = priorCashFlow.EndingBalance - currentCashFlow.EndingBalance;
                var timeWeightedPrincipalChange = timePeriodInYears * principalChange;

                totalWeightedPrincipalChange += timeWeightedPrincipalChange;
            }

            var weightedAverageLife = totalWeightedPrincipalChange / totalPrincipal;
            return weightedAverageLife;
        }

        /// <summary>
        /// Calculates the forward WAC for a given list of cash flows. Assumes cash-flows are in monthly period.
        /// </summary>
        public static double CalculateForwardWeightedAverageCoupon<T>(List<T> cashFlows) where T : ContractualCashFlow
        {
            var sumOfAllInterestAccrued = cashFlows.Sum(c => c.Interest);
            var sumOfAllBalances = cashFlows.Sum(c => c.EndingBalance);

            var forwardWeightedAverageCoupon = Constants.MonthsInOneYear * (sumOfAllInterestAccrued / sumOfAllBalances);
            return forwardWeightedAverageCoupon;
        }

        /// <summary>
        /// Calculates the lifetime CPR for a given list of cash flows. Assumes cash-flows are in monthly period.
        /// </summary>
        public static double CalculateLifetimeConstantPrepaymentRate<T>(List<T> cashFlows) where T : ProjectedCashFlow
        {
            var sumOfAllPrepayments = cashFlows.Sum(c => c.Prepayment);
            var sumOfAllBalances = cashFlows.Sum(c => c.EndingBalance);

            var lifetimeSingleMonthlyMortality = sumOfAllPrepayments / sumOfAllBalances;
            var lifetimeConstantPrepaymentRate = MathUtility.ConvertMonthlyRateToAnnualRate(lifetimeSingleMonthlyMortality);
            return lifetimeConstantPrepaymentRate;
        }

        public static PaymentCorridor CalculatePaymentCorridor<T>(List<T> cashFlows) where T : CashFlow
        {
            var firstPaymentCashFlow = cashFlows.FirstOrDefault(c => c.Payment > 0.0);
            var lastPaymentCashFlow = cashFlows.LastOrDefault(c => c.Payment > 0.0);

            if (firstPaymentCashFlow == null || lastPaymentCashFlow == null) return new PaymentCorridor();

            var paymentCorridor = new PaymentCorridor
            {
                FirstPaymentPeriod = firstPaymentCashFlow.Period,
                FirstPaymentPeriodDate = firstPaymentCashFlow.PeriodDate,

                LastPaymentPeriod = lastPaymentCashFlow.Period,
                LastPaymentPeriodDate = lastPaymentCashFlow.PeriodDate
            };

            return paymentCorridor;
        }

        public static PaymentCorridor CalculatePrinicipalPaymentCorridor<T>(List<T> cashFlows) where T : ContractualCashFlow
        {
            var firstPaymentCashFlow = cashFlows.FirstOrDefault(c => c.Principal > 0.0);
            var lastPaymentCashFlow = cashFlows.LastOrDefault(c => c.Principal > 0.0);

            if (firstPaymentCashFlow == null || lastPaymentCashFlow == null) return new PaymentCorridor();

            var paymentCorridor = new PaymentCorridor
            {
                FirstPaymentPeriod = firstPaymentCashFlow.Period,
                FirstPaymentPeriodDate = firstPaymentCashFlow.PeriodDate,

                LastPaymentPeriod = lastPaymentCashFlow.Period,
                LastPaymentPeriodDate = lastPaymentCashFlow.PeriodDate
            };

            return paymentCorridor;
        }

        public static InterestShortfallRecord CalculateInterestShortfallRecord<T>(List<T> securitizationCashFlows) where T: SecuritizationCashFlow
        {
            var firstInterestShortfall = securitizationCashFlows.FirstOrDefault(c => c.InterestShortfall > 0.0);
            var lastInterestShortfall = securitizationCashFlows.LastOrDefault(c => c.InterestShortfall > 0.0);

            if (firstInterestShortfall == null) return new InterestShortfallRecord();

            var interstShortfallRecord = new InterestShortfallRecord
            {
                FirstInterestShortfallPeriod = firstInterestShortfall.Period,
                FirstInterestShortfallPeriodDate = firstInterestShortfall.PeriodDate,

                LastInterestShortfallPeriod = lastInterestShortfall.Period,
                LastInterestShortfallPeriodDate = lastInterestShortfall.PeriodDate,

                MaximumInterestShortfall = securitizationCashFlows.Max(c => c.InterestShortfall)
            };

            return interstShortfallRecord;
        }
    }
}
