using System;

namespace Dream.IO.Database.Entities.Securitization
{
    public class PriorityOfPaymentsSetEntity
    {
        public int PriorityOfPaymentsSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string PriorityOfPaymentsSetDescription { get; set; }
    }
}
