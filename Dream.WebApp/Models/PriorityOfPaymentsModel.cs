using Dream.WebApp.ModelEntries;

namespace Dream.WebApp.Models
{
    public class PriorityOfPaymentsModel
    {
        public bool IsModified { get; set; }
        public PriorityOfPaymentsModelEntry[][] PriorityOfPaymentsModelEntries { get; set; }
    }
}