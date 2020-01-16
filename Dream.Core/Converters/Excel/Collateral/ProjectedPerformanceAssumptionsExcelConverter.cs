using ClosedXML.Excel;
using Dream.Common.Enums;
using Dream.Common.ExtensionMethods;
using Dream.Core.BusinessLogic.ProjectedCashFlows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Excel.Collateral
{
    /// <summary>
    /// Converts various performance assumptions curves (i.e. CPR, CDR, LGD, etc.) into a projected performance assumptions business object.
    /// </summary>
    public class ProjectedPerformanceAssumptionsExcelConverter
    {
        /// <summary>
        /// Converts a collection of Excel rows into a ProjectedPerformanceAssumptions business object.
        /// </summary>
        public static ProjectedPerformanceAssumptions ConvertExcelRowsToProjectedPeformanceAssumptions(List<IXLRangeRow> excelDataRows)
        {
            // This is the header row that will contain the performance curve names
            var performanceCurveNames = excelDataRows.First();
            var performanceCurveNameColumns = performanceCurveNames.CellCount();

            // This is the header row that will contain the performance curve types
            var performanceCurveTypes = performanceCurveNames.RowBelow();
            var performanceCurveTypeColumns = performanceCurveTypes.CellCount();

            if (performanceCurveNameColumns != performanceCurveTypeColumns)
            {
                throw new Exception("ERROR: Performance number of curve names and types do not match. Cannot load performance assumptions.");
            }

            // Note that column indexing in ClosedXML starts at unity, not zero
            var listOfCurveNames = new List<string>();
            var listOfCurveTypes = new List<PerformanceCurveType>();
            for (var columnNumber = 1; columnNumber < performanceCurveTypeColumns; columnNumber++)
            {
                var curveName = performanceCurveNames.Cell(columnNumber + 1).GetValue<string>();
                var curveTypeAsString = performanceCurveTypes.Cell(columnNumber + 1).GetValue<string>();

                var curveTypeTitleCase = curveTypeAsString.ToTitleCase();
                var curveType = (PerformanceCurveType) Enum.Parse(typeof(PerformanceCurveType), curveTypeTitleCase);

                listOfCurveNames.Add(curveName);
                listOfCurveTypes.Add(curveType);
            }

            // This remaining block of data should contain the curve values
            // The first cell in each row should contain the integer period number
            var curveValuesDataRows = excelDataRows.Skip(2)
                .OrderBy(r => r.FirstCell().GetValue<int>())
                .ToList();

            var projectedPerformanceAssumptions = new ProjectedPerformanceAssumptions();

            // Again, note that column indexing in ClosedXML starts at unity, not zero
            foreach (var curveValuesDataRow in curveValuesDataRows)
            {
                var periodNumber = curveValuesDataRow.FirstCell().GetValue<int>();

                for (var columnNumber = 1; columnNumber < performanceCurveTypeColumns; columnNumber++)
                {
                    var performanceCurveName = listOfCurveNames[columnNumber - 1];
                    var performanceCurveType = listOfCurveTypes[columnNumber - 1];

                    var performanceCurveValue = curveValuesDataRow.Cell(columnNumber + 1).GetValue<double>();
                    projectedPerformanceAssumptions[performanceCurveName, performanceCurveType, periodNumber] = performanceCurveValue;
                }
            }

            // Note that all assumptions will be applied on a monthly basis
            projectedPerformanceAssumptions.ConvertAllCurvesToMonthlyRate();
            return projectedPerformanceAssumptions;
        }
    }
}
