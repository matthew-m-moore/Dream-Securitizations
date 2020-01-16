using System;
using System.Linq;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Common.Enums;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution
{
    public class NextFeePaymentProRataDistributionRule : ProRataDistributionRule
    {
        private Dictionary<int, Dictionary<TrancheCashFlowType, double>> _totalFeePaymentDictionary = new Dictionary<int, Dictionary<TrancheCashFlowType, double>>();

        public override void CalculateProportionToDistribute(
            int monthlyPeriod, 
            TrancheCashFlowType trancheCashFlowType,
            Tranche currentSecuritizationTranche,
            SecuritizationNodeTree securitizationNode)
        {
            if (!securitizationNode.AnyNodesOrTranches)
            {
                ProportionToDistribute = 1.0;
            }
            
            if (!_totalFeePaymentDictionary.ContainsKey(monthlyPeriod))
            {
                _totalFeePaymentDictionary.Add(monthlyPeriod, new Dictionary<TrancheCashFlowType, double>());
            }

            double totalFeePayment;
            if (_totalFeePaymentDictionary[monthlyPeriod].ContainsKey(trancheCashFlowType))
            {
                totalFeePayment = _totalFeePaymentDictionary[monthlyPeriod][trancheCashFlowType];
            }
            else
            {
                totalFeePayment = CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationNode);
                _totalFeePaymentDictionary[monthlyPeriod].Add(trancheCashFlowType, totalFeePayment);
            }

            if (totalFeePayment <= 0.0)
            {
                ProportionToDistribute = 1.0;
                return;
            }

            var returnOnlyShortfall = (trancheCashFlowType == TrancheCashFlowType.FeesShortfall);
            var currentFeeTranche = currentSecuritizationTranche as FeeTranche;
            var currentFeePayment = currentFeeTranche.GetNextScheduledFeePayment(monthlyPeriod, returnOnlyShortfall);

            var proportionToDistribute = currentFeePayment / totalFeePayment;
            ProportionToDistribute = proportionToDistribute;
        }

        public override DistributionRule Copy()
        {
            return new NextFeePaymentProRataDistributionRule();
        }

        protected override double CalculateSumOfBalancesAtNode(int monthlyPeriod, TrancheCashFlowType trancheCashFlowType, SecuritizationNodeTree securitizationNode)
        {
            var totalFeePayment = 0.0;

            if (securitizationNode.AnyNodes)
            {
                foreach (var securitizationSubNode in securitizationNode.SecuritizationNodes)
                {
                    var sumOfInitialBalancesAtSubNode = 
                        CalculateSumOfBalancesAtNode(monthlyPeriod, trancheCashFlowType, securitizationSubNode);

                    totalFeePayment += sumOfInitialBalancesAtSubNode;
                }
            }

            if (securitizationNode.AnyTranches)
            {
                var securitizationTranchesAtNode = securitizationNode.SecuritizationTranches;
                if (securitizationTranchesAtNode.Any(s => (s as FeeTranche) == null))
                {
                    throw new Exception("ERROR: All elements subject to pro-rating based on the next payment must be fee payments");
                }

                var returnOnlyShortfall = (trancheCashFlowType == TrancheCashFlowType.FeesShortfall);

                foreach (var securitizationTranche in securitizationTranchesAtNode)
                {
                    var feePaymentTranche = securitizationTranche as FeeTranche;
                    var nextFeePayment = feePaymentTranche.GetNextScheduledFeePayment(monthlyPeriod, returnOnlyShortfall);
                    totalFeePayment += nextFeePayment;
                }
            }

            return totalFeePayment;
        }
    }
}
