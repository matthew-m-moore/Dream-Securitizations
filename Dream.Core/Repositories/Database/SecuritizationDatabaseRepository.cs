using Dream.Core.BusinessLogic.Aggregation;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.BusinessLogic.Scenarios;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Converters.Database.Collateral;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.Interfaces;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Repositories.Database
{
    /// <summary>
    /// A repository to load securitizations from database inputs in the SecuritizationEngineContext. Note that it is always assumed that a scenario ID of unity is the
    /// base securitization, all other IDs will then be treated as scenarios. This will be enforced from the UI as well.
    /// </summary>
    public class SecuritizationDatabaseRepository : DatabaseRepository
    {
        public const string AggregationGroupings = "Aggregation Groupings";
        public const string PerformanceAssumptions = "Performance Assumptions";
        public const string PricingAssumptionGroupings = "Pricing Assumption Groupings";
        public const string MarketRateEnvironment = "Market Rate Environment";
        public const string PaceAssessmentCollateral = "PACE Assessment Collateral";
        public const string PriorityOfPayments = "Priority of Payments";
        public const string RedemptionLogic = "Redemption Logic";
        public const string SecuritizationNodesStructure = "Securitization Nodes Structure";
        public const string SecuritizationAnalysisInput = "Securitization Analysis Input";

        public const int BaseScenarioId = 1;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();
        public List<ScenarioAnalysis> ScenariosToAnalyze => _scenarioAnalysisDictionary.Values.ToList();

        // This will only hold the overall descriptive information of the securitization
        protected SecuritizationAnalysisDataSetEntity _SecuritizationAnalysisDataSetEntity;
        protected SecuritizationAnalysisOwnerEntity _SecuritizationAnalysisOwnerEntity;
        protected SecuritizationAnalysisCommentEntity _SecuritizationAnalysisCommentEntity;
        protected List<SecuritizationAnalysisSummaryEntity> _SecuritizationAnalysisSummaryEntities;
        protected int _SecuritizationAnalysisVersionId;

        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        // Note, the key on these dictionaries would be the scenario ID
        private Dictionary<int, SecuritizationInput> _securitizationInputsDictionary;
        private Dictionary<int, ScenarioAnalysis> _scenarioAnalysisDictionary;
        public readonly Dictionary<int, List<SecuritizationAnalysisEntity>> SecuritizationAnalysisEntityDictionary = new Dictionary<int, List<SecuritizationAnalysisEntity>>();
        protected readonly Dictionary<int, List<SecuritizationAnalysisResultEntity>> SecuritizationAnalysisResultEntityDictionary = new Dictionary<int, List<SecuritizationAnalysisResultEntity>>();

        // Note, the key on this dictionary is the TrancheDetailId
        private Dictionary<int, Tranche> _securitizationTranchesDictionary;

        private Dictionary<int, (string UserName, string NickName)> _applicationUsers;
        public Dictionary<string, int> ApplicationUsersReversed => ApplicationUsers.ToDictionary(kvp => kvp.Value.UserName, kvp => kvp.Key);
        public Dictionary<int, (string UserName, string NickName)> ApplicationUsers
        {
            get
            {
                if (_applicationUsers == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var applicationUserEntities = securitizationEngineContext.ApplicationUserEntities.ToList();
                        _applicationUsers = applicationUserEntities.ToDictionary(
                            e => e.ApplicationUserId, 
                            e => (UserName: e.NetworkUserNameIdentifier, NickName: e.ApplicationDisplayableNickName));
                    }
                }

                return _applicationUsers;
            }
        }

        private Dictionary<string, int> _securitizationInputTypes;
        public Dictionary<string, int> SecuritizationInputTypes
        {
            get
            {
                if (_securitizationInputTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var securitizationInputTypeEntities = securitizationEngineContext.SecuritizationInputTypeEntities.ToList();
                        _securitizationInputTypes = securitizationInputTypeEntities.ToDictionary(e => e.SecuritizationInputTypeDescription, e => e.SecuritizationInputTypeId);
                    }
                }

                return _securitizationInputTypes;
            }
        }

        private Dictionary<int, (string Description, double? Divisor)> _securitizationResultTypes;
        public Dictionary<string, int> SecuritizationResultTypesReversed => SecuritizationResultTypes.ToDictionary(kvp => kvp.Value.Description, kvp => kvp.Key);
        public Dictionary<int, (string Description, double? Divisor)> SecuritizationResultTypes
        {
            get
            {
                if (_securitizationResultTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var securitizationResultTypeEntities = securitizationEngineContext.SecuritizationResultTypeEntities.ToList();
                        _securitizationResultTypes = securitizationResultTypeEntities.ToDictionary(
                            e => e.SecuritizationResultTypeId, 
                            e => (Description: e.SecuritizationResultTypeDescription, Divisor: e.StandardDivisor));
                    }
                }

                return _securitizationResultTypes;
            }
        }

        private Dictionary<int, TrancheDetailEntity> _trancheDetails;
        public Dictionary<int, TrancheDetailEntity> TrancheDetails
        {
            get
            {
                if (_trancheDetails == null)
                {
                    var securitizationNodesStructureInputId = SecuritizationInputTypes[SecuritizationNodesStructure];
                    var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary.Values
                        .SelectMany(v => v.Where(e => e.SecuritizationInputTypeId == securitizationNodesStructureInputId));

                    var securitizationNodeStructureDataSetIds = 
                        securitizationAnalysisEntities.Select(e => e.SecuritizationInputTypeDataSetId).Distinct().ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var trancheDetailIds = securitizationEngineContext.SecuritizationNodeEntities
                            .Where(e => securitizationNodeStructureDataSetIds.Contains(e.SecuritizationNodeDataSetId))
                            .Select(s => s.TrancheDetailId)
                            .Distinct().ToList();

                        var trancheDetailEntities = securitizationEngineContext.TrancheDetailEntities
                            .Where(e => trancheDetailIds.Contains(e.TrancheDetailId)).ToList();

                        _trancheDetails = trancheDetailEntities.ToDictionary(e => e.TrancheDetailId, e => e);
                    }
                }

                return _trancheDetails;
            }
        }


        /// <summary>
        /// Used to load a securitization from a given securitization identifier object, for example in cases where nothing has been modified from the database.
        /// </summary>
        public SecuritizationDatabaseRepository(SecuritizationAnalysisIdentifier securitizationAnalysisIdentifier) 
            : this(securitizationAnalysisIdentifier.SecuritizationAnalysisDataSetId, 
                   securitizationAnalysisIdentifier.SecuritizationAnalysisVersionId)
        { }

        /// <summary>
        /// Used to load a securitization from a given securitization DataSetId and VersionId, for example in cases where nothing has been modified from the database.
        /// </summary>
        public SecuritizationDatabaseRepository(int securitizationAnalysisDataSetId, int securitizationAnalysisVersionId) : this()
        {
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                _SecuritizationAnalysisDataSetEntity = securitizationEngineContext.SecuritizationAnalysisDataSetEntities
                    .Single(s => s.SecuritizationAnalysisDataSetId == securitizationAnalysisDataSetId);

                _SecuritizationAnalysisVersionId = securitizationAnalysisVersionId;

                _SecuritizationAnalysisCommentEntity = securitizationEngineContext.SecuritizationAnalysisCommentEntities
                    .SingleOrDefault(s => s.SecuritizationAnalysisDataSetId == securitizationAnalysisDataSetId
                                       && s.SecuritizationAnalysisVersionId == securitizationAnalysisVersionId
                                       && s.IsVisible);

                _SecuritizationAnalysisOwnerEntity = securitizationEngineContext.SecuritizationAnalysisOwnerEntities
                    .SingleOrDefault(s => s.SecuritizationAnalysisDataSetId == securitizationAnalysisDataSetId
                                       && s.SecuritizationAnalysisVersionId == securitizationAnalysisVersionId);

                _SecuritizationAnalysisSummaryEntities = securitizationEngineContext.SecuritizationAnalysisSummaryEntities
                    .Where(s => s.SecuritizationAnalysisDataSetId == securitizationAnalysisDataSetId
                             && s.SecuritizationAnalysisVersionId == securitizationAnalysisVersionId).ToList();

                // Get all securitization analysis entities
                var securitizationAnalysisEntities = securitizationEngineContext.SecuritizationAnalysisEntities
                    .Where(s => s.SecuritizationAnalysisDataSetId == securitizationAnalysisDataSetId
                             && s.SecuritizationAnalysisVersionId == securitizationAnalysisVersionId);

                foreach (var securitizationAnalysisEntity in securitizationAnalysisEntities)
                {
                    var securitizationAnalysisScenarioId = securitizationAnalysisEntity.SecuritizationAnalysisScenarioId;
                    if (!SecuritizationAnalysisEntityDictionary.ContainsKey(securitizationAnalysisScenarioId))
                    {
                        SecuritizationAnalysisEntityDictionary.Add(securitizationAnalysisScenarioId, new List<SecuritizationAnalysisEntity>());
                    }

                    SecuritizationAnalysisEntityDictionary[securitizationAnalysisScenarioId].Add(securitizationAnalysisEntity);
                }

                // Get all securitization analysis result entities
                var securitizationAnalysisResultEntities = securitizationEngineContext.SecuritizationAnalysisResultEntities
                    .Where(s => s.SecuritizationAnalysisDataSetId == securitizationAnalysisDataSetId
                             && s.SecuritizationAnalysisVersionId == securitizationAnalysisVersionId);

                foreach (var securitizationAnalysisResultEntity in securitizationAnalysisResultEntities)
                {
                    var securitizationAnalysisScenarioId = securitizationAnalysisResultEntity.SecuritizationAnalysisScenarioId;
                    if (!SecuritizationAnalysisResultEntityDictionary.ContainsKey(securitizationAnalysisScenarioId))
                    {
                        SecuritizationAnalysisResultEntityDictionary.Add(securitizationAnalysisScenarioId, new List<SecuritizationAnalysisResultEntity>());
                    }

                    SecuritizationAnalysisResultEntityDictionary[securitizationAnalysisScenarioId].Add(securitizationAnalysisResultEntity);
                }
            }
        }

        /// <summary>
        /// Used to access the funtionality of the repositories dictionaries, if no specific securitization is in context.
        /// </summary>
        public SecuritizationDatabaseRepository()
        {
            _securitizationInputsDictionary = new Dictionary<int, SecuritizationInput>();
            _scenarioAnalysisDictionary = new Dictionary<int, ScenarioAnalysis>();

            _typesAndConventionsDatabaseRepository = new TypesAndConventionsDatabaseRepository();
        }

        /// <summary>
        /// Loads a securitization based on PACE assessment collateral.
        /// </summary>
        public virtual Securitization GetPaceSecuritization()
        {
            // Note, securitization inputs has to be loaded first
            var securitizationInputs = GetSecuritizationInputs();
            var paceAsssessmentCollateral = GetPaceAssessmentCollateral();
            var aggregegationGroupings = GetAggregationGroupings();
            var projectedCashFlowLogic = GetProjectedCashFlowLogic();

            // Scenarios cannot be loaded from the database for the inputs below this point
            var priorityOfPayments = GetPriorityOfPayments();
            var securitizationNodes = GetSecuritizationNodes(priorityOfPayments);
            var redemptionLogicList = GetRedemptionLogicList(priorityOfPayments);

            var securitization = new Securitization(
                paceAsssessmentCollateral,
                securitizationInputs,
                aggregegationGroupings,
                projectedCashFlowLogic,
                priorityOfPayments,
                redemptionLogicList,
                securitizationNodes)
            {
                Description = _SecuritizationAnalysisDataSetEntity.SecuritizationAnalysisDataSetDescription,
                SecuritizationAnalysisDataSetId = _SecuritizationAnalysisDataSetEntity.SecuritizationAnalysisDataSetId,
                SecuritizationAnalysisVersionId = _SecuritizationAnalysisVersionId,
            };

            AddSecuritizationReferenceToSpecialFeeTranches(securitization);
            AddSecuritizationCommentsAndOwnerIfAvailable(securitization);
            AddSecuritizationSummaryInformationIfAvailable(securitization);
            AddSecuritizationResultsIfAvailable(securitization);

            return securitization;
        }

        protected SecuritizationInput GetSecuritizationInputs()
        {
            SecuritizationInput baseScenarioSecuritizationInputs = null;
            MarketRateEnvironment baseMarketRateEnvironment = null;

            var securitizationAnalysisInputId = SecuritizationInputTypes[SecuritizationAnalysisInput];
            var marketRateEnvironmentId = SecuritizationInputTypes[MarketRateEnvironment];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];

                var securitizationAnalysisEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == securitizationAnalysisInputId);

                if (securitizationAnalysisEntity != null)
                {
                    var securitizationAnalysisInputDataSetId = securitizationAnalysisEntity.SecuritizationInputTypeDataSetId;

                    SecuritizationAnalysisInputEntity securitizationAnalysisInputEntity;
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        securitizationAnalysisInputEntity = securitizationEngineContext.SecuritizationAnalysisInputEntities
                            .SingleOrDefault(e => e.SecuritizationAnalysisInputId == securitizationAnalysisInputDataSetId);
                    }

                    var typesAndConventionsDatabaseRepository = new TypesAndConventionsDatabaseRepository();
                    var securitizationInputs = SecuritizationInputDatabaseConverter.ConvertToSecuritizationInput(
                        securitizationAnalysisInputEntity,
                        typesAndConventionsDatabaseRepository);

                    if (scenarioId == BaseScenarioId)
                    {
                        baseScenarioSecuritizationInputs = securitizationInputs;
                    }
                    else
                    {
                        ProcessAdditionalSecuritizationInputsAsScenario(scenarioId, securitizationInputs);
                    }

                    _securitizationInputsDictionary.Add(scenarioId, securitizationInputs);
                }

                var marketRateEnvironmentInputEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == marketRateEnvironmentId);

                if (marketRateEnvironmentInputEntity != null)
                {
                    var marketRateEnvironmentDataSetId = marketRateEnvironmentInputEntity.SecuritizationInputTypeDataSetId;
                    var marketRateEnvironmentDatabaseRepository = new MarketRateEnvironmentDatabaseRepository(marketRateEnvironmentDataSetId);
                    var marketRateEnvironment = marketRateEnvironmentDatabaseRepository.GetMarketRateEnvironment();

                    if (scenarioId == BaseScenarioId)
                    {
                        baseMarketRateEnvironment = marketRateEnvironment;
                        baseScenarioSecuritizationInputs.MarketRateEnvironment = marketRateEnvironment;
                    }
                    else
                    {
                        ProcessAdditionalMarketRateEnvironmentsAsScenario(scenarioId, marketRateEnvironment);
                    }
                }        
            }

            if (baseScenarioSecuritizationInputs == null)
            {
                throw new Exception("INTERNAL ERROR: Securitization does not contain inputs for the base scenario. Please report this error.");
            }

            return baseScenarioSecuritizationInputs;
        }

        private ICollateralRetriever GetPaceAssessmentCollateral()
        {
            ICollateralRetriever baseScenarioCollateralRetriever = null;
            var paceCollateralInputId = SecuritizationInputTypes[PaceAssessmentCollateral];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var paceAssesmentRecordDataSetIds = securitizationAnalysisEntities
                    .Where(e => e.SecuritizationInputTypeId == paceCollateralInputId 
                             && e.SecuritizationAnalysisScenarioId == scenarioId)
                    .Select(p => p.SecuritizationInputTypeDataSetId)
                    .ToList();

                if (paceAssesmentRecordDataSetIds.Any())
                {
                    var paceAssessmentDatabaseRepository = new PaceAssessmentDatabaseRepository(paceAssesmentRecordDataSetIds);
                    if (scenarioId == BaseScenarioId)
                    {
                        baseScenarioCollateralRetriever = paceAssessmentDatabaseRepository;
                    }
                    else
                    {
                        ProcessAdditionalCollateralAsScenario(scenarioId, paceAssessmentDatabaseRepository);
                    }
                }
            }

            if (baseScenarioCollateralRetriever == null)
            {
                throw new Exception("INTERNAL ERROR: Securitization does not contain collateral for the base scenario. Please report this error.");
            }

            return baseScenarioCollateralRetriever;
        }

        private AggregationGroupings GetAggregationGroupings()
        {
            var baseScenarioAggregationGroupings = new AggregationGroupings();
            var aggregationGroupingsInputId = SecuritizationInputTypes[AggregationGroupings];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var securitizationAnalysisEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == aggregationGroupingsInputId);

                if (securitizationAnalysisEntity != null)
                {
                    var aggregationGroupingsDataSetId = securitizationAnalysisEntity.SecuritizationInputTypeDataSetId;
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var aggregationGroupAssignmentEntities = securitizationEngineContext.AggregationGroupAssignmentEntities
                            .Where(e => e.AggregationGroupDataSetId == aggregationGroupingsDataSetId)
                            .ToList();

                        var aggregationGroupings =
                            AggregationGroupingsDatabaseConverter.ConvertToAggregationGroupings(aggregationGroupAssignmentEntities);

                        if (scenarioId == BaseScenarioId)
                        {
                            baseScenarioAggregationGroupings = aggregationGroupings;
                        }
                        else
                        {
                            ProcessAdditionalAggregationGroupingsAsScenario(scenarioId, aggregationGroupings);
                        }
                    }
                }
            }

            return baseScenarioAggregationGroupings;
        }

        protected ProjectedCashFlowLogic GetProjectedCashFlowLogic()
        {
            ProjectedCashFlowLogic baseProjectedCashFlowLogic = null;
            var performanceAssumptionsInputId = SecuritizationInputTypes[PerformanceAssumptions];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var securitizationAnalysisEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == performanceAssumptionsInputId);

                if (securitizationAnalysisEntity != null)
                {
                    var selectedAssumptionsGrouping = (_securitizationInputsDictionary.ContainsKey(scenarioId))
                        ? _securitizationInputsDictionary[scenarioId].SelectedPerformanceAssumptionGrouping
                        : _securitizationInputsDictionary[BaseScenarioId].SelectedPerformanceAssumptionGrouping;

                    var performanceAssumptionsDataSetId = securitizationAnalysisEntity.SecuritizationInputTypeDataSetId;
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var performanceAssumptionAssignmentEntities = securitizationEngineContext.PerformanceAssumptionAssignmentEntities
                            .Where(e => e.PerformanceAssumptionDataSetId == performanceAssumptionsDataSetId)
                            .ToList();

                        var vectorParentIds = performanceAssumptionAssignmentEntities.Select(e => e.VectorParentId).Distinct().ToList();
                        var vectorEntities = securitizationEngineContext.VectorEntities.Where(e => vectorParentIds.Contains(e.VectorParentId)).ToList();
                        var vectorEntitiesDictionary = vectorParentIds.ToDictionary(id => id, id => vectorEntities.Where(e => e.VectorParentId == id).ToList());

                        var projectedPerformanceAssumptions = 
                            ProjectedPerformanceAssumptionsDatabaseConverter.ConvertToProjectedPerformanceAssumptions(
                                performanceAssumptionAssignmentEntities, 
                                vectorEntitiesDictionary, 
                                _typesAndConventionsDatabaseRepository.PerformanceCurveTypes);

                        // Later, this may need to be fleshed out with something more complex since there could be other subtypes of the cash flow logic
                        var projectedCashFlowLogic = new ProjectedCashFlowLogic(projectedPerformanceAssumptions, selectedAssumptionsGrouping);

                        if (scenarioId == BaseScenarioId)
                        {
                            baseProjectedCashFlowLogic = projectedCashFlowLogic;
                        }
                        else
                        {
                            ProcessAdditionalProjectedCashFlowLogicAsScenario(scenarioId, projectedCashFlowLogic);
                        }
                    }
                }
            }

            if (baseProjectedCashFlowLogic == null)
            {
                throw new Exception("INTERNAL ERROR: Securitization does not contain performance assumptions for the base scenario. Please report this error.");
            }

            return baseProjectedCashFlowLogic;
        }

        protected PriorityOfPayments GetPriorityOfPayments()
        {
            PriorityOfPayments basePriorityOfPayments = null;
            var priorityOfPaymentsInputId = SecuritizationInputTypes[PriorityOfPayments];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var securitizationAnalysisEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == priorityOfPaymentsInputId);

                if (securitizationAnalysisEntity != null)
                {
                    if (scenarioId != BaseScenarioId)
                    {
                        throw new Exception("INTERNAL ERROR: Additional scenarios are not supported from a database load for the priority of payments input. Please report this error.");
                    }

                    var priorityOfPaymentsDataSetId = securitizationAnalysisEntity.SecuritizationInputTypeDataSetId;
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var priorityOfPaymentsAssignmentEntities = securitizationEngineContext.PriorityOfPaymentsAssignmentEntities
                            .Where(e => e.PriorityOfPaymentsSetId == priorityOfPaymentsDataSetId)
                            .ToList();

                        var priorityOfPayments = PriorityOfPaymentsDatabaseConverter
                            .ConvertToPriorityOfPayments(priorityOfPaymentsAssignmentEntities, _typesAndConventionsDatabaseRepository.TrancheCashFlowTypes, TrancheDetails);

                        basePriorityOfPayments = priorityOfPayments;
                    }
                }
            }

            if (basePriorityOfPayments == null)
            {
                throw new Exception("INTERNAL ERROR: Securitization does not contain priority of payments input for the base scenario. Please report this error.");
            }

            return basePriorityOfPayments;
        }

        protected List<SecuritizationNodeTree> GetSecuritizationNodes(PriorityOfPayments priorityOfPayments)
        {
            List<SecuritizationNodeTree> baseSecuritizationNodeStructure = new List<SecuritizationNodeTree>();
            var securitizationNodesStructureInputId = SecuritizationInputTypes[SecuritizationNodesStructure];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var securitizationAnalysisEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == securitizationNodesStructureInputId);

                if (securitizationAnalysisEntity != null)
                {
                    if (scenarioId != BaseScenarioId)
                    {
                        throw new Exception("INTERNAL ERROR: Additional scenarios are not supported from a database load for the securitization tranche/node structure input. Please report this error.");
                    }

                    var securitizationNodeStructureDataSetId = securitizationAnalysisEntity.SecuritizationInputTypeDataSetId;

                    var securitizationStartDate = _securitizationInputsDictionary[BaseScenarioId].SecuritizationStartDate;
                    var marketRateEnvironment = _securitizationInputsDictionary[BaseScenarioId].MarketRateEnvironment;

                    var securitizationNodeTreeDatabaseRepository = new SecuritizationTrancheDatabaseRepository(
                        securitizationStartDate, 
                        priorityOfPayments, 
                        marketRateEnvironment,                      
                        TrancheDetails,
                        _typesAndConventionsDatabaseRepository);

                    var securitizationNodeStructure = securitizationNodeTreeDatabaseRepository
                        .GetSecuritizationNodeStructure(securitizationNodeStructureDataSetId, out _securitizationTranchesDictionary);

                    baseSecuritizationNodeStructure = securitizationNodeStructure;
                }
            }

            if (!baseSecuritizationNodeStructure.Any())
            {
                throw new Exception("INTERNAL ERROR: Securitization does not contain securitization tranche/node structure input for the base scenario. Please report this error.");
            }

            return baseSecuritizationNodeStructure;
        }

        protected List<RedemptionLogic> GetRedemptionLogicList(PriorityOfPayments priorityOfPayments)
        {
            List<RedemptionLogic> baseRedemptionLogicList = new List<RedemptionLogic>();
            var redemptionLogicInputId = SecuritizationInputTypes[RedemptionLogic];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var redemptionLogicDataSetIds = securitizationAnalysisEntities
                    .Where(e => e.SecuritizationInputTypeId == redemptionLogicInputId)
                    .Select(p => p.SecuritizationInputTypeDataSetId)
                    .ToList();

                if (redemptionLogicDataSetIds.Any())
                {
                    if (scenarioId != BaseScenarioId)
                    {
                        throw new Exception("INTERNAL ERROR: Additional scenarios are not supported from a database load for the securitization redemption logic input. Please report this error.");
                    }

                    var redemptionLogicDatabaseRepository = new RedemptionLogicDatabaseRepository(
                        redemptionLogicDataSetIds,
                        priorityOfPayments,
                        _securitizationTranchesDictionary,
                        _typesAndConventionsDatabaseRepository.TrancheCashFlowTypes,
                        _trancheDetails);

                    baseRedemptionLogicList = redemptionLogicDatabaseRepository.PrepareListOfRedemptionLogic();
                }
            }

            return baseRedemptionLogicList;
        }

        private void ProcessAdditionalSecuritizationInputsAsScenario(int scenarioId, SecuritizationInput scenarioSecuritizationInputs)
        {
            InputsScenario securitizationInputsScenario;
            if (_scenarioAnalysisDictionary.ContainsKey(scenarioId))
            {
                securitizationInputsScenario = new InputsScenario(scenarioSecuritizationInputs);
            }
            else
            {
                var scenarioDescription = scenarioSecuritizationInputs.ScenarioDescription;
                securitizationInputsScenario = new InputsScenario(scenarioSecuritizationInputs);
                _scenarioAnalysisDictionary.Add(scenarioId, new ScenarioAnalysis(scenarioDescription));
            }

            _scenarioAnalysisDictionary[scenarioId].AddScenarioLogic(securitizationInputsScenario);
        }

        private void ProcessAdditionalMarketRateEnvironmentsAsScenario(int scenarioId, MarketRateEnvironment scenarioMarketRateEnvironment)
        {
            var marketRateEnvironmentScenario = new MarketRateEnvironmentScenario(scenarioMarketRateEnvironment);

            if (!_scenarioAnalysisDictionary.ContainsKey(scenarioId))
            {
                var scenarioDescription = _securitizationInputsDictionary.ContainsKey(scenarioId)
                    ? _securitizationInputsDictionary[scenarioId].ScenarioDescription
                    : "Market Rate Env. Scenario - " + scenarioId;

                _scenarioAnalysisDictionary.Add(scenarioId, new ScenarioAnalysis(scenarioDescription));
            }

            _scenarioAnalysisDictionary[scenarioId].AddScenarioLogic(marketRateEnvironmentScenario);
        }

        private void ProcessAdditionalCollateralAsScenario(int scenarioId, ICollateralRetriever scenarioCollateralRetriever)
        {
            var collateralRetrieverScenario = new CollateralRetrieverScenario(scenarioCollateralRetriever);

            if (!_scenarioAnalysisDictionary.ContainsKey(scenarioId))
            {
                var scenarioDescription = _securitizationInputsDictionary.ContainsKey(scenarioId)
                    ? _securitizationInputsDictionary[scenarioId].ScenarioDescription
                    : "Addtl. Collat. Scenario - " + scenarioId;

                _scenarioAnalysisDictionary.Add(scenarioId, new ScenarioAnalysis(scenarioDescription));
            }

            _scenarioAnalysisDictionary[scenarioId].AddScenarioLogic(collateralRetrieverScenario);
        }

        private void ProcessAdditionalAggregationGroupingsAsScenario(int scenarioId, AggregationGroupings scenarioAggregationGroupings)
        {
            var aggregationGroupingsScenario = new AggregationGroupingsScenario(scenarioAggregationGroupings);

            if (!_scenarioAnalysisDictionary.ContainsKey(scenarioId))
            {
                var scenarioDescription = _securitizationInputsDictionary.ContainsKey(scenarioId)
                    ? _securitizationInputsDictionary[scenarioId].ScenarioDescription
                    : "Agg. Group Scenario - " + scenarioId;

                _scenarioAnalysisDictionary.Add(scenarioId, new ScenarioAnalysis(scenarioDescription));
            }

            _scenarioAnalysisDictionary[scenarioId].AddScenarioLogic(aggregationGroupingsScenario);
        }

        private void ProcessAdditionalProjectedCashFlowLogicAsScenario(int scenarioId, ProjectedCashFlowLogic scenarioProjectedCashFlowLogic)
        {
            var projectedCashFlowLogicScenario = new ProjectedCashFlowLogicScenario(scenarioProjectedCashFlowLogic);

            if (!_scenarioAnalysisDictionary.ContainsKey(scenarioId))
            {
                var scenarioDescription = _securitizationInputsDictionary.ContainsKey(scenarioId)
                    ? _securitizationInputsDictionary[scenarioId].ScenarioDescription
                    : "Cash-Flow Logic Scenario - " + scenarioId;

                _scenarioAnalysisDictionary.Add(scenarioId, new ScenarioAnalysis(scenarioDescription));
            }

            _scenarioAnalysisDictionary[scenarioId].AddScenarioLogic(projectedCashFlowLogicScenario);
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

        protected void AddSecuritizationCommentsAndOwnerIfAvailable(Securitization securitization)
        {
            if (_SecuritizationAnalysisCommentEntity != null)
            {
                securitization.Comments = _SecuritizationAnalysisCommentEntity.CommentText;
            }

            if (_SecuritizationAnalysisOwnerEntity != null)
            {
                var ownerUserName = ApplicationUsers[_SecuritizationAnalysisOwnerEntity.ApplicationUserId].UserName;
                securitization.Owner = ownerUserName;
            }
        }

        protected void AddSecuritizationSummaryInformationIfAvailable(Securitization securitization)
        {
            if (_SecuritizationAnalysisSummaryEntities.Any())
            {
                foreach (var securitizationAnalysisSummaryEntity in _SecuritizationAnalysisSummaryEntities)
                {
                    var trancheDetailId = securitizationAnalysisSummaryEntity.SecuritizationTrancheDetailId;
                    var scenarioId = securitizationAnalysisSummaryEntity.SecuritizationAnalysisScenarioId;

                    var pricingScenario = (_scenarioAnalysisDictionary.ContainsKey(scenarioId))
                        ? _scenarioAnalysisDictionary[scenarioId].Description
                        : securitization.Inputs.ScenarioDescription;

                    if (trancheDetailId.HasValue)
                    {
                        var trancheName = TrancheDetails[trancheDetailId.Value].TrancheName;
                        if (!securitization.TranchesDictionary.ContainsKey(trancheName)) continue;
                        securitization.TranchesDictionary[trancheName].Tranche.TrancheDescription = securitizationAnalysisSummaryEntity.SecuritizationTrancheType;
                        securitization.TranchesDictionary[trancheName].Tranche.TrancheRating = securitizationAnalysisSummaryEntity.SecuritizationTrancheRating;
                        securitization.TranchesDictionary[trancheName].Tranche.TranchePricingScenario = pricingScenario;
                    }
                    else
                    {
                        var securitizationNodeName = securitizationAnalysisSummaryEntity.SecuritizationNodeName;
                        if (!securitization.NodesDictionary.ContainsKey(securitizationNodeName)) continue;
                        securitization.NodesDictionary[securitizationNodeName].SecuritizationNodeType = securitizationAnalysisSummaryEntity.SecuritizationTrancheType;
                        securitization.NodesDictionary[securitizationNodeName].SecuritizationNodeRating = securitizationAnalysisSummaryEntity.SecuritizationTrancheRating;
                        securitization.NodesDictionary[securitizationNodeName].SecuritizationNodePricingScenario = pricingScenario;
                    }
                }
            }
        }

        protected void AddSecuritizationResultsIfAvailable(Securitization securitization)
        {
            if (SecuritizationAnalysisResultEntityDictionary.Any())
            {
                foreach (var securitizationAnalysisResultEntry in SecuritizationAnalysisResultEntityDictionary)
                {
                    var scenarioId = securitizationAnalysisResultEntry.Key;
                    if (_scenarioAnalysisDictionary.ContainsKey(scenarioId) || scenarioId == BaseScenarioId)
                    {
                        var securitizationResultDatabaseConverter = new SecuritizationResultDatabaseConverter(SecuritizationResultTypes);

                        var scenarioDescription = _scenarioAnalysisDictionary.ContainsKey(scenarioId)
                            ? _scenarioAnalysisDictionary[securitizationAnalysisResultEntry.Key].Description
                            : securitization.Inputs.ScenarioDescription;

                        var securitizationAnalysisResultEntities = securitizationAnalysisResultEntry.Value;
                        var securitizationResult = securitizationResultDatabaseConverter.ConvertToSecuritizationResult(securitizationAnalysisResultEntities, TrancheDetails);

                        if (!securitization.ResultsDictionary.ContainsKey(scenarioDescription))
                        {
                            securitization.ResultsDictionary.Add(scenarioDescription, securitizationResult);
                        }
                        else
                        {
                            securitization.ResultsDictionary[scenarioDescription] = securitizationResult;
                        }
                    }                   
                }
            }
        }
    }
}
