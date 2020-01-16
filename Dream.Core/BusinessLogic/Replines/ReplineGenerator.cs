using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Replines
{
    public abstract class ReplineGenerator<T> : IReplineGenerator where T : ReplineCriteria
    {
        protected ReplineCriteriaFactory _ReplineCriteriaFactor;

        protected ReplineLogic<T> _ReplineLogic;       
        protected List<Loan> _ListOfLoans;

        public ReplineGenerator(List<Loan> listOfLoans)
        {
            _ListOfLoans = listOfLoans;
        }

        /// <summary>
        /// Generates replines form the given list of loans provided when initializing the object.
        /// </summary>
        /// <returns></returns>
        public List<Loan> GenerateReplines()
        {
            var listOfDistinctReplineCritera = _ReplineCriteriaFactor.RetrieveDistinctReplineCriteria<T>();

            _ReplineLogic = new ReplineLogic<T>(listOfDistinctReplineCritera);

            var listOfReplines = _ReplineLogic.CreateReplinesFromLoans(_ListOfLoans);
            return listOfReplines;
        }

        /// <summary>
        /// Creates additional replines with a size equal to a factor percentage of the total balance for each type of repline.
        /// </summary>
        public List<Loan> CreateAdditionalReplines(List<Loan> listOfReplines, double percentageOfBalanceFactor, string prefixDescription)
        {
            var additionalReplines = listOfReplines
                .Select(r => CreateAdditionalRepline(r, percentageOfBalanceFactor, prefixDescription)).ToList();

            return additionalReplines;
        }

        /// <summary>
        /// Creates an additional repline with a size equal to a factor percentage of the total final balance for this type of repline.
        /// </summary>
        public virtual Loan CreateAdditionalRepline(Loan repline, double percentageOfBalanceFactor, string prefixDescription)
        {
            var additionalRepline = repline.Copy();

            // Note the math here is such that the percentage will represent a percentage of the total final amount of this repline
            var totalBalance = additionalRepline.Balance + additionalRepline.ActualPrepayments;
            additionalRepline.Balance = (totalBalance * percentageOfBalanceFactor) / (1.0 - percentageOfBalanceFactor);
            additionalRepline.StringId = prefixDescription + ": " + additionalRepline.StringId;

            additionalRepline.AccruedInterest = 0.0;
            additionalRepline.ActualPrepayments = 0.0;

            return additionalRepline;
        }
    }
}
