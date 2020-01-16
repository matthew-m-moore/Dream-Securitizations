using Dream.ConsoleApp.Interfaces;
using Dream.Core.Repositories.Database;
using Dream.Core.Repositories.Excel;
using System.Collections.Generic;

namespace Dream.ConsoleApp.Scripts.WinApp
{
    public class UpdateFinanceManagementMappings : IScript
    {
        public List<string> GetArgumentsList()
        {
            return new List<string>
            {
                "[1] Valid file path to Excel inputs file",
            };
        }

        public string GetFriendlyName()
        {
            return "Load/Update FP&A Mappings";
        }

        public bool GetVisibilityStatus()
        {
            return true;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];
            var financeManagementExcelDataRepository = new FinanceManagementExcelDataRepository(inputsFilePath);

            var ledgerAccountMappingsDictionary = financeManagementExcelDataRepository.GetLegderAccountMappings();
            var costCenterMappingsDictionary = financeManagementExcelDataRepository.GetCostCenterMappings();
            var productLineMappingsDictionary = financeManagementExcelDataRepository.GetProductLineMappings();
            var regionMappingsDictionary = financeManagementExcelDataRepository.GetRegionMappings();

            var financeManagementDatabaseDataRepository = new FinanceManagementDatabaseRepository();

            if (ledgerAccountMappingsDictionary != null)
            {
                foreach (var keyValuePair in ledgerAccountMappingsDictionary)
                {
                    var aggregationGroupName = keyValuePair.Key;
                    var listOfFinanceManagementMappingRecords = keyValuePair.Value;

                    financeManagementDatabaseDataRepository
                        .LoadOrUpdateLegderAccountAggregationGroups(aggregationGroupName, listOfFinanceManagementMappingRecords);
                }
            }

            if (costCenterMappingsDictionary != null)
            {
                foreach (var keyValuePair in costCenterMappingsDictionary)
                {
                    var aggregationGroupName = keyValuePair.Key;
                    var listOfFinanceManagementMappingRecords = keyValuePair.Value;

                    financeManagementDatabaseDataRepository
                        .LoadOrUpdateCostCenterAggregationGroups(aggregationGroupName, listOfFinanceManagementMappingRecords);
                }
            }

            if (productLineMappingsDictionary != null)
            {
                foreach (var keyValuePair in productLineMappingsDictionary)
                {
                    var aggregationGroupName = keyValuePair.Key;
                    var listOfFinanceManagementMappingRecords = keyValuePair.Value;

                    financeManagementDatabaseDataRepository
                        .LoadOrUpdateProductLineAggregationGroups(aggregationGroupName, listOfFinanceManagementMappingRecords);
                }
            }

            if (regionMappingsDictionary != null)
            {
                foreach (var keyValuePair in regionMappingsDictionary)
                {
                    var aggregationGroupName = keyValuePair.Key;
                    var listOfFinanceManagementMappingRecords = keyValuePair.Value;

                    financeManagementDatabaseDataRepository
                        .LoadOrUpdateRegionAggregationGroups(aggregationGroupName, listOfFinanceManagementMappingRecords);
                }
            }
        }
    }
}
