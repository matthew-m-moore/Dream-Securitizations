using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.ProductTypes;

namespace Dream.Core.BusinessLogic.Replines
{
    public class FixedRateLoanReplineCriteria : ReplineCriteria
    {
        public override string Description =>
            FixedRateCoupon.ToString("0.000%") + " Rate, " + MaturityTermInYears + "-Yr Term";

        public double FixedRateCoupon { get; set; }
        public int MaturityTermInMonths { get; set; }
        public int MaturityTermInYears { get; set; }

        public override Loan AggregateLoansIntoRepline(List<Loan> loans)
        {
            // Since the first loan is just as good as any, start there
            var replineTemplate = loans.First().Copy();
            replineTemplate.StringId = Description;

            foreach (var loan in loans.Skip(1))
            {
                AggregateLoanIntoRepline(replineTemplate, loan);
            }

            return replineTemplate;
        }

        public override bool DetermineIfLoanMeetsCriteria(Loan loan)
        {
            var fixedRateLoanReplineCriteriaFactory = new FixedRateLoanReplineCriteriaFactory();
            var fixedRateLoanReplineCriteria = fixedRateLoanReplineCriteriaFactory.ConstructReplineCriteriaFromLoan<FixedRateLoanReplineCriteria>(loan);

            var doesLoanMeetThisCriteria = (this == fixedRateLoanReplineCriteria);
            return doesLoanMeetThisCriteria;
        }

        protected virtual Loan AggregateLoanIntoRepline(Loan repline, Loan loan)
        {
            repline.Balance += loan.Balance;
            repline.AccruedInterest += loan.AccruedInterest;
            repline.ActualPrepayments += loan.ActualPrepayments;

            return repline;
        }

        public static bool operator ==(FixedRateLoanReplineCriteria fixedRateLoanReplineCriteriaOne, FixedRateLoanReplineCriteria fixedRateLoanReplineCriteriaTwo)
        {
            if (ReferenceEquals(fixedRateLoanReplineCriteriaOne, fixedRateLoanReplineCriteriaTwo))
            {
                return true;
            }

            if (ReferenceEquals(fixedRateLoanReplineCriteriaOne, null))
            {
                return false;
            }

            return fixedRateLoanReplineCriteriaOne.Equals(fixedRateLoanReplineCriteriaTwo);
        }

        public static bool operator !=(FixedRateLoanReplineCriteria fixedRateLoanReplineCriteriaOne, FixedRateLoanReplineCriteria fixedRateLoanReplineCriteriaTwo)
        {
            return !(fixedRateLoanReplineCriteriaOne == fixedRateLoanReplineCriteriaTwo);
        }

        public override bool Equals(object obj)
        {
            // Note, if the obj was null, this safe caste would just return null
            var fixedRateLoanReplineCriteria = obj as FixedRateLoanReplineCriteria;
            if (fixedRateLoanReplineCriteria == null) return false;

            var isEqual = fixedRateLoanReplineCriteria.FixedRateCoupon == FixedRateCoupon
                       && fixedRateLoanReplineCriteria.MaturityTermInMonths == MaturityTermInMonths
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
                hash = (hash * primeNumberTwo) + FixedRateCoupon.GetHashCode();
                hash = (hash * primeNumberTwo) + MaturityTermInMonths.GetHashCode();              

                return hash;
            }
        }
    }
}
