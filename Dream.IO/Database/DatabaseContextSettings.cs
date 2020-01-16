using Dream.IO.Database.Contexts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dream.IO.Database
{
    public static class DatabaseContextSettings
    {
        private const int _databaseConnectionTimeoutInMilliseconds = 300;

        // Note, new contexts can be added to this dictionary as needed
        public static Dictionary<Type, SqlConnectionStringBuilder> DatabaseContextConnectionsDictionary =
            new Dictionary<Type, SqlConnectionStringBuilder>
            {
                [typeof(SecuritizationEngineContext)] = new SqlConnectionStringBuilder
                {

                    IntegratedSecurity = true,
                    MultipleActiveResultSets = true,
                    ConnectTimeout = _databaseConnectionTimeoutInMilliseconds,

                    DataSource = DatabaseConnectionCache.SecuritizationEngineServerName,
                    InitialCatalog = DatabaseConnectionCache.SecuritizationEngineDatabaseName
                },

                [typeof(FinanceManagementContext)] = new SqlConnectionStringBuilder
                {

                    IntegratedSecurity = true,
                    MultipleActiveResultSets = true,
                    ConnectTimeout = _databaseConnectionTimeoutInMilliseconds,

                    DataSource = DatabaseConnectionCache.FinanceManagementServerName,
                    InitialCatalog = DatabaseConnectionCache.FinanceManagementDatabaseName
                },
            };
    }
}
