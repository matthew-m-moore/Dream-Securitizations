using System.Collections.Generic;
using System.Linq;
using Dream.IO.Database.Entities;
using Dream.IO.Database.Entities.Collateral;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Common.Enums;

namespace Dream.Core.Converters.Database.Collateral
{
    public class ProjectedPerformanceAssumptionsDatabaseConverter
    {
        public static ProjectedPerformanceAssumptions ConvertToProjectedPerformanceAssumptions(            
            List<PerformanceAssumptionAssignmentEntity> performanceAssumptionAssignmentEntities,
            Dictionary<int, List<VectorEntity>> vectorEntitiesDictionary,
            Dictionary<int, PerformanceCurveType> performanceCurveTypesDictionary)
        {
            var projectedPerformanceAssumptions = new ProjectedPerformanceAssumptions();

            // Note that a value tuple is used here instead of an anonymous type because anonymous types are reference types
            var groupedPerformanceAssumptionAssignmentEntities = performanceAssumptionAssignmentEntities
                .GroupBy(e => ( PerformanceCurveName: e.PerformanceAssumptionIdentifier,
                                PerformanceCurveTypeId: e.PerformanceAssumptionTypeId,
                                VectorParentId: e.VectorParentId));

            foreach (var groupOfPerformanceAssumptionAssignmentEntities in groupedPerformanceAssumptionAssignmentEntities)
            {
                var performanceCurveName = groupOfPerformanceAssumptionAssignmentEntities.Key.PerformanceCurveName;
                var performanceCurveTypeId = groupOfPerformanceAssumptionAssignmentEntities.Key.PerformanceCurveTypeId;
                var performanceCurveType = performanceCurveTypesDictionary[performanceCurveTypeId];

                var vectorParentId = groupOfPerformanceAssumptionAssignmentEntities.Key.VectorParentId;
                var vectorEntities = vectorEntitiesDictionary[vectorParentId];

                foreach (var vectorEntity in vectorEntities)
                {
                    var periodNumber = vectorEntity.VectorPeriod;
                    var performanceCurveValue = vectorEntity.VectorValue;

                    projectedPerformanceAssumptions[performanceCurveName, performanceCurveType, periodNumber] = performanceCurveValue;
                }
            }

            // Note that all assumptions will be applied on a monthly basis
            projectedPerformanceAssumptions.ConvertAllCurvesToMonthlyRate();

            AddPerformanceAssumptionsMapping(
                projectedPerformanceAssumptions, 
                performanceAssumptionAssignmentEntities, 
                performanceCurveTypesDictionary);

            return projectedPerformanceAssumptions;
        }

        private static void AddPerformanceAssumptionsMapping(
            ProjectedPerformanceAssumptions projectedPerformanceAssumptions,
            List<PerformanceAssumptionAssignmentEntity> performanceAssumptionAssignmentEntities,
            Dictionary<int, PerformanceCurveType> performanceCurveTypesDictionary)
        {
            // All assumptions will be used on a monthly basis in most cases
            var convertAssumptionsToAnnual = false;
            var convertAssumptionsToMonthly = true;

            var performanceAssumptionsMappingConverter = new PerformanceAssumptionsMappingDatabaseConverter(
                performanceCurveTypesDictionary,
                convertAssumptionsToAnnual,
                convertAssumptionsToMonthly);

            var performanceAssumptionsMapping =
                performanceAssumptionsMappingConverter.ConvertToPerformanceAssumptionsMapping(performanceAssumptionAssignmentEntities);

            projectedPerformanceAssumptions.PerformanceAssumptionsMapping = performanceAssumptionsMapping;
        }
    }
}
