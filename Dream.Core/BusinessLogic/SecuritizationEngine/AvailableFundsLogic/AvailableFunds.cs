using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic
{
    public class AvailableFunds
    {
        private List<AvailableFundsCashFlow> _availableFundsCashFlows;

        public List<ProjectedCashFlow> ProjectedCashFlowsOnCollateral { get; }

        public AvailableFunds(List<ProjectedCashFlow> projectedCashFlowsOnCollateral)
        {
            ProjectedCashFlowsOnCollateral = projectedCashFlowsOnCollateral;
            _availableFundsCashFlows = new List<AvailableFundsCashFlow>();
        }

        public AvailableFundsCashFlow this[int monthlyPeriod]
        {
            get { return _availableFundsCashFlows[monthlyPeriod]; }

            private set { _availableFundsCashFlows[monthlyPeriod] = value; }
        }

        public List<AvailableFundsCashFlow> GetAvailableFundsCashFlows()
        {
            return _availableFundsCashFlows;
        }

        public void PopulateStartingAvailableFunds(int monthlyPeriod, DateTime securitizationStartDate, DateTime securitizationFirstCashFlowDate)
        {
            if (_availableFundsCashFlows.Any() && monthlyPeriod > 0)
            {
                var fundsAvailableDictionary = GetAvailableFundsForGivenPeriod(monthlyPeriod);

                var availableFundsCashFlow = CreateNewAvailableFundsCashFlow(
                    monthlyPeriod,
                    securitizationFirstCashFlowDate,
                    fundsAvailableDictionary);

                // This total value is ultimately what is going to be tracked during the waterfall
                availableFundsCashFlow.TotalAvailableFunds = availableFundsCashFlow.Payment;

                if (_availableFundsCashFlows.ElementAtOrDefault(monthlyPeriod) != null)
                {
                    _availableFundsCashFlows[monthlyPeriod] = availableFundsCashFlow;
                }
                else
                {
                    _availableFundsCashFlows.Add(availableFundsCashFlow);
                }
            }
            else
            {
                var zerothPeriodAvailableFundsCashFlow = new AvailableFundsCashFlow
                {
                    PeriodDate = securitizationStartDate,
                    BondCount = ProjectedCashFlowsOnCollateral[monthlyPeriod].BondCount,
                    AvailableReserveFundsDictionary = new Dictionary<string, ReserveFund>()
                };

                _availableFundsCashFlows.Add(zerothPeriodAvailableFundsCashFlow);
            }
        }

        public void PopulateStartingAvailableFundsForCleanUpCall(int monthlyPeriod, DateTime securitizationFirstCashFlowDate)
        {
            if (monthlyPeriod == 0)
            {
                throw new Exception("INTERNAL ERROR: Clean-up call cannot occur in zeroth period. Please report this error.");
            }

            var fundsAvailableForCleanUpCallDictionary = GetAvailableFundsForCleanUpCall(monthlyPeriod);

            var availableFundsCashFlow = CreateNewAvailableFundsCashFlow(
                monthlyPeriod,
                securitizationFirstCashFlowDate,
                fundsAvailableForCleanUpCallDictionary);

            // This total value is ultimately what is going to be tracked during the waterfall
            availableFundsCashFlow.TotalAvailableFunds = availableFundsCashFlow.Payment;

            if (_availableFundsCashFlows.ElementAtOrDefault(monthlyPeriod) != null)
            {
                _availableFundsCashFlows[monthlyPeriod] = availableFundsCashFlow;
            }
            else
            {
                _availableFundsCashFlows.Add(availableFundsCashFlow);
            }
        }

        public void PopulateReserveFundsFromTrancheForGivenPeriod(Tranches.ReserveFunds.ReserveFundTranche reserveTranche, int monthlyPeriod)
        {
            var keyValuePair = reserveTranche.PopulateReserveFundFromTranche(monthlyPeriod);
            if (this[monthlyPeriod].AvailableReserveFundsDictionary.ContainsKey(keyValuePair.Key))
            {
                this[monthlyPeriod].AvailableReserveFundsDictionary[keyValuePair.Key] = keyValuePair.Value;
            }
            else
            {
                this[monthlyPeriod].AvailableReserveFundsDictionary.Add(keyValuePair);
            }
        }

        public double GetAllAvailableReservesReleased()
        {
            var availableReservesReleased = 0.0;
            foreach (var availableFundsCashFlow in _availableFundsCashFlows)
            {
                if (availableFundsCashFlow.AvailableReserveFundsDictionary.Any())
                {
                    foreach (var reserveFundKeyValuePair in availableFundsCashFlow.AvailableReserveFundsDictionary)
                    {
                        var reservesReleased = reserveFundKeyValuePair.Value.ReservesReleased;
                        availableReservesReleased += reservesReleased;
                    }
                }
            }

            return availableReservesReleased;
        }

        public ReserveFund GetFirstAvailableReservesReleased(List<string> associatedReserveAccounts)
        {
            var firstAvailableFundWithReservesReleasedCashFlow = _availableFundsCashFlows
                .FirstOrDefault(f => f.ReservesReleased > 0.0);

            if (firstAvailableFundWithReservesReleasedCashFlow != null)
            {
                var lastAvailableFundWithReservesReleased = firstAvailableFundWithReservesReleasedCashFlow
                    .AvailableReserveFundsDictionary.Values
                    .Where(f => associatedReserveAccounts.Contains(f.Name))
                    .LastOrDefault(r => r.ReservesReleased > 0.0);

                // It doesn't seem possible that this could be null, given the criteria above, but let's
                // check just to cover all unknown scenarios
                if (lastAvailableFundWithReservesReleased != null)
                {
                    return lastAvailableFundWithReservesReleased;
                }
            }

            return null;
        }

        private AvailableFundsCashFlow CreateNewAvailableFundsCashFlow(
            int monthlyPeriod, 
            DateTime securitizationFirstCashFlowDate,
            Dictionary<AvailableFundsType, double> fundsAvailableDictionary)
        {
            var availableFundsCashFlow = new AvailableFundsCashFlow
            {
                Period = monthlyPeriod,
                PeriodDate = securitizationFirstCashFlowDate.AddMonths(monthlyPeriod - 1),
                BondCount = ProjectedCashFlowsOnCollateral[monthlyPeriod].BondCount,

                AvailablePrincipal = fundsAvailableDictionary[AvailableFundsType.Principal],
                AvailableInterest = fundsAvailableDictionary[AvailableFundsType.Interest],
                AvailablePrepaymentInterest = fundsAvailableDictionary[AvailableFundsType.PrepaymentInterest],
                AvailablePrepayments = fundsAvailableDictionary[AvailableFundsType.Prepayment],
                AvailablePrepaymentPenalties = fundsAvailableDictionary[AvailableFundsType.PrepaymentPenalty],
                AvailablePrincipalRecoveries = fundsAvailableDictionary[AvailableFundsType.PrincipalRecovery],
                AvailableInterestRecoveries = fundsAvailableDictionary[AvailableFundsType.InterestRecovery],

                // The ToDictionary and new calls are sufficient to create no passing by reference
                AvailableReserveFundsDictionary = this[monthlyPeriod - 1].AvailableReserveFundsDictionary
                    .ToDictionary(entry => entry.Key,
                      entry => new ReserveFund(entry.Value)
                      {
                          FundStartingBalance = entry.Value.FundEndingBalance,
                          FundEndingBalance = entry.Value.FundEndingBalance,
                          ReservesReleased = 0.0,
                          ShortfallAbsorbed = 0.0
                      })
            };

            return availableFundsCashFlow;
        }

        private Dictionary<AvailableFundsType, double> GetAvailableFundsForGivenPeriod(int monthlyPeriod)
        {
            var principal = GetAvailablePrincipalForGivenPeriod(monthlyPeriod);
            var interest = GetAvailableInterestForGivenPeriod(monthlyPeriod);
            var prepayInterest = GetAvailablePrepaymentInterestForGivenPeriod(monthlyPeriod);
            var prepayments = GetAvailablePrepaymentsForGivenPeriod(monthlyPeriod);
            var penalties = GetAvailablePrepaymentPenaltiesForGivenPeriod(monthlyPeriod);
            var prinRecoveries = GetAvailableRecoveriesForGivenPeriod(monthlyPeriod);
            var interestRecoveries = GetAvailableInterestRecoveriesForGivenPeriod(monthlyPeriod);

            var amountsAvailable = new Dictionary<AvailableFundsType, double>
            {
                [AvailableFundsType.Principal] = principal,
                [AvailableFundsType.Interest] = interest,
                [AvailableFundsType.PrepaymentInterest] = prepayInterest,
                [AvailableFundsType.Prepayment] = prepayments,
                [AvailableFundsType.PrepaymentPenalty] = penalties,
                [AvailableFundsType.PrincipalRecovery] = prinRecoveries,
                [AvailableFundsType.InterestRecovery] = interestRecoveries,
            };

            return amountsAvailable;
        }

        private Dictionary<AvailableFundsType, double> GetAvailableFundsForCleanUpCall(int monthlyPeriod)
        {
            var principal = GetAvailablePrincipalForCleanUpCall(monthlyPeriod);
            var interest = GetAvailableInterestForCleanUpCall(monthlyPeriod);
            var prepayInterest = GetAvailablePrepaymentInterestForGivenPeriod(monthlyPeriod);
            var prepayments = GetAvailablePrepaymentsForGivenPeriod(monthlyPeriod);
            var penalties = GetAvailablePrepaymentPenaltiesForGivenPeriod(monthlyPeriod);
            var prinRecoveries = GetAvailableRecoveriesForGivenPeriod(monthlyPeriod);
            var interestRecoveries = GetAvailableInterestRecoveriesForGivenPeriod(monthlyPeriod);

            var amountsAvailableForCleanUpCall = new Dictionary<AvailableFundsType, double>
            {
                [AvailableFundsType.Principal] = principal,
                [AvailableFundsType.Interest] = interest,
                [AvailableFundsType.PrepaymentInterest] = prepayInterest,
                [AvailableFundsType.Prepayment] = prepayments,
                [AvailableFundsType.PrepaymentPenalty] = penalties,
                [AvailableFundsType.PrincipalRecovery] = prinRecoveries,
                [AvailableFundsType.InterestRecovery] = interestRecoveries,
            };

            return amountsAvailableForCleanUpCall;
        }

        private double GetAvailablePrincipalForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalPrincipal = ProjectedCashFlowsOnCollateral[monthlyPeriod].Principal;
            return totalPrincipal;
        }

        private double GetAvailableInterestForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalInterest = ProjectedCashFlowsOnCollateral[monthlyPeriod].Interest;
            return totalInterest;
        }

        private double GetAvailablePrepaymentInterestForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalPrepaymentInterest = ProjectedCashFlowsOnCollateral[monthlyPeriod].PrepaymentInterest;
            return totalPrepaymentInterest;
        }

        private double GetAvailableInterestRecoveriesForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalInterestRecoveries = ProjectedCashFlowsOnCollateral[monthlyPeriod].InterestRecovery;
            return totalInterestRecoveries;
        }

        private double GetAvailablePrepaymentsForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalPrepayments = ProjectedCashFlowsOnCollateral[monthlyPeriod].Prepayment;
            return totalPrepayments;
        }

        private double GetAvailablePrepaymentPenaltiesForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var prepaymentPenalties = ProjectedCashFlowsOnCollateral[monthlyPeriod].PrepaymentPenalty;
            return prepaymentPenalties;
        }

        private double GetAvailableRecoveriesForGivenPeriod(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalRecoveries = ProjectedCashFlowsOnCollateral[monthlyPeriod].Recovery;
                                
            return totalRecoveries;
        }

        private double GetAvailablePrincipalForCleanUpCall(int monthlyPeriod)
        {
            // The open question here is whether this all counts as prepayments or not
            var totalPrincipal = ProjectedCashFlowsOnCollateral[monthlyPeriod].StartingBalance
                               - ProjectedCashFlowsOnCollateral[monthlyPeriod].Prepayment;
            return totalPrincipal;
        }

        private double GetAvailableInterestForCleanUpCall(int monthlyPeriod)
        {
            // Note: We can assume that the cash flows were created in order, and that a look up by index will work
            var totalInterest = CalculatePreviouslyAccruedInterest(monthlyPeriod);
            return totalInterest;
        }

        private double CalculatePreviouslyAccruedInterest(int monthlyPeriod)
        {
            var minimumMonthlyPeriodForLookback = 0;

            // Balloon out all remaining accrued interest
            var lastCashFlowWithAnInterestPayment = ProjectedCashFlowsOnCollateral
                .Where(p => p.Period <= monthlyPeriod)
                .LastOrDefault(c => c.Interest > 0);

            if (lastCashFlowWithAnInterestPayment != null)
            {
                minimumMonthlyPeriodForLookback = lastCashFlowWithAnInterestPayment.Period;
            }

            if (minimumMonthlyPeriodForLookback == monthlyPeriod)
            {
                return ProjectedCashFlowsOnCollateral[monthlyPeriod].Interest;
            }

            var previouslyAccruedInterest = ProjectedCashFlowsOnCollateral
                .Where(c => c.Period > minimumMonthlyPeriodForLookback
                         && c.Period <= monthlyPeriod)
                .Sum(s => s.AccruedInterest);

            return previouslyAccruedInterest;
        }
    }
}
