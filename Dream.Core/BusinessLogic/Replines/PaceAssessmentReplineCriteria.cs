using System;
using Dream.Core.BusinessLogic.ProductTypes;

namespace Dream.Core.BusinessLogic.Replines
{
    public class PaceAssessmentReplineCriteria : FixedRateLoanReplineCriteria
    {
        public override string Description {
            get
            {
                var ratePlanName = (RatePlanName != null)
                    ? RatePlanName + " ,"
                    : string.Empty;

                var description = ratePlanName + "First Prin Pmt Year " + StartDate.AddMonths(MonthsToNextPrincipalPayment).Year + ", "
                    + FixedRateCoupon.ToString("0.000%") + " Rate, "
                    + BuyDownRate.ToString("0.000%") + " Buy-Down, "
                    + AmortizationTermInYears.ToString("00") + "-Yr Term";

                return description;
            }
        }

        public string RatePlanName { get; set; }
        public double BuyDownRate { get; set; }
        public int AmortizationTermInMonths { get; set; }
        public int AmortizationTermInYears { get; set; }

        public override bool DetermineIfLoanMeetsCriteria(Loan loan)
        {
            var fixedRateLoanReplineCriteriaFactory = new PaceAssessmentReplineCriteriaFactory();
            var fixedRateLoanReplineCriteria = fixedRateLoanReplineCriteriaFactory.ConstructReplineCriteriaFromLoan<PaceAssessmentReplineCriteria>(loan);

            var doesLoanMeetThisCriteria = (this == fixedRateLoanReplineCriteria);
            return doesLoanMeetThisCriteria;
        }

        protected override Loan AggregateLoanIntoRepline(Loan repline, Loan loan)
        {
            repline = base.AggregateLoanIntoRepline(repline, loan);

            var paceAssessmentRepline = repline as PaceAssessment;
            var paceAssessment = loan as PaceAssessment;

            if (paceAssessmentRepline == null || paceAssessment == null)
            {
                throw new Exception("INTERNAL ERROR: Could not cast replines or loans as PACE assessments. Please report this error.");
            }

            if (paceAssessmentRepline.RatePlan == null)
            {
                paceAssessmentRepline.RatePlan = paceAssessment.RatePlan.Copy();
            }

            paceAssessmentRepline.BondCount += paceAssessment.BondCount;
            repline = paceAssessmentRepline;

            return repline;
        }

        public static bool operator ==(PaceAssessmentReplineCriteria paceAssessmentReplineCriteriaOne, PaceAssessmentReplineCriteria paceAssessmentReplineCriteriaTwo)
        {
            if (ReferenceEquals(paceAssessmentReplineCriteriaOne, paceAssessmentReplineCriteriaTwo))
            {
                return true;
            }

            if (ReferenceEquals(paceAssessmentReplineCriteriaOne, null))
            {
                return false;
            }

            return paceAssessmentReplineCriteriaOne.Equals(paceAssessmentReplineCriteriaTwo);
        }

        public static bool operator !=(PaceAssessmentReplineCriteria paceAssessmentReplineCriteriaOne, PaceAssessmentReplineCriteria paceAssessmentReplineCriteriaTwo)
        {
            return !(paceAssessmentReplineCriteriaOne == paceAssessmentReplineCriteriaTwo);
        }

        public override bool Equals(object obj)
        {
            // Note, if the obj was null, this safe caste would just return null
            var paceAssessmentReplineCriteria = obj as PaceAssessmentReplineCriteria;
            if (paceAssessmentReplineCriteria == null) return false;

            var isEqual = paceAssessmentReplineCriteria.BuyDownRate == BuyDownRate
                       && paceAssessmentReplineCriteria.AmortizationTermInMonths == AmortizationTermInMonths
                       && base.Equals(obj);

            return isEqual;
        }

        /// <summary>
        /// Note, this implemenatation was informed by the following thread:
        /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int primeNumberOne = 13;
                int primeNumberTwo = 5;

                var hash = primeNumberOne;

                hash += base.GetHashCode();
                hash = (hash * primeNumberTwo) + BuyDownRate.GetHashCode();
                hash = (hash * primeNumberTwo) + AmortizationTermInMonths.GetHashCode();

                return hash;
            }
        }
    }
}
