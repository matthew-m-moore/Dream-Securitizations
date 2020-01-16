using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.Reporting.Results;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Common.Curves;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Scenarios.PerformanceAssumptionShocks
{
    public class PrepaymentShockScenario : IScenarioLogic
    {
        private ScenarioShock _scenarioShock;

        public bool RequiresRunningCashFlows => true;
        public bool RequiresLoadingCollateral => false;

        public PrepaymentShockScenario(double annulizedShockValue, ShockStrategy shockStrategy)
        {
            var monthlyShockValue = MathUtility.ConvertAnnualRateToMonthlyRate(annulizedShockValue);
            _scenarioShock = new ScenarioShock(shockStrategy, monthlyShockValue);
        }

        public Securitization ApplyScenarioLogic(Securitization securitization, SecuritizationResult securitizationResult)
        {
            if (securitization is Resecuritization resecuritization)
            {
                foreach (var securitizationName in resecuritization.CollateralizedSecuritizationsDictionary.Keys)
                {
                    var performanceAssumptions = resecuritization.ProjectedCashFlowLogicDictionary[securitizationName].ProjectedPerformanceAssumptions;
                    var listOfAllPerformanceCurves = performanceAssumptions.GetAllPerformanceCurveNames();

                    foreach (var performanceCurve in listOfAllPerformanceCurves)
                    {
                        var shockedCurve = ShockPrepaymentCurve(performanceAssumptions, performanceCurve);
                        resecuritization.ProjectedCashFlowLogicDictionary[securitizationName]
                            .ProjectedPerformanceAssumptions[performanceCurve, PerformanceCurveType.Smm] = shockedCurve;
                    }
                }

                securitization = resecuritization;
            }
            else
            {
                var performanceAssumptions = securitization.ProjectedCashFlowLogic.ProjectedPerformanceAssumptions;
                var listOfAllPerformanceCurves = performanceAssumptions.GetAllPerformanceCurveNames();

                foreach (var performanceCurve in listOfAllPerformanceCurves)
                {
                    var shockedCurve = ShockPrepaymentCurve(performanceAssumptions, performanceCurve);
                    securitization.ProjectedCashFlowLogic.ProjectedPerformanceAssumptions[performanceCurve, PerformanceCurveType.Smm] = shockedCurve;
                }
            }

            return securitization;
        }

        public LoanPool ApplyScenarioLogic(LoanPool loanPool, ProjectedCashFlowsSummaryResult totalProjectedCashFlowSummaryResult)
        {
            var performanceAssumptions = loanPool.ProjectedCashFlowLogic.ProjectedPerformanceAssumptions;
            var listOfAllPerformanceCurves = performanceAssumptions.GetAllPerformanceCurveNames();

            foreach (var performanceCurve in listOfAllPerformanceCurves)
            {
                var shockedCurve = ShockPrepaymentCurve(performanceAssumptions, performanceCurve);
                loanPool.ProjectedCashFlowLogic.ProjectedPerformanceAssumptions[performanceCurve, PerformanceCurveType.Smm] = shockedCurve;
            }

            return loanPool;
        }

        private Curve<double> ShockPrepaymentCurve(ProjectedPerformanceAssumptions performanceAssumptions, string performanceCurve)
        {
            List<double> shockedValuesList;
            switch (_scenarioShock.ShockStrategy)
            {
                case ShockStrategy.Additive:
                    shockedValuesList = performanceAssumptions[performanceCurve, PerformanceCurveType.Smm]
                        .Select(c => c + _scenarioShock.ShockValue).ToList();
                    break;

                case ShockStrategy.Multiplicative:
                    shockedValuesList = performanceAssumptions[performanceCurve, PerformanceCurveType.Smm]
                        .Select(c => c * _scenarioShock.ShockValue).ToList();
                    break;

                case ShockStrategy.Replacement:
                    shockedValuesList = new List<double> { _scenarioShock.ShockValue };
                    break;

                default:
                    shockedValuesList = performanceAssumptions[performanceCurve, PerformanceCurveType.Smm].ToList();
                    break;
            }

            var shockedCurve = new Curve<double>(shockedValuesList);
            return shockedCurve;
        }
    }
}
