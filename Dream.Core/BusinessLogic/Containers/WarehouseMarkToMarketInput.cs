using Dream.Common.Enums;

namespace Dream.Core.BusinessLogic.Containers
{
    public class WarehouseMarkToMarketInput
    {
        public double NominalSpread { get; set; }
        public MarketDataGrouping MarketDataForNominalSpread { get; set; }
        public DayCountConvention DayCountConvention { get; set; }
        public CompoundingConvention CompoundingConvention { get; set; }
        public double AdvanceRate { get; set; }
        public double MinimumCushion { get; set; }
    }
}
