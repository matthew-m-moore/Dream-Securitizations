using Dream.IO.Database.Contexts;

namespace Dream.IO.Database.Interfaces
{
    public interface IDatabaseContextRetriever
    {
        SecuritizationEngineContext GetSecuritizationEngineContext();
        FinanceManagementContext GetFinanceManagementContext();
    }
}
