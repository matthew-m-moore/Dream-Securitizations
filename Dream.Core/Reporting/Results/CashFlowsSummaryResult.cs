using Dream.Common.Enums;

namespace Dream.Core.Reporting.Results
{
    public abstract class CashFlowsSummaryResult
    {
        public double? Balance { get; set; }
        public double? DollarPrice { get; set; }
        public double PresentValue { get; set; }
        public double InternalRateOfReturn { get; set; }
        public double ZeroVolatilitySpread { get; set; }
        public double? NominalSpread { get; set; }
        public double? NominalBenchmarkRate { get; set; }
        public double WeightedAverageLife { get; set; }
        public double ForwardWeightedAverageCoupon { get; set; }
        public double MacaulayDuration { get; set; }
        public double ModifiedDurationAnalytical { get; set; }
        public double ModifiedDurationNumerical { get; set; }
        public double EffectiveDuration { get; set; }
        public double SpreadDuration { get; set; }
        public double DollarDuration { get; set; }
        public double EffectiveDollarDuration { get; set; }

        public double TotalCashFlow { get; set; }
        public double TotalPrincipal { get; set; }
        public double TotalInterest { get; set; }

        public DayCountConvention DayCountConvention { get; set; }
        public CompoundingConvention CompoundingConvention { get; set; }
        public MarketDataGrouping MarketDataUseForNominalSpread { get; set; }
        public InterestRateCurveType CurveTypeUsedForSpreadCalculation { get; set; }
    }
}
