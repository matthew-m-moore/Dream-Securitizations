using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.Replines
{
    public class FixedRateLoanReplineGenerator<T> : ReplineGenerator<T>
        where T : FixedRateLoanReplineCriteria
    {
        public FixedRateLoanReplineGenerator(List<Loan> listOfPaceAssessments) : base(listOfPaceAssessments)
        {
            _ReplineCriteriaFactor = new FixedRateLoanReplineCriteriaFactory(_ListOfLoans);
        }
    }
}
