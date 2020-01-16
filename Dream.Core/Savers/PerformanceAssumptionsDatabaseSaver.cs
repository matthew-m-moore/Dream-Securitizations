using Dream.Common.Enums;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities;
using Dream.IO.Database.Entities.Collateral;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Savers
{
    public class PerformanceAssumptionsDatabaseSaver : DatabaseSaver
    {
        private ProjectedCashFlowLogic _projectedCashFlowLogic;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        private Dictionary<string, Dictionary<PerformanceCurveType, int>> _vectorParentsDictionary;
        private List<PerformanceAssumptionAssignmentEntity> _listOfPerformanceAssumptionAssignmentEntities;   

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public PerformanceAssumptionsDatabaseSaver(
            ProjectedCashFlowLogic projectedCashFlowLogic,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository,
            DateTime cutOffDate,
            string description)
        : base(cutOffDate, description)
        {
            _projectedCashFlowLogic = projectedCashFlowLogic;
            _typesAndConventionsDatabaseRepository = typesAndConventionsDatabaseRepository;

            _vectorParentsDictionary = new Dictionary<string, Dictionary<PerformanceCurveType, int>>();
            _listOfPerformanceAssumptionAssignmentEntities = new List<PerformanceAssumptionAssignmentEntity>();
        }

        public int SavePerformanceAssumptions()
        {
            var performanceAssumptionDataSetEntity = new PerformanceAssumptionDataSetEntity
            {
                CutOffDate = _CutOffDate,
                PerformanceAssumptionDataSetDescription = _Description
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.PerformanceAssumptionDataSetEntities.Add(performanceAssumptionDataSetEntity);
                securitizationEngineContext.SaveChanges();
            }

            SavePerformanceAssumptionsCurvesAndAssignments(performanceAssumptionDataSetEntity.PerformanceAssumptionDataSetId);

            return performanceAssumptionDataSetEntity.PerformanceAssumptionDataSetId;
        }

        private void SavePerformanceAssumptionsCurvesAndAssignments(int performanceAssumptionDataSetId)
        {
            // Note, vectors parents must be saved first, and it cannot happen asynchronously
            SavePerformanceCurves(performanceAssumptionDataSetId);
            SavePerformanceAssumptionAssignments(performanceAssumptionDataSetId);
        }

        private void SavePerformanceCurves(int performanceAssumptionDataSetId)
        {
            var performanceAssumptions = _projectedCashFlowLogic.ProjectedPerformanceAssumptions;
            var performanceCurveNames = performanceAssumptions.GetAllPerformanceCurveNames();

            var listOfVectorEntities = new List<VectorEntity>();
            foreach (var performanceCurveName in performanceCurveNames)
            {
                var performanceCurveEntries = _projectedCashFlowLogic.ProjectedPerformanceAssumptions[performanceCurveName];
                foreach(var performanceCurveEntry in performanceCurveEntries)
                {
                    var performanceCurveType = performanceCurveEntry.Key;
                    var performanceCurve = performanceCurveEntry.Value;

                    var vectorParentDescription = performanceCurveName + " - " + performanceCurveType.ToString().ToUpper();
                    var vectorIsFlat = (performanceCurve.Vector.Skip(1).Distinct().Count() == 1);

                    var vectorParentEntity = new VectorParentEntity
                    {
                        CutOffDate = _CutOffDate,
                        VectorParentDescription = vectorParentDescription,
                        IsFlatVector = vectorIsFlat
                    };

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        securitizationEngineContext.VectorParentEntities.Add(vectorParentEntity);
                        securitizationEngineContext.SaveChanges();
                    }

                    if (!_vectorParentsDictionary.ContainsKey(performanceCurveName))
                        _vectorParentsDictionary.Add(performanceCurveName, new Dictionary<PerformanceCurveType, int>());

                    _vectorParentsDictionary[performanceCurveName].Add(performanceCurveType, vectorParentEntity.VectorParentId);

                    var vectorValueCount = performanceCurve.Vector.Count();
                    for (var i = 0; i < vectorValueCount; i++)
                    {
                        var vectorEntity = new VectorEntity
                        {
                            VectorParentId = vectorParentEntity.VectorParentId,
                            VectorPeriod = i,
                            VectorValue = performanceCurve.Vector[i]
                        };

                        listOfVectorEntities.Add(vectorEntity);
                    }
                }
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.VectorEntities.AddRange(listOfVectorEntities);
                securitizationEngineContext.SaveChanges();
            }
        }

        private void SavePerformanceAssumptionAssignments(int performanceAssumptionDataSetId)
        {
            var performanceAssumptionsMapping = _projectedCashFlowLogic.ProjectedPerformanceAssumptions.PerformanceAssumptionsMapping;
            if (performanceAssumptionsMapping != null)
            {
                var assumptionsGroupings = performanceAssumptionsMapping.GetAllAssumptionsGroupings();
                foreach (var assumptionsGrouping in assumptionsGroupings)
                {
                    var assumptionIdentifiers = performanceAssumptionsMapping.GetAllAssumptionsIdentifiers(assumptionsGrouping);
                    foreach (var assumptionsIdentifier in assumptionIdentifiers)
                    {
                        var performanceAssumptionAssignments = performanceAssumptionsMapping[assumptionsGrouping, assumptionsIdentifier];

                        foreach (var entry in performanceAssumptionAssignments)
                        {
                            var performanceAssumptionType = entry.Key;
                            var performanceCurveName = entry.Value;

                            var performanceAssumptionAssignmentEntity = new PerformanceAssumptionAssignmentEntity
                            {
                                PerformanceAssumptionDataSetId = performanceAssumptionDataSetId,
                                InstrumentIdentifier = assumptionsIdentifier,
                                PerformanceAssumptionGrouping = assumptionsGrouping,
                                PerformanceAssumptionIdentifier = performanceCurveName,
                                PerformanceAssumptionTypeId = _typesAndConventionsDatabaseRepository.PerformanceCurveTypesReversed[performanceAssumptionType],
                                VectorParentId = _vectorParentsDictionary[performanceCurveName][performanceAssumptionType]
                            };

                            _listOfPerformanceAssumptionAssignmentEntities.Add(performanceAssumptionAssignmentEntity);
                        }
                    }
                }
            }

            // Note, we should also save any non-assigned vectors as well, in case they are needed by future users
            SaveEmptyPerformanceAssumptionAssignments(performanceAssumptionDataSetId);

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.PerformanceAssumptionAssignmentEntities.AddRange(_listOfPerformanceAssumptionAssignmentEntities);
                securitizationEngineContext.SaveChanges();
            }
        }

        private void SaveEmptyPerformanceAssumptionAssignments(int performanceAssumptionDataSetId)
        {
            foreach (var performanceCurveName in _vectorParentsDictionary.Keys)
            {
                var performanceCurveEntries = _projectedCashFlowLogic.ProjectedPerformanceAssumptions[performanceCurveName];
                var performanceCurveTypes = performanceCurveEntries.Keys.ToList();

                foreach (var performanceCurveType in performanceCurveTypes)
                {
                    var performanceAssumptionAssignmentEntity = new PerformanceAssumptionAssignmentEntity
                    {
                        PerformanceAssumptionDataSetId = performanceAssumptionDataSetId,
                        PerformanceAssumptionIdentifier = performanceCurveName,
                        PerformanceAssumptionTypeId = _typesAndConventionsDatabaseRepository.PerformanceCurveTypesReversed[performanceCurveType],
                        VectorParentId = _vectorParentsDictionary[performanceCurveName][performanceCurveType]
                    };

                    _listOfPerformanceAssumptionAssignmentEntities.Add(performanceAssumptionAssignmentEntity);
                }
            }
        }
    }
}
