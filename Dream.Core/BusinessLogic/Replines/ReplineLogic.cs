using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Replines
{
    public class ReplineLogic<T> where T : ReplineCriteria
    {
        private List<T> _listOfReplineCriteria;

        public ReplineLogic(List<T> listOfReplineCriteria)
        {
            _listOfReplineCriteria = listOfReplineCriteria;
        }

        /// <summary>
        /// Creates a list of replines from a list of loans according the repline criteria specified.
        /// </summary>
        public List<Loan> CreateReplinesFromLoans(List<Loan> listOfLoans)
        {
            // TODO: I should probably perform some wort of error-check that the same loan doesn't make it into multiple replines,
            // unless of course that was somehow intentional.
            var listOfReplines = new List<Loan>();
            foreach (var replineCriteria in _listOfReplineCriteria)
            {
                var batchOfLoansToRepline = listOfLoans.Where(loan => replineCriteria.DetermineIfLoanMeetsCriteria(loan)).ToList();
                var repline = replineCriteria.AggregateLoansIntoRepline(batchOfLoansToRepline);

                listOfReplines.Add(repline);
            }

            return listOfReplines;
        }
    }
}
