using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.Valuation;
using System;

namespace Dream.Core.BusinessLogic.SecuritizationEngine
{
    public class SecuritizationInput : CashFlowGenerationInput
    {
        public string AdditionalYieldScenarioDescription { get; set; }

        public DateTime SecuritizationStartDate { get; set; }
        public DateTime SecuritizationFirstCashFlowDate { get; set; }
        public DateTime? LastPreFundingDate { get; set; }

        public double? PreFundingPercentageAmount { get; set; }
        public double BondCountPerPreFunding { get; set; }

        public double? CleanUpCallPercentage { get; set; }
        public bool? UsePreFundingStartDate { get; set; }

        public SecuritizationInput() : base() { }

        public new SecuritizationInput Copy()
        {
            var securitizationInput = new SecuritizationInput
            {
                ScenarioDescription = (ScenarioDescription == null) ? null
                    : new string(ScenarioDescription.ToCharArray()),

                AdditionalYieldScenarioDescription = (AdditionalYieldScenarioDescription == null) ? null
                    : new string(AdditionalYieldScenarioDescription.ToCharArray()),

                CollateralCutOffDate = new DateTime(CollateralCutOffDate.Ticks),
                CashFlowStartDate = new DateTime(CashFlowStartDate.Ticks),
                InterestAccrualStartDate = new DateTime(InterestAccrualStartDate.Ticks),
                SecuritizationStartDate = new DateTime(SecuritizationStartDate.Ticks),
                SecuritizationFirstCashFlowDate = new DateTime(SecuritizationFirstCashFlowDate.Ticks),
                LastPreFundingDate = (LastPreFundingDate.HasValue) ? new DateTime?(LastPreFundingDate.Value) : null,

                SelectedAggregationGrouping = (SelectedAggregationGrouping == null) ? null
                    : new string(SelectedAggregationGrouping.ToCharArray()),
                SelectedPerformanceAssumption = (SelectedPerformanceAssumption == null) ? null 
                    : new string(SelectedPerformanceAssumption.ToCharArray()),

                CashFlowPricingStrategy = CashFlowPricingStrategy.Copy(),

                MarketRateEnvironment = MarketRateEnvironment.Copy(),

                MarketDataGroupingForNominalSpread = MarketDataGroupingForNominalSpread,
                CurveTypeForSpreadCalcultion = CurveTypeForSpreadCalcultion,

                UseReplines = UseReplines,
                UsePreFundingStartDate = UsePreFundingStartDate,

                SeparatePrepaymentInterest = SeparatePrepaymentInterest,
                PreFundingPercentageAmount = PreFundingPercentageAmount,
                BondCountPerPreFunding = BondCountPerPreFunding,
                CleanUpCallPercentage = CleanUpCallPercentage,            
            };

            return securitizationInput;
        }
    }
}
