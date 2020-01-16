using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Reporting.Results;
using Dream.Core.BusinessLogic.Valuation;

namespace Dream.Core.BusinessLogic.Scenarios
{
    public class InputsScenario : IScenarioLogic
    {
        private SecuritizationInput _scenarioSecuritizationInputs;
        public SecuritizationInput ScenarioSecuritizationInputs => _scenarioSecuritizationInputs;

        private CashFlowGenerationInput _scenarioCashFlowGenerationInputs;
        public CashFlowGenerationInput ScenarioCashFlowGenerationInputs => _scenarioCashFlowGenerationInputs;

        private bool _requiresRunningCashFlows;
        public bool RequiresRunningCashFlows => _requiresRunningCashFlows;

        private bool _requiresLoadingCollateral;
        public bool RequiresLoadingCollateral => _requiresLoadingCollateral;

        public InputsScenario(SecuritizationInput scenarioInputs)
        {
            _scenarioSecuritizationInputs = scenarioInputs;
        }

        public InputsScenario(CashFlowGenerationInput cashFlowGenerationInputs)
        {
            _scenarioCashFlowGenerationInputs = cashFlowGenerationInputs;
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            SetRequiresRunningCashFlows(securitization.Inputs);
            SetRequiresLoadingCollateral(securitization.Inputs);

            // Note that the securitization inputs are not replaced by the entire scenario object.
            securitization.Inputs.ScenarioDescription = _scenarioSecuritizationInputs.ScenarioDescription;
            securitization.Inputs.AdditionalYieldScenarioDescription = _scenarioSecuritizationInputs.AdditionalYieldScenarioDescription;

            securitization.Inputs.CollateralCutOffDate = _scenarioSecuritizationInputs.CollateralCutOffDate;
            securitization.Inputs.CashFlowStartDate = _scenarioSecuritizationInputs.CashFlowStartDate;
            securitization.Inputs.InterestAccrualStartDate = _scenarioSecuritizationInputs.InterestAccrualStartDate;

            securitization.Inputs.SecuritizationStartDate = _scenarioSecuritizationInputs.SecuritizationStartDate;
            securitization.Inputs.SecuritizationFirstCashFlowDate = _scenarioSecuritizationInputs.SecuritizationFirstCashFlowDate;

            securitization.Inputs.SelectedAggregationGrouping = _scenarioSecuritizationInputs.SelectedAggregationGrouping;
            securitization.Inputs.SelectedPerformanceAssumption = _scenarioSecuritizationInputs.SelectedPerformanceAssumption;

            securitization.Inputs.PreFundingPercentageAmount = _scenarioSecuritizationInputs.PreFundingPercentageAmount;
            securitization.Inputs.BondCountPerPreFunding = _scenarioSecuritizationInputs.BondCountPerPreFunding;

            securitization.Inputs.UseReplines = _scenarioSecuritizationInputs.UseReplines;
            securitization.Inputs.UsePreFundingStartDate = _scenarioSecuritizationInputs.UsePreFundingStartDate;

            securitization.Inputs.SeparatePrepaymentInterest = _scenarioSecuritizationInputs.SeparatePrepaymentInterest;
            securitization.Inputs.CleanUpCallPercentage = _scenarioSecuritizationInputs.CleanUpCallPercentage;

            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            SetRequiresRunningCashFlows(loanPool.Inputs);
            SetRequiresLoadingCollateral(loanPool.Inputs);

            loanPool.Inputs.ScenarioDescription = _scenarioCashFlowGenerationInputs.ScenarioDescription;

            loanPool.Inputs.CollateralCutOffDate = _scenarioCashFlowGenerationInputs.CollateralCutOffDate;
            loanPool.Inputs.CashFlowStartDate = _scenarioCashFlowGenerationInputs.CashFlowStartDate;
            loanPool.Inputs.InterestAccrualStartDate = _scenarioCashFlowGenerationInputs.InterestAccrualStartDate;

            loanPool.Inputs.SelectedAggregationGrouping = _scenarioCashFlowGenerationInputs.SelectedAggregationGrouping;
            loanPool.Inputs.SelectedPerformanceAssumption = _scenarioCashFlowGenerationInputs.SelectedPerformanceAssumption;

            loanPool.Inputs.UseReplines = _scenarioCashFlowGenerationInputs.UseReplines;
            loanPool.Inputs.SeparatePrepaymentInterest = _scenarioCashFlowGenerationInputs.SeparatePrepaymentInterest;

            return loanPool;
        }

        private void SetRequiresRunningCashFlows(CashFlowGenerationInput cashFlowGenerationInput)
        {
            _requiresRunningCashFlows = cashFlowGenerationInput.SelectedPerformanceAssumption != _scenarioCashFlowGenerationInputs.SelectedPerformanceAssumption
                                     || cashFlowGenerationInput.SelectedAggregationGrouping != _scenarioCashFlowGenerationInputs.SelectedAggregationGrouping
                                     || cashFlowGenerationInput.SeparatePrepaymentInterest != _scenarioCashFlowGenerationInputs.SeparatePrepaymentInterest;
        }

        private void SetRequiresRunningCashFlows(SecuritizationInput securitizationInputs)
        {
            _requiresRunningCashFlows = securitizationInputs.SelectedPerformanceAssumption != _scenarioSecuritizationInputs.SelectedPerformanceAssumption
                                     || securitizationInputs.SelectedAggregationGrouping != _scenarioSecuritizationInputs.SelectedAggregationGrouping
                                     || securitizationInputs.SeparatePrepaymentInterest != _scenarioSecuritizationInputs.SeparatePrepaymentInterest
                                     || securitizationInputs.PreFundingPercentageAmount.HasValue != _scenarioSecuritizationInputs.PreFundingPercentageAmount.HasValue
                                     || (
                                            securitizationInputs.PreFundingPercentageAmount.HasValue && _scenarioSecuritizationInputs.PreFundingPercentageAmount.HasValue && 
                                           (securitizationInputs.PreFundingPercentageAmount.Value != _scenarioSecuritizationInputs.PreFundingPercentageAmount.Value)
                                        )
                                     || securitizationInputs.CleanUpCallPercentage.HasValue != _scenarioSecuritizationInputs.CleanUpCallPercentage.HasValue
                                     || (
                                            securitizationInputs.CleanUpCallPercentage.HasValue && _scenarioSecuritizationInputs.CleanUpCallPercentage.HasValue &&
                                           (securitizationInputs.CleanUpCallPercentage.Value != _scenarioSecuritizationInputs.CleanUpCallPercentage.Value)
                                        );
        }

        private void SetRequiresLoadingCollateral(CashFlowGenerationInput cashFlowGenerationInput)
        {
            _requiresLoadingCollateral = cashFlowGenerationInput.CollateralCutOffDate.Ticks != _scenarioCashFlowGenerationInputs.CollateralCutOffDate.Ticks
                                      || cashFlowGenerationInput.CashFlowStartDate.Ticks != _scenarioCashFlowGenerationInputs.CashFlowStartDate.Ticks
                                      || cashFlowGenerationInput.InterestAccrualStartDate.Ticks != _scenarioCashFlowGenerationInputs.InterestAccrualStartDate.Ticks;
        }

        private void SetRequiresLoadingCollateral(SecuritizationInput securitizationInputs)
        {
            _requiresLoadingCollateral = securitizationInputs.CollateralCutOffDate.Ticks != _scenarioSecuritizationInputs.CollateralCutOffDate.Ticks
                                      || securitizationInputs.CashFlowStartDate.Ticks != _scenarioSecuritizationInputs.CashFlowStartDate.Ticks
                                      || securitizationInputs.InterestAccrualStartDate.Ticks != _scenarioSecuritizationInputs.InterestAccrualStartDate.Ticks
                                      || securitizationInputs.UsePreFundingStartDate.HasValue != _scenarioSecuritizationInputs.UsePreFundingStartDate.HasValue
                                      || (
                                            securitizationInputs.UsePreFundingStartDate.HasValue && _scenarioSecuritizationInputs.UsePreFundingStartDate.HasValue &&
                                           (securitizationInputs.UsePreFundingStartDate.Value != _scenarioSecuritizationInputs.UsePreFundingStartDate.Value)
                                         );
        }
    }
}
