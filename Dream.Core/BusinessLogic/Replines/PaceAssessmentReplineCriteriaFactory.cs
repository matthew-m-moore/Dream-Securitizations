using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.Replines
{
    public class PaceAssessmentReplineCriteriaFactory : FixedRateLoanReplineCriteriaFactory
    {
        public PaceAssessmentReplineCriteriaFactory() : base() { }

        public PaceAssessmentReplineCriteriaFactory(List<Loan> listOfPaceAssessments) : base(listOfPaceAssessments) { }

        public override T ConstructReplineCriteriaFromLoan<T>(Loan loan)
        {
            var replineCriteria = base.ConstructReplineCriteriaFromLoan<T>(loan);

            var paceAssessment = loan as PaceAssessment;
            var paceAssessmentReplineCriteria = replineCriteria as PaceAssessmentReplineCriteria;

            if (paceAssessment == null || paceAssessmentReplineCriteria == null)
            {
                throw new Exception();
            }

            if (paceAssessment.RatePlan != null)
            {
                paceAssessmentReplineCriteria.RatePlanName = paceAssessment.RatePlan.Description;
                paceAssessmentReplineCriteria.BuyDownRate = paceAssessment.RatePlan.BuyDownRate;
            }

            paceAssessmentReplineCriteria.AmortizationTermInMonths = paceAssessment.AmortizationTermInMonths;
            paceAssessmentReplineCriteria.AmortizationTermInYears = paceAssessment.AmortizationTermInYears;
            paceAssessmentReplineCriteria.FirstPaymentDate = paceAssessment.FirstPaymentDate;

            replineCriteria = paceAssessmentReplineCriteria as T;
            return replineCriteria;
        }
    }
}
