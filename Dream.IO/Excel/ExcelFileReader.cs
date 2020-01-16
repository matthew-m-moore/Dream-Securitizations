using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Dream.IO.Excel.Entities;
using ClosedXML.Excel;

namespace Dream.IO.Excel
{
    /// <summary>
    /// Reads data from Microsoft Excel files, assuming that each tab of the file only contains
    /// one block of data. If attempting to read from all tabs, a tab called DataTypeDictionary
    /// must be supplied that maps each tab name to an appropriate data type.
    /// 
    /// If a DataTypeDictionary is present, but it does not include a mapping for all tabs,
    /// the unmapped tabs will be ignored. Additionally, if fields are listed for a data type
    /// that are not in the mapped object, those fields will be skipped.
    /// </summary>
    public class ExcelFileReader
    {
        private const string _dataTypeDictionaryTabName = "DataTypeDictionary";
        private string _convertTabFunctionName = nameof(ConvertTabDataToSpecificType);

        private string _filePath;
        private Stream _fileStream;

        private XLWorkbook _excelWorkbook;
        private List<IXLWorksheet> _excelWorksheets;
        private List<string> _excelWorksheetTabNames;

        // This variable holds the expected type of each object on each tab
        private Dictionary<string, Type> _tabSpecificTypeDictionary;
        private Dictionary<Type, Func<object>> _objectCreationFunctionsDictionary;
        private Dictionary<Type, Dictionary<string, PropertyInfo>> _dataTypePropertiesDictionary;

        // A public list of tabs that can be set to be ignored, if necessary
        public List<string> TabsToIgnore = new List<string>();

        public string FileName => Path.GetFileNameWithoutExtension(_filePath);

        public ExcelFileReader(string filePath) : this(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            _filePath = filePath;
        }

        public ExcelFileReader(Stream fileStream)
        {
            _fileStream = fileStream;

            _excelWorkbook = new XLWorkbook(fileStream);
            _excelWorksheets = _excelWorkbook.Worksheets.ToList();
            _excelWorksheetTabNames = _excelWorksheets.Select(w => w.Name).ToList();

            _objectCreationFunctionsDictionary = new Dictionary<Type, Func<object>>();
            _dataTypePropertiesDictionary = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }

        public ExcelFileReader(Stream fileStream, Dictionary<string, Type> tabSpecificTypeDictionary) : this(fileStream)
        {
            _tabSpecificTypeDictionary = tabSpecificTypeDictionary;
            _objectCreationFunctionsDictionary = new Dictionary<Type, Func<object>>();
            _dataTypePropertiesDictionary = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }

        /// <summary>
        /// Note, this copy method does not handle the case of supplying a tab-specific type dictionary.
        /// </summary>
        public ExcelFileReader Copy()
        {
            // Getting a new version of an XLWorkbook object takes 0.5s, so it's better to just fake the copy.
            // There's really no reason this object would change anyhow, it doesn't need to be new.
            return this;
        }

        public void Dispose()
        {
            _filePath = null;
            _fileStream.Dispose();

            _excelWorkbook = null;
            _excelWorksheets = null;
            _excelWorksheetTabNames = null;

            _tabSpecificTypeDictionary = null;
            _objectCreationFunctionsDictionary = null;
            _dataTypePropertiesDictionary = null;
        }

        /// <summary>
        /// Grabs data from all tabs in the Excel workbook provided. Assumes that each tab
        /// only has one block of data.
        /// </summary>
        public Dictionary<string, IList> GetDataFromAllTabs()
        {
            if (_tabSpecificTypeDictionary == null)
            {
                if (_excelWorksheetTabNames.Any(tabName => tabName == _dataTypeDictionaryTabName))
                {
                    LoadDataTypeDictionary();
                }
                else
                {
                    throw new Exception("ERROR: Cannot get data from all tabs. A tab called DataTypeDictionary was not supplied");
                }
            }

            var excelDataDictionary = new Dictionary<string, IList>();

            foreach (var excelWorksheet in _excelWorksheets)
            {
                var tabName = excelWorksheet.Name;
                if (TabsToIgnore.Contains(tabName)) continue;

                // If the data type dictionary has no record for the tab, skip it
                if (!_tabSpecificTypeDictionary.ContainsKey(tabName)) continue;

                var dataType = _tabSpecificTypeDictionary[tabName];
                var listOfData = GetDataFromSpecificTab(excelWorksheet, dataType);

                excelDataDictionary.Add(tabName, listOfData);
            }

            return excelDataDictionary;
        }

        /// <summary>
        /// Grabs data from a specified tab in the Excel workbook provided. Assumes that only
        /// one block of data exists in the tab, conforming to the type T supplied.
        /// </summary>
        public List<T> GetDataFromSpecificTab<T>(string tabName) where T : class, new()
        {
            var doesWorkbookContainSpecifiedTab = _excelWorksheetTabNames.Contains(tabName);
            if (!doesWorkbookContainSpecifiedTab)
            {
                return new List<T>();
            }

            var excelWorksheet = _excelWorkbook.Worksheet(tabName);

            IXLRangeRow headersRow;
            var dataHasOneRowOfHeaders = true;
            var dataRows = GetExcelDataRowsFromWorksheet(excelWorksheet, dataHasOneRowOfHeaders, out headersRow);

            var listOfSpecificallyTypedData = ConvertTabDataToSpecificType<T>(dataRows, headersRow);

            return listOfSpecificallyTypedData;
        }

        /// <summary>
        /// Grabs transposed data from a specified tab in the Excel workbook provided. Assumes that only
        /// one block of data exists in the tab, conforming to the type T supplied.
        /// </summary>
        public List<T> GetTransposedDataFromSpecificTab<T>(string tabName) where T : class, new()
        {
            var doesWorkbookContainSpecifiedTab = _excelWorksheetTabNames.Contains(tabName);
            if (!doesWorkbookContainSpecifiedTab)
            {
                return new List<T>();
            }

            var excelWorksheet = _excelWorkbook.Worksheet(tabName);

            IXLRangeRow headersRow;
            var dataHasOneRowOfHeaders = false;
            var dataTable = GetExcelDataTableFromWorksheet(excelWorksheet, dataHasOneRowOfHeaders, out headersRow);

            // Note that this funny bit of code is important for cells with formulas, see below for explanation
            foreach (var cell in dataTable.Cells())
            {
                if (cell.HasFormula)
                {
                    var evaluatedCellValue = cell.Value;
                    cell.Value = evaluatedCellValue;
                }
            }

            // When the tranpose happens here, if the cells in any formulas were not locked (i.e. with $ symbols)
            // this transpose operation will alter those formula, deviating from the original intent of the user
            dataTable.Transpose(XLTransposeOptions.MoveCells);
            var dataRows = dataTable.Rows().ToList();

            headersRow = dataRows.First();
            dataRows = dataRows.Skip(1).ToList();
            var listOfSpecificallyTypedData = ConvertTabDataToSpecificType<T>(dataRows, headersRow);

            return listOfSpecificallyTypedData;
        }

        /// <summary>
        /// Pulls the raw data rows from a tabe in the Excel workbook provide. Assumes that only
        /// one block of data exists in the tab, but does not make any assumptiongs about where the
        /// headers of that data might be.
        /// </summary>
        public List<IXLRangeRow> GetExcelDataRowsFromWorksheet(string tabName)
        {
            var doesWorkbookContainSpecifiedTab = _excelWorksheetTabNames.Contains(tabName);
            if (!doesWorkbookContainSpecifiedTab)
            {
                return new List<IXLRangeRow>();
            }

            var excelWorksheet = _excelWorkbook.Worksheet(tabName);

            IXLRangeRow headersRow;
            var dataHasOneRowOfHeaders = false;
            var dataRows = GetExcelDataRowsFromWorksheet(excelWorksheet, dataHasOneRowOfHeaders, out headersRow);

            // Note that the headers row out parameter is discarded, since these may not be the headers at all
            return dataRows;
        }

        private IList GetDataFromSpecificTab(IXLWorksheet excelWorksheet, Type dataType)
        {
            IXLRangeRow headersRow;
            var dataHasOneRowOfHeaders = true;
            var dataRows = GetExcelDataRowsFromWorksheet(excelWorksheet, dataHasOneRowOfHeaders, out headersRow);

            // Note that, by default, "GetMethod()" only looks for public static methods
            var listOfData = GetType()
                            .GetMethod(_convertTabFunctionName, BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(dataType)
                            .Invoke(this, new object[] { dataRows, headersRow });

            return listOfData as IList;
        }

        private IXLRange GetExcelDataTableFromWorksheet(IXLWorksheet excelWorksheet, bool dataHasOneRowOfHeaders, out IXLRangeRow excelHeadersRow)
        {
            var firstPopulatedRow = excelWorksheet.FirstRowUsed();
            var headersRow = firstPopulatedRow.RowUsed();
            excelHeadersRow = headersRow;

            // In some cases, data may have multiple rows of headers, in which case returning the whole data table
            // would be easier to work with, rather than splicing out one odd line at the top
            var dataStartRowNumber = headersRow.RowNumber();
            if (dataHasOneRowOfHeaders)
            {
                dataStartRowNumber = headersRow.RowBelow().RowNumber();
            }

            // Find the first and last populated cells in the given data block
            var firstPopulatedCell = excelWorksheet.Row(dataStartRowNumber).FirstCell().Address;
            var lastPopulatedCell = excelWorksheet.LastCellUsed().Address;

            // Capture the data block and conver it into rows of a specific data type
            var dataRange = excelWorksheet.Range(firstPopulatedCell, lastPopulatedCell).RangeUsed();

            return dataRange;
        }

        private List<IXLRangeRow> GetExcelDataRowsFromWorksheet(IXLWorksheet excelWorksheet, bool dataHasOneRowOfHeaders, out IXLRangeRow excelHeadersRow)
        {
            var dataRange = GetExcelDataTableFromWorksheet(excelWorksheet, dataHasOneRowOfHeaders, out excelHeadersRow);
            if (dataRange == null) return new List<IXLRangeRow>();

            var listOfDataRows = dataRange.Rows().Select(row => row).ToList();
            return listOfDataRows;
        }

        private void LoadDataTypeDictionary()
        {
            _tabSpecificTypeDictionary = new Dictionary<string, Type>();

            var listOfDataTypeRecords = GetDataFromSpecificTab<TabDataTypeRecord>(_dataTypeDictionaryTabName);

            foreach (var dataTypeRecord in listOfDataTypeRecords)
            {
                var tabName = dataTypeRecord.TabName;
                var dataType = Type.GetType(dataTypeRecord.FullyQualifiedType);

                _tabSpecificTypeDictionary.Add(tabName, dataType);
            }
        }

        private List<T> ConvertTabDataToSpecificType<T>(
            List<IXLRangeRow> listOfExcelDataRows,
            IXLRangeRow excelHeadersRow) where T : class, new()
        {
            AssembleObjectCreationFunction<T>();
            AssembleDataTypeProperties<T>();

            var dataType = typeof(T);
            var objectProperties = _dataTypePropertiesDictionary[dataType];
            var objectCreationFunction = _objectCreationFunctionsDictionary[dataType];

            var listOfSpecificallyTypedData = listOfExcelDataRows
                .Select(excelDataRow => CreateObject<T>(excelDataRow, excelHeadersRow, objectProperties, objectCreationFunction))
                .Where(l => l != null)
                .ToList();

            return listOfSpecificallyTypedData;
        }

        private void AssembleObjectCreationFunction<T>() where T : class, new()
        {
            var dataType = typeof(T);

            // If there is no create function for this object in the dictionary yet, add it
            if (!_objectCreationFunctionsDictionary.ContainsKey(dataType))
            {
                Expression<Func<T>> functionCreator = () => new T();
                var creationFunction = functionCreator.Compile();

                _objectCreationFunctionsDictionary.Add(dataType, creationFunction);
            }
        }

        private void AssembleDataTypeProperties<T>() where T : class, new()
        {
            var dataType = typeof(T);

            // If there is no dictionary of properties for this data type in the dictionary yet, add it
            if (!_dataTypePropertiesDictionary.ContainsKey(dataType))
            {
                var dictionaryOfProperties = dataType
                    .GetProperties()
                    .Where(p => p.GetSetMethod() != null)
                    .ToDictionary(p => p.Name, p => p);

                _dataTypePropertiesDictionary.Add(dataType, dictionaryOfProperties);
            }
        }

        private T CreateObject<T>(
            IXLRangeRow excelDataRow,
            IXLRangeRow excelHeadersRow,
            Dictionary<string, PropertyInfo> objectProperties,
            Func<object> objectionCreationFunction) where T : class, new()
        {
            var newObject = objectionCreationFunction() as T;
            foreach (var cell in excelDataRow.Cells().ToList())
            {
                // Skip any cells that are not populated
                if (cell.IsEmpty()) continue;

                // Ignore the cells where the property name is not part of the target object data type
                var relativeColumnNumber = cell.Address.ColumnNumber - excelHeadersRow.FirstCell().Address.ColumnNumber;
                var propertyNameInHeader = excelHeadersRow.Cell(relativeColumnNumber + 1).GetValue<string>();
                if (!objectProperties.ContainsKey(propertyNameInHeader)) continue;

                var propertyInfo = objectProperties[propertyNameInHeader];
                var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var cellValue = HandleDateTimeCells(cell, propertyType);

                // Attempt to set the value of the new object for this particular property
                try
                {                    
                    propertyInfo.SetValue(newObject, Convert.ChangeType(cellValue, propertyType));
                }
                catch
                {
                    throw new Exception(string.Format(
                        "ERROR: Failure to read from Excel workbook for property name {0} with value of {1}",
                        propertyInfo.Name, cellValue));
                }
            }

            return newObject;
        }

        private object HandleDateTimeCells(IXLCell cell, Type propertyType)
        {
            if (propertyType == typeof(DateTime) &&
                cell.DataType != XLCellValues.DateTime)
            {
                // Use of DateTime here as a safe caste would not work, since it is a struct
                var cellValueAsDateTime = cell.Value as DateTime?;
                if (cellValueAsDateTime != null)
                {
                    return cellValueAsDateTime.Value;
                }                

                var integerDateTime = Convert.ToInt32(cell.Value);
                var dateTimeValue = DateTime.FromOADate(integerDateTime);
                return dateTimeValue;
            }

            return cell.Value;
        }
    }
}
