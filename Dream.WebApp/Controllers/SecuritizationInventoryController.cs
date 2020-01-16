using Dream.IO.Database;
using Dream.IO.Database.Entities.Securitization;
using Dream.WebApp.Adapters;
using Dream.WebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Dream.WebApp.Controllers
{
    [RoutePrefix("api/inventory")]
    public class SecuritizationInventoryController : ApiController
    {
        private static List<SecuritizationDataSetModel> _securitizationInventory = new List<SecuritizationDataSetModel>();

        // GET: api/inventory
        [Route("")]
        [HttpGet]
        public IEnumerable<SecuritizationDataSetModel> Get()
        {
            var securitizationDataSetEntities = new List<SecuritizationAnalysisDataSetEntity>();
            var securitizationAnalysisEntities = new List<SecuritizationAnalysisEntity>();
            var securitizationOwnerEntities = new List<SecuritizationAnalysisOwnerEntity>();
            var securitizationCommentEntities = new List<SecuritizationAnalysisCommentEntity>();

            using (var securitizationContext = DatabaseContextRetrieiver.GetSecuritizationEngineContext())
            {
                securitizationDataSetEntities = securitizationContext.SecuritizationAnalysisDataSetEntities.ToList();
                var securitizationDataSetIds = securitizationDataSetEntities.Select(e => e.SecuritizationAnalysisDataSetId).ToList();

                securitizationAnalysisEntities = securitizationContext.SecuritizationAnalysisEntities
                    .Where(e => securitizationDataSetIds.Contains(e.SecuritizationAnalysisDataSetId)).ToList();

                securitizationOwnerEntities = securitizationContext.SecuritizationAnalysisOwnerEntities
                    .Where(e => securitizationDataSetIds.Contains(e.SecuritizationAnalysisDataSetId)).ToList();

                securitizationCommentEntities = securitizationContext.SecuritizationAnalysisCommentEntities
                    .Where(e => securitizationDataSetIds.Contains(e.SecuritizationAnalysisDataSetId)).ToList();
            }

            var securitizationInventoryAdapter = 
                new SecuritizationInventoryAdapter(
                    securitizationDataSetEntities,
                    securitizationAnalysisEntities,
                    securitizationOwnerEntities,
                    securitizationCommentEntities);

            _securitizationInventory = securitizationInventoryAdapter.CreateSecuritizationInventory();

            return _securitizationInventory;
        }
    }
}