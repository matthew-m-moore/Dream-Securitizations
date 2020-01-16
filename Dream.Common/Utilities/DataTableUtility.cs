using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Dream.Common.Utilities
{
    /// <summary>
    /// A utility class for fluidly converting List and IEnumerable data into a DataTable object
    /// </summary>
    public static class DataTableUtility
    {
        /// <summary>
        /// Converts a list of enumerable of data into a DataTable object for ease of export.
        /// </summary>
        public static DataTable ConvertListToDataTable<T>(IEnumerable<T> data)
        {
            var dataProperties = typeof(T).GetProperties().ToList();

            if (dataProperties.Count == 0)
            {
                throw new Exception(string.Format("ERROR: Object {0} may have fields, but it contains no properties, and thus a list of this object cannot be converted to a data table",
                    typeof(T).Name));
            }

            return TransformListOfDataIntoDataTable(data, dataProperties);
        }

        private static DataTable TransformListOfDataIntoDataTable<T>(IEnumerable<T> data, List<PropertyInfo> dataProperties)
        {
            var dataTable = new DataTable();
            if (dataProperties.Count == 0) return dataTable;

            // Set up data in column headers
            foreach (var propertyInfo in dataProperties)
            {
                // The underlying type might, in fact, be nullable
                var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                dataTable.Columns.Add(propertyInfo.Name, underlyingType ?? propertyInfo.PropertyType);
            }

            // Set populate data row by row
            foreach (var item in data)
            {
                var row = dataTable.NewRow();
                foreach (var propertyInfo in dataProperties)
                {
                    var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    var value = propertyInfo.GetValue(item, null);

                    if (underlyingType != null && value == null)
                    {
                        value = DBNull.Value;
                    }

                    row[propertyInfo.Name] = value;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
