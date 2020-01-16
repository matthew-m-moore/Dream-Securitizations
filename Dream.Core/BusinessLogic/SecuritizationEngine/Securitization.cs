using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers.CashFlows;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.ProjectedCashFlows.CashFlowGeneration;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.Reporting.Results;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Dream.Core.BusinessLogic.SecuritizationEngine
{
    public class Securitization
    {
        private bool _hasRedemptionOccured = false;
        private bool _copyCollateral = false;

        protected List<ProjectedCashFlow> _ProjectedCashFlowsOnCollateral;

        public string Description { get; set; }
        public string Comments { get; set; }
        public string Owner { get; set; }
        public bool IsReadOnlyToOthers { get; set; }
        public int? SecuritizationAnalysisDataSetId { get; set; }
        public int? SecuritizationAnalysisVersionId { get; set; }

        public ICollateralRetriever CollateralRetriever { get; set; }
        public SecuritizationInput Inputs { get; set; }
        public AggregationGroupings AggregationGroupings { get; set; }
        public ProjectedCashFlowLogic ProjectedCashFlowLogic { get; set; }     
        public PriorityOfPayments PriorityOfPayments { get; set; }
        public List<RedemptionLogic> RedemptionLogicList { get; set; }
        public List<SecuritizationNodeTree> SecuritizationNodes { get; set; }  

        public AvailableFunds AvailableFunds { get; protected set; }
        public SecuritizationResultsDictionary ResultsDictionary { get; protected set; }

        private List<Loan> _collateral;
        public List<Loan> Collateral
        {
            get
            {
                if (_collateral == null)
                {
                    CollateralRetriever.SetCollateralDates(Inputs);
                    _collateral = CollateralRetriever.GetCollateral();
                }

                return _collateral;
            }
        }

        protected Dictionary<string, SecuritizationNodeTree> _NodesDictionary;
        public Dictionary<string, SecuritizationNodeTree> NodesDictionary
        {
            get
            {
                if (_NodesDictionary == null)
                {
                    // Traverse the node tree, keeping the linkage between tranches and nodes intact
                    _NodesDictionary = new Dictionary<string, SecuritizationNodeTree>();
                    foreach (var securitizationNode in SecuritizationNodes)
                    {
                        var nodesDictionary = securitizationNode.RetrieveNodesDictionary();
                        _NodesDictionary.Combine(nodesDictionary);
                    }
                }

                return _NodesDictionary;
            }
        }

        protected Dictionary<string, TrancheNodeStruct> _TranchesDictionary;
        public Dictionary<string, TrancheNodeStruct> TranchesDictionary
        {
            get
            {
                if (_TranchesDictionary == null)
                {
                    // Traverse the node tree, keeping the linkage between tranches and nodes intact
                    _TranchesDictionary = new Dictionary<string, TrancheNodeStruct>();
                    foreach (var securitizationNode in SecuritizationNodes)
                    {
                        if (securitizationNode.ParentSecuritizationNode != null) continue;
                        var nodeTranchesDictionary = securitizationNode.RetrieveTranchesDictionary();
                        _TranchesDictionary.Combine(nodeTranchesDictionary);
                    }
                }

                return _TranchesDictionary;
            }
        }

        public Securitization(SecuritizationInput securitizationInputs)
        {
            Inputs = securitizationInputs;

            SecuritizationNodes = new List<SecuritizationNodeTree>();
            RedemptionLogicList = new List<RedemptionLogic>();

            ResultsDictionary = new SecuritizationResultsDictionary();
        }

        public Securitization(
            ICollateralRetriever collateralRetriever,
            SecuritizationInput securitizationInputs,
            AggregationGroupings aggregationGroupings,
            ProjectedCashFlowLogic projectedCashFlowLogic)
        {
            CollateralRetriever = collateralRetriever;
            Inputs = securitizationInputs;
            AggregationGroupings = aggregationGroupings;
            ProjectedCashFlowLogic = projectedCashFlowLogic;

            SecuritizationNodes = new List<SecuritizationNodeTree>();
            RedemptionLogicList = new List<RedemptionLogic>();

            ResultsDictionary = new SecuritizationResultsDictionary();
        }

        public Securitization(
            ICollateralRetriever collateralRetriever,
            SecuritizationInput securitizationInputs,
            AggregationGroupings aggregationGroupings,
            ProjectedCashFlowLogic projectedCashFlowLogic,        
            PriorityOfPayments priorityOfPayments,
            List<RedemptionLogic> redemptionLogicList,
            List<SecuritizationNodeTree> securitizationNodes,
            List<ProjectedCashFlow> projectedCashFlowsOnCollateral = null) 
                : this(collateralRetriever, securitizationInputs, aggregationGroupings, projectedCashFlowLogic)
        {
            SecuritizationNodes = securitizationNodes;
            PriorityOfPayments = priorityOfPayments;
            RedemptionLogicList = redemptionLogicList;

            _ProjectedCashFlowsOnCollateral = projectedCashFlowsOnCollateral;
        }

        /// <summary>
        /// Returns a deep, member-wise copy of the object.
        /// </summary>
        public virtual Securitization Copy()
        {
            var securitization = new Securitization(
                CollateralRetriever.Copy(),
                Inputs.Copy(),
                AggregationGroupings.Copy(),
                ProjectedCashFlowLogic.Copy(),
                PriorityOfPayments.Copy(),
                RedemptionLogicList.Where(r => !r.TreatAsCleanUpCall).Select(r => r.Copy()).ToList(),
                SecuritizationNodes.Where(n => n.ParentSecuritizationNode == null).Select(s => s.Copy()).ToList(),
                _ProjectedCashFlowsOnCollateral)
            {
                Description = (Description != null) ? new string(Description.ToCharArray()) : null,
                SecuritizationAnalysisDataSetId = SecuritizationAnalysisDataSetId,
                SecuritizationAnalysisVersionId = SecuritizationAnalysisVersionId
            };

            foreach (var securitizationNode in securitization.SecuritizationNodes)
            {
                securitizationNode.AssignParentNodes();
            }

            if (_copyCollateral)
            {
                var copiedCollateral = securitization.Collateral.Select(c => c.Copy()).ToList();
                securitization.SetCollateral(copiedCollateral);
            }

            SpecialCopyLogicForRedemptionLogic(securitization);
            SpecialCopyLogicForTrancheBalanceFees(securitization);

            return securitization;
        }

        /// <summary>
        /// This is the primary method for running securitization cash flows and analysis. It returns the results at a tranche
        /// level as well as the projected cash flows for the collateral.
        /// </summary>
        public SecuritizationResult RunSecuritizationAnalysis()
        {
            // Add a clean-up call if the inputs specify it, but do not copy this with any scenarios
            if (Inputs.CleanUpCallPercentage.HasValue) AddCleanUpCallRedemptionLogic();

            // Generating the collateral cash flows populates the AvailableFunds object
            var securitizationResult = GenerateCollateralCashFlows(out int cashFlowPeriods);

            // Note, the priority of payments may change after redemption, so it must be captured here
            securitizationResult.PriorityOfPayments = PriorityOfPayments;
            if (PriorityOfPayments != null) TraversePriorityOfPaymentsWaterfall(cashFlowPeriods);

            // Run the analysis on each tranche for pricing, yield, etc.
            var securitizationResults = new Dictionary<string, SecuritizationCashFlowsSummaryResult>();
            foreach (var securitizationNode in SecuritizationNodes)
            {
                var nodeTrancheResults = securitizationNode.RunAnalysis(
                    Inputs.MarketRateEnvironment,
                    Inputs.MarketDataGroupingForNominalSpread,
                    Inputs.CurveTypeForSpreadCalcultion);
                securitizationResults.Combine(nodeTrancheResults);
            }

            // Add the tranche results to the securitization result, among other things         
            securitizationResult.SecuritizationResultsDictionary = securitizationResults;
            securitizationResult.AvailableFundsCashFlows = AvailableFunds.GetAvailableFundsCashFlows();
            securitizationResult.MarketRateEnvironment = Inputs.MarketRateEnvironment;
            securitizationResult.MarketDataForNominalSpread = Inputs.MarketDataGroupingForNominalSpread;
            securitizationResult.CurveTypeForSpreadCalcultion = Inputs.CurveTypeForSpreadCalcultion;
            securitizationResult.ScenarioDescription = Inputs.ScenarioDescription;
            return securitizationResult;
        }

        /// <summary>
        /// Runs a specified collection of securitization scenarios and produces a dictionary of results.
        /// </summary>
        public Dictionary<string, SecuritizationResult> RunSecuritizationAnalysis(List<ScenarioAnalysis> scenariosToAnalyze)
        {
            var securitizationResultsDictionary = new Dictionary<string, SecuritizationResult>();

            // Running this securitization may alter it, such as when the redemption priority of payments kicks in
            var baseSecuritization = Copy();
            var baseSecuritizationResult = baseSecuritization.RunSecuritizationAnalysis();
            securitizationResultsDictionary.Add(Inputs.ScenarioDescription, baseSecuritizationResult);
            Console.WriteLine("Ran '" + Inputs.ScenarioDescription + "' Scenario");

            if (baseSecuritizationResult.ScenarioDescription != Inputs.ScenarioDescription)
            {
                throw new Exception("INTERNAL ERROR: Scenario description must match key used in results dictionary. Please report this error.");
            }

            if (!scenariosToAnalyze.Any()) return securitizationResultsDictionary;

            foreach (var scenario in scenariosToAnalyze)
            {
                var securitizationScenario = scenario.ApplyScenario(this, baseSecuritizationResult);
                var securitizationScenarioResultsDictionary = securitizationScenario.RunSecuritizationAnalysis(scenario.NestedScenarios);
                securitizationResultsDictionary.Combine(securitizationScenarioResultsDictionary);
            }

            ResultsDictionary = new SecuritizationResultsDictionary(securitizationResultsDictionary);
            return securitizationResultsDictionary;
        }

        /// <summary>
        /// This method can be used to set the collateral cash flows to some specific values, rather than generating them from the collateral supplied.
        /// This option is more commonly used when running scenario analyses, or for re-securitization analysis.
        /// </summary>
        public void SetCollateralCashFlows(List<ProjectedCashFlow> projectedCashFlowsOnCollateral)
        {
            _ProjectedCashFlowsOnCollateral = projectedCashFlowsOnCollateral;
        }

        /// <summary>
        /// This method can be used to set the collateral explicitely, rather than getting it from the collateral retriever.
        /// This option is more commonly used when running scenario analyses, or for re-securitization analysis.
        /// </summary>
        public void SetCollateral(List<Loan> collateral)
        {
            _copyCollateral = true;
            _collateral = collateral;
        }

        protected virtual SecuritizationResult GenerateCollateralCashFlows(out int cashFlowPeriods)
        {
            var securitizationResult = new SecuritizationResult();
            var totalCashFlow = new KeyValuePair<string, List<ProjectedCashFlow>>();
            var cashFlowDictionary = new Dictionary<string, List<ProjectedCashFlow>>();

            if (_ProjectedCashFlowsOnCollateral == null)
            {
                // Generate the contractual and projected cash flows for the collateral
                var projectedCashFlowGenerator = new ProjectedCashFlowGenerator<ProjectedCashFlow>(ProjectedCashFlowLogic, Inputs);
                var listOfProjectedCashFlows = projectedCashFlowGenerator.GenerateCashFlowsOnListOfLoans(Collateral, out List<Loan> collateralOrReplines);

                // Aggregate the cash flows in total and set up the AvailableFunds object
                SetupAggregationGroupings(collateralOrReplines);
                var selectedAggregationGrouping = Inputs.SelectedAggregationGrouping;
                var cashFlowAggregator = new CashFlowAggregator(Inputs, AggregationGroupings, selectedAggregationGrouping);
                totalCashFlow = cashFlowAggregator.AggregateTotalCashFlows(collateralOrReplines, listOfProjectedCashFlows);

                // Aggregate the cash flows by the selected aggregation group
                cashFlowDictionary = cashFlowAggregator.AggregateCashFlows(collateralOrReplines, listOfProjectedCashFlows);
            }
            else
            {
                totalCashFlow = new KeyValuePair<string, List<ProjectedCashFlow>>(
                    AggregationGroupings.TotalAggregationGroupName,
                    _ProjectedCashFlowsOnCollateral);
            }

            cashFlowDictionary.Add(totalCashFlow);

            // Convert the cash flows dictionary to a list of pricing results
            var projectedCashFlowsAnalysis = new ProjectedCashFlowsAnalysis(Inputs.CashFlowPricingStrategy, cashFlowDictionary);
            var dictionaryOfResults = projectedCashFlowsAnalysis.RunAnalysis(
                Inputs.MarketRateEnvironment,
                Inputs.MarketDataGroupingForNominalSpread,
                Inputs.CurveTypeForSpreadCalcultion);

            var dateAggregatedTotalCashFlows = CashFlowAggregator.AggregateCashFlowsByDate(
                totalCashFlow.Value,
                Inputs.SecuritizationStartDate,
                Inputs.SecuritizationFirstCashFlowDate);

            AvailableFunds = new AvailableFunds(dateAggregatedTotalCashFlows);
            cashFlowPeriods = dateAggregatedTotalCashFlows.Count;

            foreach (var redemptionLogic in RedemptionLogicList) redemptionLogic.AvailableFunds = AvailableFunds;

            // Add the aggregated cash flows to the securitization result
            securitizationResult.CollateralCashFlowsResultsDictionary = dictionaryOfResults;
            securitizationResult.ProjectedCashFlowsOnCollateral = AvailableFunds.ProjectedCashFlowsOnCollateral;
            return securitizationResult;
        }

        private void SetupAggregationGroupings(List<Loan> collateralOrReplines)
        {
            var selectedAggregationGrouping = Inputs.SelectedAggregationGrouping;
            if (selectedAggregationGrouping == null 
                || selectedAggregationGrouping == Constants.Automatic 
                || selectedAggregationGrouping == Constants.LoanLevel)
            {
                if (Inputs.UseReplines || selectedAggregationGrouping == Constants.LoanLevel)
                {
                    AggregationGroupings = AggregationGroupings.SetupAutomaticLoanLevelAggregation(collateralOrReplines);
                    Inputs.SelectedAggregationGrouping = AggregationGroupings.LoanLevelAggregationGroupingIdentifier;
                }
                else
                {
                    AggregationGroupings = AggregationGroupings.SetupAutomaticTotalCollateralAggregation(collateralOrReplines);
                    Inputs.SelectedAggregationGrouping = AggregationGroupings.MaturityTermAggregationGroupingIdentifier;
                }
            }
        }

        private void AddCleanUpCallRedemptionLogic()
        {
            var cleanUpCallRedemptionLogic = new LessThanPercentOfInitalCollateralBalanceRedemptionLogic(Inputs.CleanUpCallPercentage.Value)
            {
                PriorityOfPayments = PriorityOfPayments,
                TreatAsCleanUpCall = true
            };

            RedemptionLogicList.Add(cleanUpCallRedemptionLogic);
        }

        private void TraversePriorityOfPaymentsWaterfall(int cashFlowPeriods)
        {
            // This is important to set up the redemption logic, if any exists
            foreach (var redemptionLogic in RedemptionLogicList) redemptionLogic.TranchesDictionary = TranchesDictionary;

            // Now, for each period, distribute the cash flows according to the rules of the securitization
            for (var monthlyPeriod = 0; monthlyPeriod < cashFlowPeriods; monthlyPeriod++)
            {
                AvailableFunds.PopulateStartingAvailableFunds(
                        monthlyPeriod,
                        Inputs.SecuritizationStartDate,
                        Inputs.SecuritizationFirstCashFlowDate);

                AddNewTrancheCashFlows(monthlyPeriod, TranchesDictionary);
                var shortfallReservesAllocator = new ShortfallReservesAllocator();

                foreach (var priorityOfPaymentEntry in PriorityOfPayments.OrderedListOfEntries)
                {
                    var trancheName = priorityOfPaymentEntry.TrancheName;
                    if (!TranchesDictionary.ContainsKey(trancheName)) continue;

                    var trancheNodeStruct = TranchesDictionary[trancheName];
                    var securitizationTranche = trancheNodeStruct.Tranche;
                    var securitizationNode = trancheNodeStruct.SecuritizationNode;

                    // These two items need to be in lock step throughout the waterfall
                    securitizationTranche.SetSeniorityRanking(priorityOfPaymentEntry.SeniorityRanking);
                    securitizationTranche.ShortfallReservesAllocator = shortfallReservesAllocator;
                    var availableFundsDistributionRule = securitizationNode.AvailableFundsDistributionRule;

                    // Keeping the tranches tied to their nodes allows accessibility to running the distribution rule
                    var trancheCashFlowType = priorityOfPaymentEntry.TrancheCashFlowType;
                    securitizationTranche.ShortfallReservesAllocator.TrancheCashFlowType = trancheCashFlowType;
                    availableFundsDistributionRule.CalculateProportionToDistribute(
                        monthlyPeriod,
                        trancheCashFlowType,
                        securitizationTranche,
                        securitizationNode);

                    // This is where the priority of payments can diverage for principal versus interest or other types of payments
                    switch (trancheCashFlowType)
                    {
                        case TrancheCashFlowType.Interest:
                        case TrancheCashFlowType.AccruedInterest:
                            var interestPayingTranche = TryCasteToInterestPayingTranche(securitizationTranche);
                            interestPayingTranche.AllocateInterestCashFlows(
                                AvailableFunds,
                                availableFundsDistributionRule,
                                monthlyPeriod);
                            break;

                        case TrancheCashFlowType.InterestShortfall:
                            var interestShortfallTranche = TryCasteToInterestPayingTranche(securitizationTranche);
                            interestShortfallTranche.AllocateInterestShortfall(
                                AvailableFunds,
                                availableFundsDistributionRule,
                                monthlyPeriod);
                            break;
                        
                        case TrancheCashFlowType.Reserves:
                            var reserveFundTranche = TryCasteToReserveFundTranche(securitizationTranche);
                            reserveFundTranche.AllocatePaymentCashFlows(
                                AvailableFunds,
                                availableFundsDistributionRule,
                                monthlyPeriod);

                            AvailableFunds.PopulateReserveFundsFromTrancheForGivenPeriod(reserveFundTranche, monthlyPeriod);
                            break;

                        case TrancheCashFlowType.PaymentShortfall:
                        case TrancheCashFlowType.FeesShortfall:
                            securitizationTranche.AllocatePaymentShortfall(
                                AvailableFunds, 
                                availableFundsDistributionRule,
                                monthlyPeriod);
                            break;

                        default:
                            // This has to come first in order to grab any reserves released prior to a payment being determined
                            securitizationTranche.AccrueAnyAssociatedReservesReleased(monthlyPeriod, AvailableFunds);
                            securitizationTranche.AllocatePaymentCashFlows(
                                AvailableFunds,
                                availableFundsDistributionRule,
                                monthlyPeriod);

                            // This will take care of any remaining, but unpaid available funds
                            securitizationTranche.AccrueRemainingAvailableFunds(monthlyPeriod, AvailableFunds);                           
                            break;
                    }

                    // This will ensure that reserve funds used to make up insufficient remittances
                    // are correctly reflected in the associate reserve fund tranche
                    ApplyBalanceChangesToReserveFunds(TranchesDictionary, monthlyPeriod);
                }

                // This will apply any clean-up call logic for the the securitization
                foreach (var redemptionLogic in RedemptionLogicList)
                {
                    if (redemptionLogic.IsRedemptionTriggered(monthlyPeriod) && !_hasRedemptionOccured)
                    {                       
                        ClearDistributionRuleData(monthlyPeriod, SecuritizationNodes);
                        redemptionLogic.ProcessRedemption(monthlyPeriod, Inputs);

                        // A clean-up call terminates the securitization
                        if (redemptionLogic.TreatAsCleanUpCall) return;

                        // Remember to switch over to the post-redemption priority of payments after processing the redemption
                        PriorityOfPayments = redemptionLogic.PostRedemptionPriorityOfPayments;
                        _hasRedemptionOccured = true;

                        // This break ensures that only the first, and highest priority, redemption occurs
                        break;
                    }
                }
            }
        }

        private void AddNewTrancheCashFlows(int monthlyPeriod, Dictionary<string, TrancheNodeStruct> tranchesDictionary)
        {
            foreach (var securitizationNodeTranche in tranchesDictionary.Values)
            {
                securitizationNodeTranche.Tranche.AddNewTrancheCashFlow(
                    monthlyPeriod,
                    Inputs.SecuritizationStartDate,
                    Inputs.SecuritizationFirstCashFlowDate);
            }
        }

        private ReserveFundTranche TryCasteToReserveFundTranche(Tranche securitizationTranche)
        {
            var reserveFundTranche = securitizationTranche as ReserveFundTranche;
            if (reserveFundTranche == null)
            {
                throw new Exception(string.Format("ERROR: The tranche named {0} is not a reserve fund. Please check the priority of payments setup.",
                    securitizationTranche.TrancheName));
            }

            return reserveFundTranche;
        }

        private InterestPayingTranche TryCasteToInterestPayingTranche(Tranche securitizationTranche)
        {
            var interestPayingTranche = securitizationTranche as InterestPayingTranche;
            if (interestPayingTranche == null)
            {
                throw new Exception(string.Format("ERROR: The tranche named {0} is not an interest-paying tranche. Please check the priority of payments setup.",
                    securitizationTranche.TrancheName));
            }

            return interestPayingTranche;
        }

        private void ApplyBalanceChangesToReserveFunds(Dictionary<string, TrancheNodeStruct> tranchesDictionary, int monthlyPeriod)
        {
            var reserveFunds = AvailableFunds[monthlyPeriod].AvailableReserveFundsDictionary;
            if (reserveFunds.Any())
            {
                foreach(var reserveFund in reserveFunds)
                {
                    // If a reserve fund is no longer in the priority of payments (say, due to redemption)
                    // The we can skip this step for that reserve fund
                    if (!PriorityOfPayments.OrderedListOfEntries.Any(p => p.TrancheName == reserveFund.Key)) continue;

                    var associatedTranche = tranchesDictionary[reserveFund.Key].Tranche;
                    if (associatedTranche is ReserveFundTranche)
                    {
                        if (associatedTranche.TrancheCashFlows.ElementAtOrDefault(monthlyPeriod) == null) continue;
                        var reserveAmountUsed = associatedTranche.TrancheCashFlows[monthlyPeriod].EndingBalance - reserveFund.Value.FundEndingBalance;

                        associatedTranche.TrancheCashFlows[monthlyPeriod].Interest += reserveAmountUsed;
                        associatedTranche.TrancheCashFlows[monthlyPeriod].Principal -= reserveAmountUsed;
                        associatedTranche.TrancheCashFlows[monthlyPeriod].EndingBalance = reserveFund.Value.FundEndingBalance;
                    }
                    else
                    {
                        if (associatedTranche.TrancheCashFlows.ElementAtOrDefault(monthlyPeriod) == null)
                        {
                            associatedTranche.AddNewTrancheCashFlow(
                                monthlyPeriod,
                                Inputs.SecuritizationStartDate,
                                Inputs.SecuritizationFirstCashFlowDate);
                        }

                        // This is meant to handle the way in which accrued payments are treated like a reserve fund
                        var shortfallAbsorbed = reserveFund.Value.ShortfallAbsorbed;
                        associatedTranche.TrancheCashFlows[monthlyPeriod].AccruedPayment -= shortfallAbsorbed;
                        reserveFund.Value.ShortfallAbsorbed = 0.0;
                    }
                }
            }
        }

        protected void SpecialCopyLogicForRedemptionLogic(Securitization securitization)
        {
            var copiedRedemptionLogicList = new List<RedemptionLogic>();
            foreach (var redemptionLogic in securitization.RedemptionLogicList)
            {
                var tranchesPaidOutRedemptionLogic = redemptionLogic as TranchesCanBePaidOutFromAvailableFundsRedemptionLogic;
                if (tranchesPaidOutRedemptionLogic != null)
                {
                    // This redemption logic relies on having a direct reference to the tranches in use by the securitization
                    var existingRedemptionLogic = redemptionLogic as TranchesCanBePaidOutFromAvailableFundsRedemptionLogic;
                    var listOfTranchesToBePaidOut = securitization.TranchesDictionary
                        .Values.Select(s => s.Tranche)
                        .Where(t => existingRedemptionLogic.ListOfTranchesToBePaidOut.Any(l => l.TrancheName == t.TrancheName))
                        .ToList();

                    // If this list were cloned, this reference would not remain intact
                    tranchesPaidOutRedemptionLogic.ListOfTranchesToBePaidOut = listOfTranchesToBePaidOut;
                    copiedRedemptionLogicList.Add(tranchesPaidOutRedemptionLogic);
                }
                else
                {
                    copiedRedemptionLogicList.Add(redemptionLogic);
                }
            }

            securitization.RedemptionLogicList = copiedRedemptionLogicList;
        }

        private void SpecialCopyLogicForTrancheBalanceFees(Securitization securitization)
        {
            var trancheBalanceBasedFees = securitization.TranchesDictionary.Values.Select(v => v.Tranche).OfType<PercentOfTrancheBalanceFeeTranche>().ToList();
            foreach (var trancheBalanceBasedFee in trancheBalanceBasedFees)
            {
                var castedTrancheBalanceBasedFee = trancheBalanceBasedFee as PercentOfTrancheBalanceFeeTranche;
                castedTrancheBalanceBasedFee.Securitization = securitization;

                var feeTrancheName = trancheBalanceBasedFee.TrancheName;
                var trancheNodeStruct = securitization.TranchesDictionary[feeTrancheName];

                // Not really sure how necessary it is to do this, but the potential behavior of the struct is worrisome to me
                trancheNodeStruct.Tranche = castedTrancheBalanceBasedFee;
                securitization.TranchesDictionary[feeTrancheName] = trancheNodeStruct;
            }
        }

        private void ClearDistributionRuleData(int monthlyPeriod, List<SecuritizationNodeTree> SecuritizationNodes)
        {
            foreach (var securitizationNode in SecuritizationNodes)
            {
                var distributionRule = securitizationNode.AvailableFundsDistributionRule;
                distributionRule.ClearData(monthlyPeriod);

                if (securitizationNode.AnyNodes)
                {
                    ClearDistributionRuleData(monthlyPeriod, securitizationNode.SecuritizationNodes);
                }            
            }
        }
    }
}
