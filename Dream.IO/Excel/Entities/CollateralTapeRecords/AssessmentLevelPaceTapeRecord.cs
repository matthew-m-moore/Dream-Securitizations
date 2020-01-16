using System;

namespace Dream.IO.Excel.Entities.CollateralTapeRecords
{
    public class AssessmentLevelPaceTapeRecord : PaceTapeRecord
    {
        public int LoanId { get; set; }
        public string MunicipalBondId { get; set; }
        public string ReplineId { get; set; }
        public DateTime FundingDate { get; set; }
        public DateTime BondFirstPaymentDate { get; set; }
        public DateTime BondMaturityDate { get; set; }
        public double? AccruedInterest { get; set; }
    }
}
