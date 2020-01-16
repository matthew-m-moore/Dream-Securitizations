using Dream.Common.Enums;
using Dream.Core.BusinessLogic.ProjectedCashFlows.AssumptionsApplication;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using System.Linq;
using Dream.Common.Curves;
using System;
using Dream.Common.Utilities;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows
{
    public class ProjectedCashFlowLogic
    {
        public static readonly List<PerformanceCurveType> ListOfPerformanceCurveTypes = 
            new List<PerformanceCurveType>
            {
                PerformanceCurveType.Smm,
                PerformanceCurveType.Mdr,
                PerformanceCurveType.Lgd,
                PerformanceCurveType.Dq
            };

        public ProjectedPerformanceAssumptions ProjectedPerformanceAssumptions { get; set; }
        public string SelectedAssumptionsGrouping { get; set; }
        public virtual bool CanRanInParallel => true;

        protected bool _IsPrepaymentInterestSeparate;
        public bool IsPrepaymentInterestSeparate
        {
            get { return _IsPrepaymentInterestSeparate; }
            set { SetSeparationOfPrepaymentInterest(value); }
        }

        public ProjectedCashFlowLogic(ProjectedPerformanceAssumptions projectedPerformanceAssumptions, string selectedAssumptionGrouping)
        {
            ProjectedPerformanceAssumptions = projectedPerformanceAssumptions;
            SelectedAssumptionsGrouping = selectedAssumptionGrouping ?? string.Empty;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public virtual ProjectedCashFlowLogic Copy()
        {
            return new ProjectedCashFlowLogic(ProjectedPerformanceAssumptions.Copy(), new string(SelectedAssumptionsGrouping.ToCharArray()))
            {
                IsPrepaymentInterestSeparate = _IsPrepaymentInterestSeparate,
            };
        }

        /// <summary>
        /// Uses Bond Market Association (BMA) standards to apply projected performance assumptions to the contractual cash flows
        /// of a given loan. Also applies any prepayment penalties that may be associated with the loan.
        /// </summary>
        public virtual List<ProjectedCashFlow> ProjectCashFlows(List<ContractualCashFlow> contractualCashFlows, Loan loan)
        {
            var listOfProjectedCashFlows = new List<ProjectedCashFlow>();

            if (!contractualCashFlows.Any())
            {
                return listOfProjectedCashFlows;
            }

            var prepaymentPenaltySchedule = GetPrepaymentPenaltySchedule(loan);
            var zerothPeriodProjectedCashFlow = PrepareZerothPeriodProjectedCashFlow(contractualCashFlows, loan);
            listOfProjectedCashFlows.Add(zerothPeriodProjectedCashFlow);

            var startingProjectedBalanceFactor = 1.0;
            var monthlyPeriodsToProject = contractualCashFlows.Count();

            var assumptionsIdentifer = GetAssumptionsIdentifer(loan);
            var performanceCurveDictionary = ProjectedPerformanceAssumptions.GetPerformanceCurves(
                ListOfPerformanceCurveTypes,
                SelectedAssumptionsGrouping,
                assumptionsIdentifer);

            var accruedInterest = zerothPeriodProjectedCashFlow.AccruedInterest;
            var bondMarketAssociationStandards = new BondMarketAssociationStandards(accruedInterest);

            for (var monthlyPeriod = 1; monthlyPeriod < monthlyPeriodsToProject; monthlyPeriod++)
            {
                var currentPeriodContractualCashFlow = contractualCashFlows[monthlyPeriod];

                var singleMonthlyMortality = performanceCurveDictionary[PerformanceCurveType.Smm][monthlyPeriod];
                var monthlyDelinquencyRate = performanceCurveDictionary[PerformanceCurveType.Dq][monthlyPeriod];
                var monthlyDefaultRate = performanceCurveDictionary[PerformanceCurveType.Mdr][monthlyPeriod];
                var lossGivenDefault = performanceCurveDictionary[PerformanceCurveType.Lgd][monthlyPeriod];

                var currentPeriodProjectedCashFlow = bondMarketAssociationStandards.Apply(
                    currentPeriodContractualCashFlow,
                    startingProjectedBalanceFactor,
                    singleMonthlyMortality,
                    monthlyDelinquencyRate,
                    monthlyDefaultRate,
                    lossGivenDefault,
                    out double endingProjectedBalanceFactor);

                currentPeriodProjectedCashFlow.Count *= endingProjectedBalanceFactor;
                startingProjectedBalanceFactor = endingProjectedBalanceFactor;
                ApplyPrepaymentPenalties(currentPeriodProjectedCashFlow, prepaymentPenaltySchedule);              

                listOfProjectedCashFlows.Add(currentPeriodProjectedCashFlow);
                SeparateOutPrepaymentInterest(contractualCashFlows, listOfProjectedCashFlows, loan, monthlyPeriod);
            }

            return listOfProjectedCashFlows;
        }

        protected virtual string GetAssumptionsIdentifer(Loan loan)
        {
            return loan.StringId;
        }

        protected ProjectedCashFlow PrepareZerothPeriodProjectedCashFlow(List<ContractualCashFlow> contractualCashFlows, Loan loan)
        {
            var zerothPeriodProjectedCashFlow = new ProjectedCashFlow(contractualCashFlows.First());

            zerothPeriodProjectedCashFlow.Prepayment = loan.ActualPrepayments;
            zerothPeriodProjectedCashFlow.StartingBalance += loan.ActualPrepayments;

            return zerothPeriodProjectedCashFlow;
        }

        protected Curve<double> GetPrepaymentPenaltySchedule(Loan loan)
        {
            if (loan.PrepaymentPenaltyPlan == null) return new Curve<double>(0.0);

            var prepaymentSchedule = loan.PrepaymentPenaltyPlan.GeneratePenaltySchedule(loan.SeasoningInMonths);
            return prepaymentSchedule;
        }

        protected void ApplyPrepaymentPenalties(ProjectedCashFlow currentPeriodProjectedCashFlow, Curve<double> prepaymentPenaltySchedule)
        {
            var monthlyPeriod = currentPeriodProjectedCashFlow.Period;
            var prepaymentPenalty = prepaymentPenaltySchedule[monthlyPeriod] * currentPeriodProjectedCashFlow.Prepayment;

            currentPeriodProjectedCashFlow.PrepaymentPenalty = prepaymentPenalty;
        }

        protected virtual void SetSeparationOfPrepaymentInterest(bool value)
        {
            _IsPrepaymentInterestSeparate = value;
        }

        protected virtual void SeparateOutPrepaymentInterest(
            List<ContractualCashFlow> contractualCashFlows, 
            List<ProjectedCashFlow> listOfProjectedCashFlows, 
            Loan loan, int monthlyPeriod)
        {
            if (_IsPrepaymentInterestSeparate)
            {
                var currentPeriodProjectedCashFlow = listOfProjectedCashFlows[monthlyPeriod];
                if (currentPeriodProjectedCashFlow.Interest > 0.0)
                {
                    var monthlyPeriodOfFirstAccrual = currentPeriodProjectedCashFlow.Period - loan.InterestPaymentFrequencyInMonths;
                    monthlyPeriodOfFirstAccrual = Math.Max(0, monthlyPeriodOfFirstAccrual);

                    currentPeriodProjectedCashFlow.Interest = listOfProjectedCashFlows
                        .Where(c => c.Period >= monthlyPeriodOfFirstAccrual && c.Period < monthlyPeriod)
                        .Sum(a => a.AccruedInterest);
                    return;
                }

                var previousPeriodProjectedCashFlow = listOfProjectedCashFlows[monthlyPeriod];
                var couponRate = GetLoanCouponRate(loan, previousPeriodProjectedCashFlow);

                var contractualCashFlowOfLastInterestPayment = 
                    contractualCashFlows.Where(c => c.Period < monthlyPeriod && c.Interest > 0.0).LastOrDefault();

                var monthsSinceLastInterestPayment = GetMonthsSinceLastInterestPayment(loan, monthlyPeriod);
                if (contractualCashFlowOfLastInterestPayment != null)
                {
                    monthsSinceLastInterestPayment = monthlyPeriod - contractualCashFlowOfLastInterestPayment.Period;
                }

                var timePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                    loan.InterestAccrualDayCountConvention,
                    loan.InterestAccrualStartDate.AddMonths(-1 * monthsSinceLastInterestPayment),
                    loan.InterestAccrualStartDate);

                var accruedInterestFactor = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(
                    timePeriodInYears,
                    couponRate);

                var prepaymentInterest = currentPeriodProjectedCashFlow.Prepayment * accruedInterestFactor;

                currentPeriodProjectedCashFlow.PrepaymentInterest = prepaymentInterest;
                currentPeriodProjectedCashFlow.AccruedInterest -= prepaymentInterest;
            }
        }

        protected double GetLoanCouponRate(Loan loan, ProjectedCashFlow previousPeriodProjectedCashFlow)
        {
            if (previousPeriodProjectedCashFlow.Period <= 0) return loan.InitialCouponRate;

            var monthlyTimePeriodInYears = DateUtility.CalculateTimePeriodInYearsForOneMonth(
                loan.InterestAccrualDayCountConvention,
                previousPeriodProjectedCashFlow.PeriodDate);

            var totalAccruedInterest = previousPeriodProjectedCashFlow.AccruedInterest + previousPeriodProjectedCashFlow.PrepaymentInterest;
            var monthlyCouponRate = (previousPeriodProjectedCashFlow.EndingBalance > 0.0)
                ? totalAccruedInterest / previousPeriodProjectedCashFlow.EndingBalance
                : 0.0;

            var annualCouponRate = monthlyCouponRate / monthlyTimePeriodInYears;
            return annualCouponRate;
        }

        protected virtual int GetMonthsSinceLastInterestPayment(Loan loan, int monthlyPeriod)
        {
            return (loan.AccruedInterest <= 0.0)
                ? loan.InterestPaymentFrequencyInMonths - loan.MonthsToNextInterestPayment + monthlyPeriod
                : monthlyPeriod - 1;
        }
    }
}
