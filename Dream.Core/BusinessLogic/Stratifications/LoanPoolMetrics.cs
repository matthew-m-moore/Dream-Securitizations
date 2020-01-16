using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.ProductTypes;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.BusinessLogic.Stratifications
{
    public class LoanPoolMetrics
    {
        public static double CalculateWeightedAverageInitialCoupon(List<Loan> loans)
        {
            var balanceWeights = loans.Select(l => l.Balance).ToList();
            var initialCouponValues = loans.Select(l => l.InitialCouponRate).ToList();

            var weightedAverageCoupon = MathUtility.WeightedAverage(balanceWeights, initialCouponValues);
            return weightedAverageCoupon;
        }

        public static double CalculateAverageBalance(List<Loan> loans)
        {
            var balances = loans.Select(l => l.Balance).ToList();
            var averageBalance = balances.Average();

            return averageBalance;
        }
    }
}
