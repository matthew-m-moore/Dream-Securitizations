using Dream.Common.Enums;
using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Converters.Excel;
using Dream.Core.Converters.Excel.Securitization;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;

namespace Dream.Core.Repositories.Excel
{
    public class SecuritizationExcelDataRepository : LoanPoolExcelDataRepository
    {
        private const string _securitizationInputs = "SecuritizationInputs";

        private const string _trancheStructure = "TrancheStructure";
        private const string _reserveAccount = "ReserveAccount";
        private const string _feeGroupsOverview = "FeeGroupsOverview";
        private const string _feeDetails = "FeesDetails";

        private const string _priorityOfPayments = "PriorityOfPayments";

        protected List<ScenarioAnalysis> _ScenariosToAnalyze = new List<ScenarioAnalysis>();
        public List<ScenarioAnalysis> ScenariosToAnalyze => _ScenariosToAnalyze;

        public SecuritizationExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public SecuritizationExcelDataRepository(Stream fileStream) : base(fileStream) { }

        /// <summary>
        /// Retrieves a securitization of PACE Assessments from an Excel workbook inputs file.
        /// Does not define the tranche structure of the securitization.
        /// </summary>
        public virtual Securitization GetPaceSecuritizationWithoutNodesDefined()
        {
            var securitizationInputs = GetSecuritizationInputs(out SecuritizationInputsRecord baseSecuritizationInputsRecord);
            var baseSecuritizationInput = securitizationInputs.First();
            
            var paceSecuritization = GetPaceSecuritizationWithoutNodesDefined(baseSecuritizationInput);
            ProcessRemainingSecuritizationInputsAsScenarios(securitizationInputs);

            return paceSecuritization;
        }

        /// <summary>
        /// Retrieves a securitization of PACE Assessments from an Excel workbook inputs file using a specified securitization input.
        /// Does not define the tranche structure of the securitization.
        /// </summary>
        public virtual Securitization GetPaceSecuritizationWithoutNodesDefined(SecuritizationInput securitizationInput)
        {
            var rateEnvironmentRepository = new MarketRateEnvironmentExcelDataRepository(_ExcelFileReader);
            var marketRateEnvironment = rateEnvironmentRepository.GetMarketRateEnvironmentWithoutRateCurves(securitizationInput.SecuritizationStartDate);
            securitizationInput.MarketRateEnvironment = marketRateEnvironment;

            var paceExcelDataRepository = new PaceAssessmentExcelDataRepository(_ExcelFileReader);

            var aggregationGroupings = GetAggregationGroupings();
            var performanceAssumptions = GetProjectedPerformanceAssumptions();
            var selectedAssumptionsGrouping = securitizationInput.SelectedPerformanceAssumptionGrouping ?? string.Empty;
            var projectedCashFlowLogic = new ProjectedCashFlowLogic(performanceAssumptions, selectedAssumptionsGrouping);

            var paceSecuritization = new Securitization(paceExcelDataRepository, securitizationInput, aggregationGroupings, projectedCashFlowLogic)
                { Description = _ExcelFileReader.FileName };

            return paceSecuritization;
        }

        /// <summary>
        /// Retrieves a wholly defined securitization of PACE Assessments from an Excel workbook inputs file.
        /// </summary>
        public Securitization GetPaceSecuritization()
        {
            // The base securitization input will be needed later to specify redemption logic
            var securitizationInputs = GetSecuritizationInputs(out SecuritizationInputsRecord baseSecuritizationInputsRecord);
            var baseSecuritizationInput = securitizationInputs.First();

            var paceSecuritization = GetPaceSecuritizationWithoutNodesDefined(baseSecuritizationInput);
            ProcessRemainingSecuritizationInputsAsScenarios(securitizationInputs);

            // Priority of payments is cross-checked against all tranches loaded, so it is made first
            var isRedemptionPriorityOfPayments = false;
            var priorityOfPaymentsRecords = _ExcelFileReader.GetDataFromSpecificTab<PriorityOfPaymentsRecord>(_priorityOfPayments);
            var priorityOfPayments = PriorityOfPaymentsExcelConverter.ConvertListOfPriorityOfPaymentsRecords(
                priorityOfPaymentsRecords,
                isRedemptionPriorityOfPayments);

            // The reserve account must be created before it can be added to other tranches
            ReserveFundTranche reserveAccount = null;
            var reserveAccountRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<ReserveAccountRecord>(_reserveAccount);
            if (reserveAccountRecords.Any())
                reserveAccount = ReserveFundTrancheExcelConverter.ConvertListOfReserveAccountRecords(
                    paceSecuritization.Inputs.SecuritizationFirstCashFlowDate,
                    reserveAccountRecords);

            var trancheStructureRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<TrancheStructureRecord>(_trancheStructure);

            var trancheExcelConverter = new TrancheExcelConverter(
                paceSecuritization.Inputs.SecuritizationStartDate,
                paceSecuritization.Inputs.SecuritizationFirstCashFlowDate,
                paceSecuritization.Inputs.MarketRateEnvironment,
                priorityOfPayments,
                reserveAccount);

            var securitizationTranches = trancheExcelConverter.ConvertListOfTrancheStructureRecords(trancheStructureRecords);

            // Both fee groups and fee details are needed to create fee tranches
            var feeGroupRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<FeeGroupRecord>(_feeGroupsOverview);
            var feeDetailRecords = _ExcelFileReader.GetDataFromSpecificTab<FeeDetailRecord>(_feeDetails);

            var feeTrancheExcelConverter = new FeeTrancheExcelConverter(
                paceSecuritization.Inputs.SecuritizationFirstCashFlowDate,
                priorityOfPayments);

            var feeTranches = feeTrancheExcelConverter.ConvertListOfFeeGroupRecordsAndListOfFeeDetailRecords(feeGroupRecords, feeDetailRecords);
            if (reserveAccount != null)
            {
                AddReserveAccountToListOfTranches(reserveAccount, TrancheCashFlowType.Fees, feeTranches);
                AddReserveAccountToListOfTranches(reserveAccount, TrancheCashFlowType.FeesShortfall, feeTranches);
            }          

            // Here all the tranches are combined into one list
            securitizationTranches.AddRange(feeTranches);
            AddResidualTranchesAsReserves(securitizationTranches, priorityOfPayments);
            if (reserveAccount != null) securitizationTranches.Add(reserveAccount);

            // Setting up nodes again requires access to the Excel records
            var securitizationNodeId = 1;
            var securitizationNodeTreeConverter = new SecuritizationNodeTreeExcelConverter(securitizationTranches);
            securitizationNodeTreeConverter.AddReserveAccountRecordsToTree(reserveAccountRecords);
            securitizationNodeTreeConverter.AddTrancheStructureRecordsToTree(trancheStructureRecords, ref securitizationNodeId);
            securitizationNodeTreeConverter.AddFeeGroupRecordsToTree(feeGroupRecords, ref securitizationNodeId);

            // Last to get set up is redemption logic and the redemption priority of payments
            AddRedemptionLogicToSecuritization(
                paceSecuritization,
                baseSecuritizationInputsRecord,
                securitizationTranches,
                trancheStructureRecords,
                feeGroupRecords,
                priorityOfPaymentsRecords);

            // Now, add everything into the securitization object itself and return it
            paceSecuritization.SecuritizationNodes = securitizationNodeTreeConverter.SecuritizationNodes;
            paceSecuritization.PriorityOfPayments = priorityOfPayments;

            // Note, this could be read in from the inputs file, but N-Spread is the major metric in use here
            // If/when I-Spread becomes of interest, a more robust loading process could be set up
            paceSecuritization.Inputs.MarketDataGroupingForNominalSpread = MarketDataGrouping.Swaps;

            AddSecuritizationReferenceToSpecialFeeTranches(paceSecuritization);

            return paceSecuritization;
        }

        /// <summary>
        /// Retrieves the high-level securitization inputs from an Excel tab in a transposed format. 
        /// Assumes there is only one set of inputs provided.
        /// </summary>
        public virtual List<SecuritizationInput> GetSecuritizationInputs(out SecuritizationInputsRecord baseSecuritizationInputsRecord)
        {
            var securitizationInputRecords = _ExcelFileReader.GetTransposedDataFromSpecificTab<SecuritizationInputsRecord>(_securitizationInputs);
            var securitizationInputs = SecuritizationInputExcelConverter.ConvertListOfSecuritizationInputsRecords(securitizationInputRecords);

            baseSecuritizationInputsRecord = securitizationInputRecords.First();
            return securitizationInputs;
        }

        /// <summary>
        /// Adds additional securitization inputs provide as scenarios based of the first inputs provided
        /// </summary>
        protected virtual void ProcessRemainingSecuritizationInputsAsScenarios(List<SecuritizationInput> securitizationInputs)
        {
            var listOfSecuritizationInputsScenarios = new List<ScenarioAnalysis>();
            foreach (var securitizationInputsForScenario in securitizationInputs.Skip(1))
            {
                var securitizationInputsScenario = new InputsScenario(securitizationInputsForScenario);
                var scenarioLogicList = new List<IScenarioLogic> { securitizationInputsScenario };
                var scenarioAnalysis = new ScenarioAnalysis(securitizationInputsForScenario.ScenarioDescription, scenarioLogicList);

                listOfSecuritizationInputsScenarios.Add(scenarioAnalysis);
            }

            _ScenariosToAnalyze.AddRange(listOfSecuritizationInputsScenarios);
        }

        private void AddReserveAccountToListOfTranches(ReserveFundTranche reserveAccount, TrancheCashFlowType trancheCashFlowType, List<Tranche> securitizationTranches)
        {
            foreach (var securitizationTranche in securitizationTranches)
            {
                if (!securitizationTranche.IsShortfallPaidFromReserves && 
                    !securitizationTranche.AbsorbsAssociatedReservesReleased) continue;

                securitizationTranche.AddAssociatedReserveAccount(reserveAccount, trancheCashFlowType);
            }
        }

        private void AddResidualTranchesAsReserves(List<Tranche> securitizationTranches, PriorityOfPayments priorityOfPayments)
        {
            var trancheSeniorityDictionary = priorityOfPayments.OrderedListOfEntries
                .GroupBy(e => e.TrancheName, e => e.SeniorityRanking)
                .ToDictionary(g => g.Key, g => g.Min());

            var residualTranches = new List<Tranche> (securitizationTranches.OfType<ResidualTranche>());
            var additionalResidualTranches = securitizationTranches
                .Where(t => t.AbsorbsAssociatedReservesReleased && t.AbsorbsRemainingAvailableFunds &&
                            !(t is ResidualTranche)).ToList();

            residualTranches.AddRange(additionalResidualTranches);

            // This is kind of an icky nested loop, but I see no other more convenient way around it
            foreach (var securitizationTranche in securitizationTranches)
            {
                foreach (var residualTranche in residualTranches)
                {
                    var residualSeniority = trancheSeniorityDictionary[residualTranche.TrancheName];
                    var trancheSeniority = trancheSeniorityDictionary[securitizationTranche.TrancheName];

                    // Note, seniority is measured such that lower numbers are more senior, more like a priority
                    if (trancheSeniority <= residualSeniority)
                    {
                        if (securitizationTranche is FeeTranche)
                        {
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.Fees);
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.FeesShortfall);
                        }
                        if (securitizationTranche is InterestPayingTranche)
                        {
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.Interest);
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.InterestShortfall);
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.Principal);
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.PrincipalShortfall);
                        }
                        if (securitizationTranche is ResidualTranche)
                        {
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.Payment);
                            securitizationTranche.AddAssociatedReserveAccount(residualTranche, TrancheCashFlowType.PaymentShortfall);
                        }                       
                    }
                }
            }
        }

        private void AddRedemptionLogicToSecuritization(
            Securitization paceSecuritization,
            SecuritizationInputsRecord baseSecuritizationInputsRecord,
            List<Tranche> securitizationTranches,
            List<TrancheStructureRecord> trancheStructureRecords,
            List<FeeGroupRecord> feeGroupRecords,
            List<PriorityOfPaymentsRecord> priorityOfPaymentsRecords)
        {
            var redemptionLogicConverter = new RedemptionLogicExcelConverter(
                securitizationTranches,
                baseSecuritizationInputsRecord.RedemptionLogic,
                baseSecuritizationInputsRecord.RedemptionLogicTriggerValue);

            redemptionLogicConverter.AddTrancheStructureRecordsToRedemptionLogic(trancheStructureRecords);
            redemptionLogicConverter.AddFeeGroupRecordsToRedemptionLogic(feeGroupRecords);

            var isRedemptionPriorityOfPayments = true;
            var redemptionPriorityOfPayments = PriorityOfPaymentsExcelConverter.ConvertListOfPriorityOfPaymentsRecords(
                priorityOfPaymentsRecords,
                isRedemptionPriorityOfPayments);

            var checkPostRedemptionPriorityOfPayments = true;
            var postRedemptionPriorityOfPayments = PriorityOfPaymentsExcelConverter.ConvertListOfPriorityOfPaymentsRecords(
                priorityOfPaymentsRecords,
                isRedemptionPriorityOfPayments,
                checkPostRedemptionPriorityOfPayments);

            var selectedRedemptionLogic = redemptionLogicConverter.RedemptionLogic;
            selectedRedemptionLogic.PriorityOfPayments = redemptionPriorityOfPayments;
            selectedRedemptionLogic.PostRedemptionPriorityOfPayments = postRedemptionPriorityOfPayments;

            selectedRedemptionLogic.AddAllowedMonthForRedemption(Month.March);
            selectedRedemptionLogic.AddAllowedMonthForRedemption(Month.September);

            if (baseSecuritizationInputsRecord.CleanUpCallPercentage.HasValue)
            {
                var cleanUpCallRedemptionLogicConverter = new RedemptionLogicExcelConverter(
                    securitizationTranches,
                    RedemptionLogicExcelConverter.PercentOfCollateralRedemptionLogic,
                    baseSecuritizationInputsRecord.CleanUpCallPercentage);

                var cleanUpCallRedemptionLogic = cleanUpCallRedemptionLogicConverter.RedemptionLogic;
                cleanUpCallRedemptionLogic.PriorityOfPayments = redemptionPriorityOfPayments;
                cleanUpCallRedemptionLogic.TreatAsCleanUpCall = true;

                paceSecuritization.RedemptionLogicList.Add(cleanUpCallRedemptionLogic);
            }

            paceSecuritization.RedemptionLogicList.Add(selectedRedemptionLogic);
        }

        private void AddSecuritizationReferenceToSpecialFeeTranches(Securitization paceSecuritization)
        {
            var trancheBalanceBasedFees = paceSecuritization.TranchesDictionary.Values.Select(v => v.Tranche).OfType<PercentOfTrancheBalanceFeeTranche>().ToList();
            foreach (var trancheBalanceBasedFee in trancheBalanceBasedFees)
            {
                var castedTrancheBalanceBasedFee = trancheBalanceBasedFee as PercentOfTrancheBalanceFeeTranche;
                castedTrancheBalanceBasedFee.Securitization = paceSecuritization;

                var feeTrancheName = trancheBalanceBasedFee.TrancheName;
                var trancheNodeStruct = paceSecuritization.TranchesDictionary[feeTrancheName];

                // Not really sure how necessary it is to do this, but the potential behavior of the struct is worrisome to me
                trancheNodeStruct.Tranche = castedTrancheBalanceBasedFee;
                paceSecuritization.TranchesDictionary[feeTrancheName] = trancheNodeStruct;
            }
        }
    }
}
