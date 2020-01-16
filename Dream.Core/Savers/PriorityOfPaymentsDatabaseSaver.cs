using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Dream.Core.Savers
{
    public class PriorityOfPaymentsDatabaseSaver : DatabaseSaver
    {
        private PriorityOfPayments _priorityOfPayments;     
        private Dictionary<string, int> _trancheDetailIdsDictionary;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public PriorityOfPaymentsDatabaseSaver(
            PriorityOfPayments priorityOfPayments,
            Dictionary<string, int> trancheDetailIdsDictionary,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository,
            DateTime cutOffDate,
            string description)
        : base(cutOffDate, description)
        {
            _priorityOfPayments = priorityOfPayments;
            _trancheDetailIdsDictionary = trancheDetailIdsDictionary;
            _typesAndConventionsDatabaseRepository = typesAndConventionsDatabaseRepository;
        }

        public int SavePriorityOfPayments()
        {
            var priorityOfPaymentsSetEntity = new PriorityOfPaymentsSetEntity
            {
                CutOffDate = _CutOffDate,
                PriorityOfPaymentsSetDescription = _Description
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.PriorityOfPaymentsSetEntities.Add(priorityOfPaymentsSetEntity);
                securitizationEngineContext.SaveChanges();
            }

            SavePriorityOfPaymentsAssignments(priorityOfPaymentsSetEntity.PriorityOfPaymentsSetId);

            return priorityOfPaymentsSetEntity.PriorityOfPaymentsSetId;
        }

        private void SavePriorityOfPaymentsAssignments(int priorityOfPaymentsSetId)
        {
            var listOfPriorityOfPaymentsAssignmentEntities = new List<PriorityOfPaymentsAssignmentEntity>();
            foreach (var priorityOfPaymentsEntry in _priorityOfPayments.OrderedListOfEntries)
            {
                if (!_trancheDetailIdsDictionary.ContainsKey(priorityOfPaymentsEntry.TrancheName)) continue;

                var priorityOfPaymentsAssignmentEntity = new PriorityOfPaymentsAssignmentEntity
                {
                    PriorityOfPaymentsSetId = priorityOfPaymentsSetId,
                    SeniorityRanking = priorityOfPaymentsEntry.SeniorityRanking,
                    TrancheCashFlowTypeId = _typesAndConventionsDatabaseRepository.TrancheCashFlowTypesReversed[priorityOfPaymentsEntry.TrancheCashFlowType],
                    TrancheDetailId = _trancheDetailIdsDictionary[priorityOfPaymentsEntry.TrancheName]
                };

                listOfPriorityOfPaymentsAssignmentEntities.Add(priorityOfPaymentsAssignmentEntity);
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.PriorityOfPaymentsAssignmentEntities.AddRange(listOfPriorityOfPaymentsAssignmentEntities);
                securitizationEngineContext.SaveChanges();
            }
        }
    }
}
