using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.Valuation;
using System.Collections.Generic;

namespace Dream.Core.Interfaces
{
    public interface ICollateralRetriever
    {
        void SetCollateralDates<T>(T inputs) where T : CashFlowGenerationInput;
        List<Loan> GetCollateral();
        double GetTotalCollateralBalance();
        ICollateralRetriever Copy();
    }
}
