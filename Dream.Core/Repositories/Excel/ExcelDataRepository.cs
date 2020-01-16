using Dream.IO.Excel;
using System;
using System.IO;

namespace Dream.Core.Repositories.Excel
{
    public abstract class ExcelDataRepository
    {
        protected ExcelFileReader _ExcelFileReader;
        protected DateTime _CutOffDate;

        public ExcelDataRepository(string pathToExcelFile)
        {
            _ExcelFileReader = new ExcelFileReader(pathToExcelFile);
        }

        public ExcelDataRepository(Stream fileStream)
        {
            _ExcelFileReader = new ExcelFileReader(fileStream);
        }

        public ExcelDataRepository(ExcelFileReader excelFileReader)
        {
            _ExcelFileReader = excelFileReader;
        }

        public ExcelDataRepository(ExcelFileReader excelFileReader, DateTime cutOffDate)
        {
            _ExcelFileReader = excelFileReader;
            _CutOffDate = cutOffDate;
        }

        public ExcelDataRepository(string pathToExcelFile, DateTime cutOffDate)
        {
            _ExcelFileReader = new ExcelFileReader(pathToExcelFile);
            _CutOffDate = cutOffDate;
        }

        public void Dispose()
        {
            _ExcelFileReader.Dispose();
        }
    }
}
