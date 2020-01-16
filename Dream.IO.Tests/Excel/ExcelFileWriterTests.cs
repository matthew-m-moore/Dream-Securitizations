using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities.TestRecords;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Common.Utilities;

namespace Dream.IO.Tests.Excel
{
    [TestClass]
    public class ExcelFileWriterTests
    {
        private const string _defaultListTabName = "ListOfData";
        private const string _defaultTableTabName = "DataTable";
        private const string _testTabName = "TestTabName";

        private const string _testTabOne = "TestTabOne";
        private const string _testTabTwo = "TestTabTwo";

        [TestMethod, Owner("Matthew Moore")]
        public void AddWorksheetForListOfData_AddTwoWorksheetsWithDifferentTabNames_WorkbookHasTwoTabs()
        {
            var listOfTestData = GetListOfTestData();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForListOfData(listOfTestData, _testTabOne);
            excelFileWriter.AddWorksheetForListOfData(listOfTestData, _testTabTwo);

            Assert.AreEqual(2, excelFileWriter.ExcelWorkbook.Worksheets.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddWorksheetForListOfData_AddTwoWorksheetsWithSameTabName_ThrowsException()
        {
            var listOfTestData = GetListOfTestData();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForListOfData(listOfTestData);
            excelFileWriter.AddWorksheetForListOfData(listOfTestData);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void AddWorksheetForListOfData_SupplyListOfDateWithoutWorksheetName_DataIsAddedWithDefaultTabName()
        {
            var listOfTestData = GetListOfTestData();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForListOfData(listOfTestData);

            var excelTabName = excelFileWriter.ExcelWorkbook.Worksheets.First().Name;
            Assert.AreEqual(_defaultListTabName, excelTabName);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void AddWorksheetForListOfData_SupplyListOfDateWithWorksheetName_DataIsAddedWithProvidedTabName()
        {
            var listOfTestData = GetListOfTestData();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForListOfData(listOfTestData, _testTabName);

            var excelTabName = excelFileWriter.ExcelWorkbook.Worksheets.First().Name;
            Assert.AreEqual(_testTabName, excelTabName);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void AddWorksheetForDataTable_AddTwoWorksheetsWithDifferentTabNames_WorkbookHasTwoTabs()
        {
            var testDataTable = GetTestDataTable();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForDataTable(testDataTable, _testTabOne);
            excelFileWriter.AddWorksheetForDataTable(testDataTable, _testTabTwo);

            Assert.AreEqual(2, excelFileWriter.ExcelWorkbook.Worksheets.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddWorksheetForDataTable_AddTwoWorksheetsWithSameTabName_ThrowsException()
        {
            var testDataTable = GetTestDataTable();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForDataTable(testDataTable);
            excelFileWriter.AddWorksheetForDataTable(testDataTable);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void AddWorksheetForDataTable_SupplyListOfDateWithoutWorksheetName_DataIsAddedWithDefaultTabName()
        {
            var testDataTable = GetTestDataTable();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForDataTable(testDataTable);

            var excelTabName = excelFileWriter.ExcelWorkbook.Worksheets.First().Name;
            Assert.AreEqual(_defaultTableTabName, excelTabName);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void AddWorksheetForDataTable_SupplyListOfDateWithWorksheetName_DataIsAddedWithProvidedTabName()
        {
            var testDataTable = GetTestDataTable();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForDataTable(testDataTable, _testTabName);

            var excelTabName = excelFileWriter.ExcelWorkbook.Worksheets.First().Name;
            Assert.AreEqual(_testTabName, excelTabName);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ClearWorkbook_AddTwoWorksheetsThenRemoveThem_WorkbookHasZeroTabs()
        {
            var testDataTable = GetTestDataTable();

            var excelFileWriter = new ExcelFileWriter();
            excelFileWriter.AddWorksheetForDataTable(testDataTable, _testTabOne);
            excelFileWriter.AddWorksheetForDataTable(testDataTable, _testTabTwo);
            excelFileWriter.ClearWorkbook();

            Assert.AreEqual(0, excelFileWriter.ExcelWorkbook.Worksheets.Count);
        }

        private List<TestRecord1> GetListOfTestData()
        {
            var testDataOne = new TestRecord1
            {
                Test1Date = new DateTime(2017, 1, 1),
                Test1Double = 99.99,
                Test1Int = 5,
                Test1String = "Banana"
            };

            var testDataTwo = new TestRecord1
            {
                Test1Date = new DateTime(2016, 5, 10),
                Test1Double = 8.34,
                Test1Int = 2,
                Test1String = "James"
            };

            var testDataThree = new TestRecord1
            {
                Test1Date = new DateTime(2013, 1, 3),
                Test1Double = 6.53,
                Test1Int = 1,
                Test1String = "Colin"
            };

            var listOfTestData = new List<TestRecord1>
            {
                testDataOne,
                testDataTwo,
                testDataThree
            };

            return listOfTestData;
        }

        private DataTable GetTestDataTable()
        {
            var listOfTestData = GetListOfTestData();
            var testDataTable = DataTableUtility.ConvertListToDataTable(listOfTestData);
            return testDataTable;
        }
    }
}
