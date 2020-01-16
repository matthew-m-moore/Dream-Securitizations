using Dream.IO.Database.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Dream.IO.Database
{
    public static class DatabaseConnectionSettings
    {
        // Note, new contexts can be added to this dictionary as needed
        private static Dictionary<Type, SqlConnectionStringBuilder> _databaseContextConnectionsDictionary;

        public static void SetDatabaseContextConnectionsDictionary(Dictionary<Type, SqlConnectionStringBuilder> databaseContextConnectionsDictionary)
        {
            _databaseContextConnectionsDictionary = databaseContextConnectionsDictionary;
        }

        public static string CreateDatabaseConnectionString<T>() 
            where T : DbContext
        {
            if (_databaseContextConnectionsDictionary == null) ThrowContextSettingsNotSetException();

            var databaseConnectionStringBuilder = _databaseContextConnectionsDictionary[typeof(T)];
            var databaseConnectionString = databaseConnectionStringBuilder.ConnectionString;

            return databaseConnectionString;
        }

        public static bool UpdateDatabaseConnection<T>(string updatedServerName, string updatedDatabaseName) where T : DbContext
        {
            if (_databaseContextConnectionsDictionary == null) ThrowContextSettingsNotSetException();

            var databaseConnectionStringBuilder = _databaseContextConnectionsDictionary[typeof(T)];
            var currentServerName = databaseConnectionStringBuilder.DataSource;
            var currentDatabaseName = databaseConnectionStringBuilder.InitialCatalog;

            if (!string.IsNullOrEmpty(updatedServerName) && !string.IsNullOrEmpty(updatedDatabaseName))
            {
                databaseConnectionStringBuilder.DataSource = updatedServerName;
                databaseConnectionStringBuilder.InitialCatalog = updatedDatabaseName;
            }

            var isUpdatedDatabaseConnectionSuccessful = TryCheckIfDatabaseConnectionIsValid<T>(databaseConnectionStringBuilder.ConnectionString);
            if (isUpdatedDatabaseConnectionSuccessful) return true;

            // Revert to current settings if the updating connection is not successful
            UpdateDatabaseConnection<T>(currentServerName, currentDatabaseName);
            return false;
        }

        public static bool TryCheckIfDatabaseConnectionIsValid<T>(string databaseConnectionString) where T : DbContext
        {
            try
            {
                var dbContextType = typeof(T);
                var dbContext = Activator.CreateInstance(dbContextType, new object[] { databaseConnectionString }) as T;
                if (dbContext == null) return false;

                using (var testDbContext = dbContext)
                {
                    return testDbContext.Database.Exists();
                }
            }
            catch
            {
                return false;
            }
        }

        private static void ThrowContextSettingsNotSetException()
        {
            throw new Exception("INTERNAL ERROR: Database context settings were not provided at application start-up. Please report this error.");
        }
    }
}
