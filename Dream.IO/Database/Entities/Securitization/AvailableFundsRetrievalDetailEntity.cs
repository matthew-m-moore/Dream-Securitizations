using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class AvailableFundsRetrievalDetailEntity
    {
        public int AvailableFundsRetrievalDetailId { get; set; }
        public int AvailableFundsRetrievalTypeId { get; set; }
        public double? AvailableFundsRetrievalValue { get; set; }
        public int? AvailableFundsRetrievalInteger { get; set; }
        public DateTime? AvailableFundsRetrievalDate { get; set; }
    }
}
