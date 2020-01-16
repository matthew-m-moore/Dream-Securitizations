using System;

namespace Dream.IO.Excel.Entities.CollateralTapeRecords
{
    public class CommercialPaceTapeRecord : PaceTapeRecord
    {
        public string Description { get; set; }
        public string PrepaymentPenaltyPlan { get; set; }
        public DateTime FirstPaymentDate { get; set; }
        public DateTime FirstPrincipalPaymentDate { get; set; }
    }
}
