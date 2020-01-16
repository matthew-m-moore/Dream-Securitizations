using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic
{
    public class PriorityOfPayments
    {
        public List<PriorityOfPaymentsEntry> OrderedListOfEntries { get; }

        public PriorityOfPayments(List<PriorityOfPaymentsEntry> listOfEntries)
        {
            var orderedListOfEntries = listOfEntries.OrderBy(e => e.SeniorityRanking).ToList();
            OrderedListOfEntries = orderedListOfEntries;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public PriorityOfPayments Copy()
        {
            return new PriorityOfPayments(OrderedListOfEntries.Select(e => e.Copy()).ToList());
        }
    }
}
