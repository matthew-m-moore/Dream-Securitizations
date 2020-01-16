using Dream.Core.BusinessLogic.ProductTypes;
using System;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.Replines
{
    public abstract class ReplineCriteria
    {
        public abstract string Description { get; }

        public DateTime StartDate { get; set; }
        public DateTime FirstPaymentDate { get; set; }
        public DateTime InterestAccrualStartDate { get; set; }

        public int MonthsToNextInterestPayment { get; set; }
        public int MonthsToNextPrincipalPayment { get; set; }
        public int InterestPaymentFrequencyInMonths { get; set; }
        public int PrincipalPaymentFrequencyInMonths { get; set; }

        public abstract bool DetermineIfLoanMeetsCriteria(Loan loan);

        public abstract Loan AggregateLoansIntoRepline(List<Loan> loans);

        public static bool operator ==(ReplineCriteria replineCriteriaOne, ReplineCriteria replineCriteriaTwo)
        {
            if (ReferenceEquals(replineCriteriaOne, replineCriteriaTwo))
            {
                return true;
            }

            if (ReferenceEquals(replineCriteriaOne, null))
            {
                return false;
            }

            return replineCriteriaOne.Equals(replineCriteriaTwo);
        }

        public static bool operator !=(ReplineCriteria replineCriteriaOne, ReplineCriteria replineCriteriaTwo)
        {
            return !(replineCriteriaOne == replineCriteriaTwo);
        }

        public override bool Equals(object obj)
        {
            // Note, if the obj was null, this safe caste would just return null
            var replineCriteria = obj as ReplineCriteria;
            if (replineCriteria == null) return false;

            var isEqual = replineCriteria.StartDate.Ticks == StartDate.Ticks
                       && replineCriteria.FirstPaymentDate.Ticks == FirstPaymentDate.Ticks
                       && replineCriteria.InterestAccrualStartDate.Ticks == InterestAccrualStartDate.Ticks

                       && replineCriteria.MonthsToNextInterestPayment == MonthsToNextInterestPayment
                       && replineCriteria.MonthsToNextPrincipalPayment == MonthsToNextPrincipalPayment
                       && replineCriteria.InterestPaymentFrequencyInMonths == InterestPaymentFrequencyInMonths
                       && replineCriteria.PrincipalPaymentFrequencyInMonths == PrincipalPaymentFrequencyInMonths;

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

                hash = (hash * primeNumberTwo) + StartDate.GetHashCode();
                hash = (hash * primeNumberTwo) + FirstPaymentDate.GetHashCode();
                hash = (hash * primeNumberTwo) + InterestAccrualStartDate.GetHashCode();

                hash = (hash * primeNumberTwo) + MonthsToNextInterestPayment.GetHashCode();
                hash = (hash * primeNumberTwo) + MonthsToNextPrincipalPayment.GetHashCode();
                hash = (hash * primeNumberTwo) + InterestPaymentFrequencyInMonths.GetHashCode();
                hash = (hash * primeNumberTwo) + PrincipalPaymentFrequencyInMonths.GetHashCode();

                return hash;
            }
        }
    }
}
