using ClosedXML.Excel;
using Dream.IO.Excel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Excel
{
    /// <summary>
    /// A class used to convert excel data into FinanceManagementMappingRecord objects for further processing.
    /// </summary>
    public class FinanceManagementMappingRecordExcelConverter
    {
        private const int _numberOfHeaderRows = 2;

        private const string _numberColumnName = "Number";
        private const string _descriptionColumnName = "Description";

        /// <summary>
        /// Converts a list of Excel data rows into a dictionary of FinanceMangementMappingRecord objects, with the key equal to the aggregation group.
        /// </summary>
        public static Dictionary<string, List<FinanceManagementMappingRecord>> ConvertExcelRowsToFinanceManagementMappingRecordsDictionary(List<IXLRangeRow> excelDataRows)
        {
            // This is the row that will contain the aggregation group names, which will be the dictionary keys
            var aggregationGroupNames = excelDataRows.First().RowBelow();
            var aggregationGroupNamesColumns = aggregationGroupNames.CellCount();

            var numberColumnCell = aggregationGroupNames.Cells().SingleOrDefault(c => c.GetValue<string>() == _numberColumnName);
            var startingIndexCell = aggregationGroupNames.Cells().SingleOrDefault(c => c.GetValue<string>() == _descriptionColumnName);
            if (startingIndexCell == null)
            {
                throw new Exception("ERROR: One of the mapping data tabs is missing a column header labeled 'Description' or has multiple column headers labeled 'Description'. " + 
                    "Please check the inputs file.");
            }

            var startingIndexCellColumnNumber = startingIndexCell.Address.ColumnNumber;
            var distinctAggregagationGroupNames = aggregationGroupNames.Cells().Select(c => c.GetValue<string>()).Distinct().Count();
            if (distinctAggregagationGroupNames < aggregationGroupNamesColumns)
            {
                throw new Exception("ERROR: One of the mapping data tabs has at least two mappings with the same name. This is not allowed. Please check the inputs file.");
            }

            var financeManagementMappingRecordDictionary = new Dictionary<string, List<FinanceManagementMappingRecord>>();
            foreach (var excelDataRow in excelDataRows.Skip(_numberOfHeaderRows))
            {
                for (var columnNumber = startingIndexCellColumnNumber; columnNumber < aggregationGroupNamesColumns; columnNumber++)
                {
                    var aggregagtionGroupName = aggregationGroupNames.Cell(columnNumber + 1).GetValue<string>();
                    if (!financeManagementMappingRecordDictionary.ContainsKey(aggregagtionGroupName))
                    {
                        financeManagementMappingRecordDictionary.Add(aggregagtionGroupName, new List<FinanceManagementMappingRecord>());
                    }

                    // Note, not all tabs will have a field for "Number" and a column header for it
                    var financeManagementMappingRecord = new FinanceManagementMappingRecord
                    {
                        Number = (numberColumnCell == null) ? null : TryGetInteger(excelDataRow.Cell(numberColumnCell.Address.ColumnNumber)),
                        Description = excelDataRow.Cell(startingIndexCellColumnNumber).GetValue<string>(),
                        MappingIdentifier = excelDataRow.Cell(columnNumber + 1).GetValue<string>()
                    };

                    financeManagementMappingRecordDictionary[aggregagtionGroupName].Add(financeManagementMappingRecord);
                }
            }

            return financeManagementMappingRecordDictionary;
        }

        private static int? TryGetInteger(IXLCell excelCell)
        {
            int? nullableInteger = null;
            int integerValue = 0;
            
            if (int.TryParse(excelCell.GetValue<string>(), out integerValue))
            {
                nullableInteger = integerValue;
            }

            return nullableInteger;
        }
    }
}
