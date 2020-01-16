using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.BusinessLogic.Replines
{
    public class ReplineCriteriaFactory
    {
        protected List<Loan> _ListOfLoans;

        public ReplineCriteriaFactory() { }

        public ReplineCriteriaFactory(List<Loan> listOfLoans)
        {
            _ListOfLoans = listOfLoans;
        }

        public List<T> RetrieveDistinctReplineCriteria<T>() where T : ReplineCriteria
        {
            var listOfDistinctReplineCritera = _ListOfLoans
                .Select(ConstructReplineCriteriaFromLoan<T>)
                .Distinct().ToList();

            return listOfDistinctReplineCritera;
        }

        public virtual T ConstructReplineCriteriaFromLoan<T>(Loan loan) where T : ReplineCriteria
        {
            var replineCriteria = Activator.CreateInstance<T>();

            replineCriteria.StartDate = loan.StartDate;
            replineCriteria.FirstPaymentDate = loan.FirstPaymentDate;
            replineCriteria.InterestAccrualStartDate = loan.InterestAccrualStartDate;

            replineCriteria.MonthsToNextInterestPayment = loan.MonthsToNextInterestPayment;
            replineCriteria.MonthsToNextPrincipalPayment = loan.MonthsToNextPrincipalPayment;
            replineCriteria.InterestPaymentFrequencyInMonths = loan.InterestPaymentFrequencyInMonths;
            replineCriteria.PrincipalPaymentFrequencyInMonths = loan.PrincipalPaymentFrequencyInMonths;

            return replineCriteria;
        }
    }
}
