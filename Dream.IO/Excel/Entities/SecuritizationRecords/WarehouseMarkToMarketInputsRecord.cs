using System;

namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class WarehouseMarkToMarketInputsRecord : SecuritizationInputsRecord
    {
        public DateTime RunDate { get; set; }
        public int NominalSpreadInBps { get; set; }
        public string MarketDataForSpread { get; set; }
        public string DayCountConvention { get; set; }
        public string CompoundingConvention { get; set; }
        public double AdvanceRate { get; set; }
        public double MinimumCushion { get; set; }
    }
}
