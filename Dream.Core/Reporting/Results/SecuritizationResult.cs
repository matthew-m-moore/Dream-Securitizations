using Dream.Common.Enums;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using System.Collections.Generic;

namespace Dream.Core.Reporting.Results
{
    public class SecuritizationResult
    {
        public string ScenarioDescription { get; set; }
        public Dictionary<string, SecuritizationCashFlowsSummaryResult> SecuritizationResultsDictionary { get; set; }
        public Dictionary<string, ProjectedCashFlowsSummaryResult> CollateralCashFlowsResultsDictionary { get; set; }
        public List<AvailableFundsCashFlow> AvailableFundsCashFlows { get; set; }
        public List<ProjectedCashFlow> ProjectedCashFlowsOnCollateral { get; set; }
        public PriorityOfPayments PriorityOfPayments { get; set; }
        public MarketRateEnvironment MarketRateEnvironment { get; set; }
        public MarketDataGrouping MarketDataForNominalSpread { get; set; }
        public InterestRateCurveType CurveTypeForSpreadCalcultion { get; set; }

        public SecuritizationResult()
        {
            SecuritizationResultsDictionary = new Dictionary<string, SecuritizationCashFlowsSummaryResult>();
            CollateralCashFlowsResultsDictionary = new Dictionary<string, ProjectedCashFlowsSummaryResult>();
        }
    }
}
