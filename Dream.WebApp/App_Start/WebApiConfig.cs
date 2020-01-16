using Dream.IO.Database;
using System.Web.Http;

namespace Dream.WebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(WebDatabaseContextSettings.DatabaseContextConnectionsDictionary);
        }
    }
}
