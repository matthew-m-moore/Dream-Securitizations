using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class ScenarioAnalysis
    {
        private List<IScenarioLogic> _listOfScenarioLogicToApply;
        public List<IScenarioLogic> ListOfScenarioLogicToApply => _listOfScenarioLogicToApply;

        public string Description { get; private set; }
        public int SecuritizationAnalysisScenarioId { get; set; }

        public List<ScenarioAnalysis> NestedScenarios = new List<ScenarioAnalysis>();

        public ScenarioAnalysis(string scenarioDescription)
        {
            Description = scenarioDescription;
            _listOfScenarioLogicToApply = new List<IScenarioLogic>();
        }

        public ScenarioAnalysis(string scenarioDescription, List<IScenarioLogic> listOfScenarioLogicToApply)
        {
            Description = scenarioDescription;
            _listOfScenarioLogicToApply = listOfScenarioLogicToApply;
        }

        public void AddScenarioLogic(IScenarioLogic scenarioLogic)
        {
            _listOfScenarioLogicToApply.Add(scenarioLogic);
        }

        public void AddListOfScenarioLogic(List<IScenarioLogic> listOfScenarioLogic)
        {
            _listOfScenarioLogicToApply.AddRange(listOfScenarioLogic);
        }

        public Securitization ApplyScenario(Securitization securitization, SecuritizationResult baseSecuritizationResult)
        {
            var baseScenarioDescription = baseSecuritizationResult.ScenarioDescription;

            // Make sure to copy the securitization such that the original remains unaffected
            var securitizationToAlter = securitization.Copy();

            // Concatenate names for nested scenarios, if required
            if (baseScenarioDescription != string.Empty)
            {
                // Note, using a pipe for concatentation helps tremendously with parsing out the scenarios for reporting
                securitizationToAlter.Inputs.ScenarioDescription = baseScenarioDescription + "|" + Description;
            }
            else
            {
                securitizationToAlter.Inputs.ScenarioDescription = Description;
            }

            // All bits of scenario logic in this specific scenario will be applied       
            foreach(var scenarioLogic in _listOfScenarioLogicToApply)
            {
                securitizationToAlter = scenarioLogic.ApplyScenarioLogic(securitizationToAlter, baseSecuritizationResult);
            }
         
            var requiresRunningCashFlows = _listOfScenarioLogicToApply.Any(s => s.RequiresRunningCashFlows);
            var requiresLoadingCollateral = _listOfScenarioLogicToApply.Any(s => s.RequiresLoadingCollateral);

            // If there is no requirement to re-run the cash flows or load collateral in any of the scenario logic, take them from the base result
            if (!requiresRunningCashFlows && !requiresLoadingCollateral)
            {
                var projectedCashFlowsOnCollateral =
                    baseSecuritizationResult.CollateralCashFlowsResultsDictionary[AggregationGroupings.TotalAggregationGroupName].ProjectedCashFlows;

                securitizationToAlter.SetCollateralCashFlows(projectedCashFlowsOnCollateral);
            }

            // If there is no requirement to re-load collateral, take it from the base result
            if (!requiresLoadingCollateral)
            {
                if (securitization is Resecuritization resecuritization)
                {
                    var resecuritizationToAlter = securitizationToAlter as Resecuritization;
                    foreach (var securitizationName in resecuritization.CollateralizedSecuritizationsDictionary.Keys)
                    {
                        
                        var collateral = resecuritization.CollateralizedSecuritizationsDictionary[securitizationName].Collateral;
                        resecuritizationToAlter.CollateralizedSecuritizationsDictionary[securitizationName].SetCollateral(collateral);
                    }

                    securitizationToAlter = resecuritizationToAlter;
                }
                else
                {
                    var collateral = securitization.Collateral;
                    securitizationToAlter.SetCollateral(collateral);
                }
            }

            // Send back the altered securitization, but not the original
            return securitizationToAlter;
        }

        public LoanPool ApplyScenario(LoanPool loanPool, Dictionary<string, ProjectedCashFlowsSummaryResult> baseProjectedCashFlowsResultsDictionary)
        {
            var baseScenarioDescription = loanPool.Inputs.ScenarioDescription;

            // Make sure to copy the loan pool such that the original remains unaffected
            var loanPoolToAlter = loanPool.Copy();

            // Concatenate names for nested scenarios, if required
            if (baseScenarioDescription != string.Empty)
            {
                // Note, using a pipe for concatentation helps tremendously with parsing out the scenarios for reporting
                loanPoolToAlter.Inputs.ScenarioDescription = baseScenarioDescription + "|" + Description;
            }
            else
            {
                loanPoolToAlter.Inputs.ScenarioDescription = Description;
            }

            // All bits of scenario logic in this specific scenario will be applied
            foreach (var scenarioLogic in _listOfScenarioLogicToApply)
            {
                var totalProjectedCashFlowsResult = baseProjectedCashFlowsResultsDictionary[AggregationGroupings.TotalAggregationGroupName];
                loanPoolToAlter = scenarioLogic.ApplyScenarioLogic(loanPoolToAlter, totalProjectedCashFlowsResult);
            }

            // If there is no requirement to re-run the cash flows in any of the scenario logic, take them from the base result
            var requiresRunningCashFlows = _listOfScenarioLogicToApply.Any(s => s.RequiresRunningCashFlows);
            if (!requiresRunningCashFlows)
            {
                var projectedCashFlowsDictionary = baseProjectedCashFlowsResultsDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ProjectedCashFlows);
                loanPoolToAlter.SetProjectedCashFlowsDictionary(projectedCashFlowsDictionary);
            }

            // If there is no requirement to re-load collateral, take it from the base result
            var requiresLoadingCollateral = _listOfScenarioLogicToApply.Any(s => s.RequiresLoadingCollateral);
            if (!requiresLoadingCollateral)
            {
                var listOfLoans = loanPool.ListOfLoans;
                loanPoolToAlter.SetListOfLoans(listOfLoans);
            }

            // Send back the altered securitization, but not the original
            return loanPoolToAlter;
        }
    }
}
