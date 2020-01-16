using Dream.Common.Enums;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.PricingStrategies;
using System;

namespace Dream.Core.BusinessLogic.Valuation
{
    public class CashFlowGenerationInput
    {
        protected string _ScenarioDescription;
        public string ScenarioDescription
        {
            get { return _ScenarioDescription ?? string.Empty; }
            set { _ScenarioDescription = value; }
        }

        public DateTime CollateralCutOffDate { get; set; }
        public DateTime CashFlowStartDate { get; set; }
        public DateTime InterestAccrualStartDate { get; set; }

        public string SelectedAggregationGrouping { get; set; }
        public string SelectedPerformanceAssumption { get; set; }
        public string SelectedPerformanceAssumptionGrouping { get; set; }

        public PricingStrategy CashFlowPricingStrategy { get; set; }

        public MarketRateEnvironment MarketRateEnvironment { get; set; }
        public MarketDataGrouping MarketDataGroupingForNominalSpread { get; set; }
        public InterestRateCurveType CurveTypeForSpreadCalcultion { get; set; }

        public bool UseReplines = true;
        public bool SeparatePrepaymentInterest = false;
        
        public CashFlowGenerationInput()
        {
            MarketDataGroupingForNominalSpread = MarketDataGrouping.None;
            CurveTypeForSpreadCalcultion = InterestRateCurveType.None;
        }

        public CashFlowGenerationInput Copy()
        {
            var cashFlowGenerationInput = new CashFlowGenerationInput
            {
                ScenarioDescription = (ScenarioDescription == null) ? null
                    : new string(ScenarioDescription.ToCharArray()),

                CollateralCutOffDate = new DateTime(CollateralCutOffDate.Ticks),
                CashFlowStartDate = new DateTime(CashFlowStartDate.Ticks),
                InterestAccrualStartDate = new DateTime(InterestAccrualStartDate.Ticks),

                SelectedAggregationGrouping = new string(SelectedAggregationGrouping.ToCharArray()),
                SelectedPerformanceAssumption = (SelectedPerformanceAssumption == null) ? null
                    : new string(SelectedPerformanceAssumption.ToCharArray()),

                CashFlowPricingStrategy = CashFlowPricingStrategy.Copy(),

                MarketRateEnvironment = MarketRateEnvironment.Copy(),

                MarketDataGroupingForNominalSpread = MarketDataGroupingForNominalSpread,
                CurveTypeForSpreadCalcultion = CurveTypeForSpreadCalcultion,

                UseReplines = UseReplines,
                SeparatePrepaymentInterest = SeparatePrepaymentInterest,
            };

            return cashFlowGenerationInput;
        }
    }
}
