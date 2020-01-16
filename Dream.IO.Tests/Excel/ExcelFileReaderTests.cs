using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities.TestRecords;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dream.IO.Tests.Excel
{
    [TestClass]
    public class ExcelFileReaderTests
    {
        private const string _testFileOne = "Dream.IO.Tests.Resources.Test1ExcelInput.xlsx";
        private const string _testFileTwo = "Dream.IO.Tests.Resources.Test2ExcelInput.xlsx";

        private const string _testTabOne = "TestTabOne";
        private const string _testTabTwo = "TestTabTwo";
        private const string _testTabThree = "TestTabThree";
        private const string _nonExistentTab = "NonExisitentTab";

        private const string _notLoaded = "NotLoaded";

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromAllTabs_DataDictionarySupplied_ExpectedNumberOfTabsAreLoaded()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileOne);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromAllTabs();

            Assert.AreEqual(2, excelFileData.Keys.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromAllTabs_DataDictionarySuppliedOutsideOfExcel_ExpectedNumberOfTabsAreLoaded()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var tabSpecificTypeDictionary = new Dictionary<string, Type>
            {
                [_testTabOne] = typeof(TestRecord1),
                [_testTabTwo] = typeof(TestRecord2),
            };

            var excelReader = new ExcelFileReader(inputFileStream, tabSpecificTypeDictionary);
            var excelFileData = excelReader.GetDataFromAllTabs();

            Assert.AreEqual(2, excelFileData.Keys.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromAllTabs_DataDictionarySupplied_ExpectedNumberOfRecordsOnEachTab()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileOne);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromAllTabs();

            foreach (var tab in excelFileData.Keys)
            {
                var listOfData = excelFileData[tab];
                Assert.AreEqual(4, listOfData.Count);
            }
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromAllTabs_DataDictionarySuppliedOutsideOfExcel_ExpectedNumberOfRecordsOnEachTab()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var tabSpecificTypeDictionary = new Dictionary<string, Type>
            {
                [_testTabOne] = typeof(TestRecord1),
                [_testTabTwo] = typeof(TestRecord2),
            };

            var excelReader = new ExcelFileReader(inputFileStream, tabSpecificTypeDictionary);
            var excelFileData = excelReader.GetDataFromAllTabs();

            foreach (var tab in excelFileData.Keys)
            {
                var listOfData = excelFileData[tab];
                Assert.AreEqual(4, listOfData.Count);
            }
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void GetDataFromAllTabs_NoDataTypeDictionarySupplied_ThrowsException()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            excelReader.GetDataFromAllTabs();
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromSpecificTab_OneBlockOfDataProvided_ExpectedNumberOfRecords()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromSpecificTab<TestRecord1>(_testTabOne);

            Assert.AreEqual(4, excelFileData.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromSpecificTab_OneBlockOfDataProvided_SumOfNumericalDataEqualsExpectedAmounts()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromSpecificTab<TestRecord1>(_testTabOne);

            Assert.AreEqual(20.669, excelFileData.Sum(d => d.Test1Double), 0.0001);
            Assert.AreEqual(26, excelFileData.Sum(d => d.Test1Int));
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromSpecificTab_MissingDataInColumns_MisingDataLoadsAsDefaultValueOfObject()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromSpecificTab<TestRecord2>(_testTabTwo);

            var defaultValueOfDateTime = default(DateTime);
            var firstRecordWithMissingDateTime = excelFileData.First(d => d.Test2Date == DateTime.MinValue);
            Assert.AreEqual(defaultValueOfDateTime.Ticks, firstRecordWithMissingDateTime.Test2Date.Ticks);

            var defaultValueOfString = default(string);
            var firstRecordWithMissingString = excelFileData.First(d => d.Test2String == defaultValueOfString);
            Assert.IsNull(firstRecordWithMissingString.Test2String);

            var defaultValueOfInteger = default(int);
            var firstRecordWithMissingInteger = excelFileData.First(d => d.Test2Int == 0);
            Assert.AreEqual(defaultValueOfInteger, firstRecordWithMissingInteger.Test2Int);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromSpecificTab_MixtureOfUsedAndUnusedColumns_UnusedColumnIsNotLoaded()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromSpecificTab<TestRecord2>(_testTabTwo);

            Assert.IsFalse(excelFileData.Any(d => d.Test2String == _notLoaded));
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromSpecificTab_MixtureOfUsedAndUnusedColumns_UsedColumnIsLoaded()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromSpecificTab<TestRecord1>(_testTabTwo);

            Assert.IsTrue(excelFileData.Any(d => d.Test1String == _notLoaded));
            Assert.AreEqual(4, excelFileData.Count(d => d.Test1String == _notLoaded));
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetDataFromSpecificTab_RetrievingDataFromNonExistentTab_EmptyListIsReturned()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetDataFromSpecificTab<TestRecord1>(_nonExistentTab);

            Assert.AreEqual(0, excelFileData.Count);
            Assert.IsFalse(excelFileData.Any());
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetTransposedDataFromSpecificTab_OneBlockAndOneRowOfDataProvided_OnlyOneRecordIsLoaded()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetTransposedDataFromSpecificTab<TestRecord1>(_testTabThree);

            Assert.AreEqual(1, excelFileData.Count);
            Assert.IsFalse(excelFileData.Any(d => d.Test1String == _notLoaded));
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetTransposedDataFromSpecificTab_OneBlockAndTwoRowsOfDataProvided_BothRecordsAreLoaded()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileOne);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetTransposedDataFromSpecificTab<TestRecord1>(_testTabThree);

            Assert.AreEqual(2, excelFileData.Count);
            Assert.IsTrue(excelFileData.Any(d => d.Test1String == _notLoaded));
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetTransposedDataFromSpecificTab_OneBlockOfDataProvided_UnusedColumnsAreIgnored()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetTransposedDataFromSpecificTab<TestRecord1>(_testTabThree);

            Assert.IsFalse(excelFileData.Any(d => d.Test1String == _notLoaded));
            Assert.AreEqual(7777, excelFileData.First().Test1Int);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetTransposedDataFromSpecificTab_OneBlockOfDataProvided_UsedColumnsAreNotIgnored()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetTransposedDataFromSpecificTab<TestRecord2>(_testTabThree);

            Assert.IsTrue(excelFileData.Any(d => d.Test2String == _notLoaded));
            Assert.AreEqual(DateTime.MinValue, excelFileData.First().Test2Date);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetTransposedDataFromSpecificTab_RetrievingDataFromNonExistentTab_EmptyListIsReturned()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetTransposedDataFromSpecificTab<TestRecord1>(_nonExistentTab);

            Assert.AreEqual(0, excelFileData.Count);
            Assert.IsFalse(excelFileData.Any());
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetExcelDataRowsFromWorksheet_OneBlockOfDataProvided_ExpectedNumberOfRecords()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetExcelDataRowsFromWorksheet(_testTabOne);

            Assert.AreEqual(5, excelFileData.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void GetExcelDataRowsFromWorksheet_RetrievingDataFromNonExistentTab_EmptyListIsReturned()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testFileTwo);
            var excelReader = new ExcelFileReader(inputFileStream);
            var excelFileData = excelReader.GetExcelDataRowsFromWorksheet(_nonExistentTab);

            Assert.AreEqual(0, excelFileData.Count);
            Assert.IsFalse(excelFileData.Any());
        }
    }
}
