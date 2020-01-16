using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.Converters.Excel;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities;
using System;
using System.IO;

namespace Dream.Core.Repositories.Excel
{
    public class MarketRateEnvironmentExcelDataRepository : ExcelDataRepository
    {
        private const string _marketData = "MarketData";

        private MarketRateEnvironment _marketRateEnvironment;

        public MarketRateEnvironmentExcelDataRepository(ExcelFileReader excelFileReader) : base(excelFileReader)
        {
            _marketRateEnvironment = new MarketRateEnvironment();
        }

        public MarketRateEnvironmentExcelDataRepository(string pathToExcelFile) : base(pathToExcelFile)
        {
            _marketRateEnvironment = new MarketRateEnvironment();
        }

        public MarketRateEnvironmentExcelDataRepository(Stream fileStream) : base(fileStream)
        {
            _marketRateEnvironment = new MarketRateEnvironment();
        }

        /// <summary>
        /// Retrieves a MarketRateEnvironment object that only has market data points, no rate curves or discount factors.
        /// </summary>
        public MarketRateEnvironment GetMarketRateEnvironmentWithoutRateCurves(DateTime marketDate)
        {
            var marketDataRecords = _ExcelFileReader.GetDataFromSpecificTab<MarketDataRecord>(_marketData);
            var marketDataPoints = MarketRateEnvironmentExcelConverter.ConvertListOfMarketDataRecords(marketDataRecords, marketDate);

            _marketRateEnvironment.AddMarketDataPoints(marketDataPoints);
            return _marketRateEnvironment;
        }
    }
}
