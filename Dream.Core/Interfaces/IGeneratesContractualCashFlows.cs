using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;

namespace Dream.Core.Interfaces
{
    public interface IGeneratesContractualCashFlows
    {
        ContractualCashFlow PrepareZerothPeriodCashFlow();
        List<ContractualCashFlow> GetContractualCashFlows();
    }
}
