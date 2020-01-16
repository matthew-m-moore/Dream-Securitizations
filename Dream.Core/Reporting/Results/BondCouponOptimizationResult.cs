using Dream.Core.BusinessLogic.Containers.CashFlows;
using System.Collections.Generic;

namespace Dream.Core.Reporting.Results
{
    public class BondCouponOptimizationResult
    {
        public string ScenarioName { get; set; }
        public double AssessmentTerm { get; set; }
     
        public double PaydownPercentage { get; set; }
        public double PercentageFeeCollection { get; set; }
        public double FixedFeeCollection { get; set; }
        public double AdditionalCustomerFee { get; set; }

        public double AssessmentCoupon { get; set; }
        public double? MaxBondCoupon { get; set; }       

        public double? RateDifferential => AssessmentCoupon - MaxBondCoupon;

        public List<ContractualCashFlow> PostPaydownContractualCashFlows { get; set; }
        public List<ContractualCashFlow> BondPrePaydownContractualCashFlows { get; set; }
        public List<ContractualCashFlow> BondPostPaydownContractualCashFlows { get; set; }
    }
}
