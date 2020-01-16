using Dream.Common;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.Replines;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration
{
    public abstract class CashFlowGenerator<T> where T : CashFlow
    {
        private const string _prefundingPrefix = "PF";

        protected bool _UseReplines { get; set; }
        protected double? _PreFundingPercentageAmount { get; set; }
        protected double _BondCountPerPreFunding { get; set; }

        public CashFlowGenerator(bool useReplines, 
            double? preFundingPercentageAmount = null, 
            double bondCountPerPreFunding = 0.0)
        {
            _UseReplines = useReplines;
            _PreFundingPercentageAmount = preFundingPercentageAmount;
            _BondCountPerPreFunding = bondCountPerPreFunding;
        }

        public virtual List<List<T>> GenerateCashFlowsOnListOfLoans(List<Loan> loans, out List<Loan> loansOrReplines)
        {
            loansOrReplines = ApplyUsageOfReplines(loans);
            loansOrReplines = ApplyUsageOfPrefunding(loansOrReplines);

            var listOfCashFlows = loansOrReplines
                .AsParallel()
                .WithDegreeOfParallelism(Constants.ProcessorCount)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .Select(GenerateCashFlowsOnSingleLoan)
                .ToList();

            return listOfCashFlows;
        }

        public abstract List<T> GenerateCashFlowsOnSingleLoan(Loan loan);

        protected List<Loan> ApplyUsageOfReplines(List<Loan> loans)
        {
            if (!loans.Any() || loans == null) return new List<Loan>();
            
            if (_UseReplines)
            {
                var replineGenerator = ReplineGeneratorRetriever.GetReplineGenerator(loans);
                var replines = replineGenerator.GenerateReplines();
                return replines;
            }

            return loans;
        }

        protected List<Loan> ApplyUsageOfPrefunding(List<Loan> loansOrReplines)
        {
            if (!loansOrReplines.Any() || loansOrReplines == null) return new List<Loan>();

            if (_PreFundingPercentageAmount.HasValue && _PreFundingPercentageAmount.Value > 0.0)
            {
                var listWithAddedPrefunding = new List<Loan>();
                listWithAddedPrefunding.AddRange(loansOrReplines);

                var replineGenerator = ReplineGeneratorRetriever.GetReplineGenerator(loansOrReplines, _BondCountPerPreFunding);
                var prefundingReplines = replineGenerator.CreateAdditionalReplines(loansOrReplines, _PreFundingPercentageAmount.Value, _prefundingPrefix);
                listWithAddedPrefunding.AddRange(prefundingReplines);

                return listWithAddedPrefunding;
            }

            return loansOrReplines;
        }
    }
}
