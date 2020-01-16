using ClosedXML.Excel;
using Dream.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Dream.IO.Excel
{
    public class ExcelFileWriter
    {
        private string _filePath;
        
        private const string _tempFileExtension = ".tmp";
        private const string _excelFileExtension = ".xlsx";

        private bool _openFileOnSave = false;
        private bool _overwriteFileOnSave = false;

        public XLWorkbook ExcelWorkbook { get; set; }

        public ExcelFileWriter()
        {
            _filePath = null;
            ExcelWorkbook = new XLWorkbook();
        }

        public ExcelFileWriter(bool openFileOnSave) : this()
        {
            _openFileOnSave = openFileOnSave;
        }

        public ExcelFileWriter(string filePath) : this(new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            _filePath = filePath;
        }

        public ExcelFileWriter(Stream fileStream)
        {
            ExcelWorkbook = new XLWorkbook(fileStream);
        }

        public ExcelFileWriter(string filePath, bool openFileOnSave, bool overwriteFileOnSave) : this(filePath)
        {
            _openFileOnSave = openFileOnSave;
            _overwriteFileOnSave = overwriteFileOnSave;
        }

        /// <summary>
        /// Exports the workbook internal to the ExcelFileWriter object, with whatever tabs have been populated.
        /// </summary>
        public void ExportWorkbook(string filePathOnSave = null, bool openFileOnSave = false, bool overwriteFileOnSave = false)
        {
            // Take the provided file path as preferred
            filePathOnSave = filePathOnSave ?? _filePath;

            // If the private variables were set to true, take them as preferred
            openFileOnSave = openFileOnSave || _openFileOnSave;
            overwriteFileOnSave = overwriteFileOnSave || _overwriteFileOnSave;

            // If the file path is left as null, save as temp file and open it
            if (filePathOnSave == null)
            {
                SaveWorkbookAsTempFile(openFileOnSave);
            }
            else
            {
                SaveWorkbook(filePathOnSave, openFileOnSave, overwriteFileOnSave);
            }
        }

        /// <summary>
        /// Adds a tab containing of a generic list of data to the internal workbook of the ExcelFileWriter.
        /// </summary>
        public void AddWorksheetForListOfData<T>(List<T> genericListOfData, string worksheetName = "ListOfData")
        {
            var dataTable = DataTableUtility.ConvertListToDataTable(genericListOfData);
            AddWorksheetForDataTable(dataTable, worksheetName);
        }

        /// <summary>
        /// Adds a tab containing of a generic data table to the internal workbook of the ExcelFileWriter.
        /// </summary>
        public void AddWorksheetForDataTable(DataTable dataTable, string worksheetName = "DataTable")
        {
            ExcelWorkbook.Worksheets.Add(dataTable, worksheetName);
        }

        /// <summary>
        /// Clears all the tabs (by deleting them) in the internal workbook of the ExcelFileWriter.
        /// </summary>
        public void ClearWorkbook()
        {
            var excelWorksheets = ExcelWorkbook.Worksheets.ToList();
            foreach (var excelWorksheet in excelWorksheets)
            {
                var tabName = excelWorksheet.Name;
                ExcelWorkbook.Worksheets.Delete(tabName);
            }           
        }

        // Note that saving a temp file will never overwrite an exiting file since each temp file name is unique.
        private void SaveWorkbookAsTempFile(bool openFileOnSave)
        {
            var tempDirectoryFilePath = Path.GetTempFileName().Replace(_tempFileExtension, _excelFileExtension);
            SaveWorkbook(tempDirectoryFilePath, openFileOnSave, false);
        }

        private void SaveWorkbook(string filePathOnSave, bool openFileOnSave, bool overwriteFileOnSave)
        {
            // Add a proper file extension if none exists
            if (!filePathOnSave.EndsWith(_excelFileExtension))
            {
                filePathOnSave += _excelFileExtension;
            }

            // The default behavior of ClosedXML is to overwrite on save
            if (!overwriteFileOnSave && File.Exists(filePathOnSave))
            {
                throw new Exception("ERROR: Cannot write over existing file. ExcelFileWriter overwrite option was set to false");
            }

            Console.WriteLine(string.Format("Saving File at '{0}'...", filePathOnSave));
            ExcelWorkbook.SaveAs(filePathOnSave);
            Console.WriteLine("File Saved.");

            if (openFileOnSave)
            {
                Console.WriteLine("Trying to Open File in Excel...");
                Process.Start(filePathOnSave);
                Console.WriteLine("Excel Opened.");
            }
        }
    }
}
