using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Replines
{
    public class ReplineGeneratorRetriever
    {
        public static IReplineGenerator GetReplineGenerator(List<Loan> listOfLoans, double bondCountPerPreFunding = 0.0)
        {
            if (listOfLoans.All(c => c is PaceAssessment))
            {
                var paceReplineGenerator = new PaceAssessmentReplineGenerator<PaceAssessmentReplineCriteria>(listOfLoans, bondCountPerPreFunding);
                return paceReplineGenerator;
            }

            if (listOfLoans.All(c => c is FixedRateLoan))
            {
                var fixedRateLoanReplineGenerator = new FixedRateLoanReplineGenerator<FixedRateLoanReplineCriteria>(listOfLoans);
                return fixedRateLoanReplineGenerator;
            }

            throw new Exception("INTERNAL ERROR: Replining is not yet supported for this type of pool of loans or assessments. Please report this error.");
        }
    }
}
