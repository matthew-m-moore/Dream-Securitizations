using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Dream.IO.Database;
using Dream.IO.Database.Entities.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.IO.Database.Entities.Collateral;
using Dream.IO.Database.Contexts;
using Dream.Core.BusinessLogic.Containers;

namespace Dream.Core.Repositories.Database
{
    public class ResecuritizationDatabaseRepository : SecuritizationDatabaseRepository
    {
        public const string CollateralizedSecuritizations = "Collateralized Securitizations";

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private List<CollateralizedSecuritizationTrancheEntity> _collateralizedSecuritizationTrancheEntities;
        private Dictionary<SecuritizationAnalysisIdentifier, List<CollateralizedSecuritizationTrancheEntity>> _collateralizedSecuritizationTrancheEntitiesDictionary;

        /// <summary>
        /// Used to load a securitization from a given securitization DataSetId and VersionId, for example in cases where nothing has been modified from the database.
        /// </summary>
        public ResecuritizationDatabaseRepository(int securitizationAnalysisDataSetId, int securitizationVersionId) 
            : base (securitizationAnalysisDataSetId, securitizationVersionId)
        {
            GetDictionaryOfCollateralizedSecuritizationTrancheEntities();
        }

        private Dictionary<int, TrancheDetailEntity> _collateralizedTrancheDetails;
        public Dictionary<int, TrancheDetailEntity> CollateralizedTrancheDetails
        {
            get
            {
                if (_collateralizedTrancheDetails == null)
                {
                    var trancheDetailIds = _collateralizedSecuritizationTrancheEntities
                        .Select(e => e.SecuritizatizedTrancheDetailId).Distinct().ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var trancheDetailEntities = securitizationEngineContext.TrancheDetailEntities
                            .Where(e => trancheDetailIds.Contains(e.TrancheDetailId)).ToList();

                        _collateralizedTrancheDetails = trancheDetailEntities.ToDictionary(e => e.TrancheDetailId, e => e);
                    }
                }

                return _collateralizedTrancheDetails;
            }
        }

        /// <summary>
        /// Loads a securitization based on PACE assessment collateral.
        /// </summary>
        public override Securitization GetPaceSecuritization()
        {
            var securitizationInputs = GetSecuritizationInputs();
            var resecuritization = new Resecuritization(securitizationInputs)
            {
                Description = _SecuritizationAnalysisDataSetEntity.SecuritizationAnalysisDataSetDescription,
                SecuritizationAnalysisDataSetId = _SecuritizationAnalysisDataSetEntity.SecuritizationAnalysisDataSetId,
                SecuritizationAnalysisVersionId = _SecuritizationAnalysisVersionId,
            };

            var priorityOfPayments = GetPriorityOfPayments();
            resecuritization.PriorityOfPayments = priorityOfPayments;
            resecuritization.SecuritizationNodes = GetSecuritizationNodes(priorityOfPayments);
            resecuritization.RedemptionLogicList = GetRedemptionLogicList(priorityOfPayments);
            resecuritization.ProjectedCashFlowLogic = GetProjectedCashFlowLogic();

            AddCollateralizedPaceSecuritizations(resecuritization);
            AddSecuritizationCommentsAndOwnerIfAvailable(resecuritization);
            AddSecuritizationSummaryInformationIfAvailable(resecuritization);
            AddSecuritizationResultsIfAvailable(resecuritization);

            return resecuritization;
        }

        private void AddCollateralizedPaceSecuritizations(Resecuritization resecuritization)
        {
            foreach (var securitizationIdentifier in _collateralizedSecuritizationTrancheEntitiesDictionary.Keys)
            {
                var securitizationName = securitizationIdentifier.UniqueStringIdentifer;

                if (!resecuritization.CollateralizedSecuritizationsDictionary.ContainsKey(securitizationName))
                {
                    var securitizationDataRepository = new SecuritizationDatabaseRepository(securitizationIdentifier);
                    var securitization = securitizationDataRepository.GetPaceSecuritization();

                    var projectedCashFlowLogic = resecuritization.ProjectedCashFlowLogic.Copy();

                    resecuritization.ProjectedCashFlowLogicDictionary.Add(securitizationName, projectedCashFlowLogic);
                    resecuritization.CollateralizedSecuritizationsDictionary.Add(securitizationName, securitization);
                }

                var listOfCollateralizedSecuritizationTrancheEntities = _collateralizedSecuritizationTrancheEntitiesDictionary[securitizationIdentifier];
                foreach (var collateralizedSecuritizationTrancheEntity in listOfCollateralizedSecuritizationTrancheEntities)
                {
                    var collateralizedTrancheName = CollateralizedTrancheDetails[collateralizedSecuritizationTrancheEntity.SecuritizatizedTrancheDetailId].TrancheName;
                    var collateralizedTranchePercentage = collateralizedSecuritizationTrancheEntity.SecuritizatizedTranchePercentage;

                    resecuritization.AddCollaterizedTrancheNameAndPercentage(securitizationName, collateralizedTrancheName, collateralizedTranchePercentage);
                }
            }
        }

        private void GetDictionaryOfCollateralizedSecuritizationTrancheEntities()
        {
            var collateralizedSecuritizationInputTypeId = SecuritizationInputTypes[CollateralizedSecuritizations];

            foreach (var scenarioId in SecuritizationAnalysisEntityDictionary.Keys)
            {
                var securitizationAnalysisEntities = SecuritizationAnalysisEntityDictionary[scenarioId];
                var securitizationAnalysisEntity = securitizationAnalysisEntities
                    .SingleOrDefault(e => e.SecuritizationInputTypeId == collateralizedSecuritizationInputTypeId);

                if (securitizationAnalysisEntity != null)
                {
                    if (scenarioId != BaseScenarioId)
                    {
                        throw new Exception("INTERNAL ERROR: Additional scenarios are not supported from a database load for the collateralized securitizations input. Please report this error.");
                    }

                    var collateralizedSecuritizationDataSetId = securitizationAnalysisEntity.SecuritizationInputTypeDataSetId;
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        _collateralizedSecuritizationTrancheEntities = securitizationEngineContext.CollateralizedSecuritizationTrancheEntities
                            .Where(e => e.CollateralizedSecuritizationDataSetId == collateralizedSecuritizationDataSetId)
                            .ToList();

                        var distinctSecuritizationIdentifiers = _collateralizedSecuritizationTrancheEntities
                            .Select(r => new SecuritizationAnalysisIdentifier(r.SecuritizationAnalysisDataSetId, r.SecuritizationAnalysisVersionId))
                            .Distinct().ToList();

                        _collateralizedSecuritizationTrancheEntitiesDictionary = distinctSecuritizationIdentifiers
                            .ToDictionary(identifier => identifier,
                                          identifier => _collateralizedSecuritizationTrancheEntities
                                                .Where(e => e.SecuritizationAnalysisDataSetId == identifier.SecuritizationAnalysisDataSetId
                                                         && e.SecuritizationAnalysisVersionId == identifier.SecuritizationAnalysisVersionId).ToList());
                    }
                }
            }

            if (_collateralizedSecuritizationTrancheEntitiesDictionary == null)
            {
                throw new Exception("INTERNAL ERROR: Resecuritization does not contain collateralized securitizations input for the base scenario. Please report this error.");
            }
        }
    }
}
