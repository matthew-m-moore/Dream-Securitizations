using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using System.Collections.Generic;
using System.Linq;
using Dream.Common;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration
{
    public class ProjectedCashFlowGenerator<T> : CashFlowGenerator<T>
         where T : ProjectedCashFlow
    {
        private ProjectedCashFlowLogic _projectedCashFlowLogic;
        private CashFlowGenerationInput _cashflowGenerationInput;

        public ProjectedCashFlowGenerator(ProjectedCashFlowLogic projectedCashFlowLogic, SecuritizationInput securitizationInput)
            : base(securitizationInput.UseReplines, 
                   securitizationInput.PreFundingPercentageAmount,
                   securitizationInput.BondCountPerPreFunding)
        {
            _projectedCashFlowLogic = projectedCashFlowLogic;
            _cashflowGenerationInput = securitizationInput;

            _projectedCashFlowLogic.IsPrepaymentInterestSeparate = securitizationInput.SeparatePrepaymentInterest;
        }

        public ProjectedCashFlowGenerator(ProjectedCashFlowLogic projectedCashFlowLogic, CashFlowGenerationInput cashFlowGenerationInput)
            : base(cashFlowGenerationInput.UseReplines)
        {
            _projectedCashFlowLogic = projectedCashFlowLogic;
            _cashflowGenerationInput = cashFlowGenerationInput;

            _projectedCashFlowLogic.IsPrepaymentInterestSeparate = cashFlowGenerationInput.SeparatePrepaymentInterest;
        }

        public override List<List<T>> GenerateCashFlowsOnListOfLoans(List<Loan> loans, out List<Loan> loansOrReplines)
        {
            loansOrReplines = ApplyUsageOfReplines(loans);
            loansOrReplines = ApplyUsageOfPrefunding(loansOrReplines);

            SetupSelectedPerformanceAssumptions(loansOrReplines);

            if (_projectedCashFlowLogic.CanRanInParallel)
            {
                return loansOrReplines
                    .AsParallel()
                    .WithDegreeOfParallelism(Constants.ProcessorCount)
                    .WithMergeOptions(ParallelMergeOptions.Default)
                    .Select(GenerateCashFlowsOnSingleLoan)
                    .ToList();
            }

            return loansOrReplines.Select(GenerateCashFlowsOnSingleLoan).ToList();
        }

        public override List<T> GenerateCashFlowsOnSingleLoan(Loan loan)
        {
            var contractualCashFlows = loan.GetContractualCashFlows();
            var projectedCashFlows = _projectedCashFlowLogic.ProjectCashFlows(contractualCashFlows, loan);
            return projectedCashFlows.Select(cashFlow => (T)cashFlow).ToList();
        }

        private void SetupSelectedPerformanceAssumptions(List<Loan> loans)
        {
            var selectedAssumptionsGrouping = _cashflowGenerationInput.SelectedPerformanceAssumptionGrouping ?? string.Empty;
            var selectedPerformanceAssumption = _cashflowGenerationInput.SelectedPerformanceAssumption;
            if (selectedPerformanceAssumption != null)
            {
                var performanceAssumptionsMapping = new PerformanceAssumptionsMapping();
                performanceAssumptionsMapping.SetupGlobalPerformanceAssumptionsMapping(selectedAssumptionsGrouping, selectedPerformanceAssumption, loans);

                _projectedCashFlowLogic.ProjectedPerformanceAssumptions.PerformanceAssumptionsMapping = performanceAssumptionsMapping;
            }
        }
    }
}
