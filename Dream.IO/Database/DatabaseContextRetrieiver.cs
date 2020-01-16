using Dream.IO.Database.Contexts;

namespace Dream.IO.Database
{
    public static class DatabaseContextRetrieiver
    {
        public static SecuritizationEngineContext GetSecuritizationEngineContext()
        {
            var databaseConnectionString = DatabaseConnectionSettings.CreateDatabaseConnectionString<SecuritizationEngineContext>();
            return new SecuritizationEngineContext(databaseConnectionString);
        }

        public static FinanceManagementContext GetFinanceManagementContext()
        {
            var databaseConnectionString = DatabaseConnectionSettings.CreateDatabaseConnectionString<SecuritizationEngineContext>();
            return new FinanceManagementContext(databaseConnectionString);
        }
    }
}
