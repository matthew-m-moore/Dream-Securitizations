using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.Repositories.Database;
using Dream.WebApp.Adapters;
using Dream.WebApp.Models;
using System.Web.Http;

namespace Dream.WebApp.Controllers
{
    [RoutePrefix("api/securitization")]
    public class SecuritizationModelController : ApiController
    {
        // GET: api/securitization/load/{dataSetId}/{versionId}/{isResecuritization}
        [Route("load/{dataSetId}/{versionId}/{isResecuritization}")]
        [HttpGet]
        public SecuritizationModel Get(int dataSetId, int versionId, string isResecuritization)
        {
            var securitizationDataRepository = bool.Parse(isResecuritization)
                ? new ResecuritizationDatabaseRepository(dataSetId, versionId)
                : new SecuritizationDatabaseRepository(dataSetId, versionId);

            var securitization = securitizationDataRepository.GetPaceSecuritization();
            var securitizationScenarios = securitizationDataRepository.ScenariosToAnalyze;

            var securitizationModelAdapter = new SecuritizationModelAdapter(securitization, securitizationScenarios);
            var securitizationModel = securitizationModelAdapter.CreateSecuritizationModel();

            return securitizationModel;
        }
    }
}