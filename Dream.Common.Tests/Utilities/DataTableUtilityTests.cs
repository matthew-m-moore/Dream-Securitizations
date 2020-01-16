using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dream.Common.Utilities;
using Dream.Common.TestObjects;

namespace Dream.Common.Tests.Utilities
{
    [TestClass]
    public class DataTableUtilityTests
    {
        [TestMethod, Owner("Matthew Moore")]
        public void ConvertListToDataTable_ProvideListOfDataClassWithProperties_TableHasExpectedNumberOfRowsAndColumns()
        {
            var testDataList = GetTestDataClassWithPropertiesList();
            var dataTable = DataTableUtility.ConvertListToDataTable(testDataList);

            Assert.AreEqual(6, dataTable.Rows.Count);
            Assert.AreEqual(4, dataTable.Columns.Count);
        }

        [TestMethod, Owner("Matthew Moore")]
        [ExpectedException(typeof(Exception))]
        public void ConvertListToDataTable_ProvideListOfDataClassWithFieldsOnly_ThrowsException()
        {
            var testDataList = GetTestDataClassWithFieldsList();
            DataTableUtility.ConvertListToDataTable(testDataList);
        }

        [TestMethod, Owner("Matthew Moore")]
        public void ConvertListToDataTable_ProvideListOfDataStructWithSomeProperties_TableHasExpectedNumberOfRowsAndColumns()
        {
            var testDataList = GetTestDataStructList();
            var dataTable = DataTableUtility.ConvertListToDataTable(testDataList);

            Assert.AreEqual(6, dataTable.Rows.Count);
            Assert.AreEqual(2, dataTable.Columns.Count);
        }

        private List<TestDataClassWithProperties> GetTestDataClassWithPropertiesList()
        {
            var testData = new TestDataClassWithProperties
            {
                TestString = "Orange",
                TestDouble = 65.478,
                TestInteger = 574,
                TestBoolean = true
            };

            var testDataList = new List<TestDataClassWithProperties>
            {
                testData,
                testData,
                testData,
                testData,
                testData,
                testData,
            };

            return testDataList;
        }

        private List<TestDataClassWithFields> GetTestDataClassWithFieldsList()
        {
            var testData = new TestDataClassWithFields
            {
                TestString = "Orange",
                TestDouble = 65.478,
                TestInteger = 574,
                TestBoolean = true
            };

            var testDataList = new List<TestDataClassWithFields>
            {
                testData,
                testData,
                testData,
                testData,
                testData,
                testData,
            };

            return testDataList;
        }

        private List<TestDataStruct> GetTestDataStructList()
        {
            var testData = new TestDataStruct
            {
                TestString = "Orange",
                TestDouble = 65.478,
                TestInteger = 574,
                TestBoolean = true
            };

            var testDataList = new List<TestDataStruct>
            {
                testData,
                testData,
                testData,
                testData,
                testData,
                testData,
            };

            return testDataList;
        }
    }
}
