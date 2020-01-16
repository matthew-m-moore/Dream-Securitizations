using Dream.Core.Converters.Excel;
using Dream.IO.Excel.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dream.Core.Repositories.Excel
{
    public class FinanceManagementExcelDataRepository : ExcelDataRepository
    {
        private const string _ledgerAccountMapping = "AccountMapping";
        private const string _costCenterMapping = "CostCenterMapping";
        private const string _productLineMapping = "ProductLineMapping";
        private const string _regionMapping = "RegionMapping";

        public FinanceManagementExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile) { }

        public FinanceManagementExcelDataRepository(Stream fileStream) : base(fileStream) { }

        public Dictionary<string, List<FinanceManagementMappingRecord>> GetLegderAccountMappings()
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_ledgerAccountMapping);
            if (!excelDataRows.Any()) return null;

            var ledgerAccountMappingsDictionary = 
                FinanceManagementMappingRecordExcelConverter.ConvertExcelRowsToFinanceManagementMappingRecordsDictionary(excelDataRows);

            return ledgerAccountMappingsDictionary;
        }

        public Dictionary<string, List<FinanceManagementMappingRecord>> GetCostCenterMappings()
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_costCenterMapping);
            if (!excelDataRows.Any()) return null;

            var costCenterMappingsDictionary =
                FinanceManagementMappingRecordExcelConverter.ConvertExcelRowsToFinanceManagementMappingRecordsDictionary(excelDataRows);

            return costCenterMappingsDictionary;
        }

        public Dictionary<string, List<FinanceManagementMappingRecord>> GetProductLineMappings()
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_productLineMapping);
            if (!excelDataRows.Any()) return null;

            var productLineMappingsDictionary =
                FinanceManagementMappingRecordExcelConverter.ConvertExcelRowsToFinanceManagementMappingRecordsDictionary(excelDataRows);

            return productLineMappingsDictionary;
        }

        public Dictionary<string, List<FinanceManagementMappingRecord>> GetRegionMappings()
        {
            var excelDataRows = _ExcelFileReader.GetExcelDataRowsFromWorksheet(_regionMapping);
            if (!excelDataRows.Any()) return null;

            var regionMappingsDictionary =
                FinanceManagementMappingRecordExcelConverter.ConvertExcelRowsToFinanceManagementMappingRecordsDictionary(excelDataRows);

            return regionMappingsDictionary;
        }
    }
}
