using Dream.Common.Curves;
using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.IO.Database.Entities.Collateral;
using System;
using System.Collections.Generic;

namespace Dream.Core.Converters.Database.Collateral
{
    public class PerformanceAssumptionsMappingDatabaseConverter
    {
        private bool _convertToMonthly;
        private bool _convertToAnnual;

        Dictionary<int, PerformanceCurveType> _performanceCurveTypesDictionary;

        public PerformanceAssumptionsMappingDatabaseConverter(
            Dictionary<int, PerformanceCurveType> performanceCurveTypesDictionary,
            bool convertAssumptionsToMonthly, 
            bool convertAssumptionsToAnnual)
        {
            _performanceCurveTypesDictionary = performanceCurveTypesDictionary;
            _convertToMonthly = convertAssumptionsToMonthly;
            _convertToAnnual = convertAssumptionsToAnnual;
        }

        public PerformanceAssumptionsMapping ConvertToPerformanceAssumptionsMapping(
            List<PerformanceAssumptionAssignmentEntity> performanceAssumptionAssignmentEntities)
        {
            var performanceAssumptionsMapping = new PerformanceAssumptionsMapping();
            foreach (var performanceAssumptionsAssignmentEntity in performanceAssumptionAssignmentEntities)
            {
                var assumptionsGrouping = performanceAssumptionsAssignmentEntity.PerformanceAssumptionGrouping ?? string.Empty;
                var assumptionsIdentifier = performanceAssumptionsAssignmentEntity.InstrumentIdentifier;
                if (assumptionsIdentifier == null) continue;

                var performanceCurveName = performanceAssumptionsAssignmentEntity.PerformanceAssumptionIdentifier;
                var performanceCurveTypeId = performanceAssumptionsAssignmentEntity.PerformanceAssumptionTypeId;
                var performanceCurveType = _performanceCurveTypesDictionary[performanceCurveTypeId];

                performanceCurveType = ConvertToAnnualOrMonthly(performanceCurveType);

                performanceAssumptionsMapping[assumptionsGrouping, assumptionsIdentifier, performanceCurveType] = performanceCurveName;
            }

            return performanceAssumptionsMapping;
        }

        private PerformanceCurveType ConvertToAnnualOrMonthly(PerformanceCurveType performanceCurveType)
        {
            if (_convertToMonthly && _convertToAnnual)
            {
                throw new Exception("ERROR: Cannot convert performance curve type to both annual and monthly, simultaneously.");
            }

            if (_convertToAnnual)
            {
                return PerformanceCurve.FindAnnualCompanionType(performanceCurveType);
            }

            if (_convertToMonthly)
            {
                return PerformanceCurve.FindMonthlyCompanionType(performanceCurveType);
            }

            return performanceCurveType;
        }
    }
}
