using ClosedXML.Excel;
using Dream.Core.BusinessLogic.Aggregation;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Excel.Collateral
{
    /// <summary>
    /// Converts aggregation groupings on any level into an aggregation groupings business object.
    /// </summary>
    public class AggregationGroupingsExcelConverter
    {
        /// <summary>
        /// Converts a collection of Excel rows into a ProjectedPerformanceAssumptions business object.
        /// </summary>
        public static AggregationGroupings ConvertExcelRowsToAggregationGroupings(List<IXLRangeRow> excelDataRows)
        {
            var aggregationGroupings = new AggregationGroupings();
            if (!excelDataRows.Any() || excelDataRows == null) return aggregationGroupings;

            // This is the header row that will contain the aggregation grouping names
            var aggregationGroupingIdentifiers = excelDataRows.First();
            var aggregationGroupingIdentifierColumns = aggregationGroupingIdentifiers.CellCount();

            // Note that column indexing in ClosedXML starts at unity, not zero
            var listOfAggregationGroupingIdentifiers = new List<string>();
            for (var columnNumber = 1; columnNumber < aggregationGroupingIdentifierColumns; columnNumber++)
            {
                var aggregationGroupingIdentifier = aggregationGroupingIdentifiers.Cell(columnNumber + 1).GetValue<string>();
                listOfAggregationGroupingIdentifiers.Add(aggregationGroupingIdentifier);
            }

            // This remaining block of data should contain the aggregation groupings
            // The first cell in each row should contain the product identifier
            var aggregationGroupingDataRows = excelDataRows.Skip(1).ToList();
           
            // Again, note that column indexing in ClosedXML starts at unity, not zero
            foreach (var aggregationGroupingDataRow in aggregationGroupingDataRows)
            {
                var productIdentifier = aggregationGroupingDataRow.FirstCell().GetValue<string>();

                for (var columnNumber = 1; columnNumber < aggregationGroupingIdentifierColumns; columnNumber++)
                {
                    var aggregationGroupingIdentifier = listOfAggregationGroupingIdentifiers[columnNumber - 1];
                    var aggregationGrouping = aggregationGroupingDataRow.Cell(columnNumber + 1).GetValue<string>();

                    aggregationGroupings[productIdentifier, aggregationGroupingIdentifier] = aggregationGrouping;
                }
            }

            return aggregationGroupings;
        }
    }
}
