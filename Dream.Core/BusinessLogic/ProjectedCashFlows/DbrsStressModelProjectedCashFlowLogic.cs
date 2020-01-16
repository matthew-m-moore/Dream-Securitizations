using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows.AssumptionsApplication;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.Aggregation;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows
{
    public class DbrsStressModelProjectedCashFlowLogic : ProjectedCashFlowLogic
    {
        public DbrsStressModelProjectedCashFlowLogic NextSequenceCashFlowLogic;

        public List<ProjectedCashFlow> BaseProjectedCashFlows;
        public List<List<ProjectedCashFlow>> SequenceLevelProjectedCashFlows;
        public List<ProjectedCashFlow> AggregatedSequenceLevelProjectedCashFlows;

        private DbrsStressModelInput _dbrsStressModelInput = new DbrsStressModelInput();
        private int _sequenceNumber;

        public override bool CanRanInParallel => false;

        public DbrsStressModelProjectedCashFlowLogic(
            ProjectedPerformanceAssumptions projectedPerformanceAssumptions,
            DbrsStressModelInput dbrsStressModelInput,
            string selectedAssumptionsGrouping,
            int sequenceNumber = 0) 
            : base(projectedPerformanceAssumptions, selectedAssumptionsGrouping)
        {
            _dbrsStressModelInput = dbrsStressModelInput;
            _sequenceNumber = sequenceNumber;

            SequenceLevelProjectedCashFlows = new List<List<ProjectedCashFlow>>();
            AggregatedSequenceLevelProjectedCashFlows = new List<ProjectedCashFlow>();

            CreateNextSequenceProjectedCashFlowLogic();
        }

        public override ProjectedCashFlowLogic Copy()
        {
            return new DbrsStressModelProjectedCashFlowLogic(ProjectedPerformanceAssumptions.Copy(), _dbrsStressModelInput, new string(SelectedAssumptionsGrouping.ToCharArray()))
            {
                IsPrepaymentInterestSeparate = IsPrepaymentInterestSeparate,
            };
        }

        /// <summary>
        /// Uses the DBRS Stress model to apply projected performance assumptions to the contractual cash flows
        /// of a given PACE assessment.
        /// </summary>
        public override List<ProjectedCashFlow> ProjectCashFlows(List<ContractualCashFlow> contractualCashFlows, Loan loan)
        {
            if (BaseProjectedCashFlows != null) Clear();

            var monthlyPeriodOffset = DateUtility.MonthsBetweenTwoDates(
                _dbrsStressModelInput.AssumptionsStartDate, 
                loan.InterestAccrualStartDate);

            BaseProjectedCashFlows = ProjectAllSequenceCashFlows(contractualCashFlows, loan, monthlyPeriodOffset);
            AggregateAllSequencesOfProjectedCashFlowsByDate(this, BaseProjectedCashFlows);

            var projectedCashFlowsList = CollectAllDateAlignedAggregatedSequenceCashFlows(this);
            projectedCashFlowsList.Add(BaseProjectedCashFlows);

            // Aggregate the sequence cash flows with the projected cash flows to return everything
            var aggregatedListOfProjectedCashFlows = CashFlowAggregator.AggregateCashFlows(projectedCashFlowsList);
            CleanUpProjectedCashFlows(aggregatedListOfProjectedCashFlows);
            return aggregatedListOfProjectedCashFlows;
        }

        private void CleanUpProjectedCashFlows(List<ProjectedCashFlow> aggregatedListOfProjectedCashFlows)
        {
            var totalLoanCount = aggregatedListOfProjectedCashFlows.First().Count;
            foreach (var projectedCashFlow in aggregatedListOfProjectedCashFlows)
            {
                if (projectedCashFlow.StartingBalance <= 0.0)
                {
                    projectedCashFlow.Count = 0.0;
                    projectedCashFlow.BondCount = 0.0;
                    continue;
                }

                projectedCashFlow.Count /= totalLoanCount;
                projectedCashFlow.BondCount /= totalLoanCount;
            }
        }

        private List<ProjectedCashFlow> ProjectAllSequenceCashFlows(List<ContractualCashFlow> contractualCashFlows, Loan loan,
            int monthlyPeriodOffset = 0)
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
            var originalStartingBalance = zerothPeriodProjectedCashFlow.StartingBalance;
            var dbrsStressModelStandards = new DbrsStressModelStandards(
                accruedInterest,
                originalStartingBalance,
                _dbrsStressModelInput.StateLevelDefaultRateDictionary[loan.PropertyState]);

            for (var monthlyPeriod = 1; monthlyPeriod < monthlyPeriodsToProject; monthlyPeriod++)
            {
                var currentPeriodContractualCashFlow = contractualCashFlows[monthlyPeriod];

                var performanceAssumptionsPeriod = monthlyPeriod + monthlyPeriodOffset - _sequenceNumber;
                var singleMonthlyMortality = performanceCurveDictionary[PerformanceCurveType.Smm][performanceAssumptionsPeriod];
                var monthlyDelinquencyRate = performanceCurveDictionary[PerformanceCurveType.Dq][performanceAssumptionsPeriod];
                var lossGivenDefault = _dbrsStressModelInput.StateLevelLossGivenDefaultDictionary[loan.PropertyState] ?? performanceCurveDictionary[PerformanceCurveType.Lgd][performanceAssumptionsPeriod];

                // This addresses a pro-rating of pre-payments present only in the DBRS model approach
                if (monthlyPeriod == 1 && loan is PaceAssessment paceAssessment)
                {
                    if (paceAssessment.PreFundingPrepaymentProRating.HasValue)
                        singleMonthlyMortality *= paceAssessment.PreFundingPrepaymentProRating.Value;
                }

                // For higher sequences (i.e. deeper layers), the deliquency will either be 100% or zero.
                // And, prepayments will not occur during the deliquncy period.
                if (_sequenceNumber > 0)
                {
                    monthlyDelinquencyRate = ((monthlyPeriod == _dbrsStressModelInput.TotalMonthsToNextDefault(loan.PropertyState)) && 
                                             (_sequenceNumber < _dbrsStressModelInput.TotalNumberOfDefaultSequences)) ? 1.0 : 0.0;
                    singleMonthlyMortality *= (monthlyPeriod > _dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[loan.PropertyState]) ? 1.0 : 0.0;
                }

                var currentPeriodProjectedCashFlow = dbrsStressModelStandards.Apply(
                    currentPeriodContractualCashFlow,
                    startingProjectedBalanceFactor,
                    singleMonthlyMortality,
                    monthlyDelinquencyRate,
                    out double endingProjectedBalanceFactor);

                currentPeriodProjectedCashFlow.Count *= endingProjectedBalanceFactor;
                startingProjectedBalanceFactor = endingProjectedBalanceFactor;
                ApplyPrepaymentPenalties(currentPeriodProjectedCashFlow, prepaymentPenaltySchedule);

                // Payments projected within the deliquency period will be missed, and counted as defaults.
                if (_sequenceNumber > 0 && (monthlyPeriod <= _dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[loan.PropertyState]))
                {
                    currentPeriodProjectedCashFlow.Default = currentPeriodProjectedCashFlow.Principal;
                    currentPeriodProjectedCashFlow.Loss = lossGivenDefault * currentPeriodProjectedCashFlow.Default;
                    currentPeriodProjectedCashFlow.Principal = 0.0;

                    currentPeriodProjectedCashFlow.InterestDefault = currentPeriodProjectedCashFlow.Interest;
                    currentPeriodProjectedCashFlow.InterestLoss = lossGivenDefault * currentPeriodProjectedCashFlow.InterestDefault;

                    currentPeriodProjectedCashFlow.Interest = (monthlyPeriod == loan.MonthsToNextInterestPayment)
                        ? -1.0 * currentPeriodProjectedCashFlow.InterestDefault
                        :  0.0;
                }

                listOfProjectedCashFlows.Add(currentPeriodProjectedCashFlow);
                AddNextSequenceOfProjectedCashFlows(listOfProjectedCashFlows, loan, monthlyPeriod, monthlyPeriodOffset);
                SeparateOutPrepaymentInterest(contractualCashFlows, listOfProjectedCashFlows, loan, monthlyPeriod);
            }

            ApplyLaggedRecoveryOfDefaults(listOfProjectedCashFlows, loan.PropertyState);
            return listOfProjectedCashFlows;
        }

        // Recoveries are lagged to the end of the deliquency period
        private void ApplyLaggedRecoveryOfDefaults(List<ProjectedCashFlow> listOfProjectedCashFlows, PropertyState propertyState)
        {
            var totalDefault = listOfProjectedCashFlows.Sum(c => c.Default);
            var totalLoss = listOfProjectedCashFlows.Sum(c => c.Loss);
            var totalRecovery = totalDefault - totalLoss;

            var totalInterestDefault = listOfProjectedCashFlows.Sum(c => c.InterestDefault);
            var totalInterestLoss = listOfProjectedCashFlows.Sum(c => c.InterestLoss);
            var totalInterestRecovery = totalInterestDefault - totalInterestLoss;

            // Padding will need to added when the recovery lag extends beyond the last cash flow
            if (listOfProjectedCashFlows.Count <= (_dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[propertyState] + 1))
            {
                var numberOfPaddedCashFlowsNeeded = (_dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[propertyState] + 1) - listOfProjectedCashFlows.Count;
                for (var paddedCashFlowCount = 0; paddedCashFlowCount <= numberOfPaddedCashFlowsNeeded; paddedCashFlowCount++)
                {
                    var lastProjectedCashFlow = listOfProjectedCashFlows.Last();
                    var paddedProjectectCashFlow = new ProjectedCashFlow
                    {
                        Period = lastProjectedCashFlow.Period + 1,
                        PeriodDate = lastProjectedCashFlow.PeriodDate.AddMonths(1),
                        Count = 0,
                    };

                    listOfProjectedCashFlows.Add(paddedProjectectCashFlow);
                }
            }

            foreach (var projectedCashFlow in listOfProjectedCashFlows) { projectedCashFlow.Loss = 0.0; projectedCashFlow.InterestLoss = 0.0; }

            listOfProjectedCashFlows[_dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[propertyState] + 1].Recovery = totalRecovery;
            listOfProjectedCashFlows[_dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[propertyState] + 1].Loss = totalLoss;

            listOfProjectedCashFlows[_dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[propertyState] + 1].InterestRecovery = totalInterestRecovery;
            listOfProjectedCashFlows[_dbrsStressModelInput.StateLevelForeclosureTermInMonthsDictionary[propertyState] + 1].InterestLoss = totalInterestLoss;
        }

        // If the sequence number is less than the total number of sequences, and there is delinquent principal, go to the next layer
        private void AddNextSequenceOfProjectedCashFlows(List<ProjectedCashFlow> listOfProjectedCashFlows, Loan loan, int monthlyPeriod, 
            int monthlyPeriodOffset = 0)
        {
            var currentPeriodProjectedCashFlow = listOfProjectedCashFlows[monthlyPeriod];
            var previousPeriodProjectedCashFlow = listOfProjectedCashFlows[monthlyPeriod - 1];

            if (_sequenceNumber < _dbrsStressModelInput.TotalNumberOfDefaultSequences &&
                currentPeriodProjectedCashFlow.DelinquentPrincipal > 0.0)
            {
                var copiedLoan = loan.Copy();
                var newStartDate = copiedLoan.StartDate.AddMonths(monthlyPeriod);
          
                copiedLoan.StartDate = newStartDate.AddMonths(-1);
                copiedLoan.FirstPaymentDate = newStartDate;
                copiedLoan.Balance = currentPeriodProjectedCashFlow.DelinquentPrincipal;
                copiedLoan.InitialCouponRate = GetLoanCouponRate(copiedLoan, previousPeriodProjectedCashFlow);

                if (copiedLoan is PaceAssessment copiedPaceAssessment)
                {
                    copiedPaceAssessment.PreFundingStartDate = null;
                    copiedPaceAssessment.PreFundingPrepaymentProRating = null;
                    copiedLoan = copiedPaceAssessment;
                }

                copiedLoan.AdjustLoanToCashFlowStartDate(loan.StartDate);
                copiedLoan.InterestAccrualStartDate = newStartDate;
                AddAccruedInterestToCopiedLoan(copiedLoan);

                var contractualCashFlows = copiedLoan.GetContractualCashFlows();
                var projectedCashFlows = NextSequenceCashFlowLogic
                    .ProjectAllSequenceCashFlows(contractualCashFlows, copiedLoan, monthlyPeriod + monthlyPeriodOffset);

                SequenceLevelProjectedCashFlows.Add(projectedCashFlows);
            }
        }

        private void AddAccruedInterestToCopiedLoan(Loan copiedLoan)
        {
            var monthsOfAccruedInterestToAdd = copiedLoan.InterestPaymentFrequencyInMonths - copiedLoan.MonthsToNextInterestPayment;

            var couponRate = copiedLoan.InitialCouponRate;

            var timePeriodInYears = DateUtility.CalculateTimePeriodInYears(
                copiedLoan.InterestAccrualDayCountConvention,
                copiedLoan.InterestAccrualStartDate.AddMonths(-1 * copiedLoan.InterestPaymentFrequencyInMonths),
                copiedLoan.InterestAccrualStartDate);

            var accruedInterestFactor = MathUtility.CalculateSimplyCompoundedInterestAccrualFactor(
                timePeriodInYears,
                couponRate);

            copiedLoan.AccruedInterest = copiedLoan.Balance * accruedInterestFactor;
        }

        private void AggregateAllSequencesOfProjectedCashFlowsByDate(
            DbrsStressModelProjectedCashFlowLogic dbrsStressModelProjectedCashFlowLogic,
            List<ProjectedCashFlow> listOfProjectedCashFlows)
        {
            var startState = listOfProjectedCashFlows.First().PeriodDate;
            var firstCashFlowDate = listOfProjectedCashFlows.Skip(1).First().PeriodDate;

            var dateAlignedCashFlowsList = new List<List<ProjectedCashFlow>>();
            foreach(var sequenceLevelCashFlows in dbrsStressModelProjectedCashFlowLogic.SequenceLevelProjectedCashFlows)
            {
                var copiedSequenceLevelCashFlows = 
                    sequenceLevelCashFlows.Select(c => c.Copy() as ProjectedCashFlow).ToList();

                copiedSequenceLevelCashFlows.First().Clear();
                copiedSequenceLevelCashFlows.First().StartingBalance = 0.0;
                copiedSequenceLevelCashFlows.First().EndingBalance = 0.0;

                copiedSequenceLevelCashFlows.Skip(1).First().StartingBalance = 0.0;

                var dateAlignedCashFlows = CashFlowAggregator.AggregateCashFlowsByDate(
                    copiedSequenceLevelCashFlows,
                    startState,
                    firstCashFlowDate);

                dateAlignedCashFlowsList.Add(dateAlignedCashFlows);
            }

            // Store these aggregated cash-flows for diplay and debugging purposes later
            dbrsStressModelProjectedCashFlowLogic.AggregatedSequenceLevelProjectedCashFlows = CashFlowAggregator.AggregateCashFlows(dateAlignedCashFlowsList);

            var nextSequenceAggregatedProjectedCashFlows = new List<ProjectedCashFlow>();
            if (dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic != null &&
                dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic.SequenceLevelProjectedCashFlows.Any())
            {
                AggregateAllSequencesOfProjectedCashFlowsByDate(
                    dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic, 
                    listOfProjectedCashFlows);
            }
        }

        private List<List<ProjectedCashFlow>> CollectAllDateAlignedAggregatedSequenceCashFlows(DbrsStressModelProjectedCashFlowLogic dbrsStressModelProjectedCashFlowLogic)
        {
            var aggregagatedCashFlowsList = new List<List<ProjectedCashFlow>>();

            if (dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic != null &&
                dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic.SequenceLevelProjectedCashFlows.Any())
            {
                aggregagatedCashFlowsList = CollectAllDateAlignedAggregatedSequenceCashFlows(dbrsStressModelProjectedCashFlowLogic.NextSequenceCashFlowLogic);
            }

            aggregagatedCashFlowsList.Add(dbrsStressModelProjectedCashFlowLogic.AggregatedSequenceLevelProjectedCashFlows);
            return aggregagatedCashFlowsList;
        }

        protected override void SetSeparationOfPrepaymentInterest(bool value)
        {
            _IsPrepaymentInterestSeparate = value;

            if (NextSequenceCashFlowLogic != null)
            {
                NextSequenceCashFlowLogic.IsPrepaymentInterestSeparate = value;
            }
        }

        protected override void SeparateOutPrepaymentInterest(
            List<ContractualCashFlow> contractualCashFlows,
            List<ProjectedCashFlow> listOfProjectedCashFlows,
            Loan loan, int monthlyPeriod)
        {
            if (monthlyPeriod >= loan.MonthsToNextInterestPayment)
            {
                base.SeparateOutPrepaymentInterest(
                    contractualCashFlows,
                    listOfProjectedCashFlows,
                    loan, monthlyPeriod);
            }
            else
            {
                var preAnalysisInterest = listOfProjectedCashFlows.First().AccruedInterest;
                if (_IsPrepaymentInterestSeparate && preAnalysisInterest > 0.0)
                {
                    var currentPeriodProjectedCashFlow = listOfProjectedCashFlows[monthlyPeriod];
                    var singleMonthlyMortality = currentPeriodProjectedCashFlow.Prepayment / 
                        (currentPeriodProjectedCashFlow.StartingBalance - currentPeriodProjectedCashFlow.Principal);

                    var accuredInterestBalance = listOfProjectedCashFlows.Where(c => c.Period < monthlyPeriod).Sum(a => a.AccruedInterest);
                    var prepaymentInterest = accuredInterestBalance * singleMonthlyMortality;

                    currentPeriodProjectedCashFlow.PrepaymentInterest = prepaymentInterest;
                    currentPeriodProjectedCashFlow.AccruedInterest -= prepaymentInterest;
                }
            }
        }

        protected override int GetMonthsSinceLastInterestPayment(Loan loan, int monthlyPeriod)
        {
            return loan.InterestPaymentFrequencyInMonths - loan.MonthsToNextInterestPayment + monthlyPeriod;
        }

        private void CreateNextSequenceProjectedCashFlowLogic()
        {
            if (_sequenceNumber < _dbrsStressModelInput.TotalNumberOfDefaultSequences)
            {
                NextSequenceCashFlowLogic = new DbrsStressModelProjectedCashFlowLogic(
                    ProjectedPerformanceAssumptions,
                    _dbrsStressModelInput,
                    SelectedAssumptionsGrouping,
                    _sequenceNumber + 1);
            }
        }

        private void Clear()
        {
            BaseProjectedCashFlows = new List<ProjectedCashFlow>();
            SequenceLevelProjectedCashFlows = new List<List<ProjectedCashFlow>>();
            AggregatedSequenceLevelProjectedCashFlows = new List<ProjectedCashFlow>();

            if (NextSequenceCashFlowLogic != null) NextSequenceCashFlowLogic.Clear();
        }
    }
}
