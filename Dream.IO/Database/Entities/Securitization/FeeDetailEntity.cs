using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class FeeDetailEntity
    {
        public int FeeDetailId { get; set; }
        public int FeeGroupDetailId { get; set; }
        public string FeeName { get; set; }
        public int? FeeAssociatedTrancheDetailId { get; set; }
        public double FeeAmount { get; set; }
        public DateTime? FeeEffectiveDate { get; set; }
        public bool? IsIncreasingFee { get; set; }

    }
}
