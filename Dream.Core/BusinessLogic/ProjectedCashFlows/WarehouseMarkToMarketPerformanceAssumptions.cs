using Dream.Common.Curves;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.Stratifications;
using System.Collections.Generic;

namespace Dream.Core.BusinessLogic.ProjectedCashFlows
{
    public class WarehouseMarkToMarketPerformanceAssumptions
    {
        // Note, the language from the credit agreement for the warehouse line is
        // listed below.
        // ===========================================================================
        // "Base Case" means the set of performance assumptions necessary to project
        // monthly cash receipts from one or multiple Borrower Bonds:	
        // (a) prepayment rate of:	
        //     (i) CPR equal to 6.0% if the weighted average interest rate of the
        //     Borrower Bonds is less than 5.0% per annum,
        //     (ii) CPR equal to 7.0% if the weighted average interest rate of the
        //     Borrower Bonds is less than 6.0% and greater than or equal to 5.0% per
        //     annum,
        //     (iii) CPR equal to 8.0% if the weighted average interest rate of the
        //     Borrower Bonds is greater than or equal to 6.0% per annum;
        // (b) CDR equal to 0.0%; and
        // (c) loss severity of 0.0%.	
        // ===========================================================================

        /// <summary>
        /// Sets up the flat CPR assumption associated with the WAC of a set of loans specified.
        /// </summary>
        public static void SetupPerformanceAssumptions(ProjectedCashFlowLogic cashFlowLogic, List<Loan> loans)
        {
            var constantPrepaymentRate = GetConstantPrepaymentAssumption(loans);
            var singleMonthlyMortality = MathUtility.ConvertAnnualRateToMonthlyRate(constantPrepaymentRate);
            var performanceAssumptionDescription = constantPrepaymentRate.ToString("0.0%") + " CPR";

            var constantPrepaymentRateCurve = new Curve<double>(singleMonthlyMortality);
            cashFlowLogic.ProjectedPerformanceAssumptions
                [performanceAssumptionDescription, PerformanceCurveType.Smm] = constantPrepaymentRateCurve;

            cashFlowLogic.ProjectedPerformanceAssumptions
                .PerformanceAssumptionsMapping
                .SetupGlobalPerformanceAssumptionsMapping(string.Empty, performanceAssumptionDescription, loans);
        }

        /// <summary>
        /// Returns the flat CPR assumption required for the warehouse mark-to-market process, per the WAC of the loans specified.
        /// </summary>
        private static double GetConstantPrepaymentAssumption(List<Loan> loans)
        {
            var weightedAverageCoupon = LoanPoolMetrics.CalculateWeightedAverageInitialCoupon(loans);
            
            if (weightedAverageCoupon < 0.05)
            {
                return 0.06;
            }
            else if (weightedAverageCoupon < 0.06)
            {
                return 0.07;
            }

            return 0.08;
        }
    }
}
