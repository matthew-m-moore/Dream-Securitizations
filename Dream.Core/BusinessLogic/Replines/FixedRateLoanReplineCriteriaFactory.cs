using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.Replines
{
    public class FixedRateLoanReplineCriteriaFactory : ReplineCriteriaFactory
    {
        public FixedRateLoanReplineCriteriaFactory() : base() { }

        public FixedRateLoanReplineCriteriaFactory(List<Loan> listOfFixedRateLoans) : base(listOfFixedRateLoans) { }

        public override T ConstructReplineCriteriaFromLoan<T>(Loan loan)
        {
            var replineCriteria = base.ConstructReplineCriteriaFromLoan<T>(loan);

            var fixedRateLoan = loan as FixedRateLoan;
            var fixedRateLoanReplineCriteria = replineCriteria as FixedRateLoanReplineCriteria;

            if (fixedRateLoan == null || fixedRateLoanReplineCriteria == null)
            {
                throw new Exception();
            }

            fixedRateLoanReplineCriteria.FixedRateCoupon = fixedRateLoan.InitialCouponRate;
            fixedRateLoanReplineCriteria.MaturityTermInMonths = fixedRateLoan.MaturityTermInMonths;
            fixedRateLoanReplineCriteria.MaturityTermInYears = fixedRateLoan.MaturityTermInYears;

            replineCriteria = fixedRateLoanReplineCriteria as T;
            return replineCriteria;
        }
    }
}
