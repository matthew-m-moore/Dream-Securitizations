using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Dream.IO.Database;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.IO.Database.Contexts;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.Converters.Database;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;

namespace Dream.Core.Repositories.Database
{
    public class SecuritizationTrancheDatabaseRepository : DatabaseRepository
    {
        private DateTime _securitizationStartDate;
        private PriorityOfPayments _priorityOfPayments;
        private MarketRateEnvironment _marketRateEnvironment;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        private Dictionary<int, TrancheDetailEntity> _trancheDetailsDictionary;
        private List<int> _trancheDetailIds => _trancheDetailsDictionary.Keys.ToList();

        private List<SecuritizationNodeEntity> _securitizationNodeEntities;
        private Dictionary<string, List<SecuritizationNodeEntity>> _securitizationNodeEntitiesDictionary;

        private Dictionary<string, SecuritizationNodeTree> _securitizationNodeDictionary = new Dictionary<string, SecuritizationNodeTree>();
        private Dictionary<int, Tranche> _securitizationTranchesDictionary = new Dictionary<int, Tranche>();

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private Dictionary<int, DistributionRule> _distributionRules;
        public Dictionary<int, DistributionRule> DistributionRules
        {
            get
            {
                if (_distributionRules == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var fundsDistributionTypeEntities = securitizationEngineContext.FundsDistributionTypeEntities.ToList();
                        _distributionRules = fundsDistributionTypeEntities.ToDictionary(
                            e => e.FundsDistributionTypeId,
                            e => DistributionRuleDatabaseConverter.DetermineDistributionRuleFromDescription(e.FundsDistributionTypeDescription));
                    }
                }

                return _distributionRules;
            }
        }

        private Dictionary<int, (Type TrancheType, bool IsResidualTranche)> _trancheTypes;
        public Dictionary<int, (Type TrancheType, bool IsResidualTranche)> TrancheTypes
        {
            get
            {
                if (_trancheTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var trancheTypeEntities = securitizationEngineContext.TrancheTypeEntities.ToList();
                        _trancheTypes = trancheTypeEntities.ToDictionary(
                            e => e.TrancheTypeId,
                            e => TrancheTypeDatabaseConverter.ConvertString(e.TrancheTypeDescription));
                    }
                }

                return _trancheTypes;
            }
        }

        private Dictionary<int, TrancheCouponEntity> _trancheCoupons;
        public Dictionary<int, TrancheCouponEntity> TrancheCoupons
        {
            get
            {
                if (_trancheCoupons == null)
                {
                    var trancheCouponIds = _trancheDetailsDictionary.Values.Select(t => t.TrancheCouponId)
                        .Where(id => id.HasValue).Select(id => id.Value).ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var trancheCouponEntities = securitizationEngineContext.TrancheCouponEntities
                            .Where(e => trancheCouponIds.Contains(e.TrancheCouponId)).ToList();

                        _trancheCoupons = trancheCouponEntities.ToDictionary(e => e.TrancheCouponId, e => e);
                    }
                }

                return _trancheCoupons;
            }
        }

        private Dictionary<int, (FeeGroupDetailEntity FeeGroup, List<FeeDetailEntity> FeeDetails)> _feeGroupDetails;
        public Dictionary<int, (FeeGroupDetailEntity FeeGroup, List<FeeDetailEntity> FeeDetails)> FeeGroupDetails
        {
            get
            {
                if (_feeGroupDetails == null)
                {
                    var feeGroupDetailIds = _trancheDetailsDictionary.Values.Select(t => t.FeeGroupDetailId)
                        .Where(id => id.HasValue).Select(id => id.Value).ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var feeGroupDetailEntities = securitizationEngineContext.FeeGroupDetailEntities
                            .Where(e => feeGroupDetailIds.Contains(e.FeeGroupDetailId)).ToList();

                        var feeDetailEntities = securitizationEngineContext.FeeDetailEntities
                            .Where(e => feeGroupDetailIds.Contains(e.FeeGroupDetailId)).ToList();

                        _feeGroupDetails = feeGroupDetailEntities.ToDictionary(
                            e => e.FeeGroupDetailId, 
                            e => (FeeGroup: e, FeeDetails: feeDetailEntities.Where(f => f.FeeGroupDetailId == e.FeeGroupDetailId).ToList()));
                    }
                }

                return _feeGroupDetails;
            }
        }

        private Dictionary<int, (AvailableFundsRetriever PaymentAvailableFundsRetriever, AvailableFundsRetriever InterestAvailableFundsRetriever)> _availableFundsRetrievers;
        private Dictionary<int, (AvailableFundsRetriever PaymentAvailableFundsRetriever, AvailableFundsRetriever InterestAvailableFundsRetriever)> AvailableFundsRetrievers
        {
            get
            {
                if (_availableFundsRetrievers == null)
                {
                    var paymentAvailableFundsRetrievalDetailIds = _trancheDetailsDictionary.Values.Select(t => t.PaymentAvailableFundsRetrievalDetailId).ToList();
                    var interestAvailableFundsRetrievalDetailIds = _trancheDetailsDictionary.Values.Select(t => t.InterestAvailableFundsRetrievalDetailId)
                        .Where(id => id.HasValue).Select(id => id.Value).ToList();

                    var availableFundsRetrievalDetailIds = new List<int>();
                    availableFundsRetrievalDetailIds.AddRange(paymentAvailableFundsRetrievalDetailIds);
                    availableFundsRetrievalDetailIds.AddRange(interestAvailableFundsRetrievalDetailIds);

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var availableFundsRetrievalTypesDictionary = securitizationEngineContext.AvailableFundsRetrievalTypeEntities
                            .ToDictionary(e => e.AvailableFundsRetrievalTypeId,
                                          e => e.AvailableFundsRetrievalTypeDescription);

                        var availableFundsRetrievalDetailsDictionary = securitizationEngineContext.AvailableFundsRetrievalDetailEntities
                            .Where(e => availableFundsRetrievalDetailIds.Contains(e.AvailableFundsRetrievalDetailId))
                            .ToDictionary(d => d.AvailableFundsRetrievalDetailId, d => d);

                        _availableFundsRetrievers = _trancheDetailsDictionary.Values
                            .ToDictionary(t => t.TrancheDetailId,
                                          t => 
                                          (
                                            PaymentAvailableFundsRetriever: 
                                                AvailableFundsRetrieverDatabaseConverter.ConvertId(
                                                    t.PaymentAvailableFundsRetrievalDetailId, 
                                                    availableFundsRetrievalDetailsDictionary, 
                                                    availableFundsRetrievalTypesDictionary),

                                            InterestAvailableFundsRetriever: 
                                                AvailableFundsRetrieverDatabaseConverter.ConvertId(
                                                    t.InterestAvailableFundsRetrievalDetailId, 
                                                    availableFundsRetrievalDetailsDictionary, 
                                                    availableFundsRetrievalTypesDictionary)
                                          ));
                    }
                }

                return _availableFundsRetrievers;
            }
        }

        private Dictionary<int, List<BalanceCapAndFloorDetailEntity>> _balanceCapAndFloorDetails;
        public Dictionary<int, List<BalanceCapAndFloorDetailEntity>> BalanceCapAndFloorDetails
        {
            get
            {
                if (_balanceCapAndFloorDetails == null)
                {
                    var balanceCapAndFloorSetIds = _trancheDetailsDictionary.Values.Select(t => t.BalanceCapAndFloorSetId)
                        .Where(id => id.HasValue).Select(id => id.Value).ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var balanceCapAndFloorDetailEntities = securitizationEngineContext.BalanceCapAndFloorDetailEntities
                            .Where(e => balanceCapAndFloorSetIds.Contains(e.BalanceCapAndFloorSetId)).ToList();

                        _balanceCapAndFloorDetails = balanceCapAndFloorSetIds.ToDictionary(
                            balanceCapAndFloorSetId => balanceCapAndFloorSetId,
                            balanceCapAndFloorSetId => balanceCapAndFloorDetailEntities.Where(f => f.BalanceCapAndFloorSetId == balanceCapAndFloorSetId).ToList());
                    }
                }

                return _balanceCapAndFloorDetails;
            }
        }

        private Dictionary<int, List<ReserveAccountsDetailEntity>> _reserveAccountsDetails;
        public Dictionary<int, List<ReserveAccountsDetailEntity>> ReserveAccountsDetails
        {
            get
            {
                if (_reserveAccountsDetails == null)
                {
                    var reserveAccountsSetIds = _trancheDetailsDictionary.Values.Select(t => t.ReserveAccountsSetId)
                        .Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var reserveAccountsDetailEntities = securitizationEngineContext.ReserveAccountsDetailEntities
                            .Where(e => reserveAccountsSetIds.Contains(e.ReserveAccountsSetId)).ToList();

                        _reserveAccountsDetails = reserveAccountsSetIds.ToDictionary(
                            reserveAccountsSetId => reserveAccountsSetId,
                            reserveAccountsSetId => reserveAccountsDetailEntities.Where(f => f.ReserveAccountsSetId == reserveAccountsSetId).ToList());
                    }
                }

                return _reserveAccountsDetails;
            }
        }

        public SecuritizationTrancheDatabaseRepository(
            DateTime securitizationStartDate,
            PriorityOfPayments priorityOfPayments, 
            MarketRateEnvironment marketRateEnvironment,            
            Dictionary<int, TrancheDetailEntity> trancheDetailsDictionary,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository)
        {
            _securitizationStartDate = securitizationStartDate;
            _priorityOfPayments = priorityOfPayments;
            _marketRateEnvironment = marketRateEnvironment;
            _trancheDetailsDictionary = trancheDetailsDictionary;

            _typesAndConventionsDatabaseRepository = new TypesAndConventionsDatabaseRepository();
        }

        public List<SecuritizationNodeTree> GetSecuritizationNodeStructure(int securitizationNodeStructureDataSetId, out Dictionary<int, Tranche> securitizationTranchesDictionary)
        {
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                _securitizationNodeEntities = securitizationEngineContext.SecuritizationNodeEntities
                    .Where(e => e.SecuritizationNodeDataSetId == securitizationNodeStructureDataSetId)
                    .ToList();
            }

            var distinctSecuritizationNodeNames = _securitizationNodeEntities.Select(e => e.SecuritizationNodeName).Distinct().ToList();
            _securitizationNodeEntitiesDictionary = distinctSecuritizationNodeNames
                .ToDictionary(distinctNodeName => distinctNodeName,
                              distinctNodeName => _securitizationNodeEntities.Where(s => s.SecuritizationNodeName == distinctNodeName).ToList());

            ProduceSecuritizationNodeTree(distinctSecuritizationNodeNames);
            SetUpReserveFundAccounts();

            securitizationTranchesDictionary = _securitizationTranchesDictionary;
            return _securitizationNodeDictionary.Values.Where(n => n.ParentSecuritizationNode == null).ToList();
        }

        private void ProduceSecuritizationNodeTree(List<string> distinctSecuritizationNodeNames, 
            bool checkForChildNodes = true, 
            int securitizationNodeId = 1)
        {
            foreach (var securitizationNodeName in distinctSecuritizationNodeNames)
            {
                if (_securitizationNodeDictionary.ContainsKey(securitizationNodeName)) continue;

                var specificSecuritizationNodeEntities = _securitizationNodeEntitiesDictionary[securitizationNodeName];
                if (specificSecuritizationNodeEntities.Any(e => e.SecuritizationChildNodeName != null) && checkForChildNodes)
                {
                    var distinctSecuritizationChildNodeNames = specificSecuritizationNodeEntities
                        .Where(e => e.SecuritizationChildNodeName != null)
                        .Select(e => e.SecuritizationChildNodeName).Distinct().ToList();

                    ProduceSecuritizationNodeTree(distinctSecuritizationChildNodeNames, securitizationNodeId: securitizationNodeId);
                }
                else
                {
                    // This should return null if no parent nodes are found
                    var parentNodeName = _securitizationNodeEntities
                        .Where(n => n.SecuritizationChildNodeName == securitizationNodeName)
                        .Select(e => e.SecuritizationNodeName).Distinct().SingleOrDefault();

                    // Always create the parent first, should one exist
                    if (parentNodeName != null)
                    {
                        var continueCheckForChildNodes = false;
                        ProduceSecuritizationNodeTree(new List<string> { parentNodeName }, continueCheckForChildNodes, securitizationNodeId);
                    }

                    var listOfTranchesAtSecuritizationNode = CreatListOfTranchesAtNode(specificSecuritizationNodeEntities);

                    var fundsDistributionRuleId = specificSecuritizationNodeEntities.Select(e => e.FundsDistributionTypeId).Distinct().SingleOrDefault();
                    if (fundsDistributionRuleId == default(int))
                    {
                        throw new Exception("INTERNAL ERROR: All entries within a securitization node must share the same funds distribution rule. Please report this error.");
                    }

                    var distributionRule = DistributionRules[fundsDistributionRuleId];
                    var securitizationNode = new SecuritizationNodeTree(distributionRule)
                    {
                        SecuritizationNodeId = securitizationNodeId,
                        SecuritizationNodeName = securitizationNodeName,
                        ParentSecuritizationNode = (parentNodeName != null) ? _securitizationNodeDictionary[parentNodeName] : null,
                        SecuritizationTranches = listOfTranchesAtSecuritizationNode,
                        SecuritizationNodes = new List<SecuritizationNodeTree>()
                    };

                    // The node ID will be unique within the securitizaition, but not globally, like the tranche detail ID
                    securitizationNodeId++;

                    // Now, add the child node to it's parent, since the parent has to exist already per the lines above
                    if (parentNodeName != null)
                    {
                        _securitizationNodeDictionary[parentNodeName].SecuritizationNodes.Add(securitizationNode);
                    }

                    _securitizationNodeDictionary.Add(securitizationNodeName, securitizationNode);
                }
            }
        }
        
        private List<Tranche> CreatListOfTranchesAtNode(List<SecuritizationNodeEntity> securitizationNodeEntities)
        {
            var listOfTranchesAtSecuritizationNode = new List<Tranche>();
            foreach (var securitizationNodeEntity in securitizationNodeEntities.Where(e => e.TrancheDetailId.HasValue))
            {
                var trancheDetailId = securitizationNodeEntity.TrancheDetailId.Value;
                var trancheDetailEntity = _trancheDetailsDictionary[trancheDetailId];
                var trancheTypeInformation = TrancheTypes[trancheDetailEntity.TrancheTypeId];

                if (!_priorityOfPayments.OrderedListOfEntries.Any(p => p.TrancheName == trancheDetailEntity.TrancheName))
                {
                    throw new Exception(string.Format("ERROR: The priority of payments is missing an entry for '{0}'. Please correct this error.",
                        trancheDetailEntity.TrancheName));
                }

                PricingStrategy pricingStrategy = null;
                var pricingTypeId = securitizationNodeEntity.TranchePricingTypeId;
                if (pricingTypeId.HasValue)
                {
                    var pricingStrategyDatabaseConverter = new PricingStrategyDatabaseConverter(_marketRateEnvironment, _typesAndConventionsDatabaseRepository);
                    pricingStrategy = pricingStrategyDatabaseConverter.ExtractPricingStrategyFromSecuritizationNode(securitizationNodeEntity);
                }

                Tranche securitizationTranche = null;

                if (trancheTypeInformation.TrancheType.IsSubclassOf(typeof(FeeTranche)))
                    securitizationTranche = CreateFeeTranche(trancheTypeInformation, trancheDetailEntity);

                else if (trancheTypeInformation.TrancheType.IsSubclassOf(typeof(InterestPayingTranche)))
                    securitizationTranche = CreateInterestPayingTranche(trancheTypeInformation, trancheDetailEntity, pricingStrategy);

                else if (trancheTypeInformation.TrancheType.IsSubclassOf(typeof(ReserveFundTranche)))
                    securitizationTranche = CreateReserveFundTranche(trancheTypeInformation, trancheDetailEntity);

                else if (trancheTypeInformation.TrancheType == typeof(ResidualTranche))
                    securitizationTranche = CreateResidualTranche(trancheTypeInformation, trancheDetailEntity, pricingStrategy);

                if (securitizationTranche == null)
                    throw new Exception("INTERNAL ERROR: An supported type of tranche was encountered. Please report this error.");

                securitizationTranche.TrancheDetailId = trancheDetailId;

                listOfTranchesAtSecuritizationNode.Add(securitizationTranche);
                _securitizationTranchesDictionary.Add(trancheDetailId, securitizationTranche);
            }

            return listOfTranchesAtSecuritizationNode;
        }

        private Tranche CreateFeeTranche((Type TrancheType, bool IsResidualTranche) trancheTypeInformation, TrancheDetailEntity trancheDetailEntity)
        {
            var feeGroupDetailId = trancheDetailEntity.FeeGroupDetailId;
            if (feeGroupDetailId.HasValue)
            {
                var availableFundsRetriever = AvailableFundsRetrievers[trancheDetailEntity.TrancheDetailId].PaymentAvailableFundsRetriever;
                var feeGroupInformation = FeeGroupDetails[feeGroupDetailId.Value];

                var securitizationTranche = FeeTrancheDatabaseConverter.CreateTranche(
                    trancheTypeInformation,
                    feeGroupInformation,
                    trancheDetailEntity,                     
                    availableFundsRetriever,
                    _trancheDetailsDictionary,
                    _typesAndConventionsDatabaseRepository);

                return securitizationTranche;
            }
            else
            {
                throw new Exception(string.Format("INTERNAL ERROR: The fee named '{0}' is missing its fee group detail ID. Please report this error.",
                    trancheDetailEntity.TrancheName));
            }
        }

        private Tranche CreateInterestPayingTranche((Type TrancheType, bool IsResidualTranche) trancheTypeInformation, TrancheDetailEntity trancheDetailEntity, PricingStrategy pricingStrategy)
        {
            var trancheCouponId = trancheDetailEntity.TrancheCouponId;
            if (trancheCouponId.HasValue)
            {
                var principalAvailableFundsRetriever = AvailableFundsRetrievers[trancheDetailEntity.TrancheDetailId].PaymentAvailableFundsRetriever;
                var interestAvailableFundsRetriever = AvailableFundsRetrievers[trancheDetailEntity.TrancheDetailId].InterestAvailableFundsRetriever;
                var trancheCouponEntity = TrancheCoupons[trancheCouponId.Value];

                var securitizationTranche = InterestPayingTrancheDatabaseConverter.CreateTranche(
                    trancheTypeInformation,
                    _securitizationStartDate,
                    pricingStrategy,
                    trancheDetailEntity, 
                    trancheCouponEntity, 
                    principalAvailableFundsRetriever, 
                    interestAvailableFundsRetriever,
                    _marketRateEnvironment,
                    _typesAndConventionsDatabaseRepository);

                return securitizationTranche;
            }
            else
            {
                throw new Exception(string.Format("INTERNAL ERROR: The interest-paying tranche named '{0}' is missing its tranche coupon detail ID. Please report this error.",
                    trancheDetailEntity.TrancheName));
            }
        }

        private Tranche CreateReserveFundTranche((Type TrancheType, bool IsResidualTranche) trancheTypeInformation, TrancheDetailEntity trancheDetailEntity)
        {
            var balanceFloorAndCapSetId = trancheDetailEntity.BalanceCapAndFloorSetId;
            if (balanceFloorAndCapSetId.HasValue)
            {
                var availableFundsRetriever = AvailableFundsRetrievers[trancheDetailEntity.TrancheDetailId].PaymentAvailableFundsRetriever;
                var balanceCapAndFloorDetailEntities = BalanceCapAndFloorDetails[balanceFloorAndCapSetId.Value];

                var securitizationTranche = ReserveFundTrancheDatabaseConverter.CreateTranche(
                    trancheTypeInformation, 
                    trancheDetailEntity, 
                    balanceCapAndFloorDetailEntities, 
                    availableFundsRetriever);

                return securitizationTranche;
            }
            else
            {
                throw new Exception(string.Format("INTERNAL ERROR: The reserve account named '{0}' is missing its balance cap/floor detail ID. Please report this error.",
                    trancheDetailEntity.TrancheName));
            }
        }

        private Tranche CreateResidualTranche((Type TrancheType, bool IsResidualTranche) trancheTypeInformation, TrancheDetailEntity trancheDetailEntity, PricingStrategy pricingStrategy)
        {
            var availableFundsRetriever = AvailableFundsRetrievers[trancheDetailEntity.TrancheDetailId].PaymentAvailableFundsRetriever;
            var securitizationTranche = ResidualTrancheDatabaseConverter.CreateTranche(trancheTypeInformation, pricingStrategy, trancheDetailEntity, availableFundsRetriever);
            return securitizationTranche;
        }

        private void SetUpReserveFundAccounts()
        {
            // This method needs to loop through all the appropriate reserve account sets and use the tranches to assign out reserve accounts
            foreach (var securitizationNodeEntity in _securitizationNodeEntities.Where(e => e.TrancheDetailId.HasValue))
            {
                var trancheDetailId = securitizationNodeEntity.TrancheDetailId.Value;
                var trancheDetailEntity = _trancheDetailsDictionary[trancheDetailId];

                if (trancheDetailEntity.ReserveAccountsSetId.HasValue)
                {
                    var reserveAccountsDetailEntities = ReserveAccountsDetails[trancheDetailEntity.ReserveAccountsSetId.Value];
                    foreach (var reserveAccountDetailEntity in reserveAccountsDetailEntities)
                    {
                        var reserveAccountTrancheDetailId = reserveAccountDetailEntity.TrancheDetailId;
                        var trancheCashFlowType = _typesAndConventionsDatabaseRepository.TrancheCashFlowTypes[reserveAccountDetailEntity.TrancheCashFlowTypeId];

                        _securitizationTranchesDictionary[trancheDetailId]
                            .AddAssociatedReserveAccount(_securitizationTranchesDictionary[reserveAccountTrancheDetailId], trancheCashFlowType);
                    }
                }
            }
        }
    }
}
