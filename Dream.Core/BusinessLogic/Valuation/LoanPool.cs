using Dream.Common;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.Interfaces;
using Dream.Core.Reporting.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Valuation
{
    public class LoanPool
    {
        private Dictionary<string, List<ProjectedCashFlow>> _projectedCashFlowsDictionary;

        public ICollateralRetriever ListOfLoansRetriever { get; set; }
        public CashFlowGenerationInput Inputs { get; set; }
        public AggregationGroupings AggregationGroupings { get; set; }
        public ProjectedCashFlowLogic ProjectedCashFlowLogic { get; set; }

        private List<Loan> _listOfLoans;
        public List<Loan> ListOfLoans
        {
            get
            {
                if (_listOfLoans == null)
                {
                    ListOfLoansRetriever.SetCollateralDates(Inputs);
                    _listOfLoans = ListOfLoansRetriever.GetCollateral();
                }

                return _listOfLoans;
            }
        }

        public LoanPool(
            ICollateralRetriever listOfLoansInLoanPoolRetriever,
            CashFlowGenerationInput cashFlowGenerationInputs,
            AggregationGroupings aggregationGroupings,
            ProjectedCashFlowLogic projectedCashFlowLogic,
            Dictionary<string, List<ProjectedCashFlow>> projectedCashFlowsDictionary = null)
        {
            ListOfLoansRetriever = listOfLoansInLoanPoolRetriever;
            Inputs = cashFlowGenerationInputs;
            AggregationGroupings = aggregationGroupings;
            ProjectedCashFlowLogic = projectedCashFlowLogic;

            _projectedCashFlowsDictionary = projectedCashFlowsDictionary;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public LoanPool Copy()
        {
            var loanPool = new LoanPool(
                ListOfLoansRetriever.Copy(),
                Inputs.Copy(),
                AggregationGroupings.Copy(),
                ProjectedCashFlowLogic.Copy(),
                _projectedCashFlowsDictionary);

            return loanPool;
        }

        /// <summary>
        /// Generates the contractual cash flows for all loans in the loan pool, and aggregates them accordingly.
        /// </summary>
        public Dictionary<string, List<ContractualCashFlow>> GenerateContractualCashFlows()
        {
            // Generate the contractual cash flows for the given list of loans
            var contractualCashFlowGenerator = new ContractualCashFlowGenerator<ContractualCashFlow>(Inputs);
            var listOfContractualCashFlows = contractualCashFlowGenerator.GenerateCashFlowsOnListOfLoans(ListOfLoans, out List<Loan> loansOrReplines);

            // Aggregate the cash flows in total
            SetupAggregationGroupings(loansOrReplines);
            var selectedAggregationGrouping = Inputs.SelectedAggregationGrouping;
            var cashFlowAggregator = new CashFlowAggregator(Inputs, AggregationGroupings, selectedAggregationGrouping);
            var totalCashFlow = cashFlowAggregator.AggregateTotalCashFlows(loansOrReplines, listOfContractualCashFlows);

            // Aggregate the cash flows by the selected aggregation group
            var contractualCashFlowDictionary = cashFlowAggregator.AggregateCashFlows(loansOrReplines, listOfContractualCashFlows);
            contractualCashFlowDictionary.Add(totalCashFlow);

            return contractualCashFlowDictionary;
        }

        /// <summary>
        /// Generates the projected cash flows for all loans in the loan pool, and aggregates them accordingly.
        /// </summary>
        public Dictionary<string, List<ProjectedCashFlow>> GenerateProjectedCashFlows()
        {
            // Generate the contractual and projected cash flows for the given list of loans
            var projectedCashFlowGenerator = new ProjectedCashFlowGenerator<ProjectedCashFlow>(ProjectedCashFlowLogic, Inputs);
            var listOfProjectedCashFlows = projectedCashFlowGenerator.GenerateCashFlowsOnListOfLoans(ListOfLoans, out List<Loan> loansOrReplines);

            // Aggregate the cash flows in total
            SetupAggregationGroupings(loansOrReplines);
            var selectedAggregationGrouping = Inputs.SelectedAggregationGrouping;
            var cashFlowAggregator = new CashFlowAggregator(Inputs, AggregationGroupings, selectedAggregationGrouping);
            var totalCashFlow = cashFlowAggregator.AggregateTotalCashFlows(loansOrReplines, listOfProjectedCashFlows);

            // Aggregate the cash flows by the selected aggregation group
            var projectedCashFlowDictionary = cashFlowAggregator.AggregateCashFlows(loansOrReplines, listOfProjectedCashFlows);
            projectedCashFlowDictionary.Add(totalCashFlow);

            _projectedCashFlowsDictionary = projectedCashFlowDictionary;
            return projectedCashFlowDictionary;
        }

        /// <summary>
        /// Provides a dictionary of results from analyzing a pool of projected cash flows, grouped accordingly.
        /// </summary>
        public Dictionary<string, ProjectedCashFlowsSummaryResult> AnalyzeProjectedCashFlows()
        {
            if (_projectedCashFlowsDictionary == null) GenerateProjectedCashFlows();

            var projectedCashFlowsAnalysis = new ProjectedCashFlowsAnalysis(Inputs.CashFlowPricingStrategy, _projectedCashFlowsDictionary);
            var dictionaryOfResults = projectedCashFlowsAnalysis.RunAnalysis(
                Inputs.MarketRateEnvironment,
                Inputs.MarketDataGroupingForNominalSpread,
                Inputs.CurveTypeForSpreadCalcultion);

            return dictionaryOfResults;
        }

        /// <summary>
        /// Provides a dictionary of results from analyzing a pool of projected cash flows, grouped accordingly.
        /// </summary>
        public Dictionary<string, ProjectedCashFlowsSummaryResult> AnalyzeContractualCashFlows()
        {
            var contractualCashFlowsDictionary = GenerateContractualCashFlows();

            var projectedCashFlowsAnalysis = new ProjectedCashFlowsAnalysis(Inputs.CashFlowPricingStrategy, _projectedCashFlowsDictionary);
            var dictionaryOfResults = projectedCashFlowsAnalysis.RunAnalysis(
                Inputs.MarketRateEnvironment,
                Inputs.MarketDataGroupingForNominalSpread,
                Inputs.CurveTypeForSpreadCalcultion);

            return dictionaryOfResults;
        }

        /// <summary>
        /// Runs a specified collection of scenarios and produces a dictionary of a dictionary of results.
        /// </summary>
        public Dictionary<string, Dictionary<string, ProjectedCashFlowsSummaryResult>> AnalyzeProjectedCashFlows(List<ScenarioAnalysis> scenariosToAnalyze)
        {
            var scenarioDictionaryOfResults = new Dictionary<string, Dictionary<string, ProjectedCashFlowsSummaryResult>>();

            // Running this securitization may alter it, such as when the redemption priority of payments kicks in
            var baseLoanPool = Copy();
            var baseProjectedCashFlowsResultsDictionary = baseLoanPool.AnalyzeProjectedCashFlows();
            scenarioDictionaryOfResults.Add(Inputs.ScenarioDescription, baseProjectedCashFlowsResultsDictionary);
            Console.WriteLine("Ran '" + Inputs.ScenarioDescription + "' Scenario");

            if (!scenariosToAnalyze.Any()) return scenarioDictionaryOfResults;

            foreach (var scenario in scenariosToAnalyze)
            {
                var loanPoolScenario = scenario.ApplyScenario(this, baseProjectedCashFlowsResultsDictionary);
                var nestedScenarioResultsDictionary = loanPoolScenario.AnalyzeProjectedCashFlows(scenario.NestedScenarios);
                scenarioDictionaryOfResults.Combine(nestedScenarioResultsDictionary);
            }

            return scenarioDictionaryOfResults;
        }

        /// <summary>
        /// This method can be used to set the projected cash flows to some specific values, rather than generating them from the loan pool supplied.
        /// This option is more commonly used when running scenario analyses.
        /// </summary>
        public void SetProjectedCashFlowsDictionary(Dictionary<string, List<ProjectedCashFlow>> projectedCashFlowsDictionary)
        {
            _projectedCashFlowsDictionary = projectedCashFlowsDictionary;
        }

        /// <summary>
        /// This method can be used to set the list of loans explicitely, rather than getting it from the collateral retriever.
        /// This option is more commonly used when running scenario analyses, or for re-securitization analysis.
        /// </summary>
        public void SetListOfLoans(List<Loan> listOfLoans)
        {
            _listOfLoans = listOfLoans;
        }

        private void SetupAggregationGroupings(List<Loan> loansOrReplines)
        {
            var selectedAggregationGrouping = Inputs.SelectedAggregationGrouping;
            if (selectedAggregationGrouping == null || selectedAggregationGrouping == Constants.Automatic || Inputs.UseReplines)
            {
                AggregationGroupings = AggregationGroupings.SetupAutomaticLoanLevelAggregation(loansOrReplines);
                Inputs.SelectedAggregationGrouping = AggregationGroupings.LoanLevelAggregationGroupingIdentifier;
            }
        }
    }
}
