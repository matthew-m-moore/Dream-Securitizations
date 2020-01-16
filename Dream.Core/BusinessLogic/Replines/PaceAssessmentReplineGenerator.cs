using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using System;

namespace Dream.Core.BusinessLogic.Replines
{
    public class PaceAssessmentReplineGenerator<T> : FixedRateLoanReplineGenerator<T>
        where T : PaceAssessmentReplineCriteria
    {
        private double _bondCounterPerAdditionalRepline;

        public PaceAssessmentReplineGenerator(List<Loan> listOfPaceAssessments) : base(listOfPaceAssessments)
        {
            _ReplineCriteriaFactor = new PaceAssessmentReplineCriteriaFactory(_ListOfLoans);
        }

        public PaceAssessmentReplineGenerator(List<Loan> listOfPaceAssessments, double bondCountPerAdditionalRepline) : base(listOfPaceAssessments)
        {
            _ReplineCriteriaFactor = new PaceAssessmentReplineCriteriaFactory(_ListOfLoans);
            _bondCounterPerAdditionalRepline = bondCountPerAdditionalRepline;
        }

        public override Loan CreateAdditionalRepline(Loan repline, double percentageOfBalanceFactor, string prefixDescription)
        {
            var additionalRepline = base.CreateAdditionalRepline(repline, percentageOfBalanceFactor, prefixDescription);

            var additionalPaceAssessmentRepline = additionalRepline as PaceAssessment;
            if (additionalPaceAssessmentRepline == null)
            {
                throw new Exception("INTERNAL ERROR: Could not cast additional repline as PACE assessment. Please report this error.");
            }

            additionalPaceAssessmentRepline.BondCount = _bondCounterPerAdditionalRepline;
            additionalRepline = additionalPaceAssessmentRepline;

            return additionalRepline;
        }
    }
}
