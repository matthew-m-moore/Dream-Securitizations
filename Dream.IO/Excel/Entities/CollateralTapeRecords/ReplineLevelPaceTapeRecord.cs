using System;

namespace Dream.IO.Excel.Entities.CollateralTapeRecords
{
    public class ReplineLevelPaceTapeRecord : PaceTapeRecord
    {
        public string ReplineId { get; set; }
        public double? PreAnalysisInterest { get; set; }
        public DateTime FirstPaymentDate { get; set; }
        public DateTime FirstPrincipalPaymentDate { get; set; }
        public int NumberOfBonds { get; set; }
        public int NumberOfAssessments { get; set; }
    }
}
