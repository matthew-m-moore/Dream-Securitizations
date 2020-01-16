using System;

namespace Dream.IO.Excel.Entities.CollateralTapeRecords
{
    public class MunicipalBondLevelPaceTapeRecord : PaceTapeRecord
    {
        public string MunicipalBondId { get; set; }
        public string ReplineId { get; set; }
        public DateTime FundingDate { get; set; }
        public DateTime FirstPaymentDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public int NumberOfAssessments { get; set; }
        public double? AccruedInterest { get; set; }
    }
}
