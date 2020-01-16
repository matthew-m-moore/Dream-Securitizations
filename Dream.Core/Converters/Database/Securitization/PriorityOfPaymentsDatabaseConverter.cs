using System.Collections.Generic;
using Dream.IO.Database.Entities.Securitization;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Common.Enums;

namespace Dream.Core.Converters.Database.Securitization
{
    public class PriorityOfPaymentsDatabaseConverter
    {
        public static PriorityOfPayments ConvertToPriorityOfPayments(
            List<PriorityOfPaymentsAssignmentEntity> priorityOfPaymentsAssignmentEntities, 
            Dictionary<int, TrancheCashFlowType> trancheCashFlowTypesDictionary, 
            Dictionary<int, TrancheDetailEntity> trancheDetailsDictionary)
        {
            var priorityOfPaymentsEntries = new List<PriorityOfPaymentsEntry>();
            foreach(var priorityOfPaymentsAssignmentEntity in priorityOfPaymentsAssignmentEntities)
            {
                var priorityOfPaymentsEntry = ConvertPriorityOfPaymentsAssignmentEntity(
                    priorityOfPaymentsAssignmentEntity,
                    trancheCashFlowTypesDictionary,
                    trancheDetailsDictionary);

                priorityOfPaymentsEntries.Add(priorityOfPaymentsEntry);
            }

            var priorityOfPayments = new PriorityOfPayments(priorityOfPaymentsEntries);
            return priorityOfPayments;
        }

        private static PriorityOfPaymentsEntry ConvertPriorityOfPaymentsAssignmentEntity(
            PriorityOfPaymentsAssignmentEntity priorityOfPaymentsAssignmentEntity,
            Dictionary<int, TrancheCashFlowType> trancheCashFlowTypesDictionary,
            Dictionary<int, TrancheDetailEntity> trancheDetailsDictionary)
        {
            var trancheCashFlowType = trancheCashFlowTypesDictionary[priorityOfPaymentsAssignmentEntity.TrancheCashFlowTypeId];
            var trancheDetailEntity = trancheDetailsDictionary[priorityOfPaymentsAssignmentEntity.TrancheDetailId];

            var priorityOfPaymentsEntry = new PriorityOfPaymentsEntry(
                priorityOfPaymentsAssignmentEntity.SeniorityRanking,
                trancheDetailEntity.TrancheName,
                trancheCashFlowType);

            return priorityOfPaymentsEntry;
        }
    }
}
