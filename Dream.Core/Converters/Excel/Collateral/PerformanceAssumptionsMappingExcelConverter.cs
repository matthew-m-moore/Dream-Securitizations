using System;
using System.Collections.Generic;
using Dream.IO.Excel.Entities;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using Dream.Common.ExtensionMethods;
using Dream.Common.Enums;
using Dream.Common.Curves;

namespace Dream.Core.Converters.Excel.Collateral
{
    /// <summary>
    /// Converts various performance assumptions mapping entity objects into a performance assumptions mapping business object.
    /// </summary>
    public class PerformanceAssumptionsMappingExcelConverter
    {
        private bool _convertToMonthly;
        private bool _convertToAnnual; 

        public PerformanceAssumptionsMappingExcelConverter(bool convertAssumptionsToMonthly, bool convertAssumptionsToAnnual)
        {
            _convertToMonthly = convertAssumptionsToMonthly;
            _convertToAnnual = convertAssumptionsToAnnual;
        }

        /// <summary>
        /// Converts a collection of Excel-based PerformanceAssumptionMappingRecord objects into a PerformanceAssumptionsMapping business object.
        /// </summary>
        public PerformanceAssumptionsMapping ConvertToPeformanceAssumptionsMapping(
            List<PerformanceAssumptionMappingRecord> performanceAssumptionsMappingRecords)
        {
            var performanceAssumptionsMapping = new PerformanceAssumptionsMapping();
            foreach (var performanceAssumptionsMappingRecord in performanceAssumptionsMappingRecords)
            {
                var assumptionsGrouping = performanceAssumptionsMappingRecord.AssumptionsGrouping ?? string.Empty;

                var assumptionsIdentifier = performanceAssumptionsMappingRecord.AssumptionsIdentifier;
                var performanceCurveName = performanceAssumptionsMappingRecord.CurveName;
                
                var performanceCurveTypeAsString = performanceAssumptionsMappingRecord.CurveType;
                var performanceCurveTypeTitleCase = performanceCurveTypeAsString.ToTitleCase();
                var performanceCurveType = (PerformanceCurveType)Enum.Parse(typeof(PerformanceCurveType), performanceCurveTypeTitleCase);

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
