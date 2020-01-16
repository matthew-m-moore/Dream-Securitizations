using Dream.IO.Database;

namespace Dream.IO
{
    public static class DatabaseConnectionCache
    {
        private static string _securitizationEngineServerName;
        public static string SecuritizationEngineServerName
        {
            get
            {
                if (_securitizationEngineServerName == null)
                {
                    _securitizationEngineServerName = DatabaseConnection.Default.SecuritizationEngineServerName;
                }

                return _securitizationEngineServerName;
            }
            set
            {
                _securitizationEngineServerName = value;
            }
        }

        private static string _securitizationEngineDatabaseName;
        public static string SecuritizationEngineDatabaseName
        {
            get
            {
                if (_securitizationEngineDatabaseName == null)
                {
                    _securitizationEngineDatabaseName = DatabaseConnection.Default.SecuritizationEngineDatabaseName;
                }

                return _securitizationEngineDatabaseName;
            }
            set
            {
                _securitizationEngineDatabaseName = value;
            }
        }

        private static string _financeManagementServerName;
        public static string FinanceManagementServerName
        {
            get
            {
                if (_financeManagementServerName == null)
                {
                    _financeManagementServerName = DatabaseConnection.Default.FinanceManagementServerName;
                }

                return _financeManagementServerName;
            }
            set
            {
                _financeManagementServerName = value;
            }
        }

        private static string _financeManagementDatabaseName;
        public static string FinanceManagementDatabaseName
        {
            get
            {
                if (_financeManagementDatabaseName == null)
                {
                    _financeManagementDatabaseName = DatabaseConnection.Default.FinanceManagementDatabaseName;
                }

                return _financeManagementDatabaseName;
            }
            set
            {
                _financeManagementDatabaseName = value;
            }
        }
    }
}
