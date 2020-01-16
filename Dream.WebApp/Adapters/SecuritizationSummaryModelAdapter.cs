using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.WebApp.ModelEntries;
using Dream.WebApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dream.WebApp.Adapters
{
    public class SecuritizationSummaryModelAdapter
    {
        public static SecuritizationSummaryModel GetSecuritizationSummaryModel(Securitization securitization, bool isResecuritization)
        {
            var resultsDictionary = securitization.ResultsDictionary;

            var securitizationSummaryModel = new SecuritizationSummaryModel();
            var securitizationSummaryModelEntries = new List<SecuritizationSummaryModelEntry>();
            securitizationSummaryModel.SecuritizationSummaryModelEntries = securitizationSummaryModelEntries;

            if (resultsDictionary == null || !resultsDictionary.Any()) return securitizationSummaryModel;

            var totalCollateralBalance = isResecuritization
                ? ((Resecuritization) securitization).GetResecuritizationCollateralBalance()
                : securitization.CollateralRetriever.GetTotalCollateralBalance();

            // First, capture all the tranche information
            foreach (var securitizationTrancheName in resultsDictionary.SecuritizationTrancheNamesOfDisplayableResults)
            {
                var securitizationTranche = securitization.TranchesDictionary[securitizationTrancheName].Tranche;
                var securitizationResult = resultsDictionary[securitizationTranche.TranchePricingScenario];
                var securitizationTrancheSummaryResult = securitizationResult.SecuritizationResultsDictionary[securitizationTrancheName];

                var securitizationSummaryModelEntry = new SecuritizationSummaryModelEntry
                {
                    SecuritizationNodeId = securitizationTranche.SecuritizationNode.SecuritizationNodeId,
                    SecuritizationNodeName = securitizationTranche.SecuritizationNode.SecuritizationNodeName,
                    SecuritizationNodeTrancheId = securitizationTranche.TrancheDetailId,
                    SecuritizationNodeOrTrancheName = securitizationTrancheName,
                    SecuritizationNodeOrTrancheType = securitizationTranche.TrancheDescription,
                    SecuritizationNodeOrTrancheRating = securitizationTranche.TrancheRating,
                    SecuritizationNodeOrTrancheBalance = securitizationTranche.InitialBalance,
                    SecuritizationNodeOrTrancheSizePercentage = securitizationTranche.InitialBalance / totalCollateralBalance,
                    WeightedAverageLife = securitizationTrancheSummaryResult.WeightedAverageLife,
                    ModifiedDuration = securitizationTrancheSummaryResult.ModifiedDurationAnalytical,
                    PaymentWindow = securitizationTrancheSummaryResult.PaymentCorridor.ToString(),
                    BenchmarkYield = securitizationTrancheSummaryResult.NominalBenchmarkRate,
                    NominalSpread = securitizationTrancheSummaryResult.NominalSpread,
                    InternalRateOfReturn = securitizationTrancheSummaryResult.InternalRateOfReturn,
                    PresentValue = securitizationTrancheSummaryResult.PresentValue,
                    DollarPrice = securitizationTrancheSummaryResult.DollarPrice,
                    PercentageOfCollateral = securitizationTrancheSummaryResult.PresentValue / totalCollateralBalance,
                    IsSecuritizationNode = false,
                };

                if (securitizationTranche is InterestPayingTranche interestPayingTranche)
                {
                    securitizationSummaryModelEntry.SecuritizationNodeOrTrancheInitialCoupon = interestPayingTranche.InitialCoupon;
                }

                securitizationSummaryModelEntries.Add(securitizationSummaryModelEntry);
            }

            // Now, as long as we are looping through these nodes from child to parent (which should be the case), the algorithm below will behave
            foreach (var securitizationNodeName in resultsDictionary.SecuritizationNodeNamesOfDisplayableResults)
            {
                var securitizationNode = securitization.NodesDictionary[securitizationNodeName];
                var securitizationResult = resultsDictionary[securitizationNode.SecuritizationNodePricingScenario];
                var securitizationNodeSummaryResult = securitizationResult.SecuritizationResultsDictionary[securitizationNodeName];

                var securitizationSummaryModelEntry = new SecuritizationSummaryModelEntry
                {
                    SecuritizationNodeId = securitizationNode.SecuritizationNodeId,
                    SecuritizationNodeName = securitizationNodeName,
                    SecuritizationNodeOrTrancheType = securitizationNode.SecuritizationNodeType,
                    SecuritizationNodeOrTrancheRating = securitizationNode.SecuritizationNodeRating,
                    WeightedAverageLife = securitizationNodeSummaryResult.WeightedAverageLife,
                    ModifiedDuration = securitizationNodeSummaryResult.ModifiedDurationAnalytical,
                    PaymentWindow = securitizationNodeSummaryResult.PaymentCorridor.ToString(),
                    BenchmarkYield = securitizationNodeSummaryResult.NominalBenchmarkRate,
                    NominalSpread = securitizationNodeSummaryResult.NominalSpread,
                    InternalRateOfReturn = securitizationNodeSummaryResult.InternalRateOfReturn,
                    PresentValue = securitizationNodeSummaryResult.PresentValue,
                    DollarPrice = securitizationNodeSummaryResult.DollarPrice,
                    PercentageOfCollateral = securitizationNodeSummaryResult.PresentValue / totalCollateralBalance,
                    IsSecuritizationNode = true,
                };

                securitizationSummaryModelEntry.SecuritizationNodeOrTrancheBalance = securitizationSummaryModelEntries
                    .Where(s => s.SecuritizationNodeName == securitizationNodeName)
                    .Sum(t => t.SecuritizationNodeOrTrancheBalance);

                securitizationSummaryModelEntry.SecuritizationNodeOrTrancheInitialCoupon =
                    CalculateWeightedAverageInitialCoupon(securitizationSummaryModelEntries, securitizationNodeName);

                securitizationSummaryModelEntry.SecuritizationNodeOrTrancheSizePercentage =
                    securitizationSummaryModelEntry.SecuritizationNodeOrTrancheBalance / totalCollateralBalance;

                securitizationSummaryModelEntries.Add(securitizationSummaryModelEntry);
            }

            securitizationSummaryModel.SecuritizationSummaryModelEntries = securitizationSummaryModelEntries
                .OrderBy(e => e.SecuritizationNodeName)
                .ThenBy(e => e.SecuritizationNodeOrTrancheName).ToList();

            return securitizationSummaryModel;
        }

        private static double? CalculateWeightedAverageInitialCoupon(
            List<SecuritizationSummaryModelEntry> securitizationSummaryModelEntries,
            string securitizationNodeName)
        {
            var initialCouponValues = securitizationSummaryModelEntries
                .Where(e => e.SecuritizationNodeName == securitizationNodeName)
                .Select(t => t.SecuritizationNodeOrTrancheInitialCoupon).ToList();

            if (!initialCouponValues.All(v => v.HasValue)) return null;

            var balanceWeights = securitizationSummaryModelEntries
                .Where(e => e.SecuritizationNodeName == securitizationNodeName)
                .Select(t => t.SecuritizationNodeOrTrancheBalance).ToList();

            if (!balanceWeights.All(w => w.HasValue)) return null;

            var weightedAverageCoupon = MathUtility.WeightedAverage(
                balanceWeights.Select(w => w.Value).ToList(),
                initialCouponValues.Select(v => v.Value).ToList());

            return weightedAverageCoupon;
        }
    }
}