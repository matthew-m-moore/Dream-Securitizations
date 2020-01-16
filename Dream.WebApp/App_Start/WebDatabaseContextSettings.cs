using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;
using Dream.IO.Database.Contexts;

namespace Dream.WebApp
{
    public class WebDatabaseContextSettings
    {
        private const int _databaseConnectionTimeoutInMilliseconds = 300;

        private static string _securitizationEngineServerName = WebConfigurationManager.AppSettings["SecuritizationEngineServerName"];
        private static string _securitizationEngineDatabaseName = WebConfigurationManager.AppSettings["SecuritizationEngineDatabaseName"];

        private static string _financeManagementServerName = WebConfigurationManager.AppSettings["FinanceManagementServerName"];
        private static string _financeManagementDatabaseName = WebConfigurationManager.AppSettings["FinanceManagementDatabaseName"];

        // Note, new contexts can be added to this dictionary as needed
        public static Dictionary<Type, SqlConnectionStringBuilder> DatabaseContextConnectionsDictionary =
            new Dictionary<Type, SqlConnectionStringBuilder>
            {
                [typeof(SecuritizationEngineContext)] = new SqlConnectionStringBuilder
                {

                    IntegratedSecurity = true,
                    MultipleActiveResultSets = true,
                    ConnectTimeout = _databaseConnectionTimeoutInMilliseconds,

                    DataSource = _securitizationEngineServerName,
                    InitialCatalog = _securitizationEngineDatabaseName
                },

                [typeof(FinanceManagementContext)] = new SqlConnectionStringBuilder
                {

                    IntegratedSecurity = true,
                    MultipleActiveResultSets = true,
                    ConnectTimeout = _databaseConnectionTimeoutInMilliseconds,

                    DataSource = _financeManagementServerName,
                    InitialCatalog = _financeManagementDatabaseName
                },
            };
    }
}