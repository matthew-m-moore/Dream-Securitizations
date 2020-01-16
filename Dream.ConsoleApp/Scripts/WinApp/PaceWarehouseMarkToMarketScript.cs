using Dream.ConsoleApp.Interfaces;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using System.Collections.Generic;

namespace Dream.ConsoleApp.Scripts.WinApp
{
    public class PaceWarehouseMarkToMarketScript : IScript
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
            return "Warehouse MTM Funding Pricer";
        }

        public bool GetVisibilityStatus()
        {
            return true;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];           
            var warehouseMarkToMarketDataRepository = new WarehouseMarkToMarketExcelDataRepository(inputsFilePath);

            WarehouseMarkToMarketInput warehouseMarkToMarketInput;
            var warehouseMarkToMarketMockSecuritization = warehouseMarkToMarketDataRepository.GetMockPaceSecuritizationForMarkToMarket(out warehouseMarkToMarketInput);
            var warehouseMarkToMarketSecuritizationResult = warehouseMarkToMarketMockSecuritization.RunSecuritizationAnalysis();

            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);

            WarehouseMarkToMarketExcelReport.AddReportTab(
                excelFileWriter.ExcelWorkbook,
                warehouseMarkToMarketMockSecuritization,
                warehouseMarkToMarketSecuritizationResult,
                warehouseMarkToMarketInput);

            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, warehouseMarkToMarketSecuritizationResult);
            excelFileWriter.ExportWorkbook();
        }
    }
}
