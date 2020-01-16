using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;

namespace Dream.Core.Interfaces
{
    public interface IReplineGenerator
    {
        List<Loan> GenerateReplines();
        List<Loan> CreateAdditionalReplines(List<Loan> listOfReplines, double percentageOfBalanceFactor, string prefixDescription);
        Loan CreateAdditionalRepline(Loan repline, double percentageOfBalanceFactor, string prefixDescription);
    }
}
