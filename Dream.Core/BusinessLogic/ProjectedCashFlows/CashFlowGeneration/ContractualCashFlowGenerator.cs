using Dream.Common;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.Valuation;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration
{
    public class ContractualCashFlowGenerator<T> : CashFlowGenerator<T>
         where T : ContractualCashFlow
    {
        public ContractualCashFlowGenerator(bool useReplines) : base(useReplines) { }

        public ContractualCashFlowGenerator(SecuritizationInput securitizationInput)
            : base(securitizationInput.UseReplines, 
                   securitizationInput.PreFundingPercentageAmount,
                   securitizationInput.BondCountPerPreFunding)
        { }

        public ContractualCashFlowGenerator(CashFlowGenerationInput cashFlowGenerationInput)
            : base(cashFlowGenerationInput.UseReplines)
        { }

        public static List<List<ProjectedCashFlow>> ConvertContractualToProjectedCashFlows(List<List<ContractualCashFlow>> listOfContractualCashFlows)
        {
            // Behold the power of LINQ
            var listOfProjectedCashFlows = listOfContractualCashFlows
                .Select(l => l.Select(c => new ProjectedCashFlow(c)).ToList()).ToList();

            return listOfProjectedCashFlows;
        }

        public static Dictionary<string, List<ProjectedCashFlow>> ConvertContractualToProjectedCashFlows(Dictionary<string, List<ContractualCashFlow>> dictionaryOfContractualCashFlows)
        {
            // Behold the power of LINQ
            var dictionaryOfProjectedCashFlows = dictionaryOfContractualCashFlows
                .ToDictionary(kvp => kvp.Key,
                              kvp => kvp.Value.Select(c => new ProjectedCashFlow(c)).ToList());

            return dictionaryOfProjectedCashFlows;
        }

        public Dictionary<string, List<T>> GenerateContractualCashFlowsDictionaryOnListOfLoans(List<Loan> loans)
        {
            var contractualCashFlowsDictionary = loans
                .AsParallel()
                .WithDegreeOfParallelism(Constants.ProcessorCount)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .ToDictionary(loan => loan.StringId,
                              loan => GenerateCashFlowsOnSingleLoan(loan));

            return contractualCashFlowsDictionary;
        }

        public override List<T> GenerateCashFlowsOnSingleLoan(Loan loan)
        {
            var contractualCashFlows = loan.GetContractualCashFlows();
            return contractualCashFlows.Select(cashFlow => (T)cashFlow).ToList();
        }
    }
}
