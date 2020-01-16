using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Savers
{
    public class RedemptionLogicDatabaseSaver : DatabaseSaver
    {
        private List<RedemptionLogic> _redemptionLogicList;
        private Dictionary<string, int> _trancheDetailIdsDictionary;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private Dictionary<string, int> _redemptionLogicTypes;
        public Dictionary<string, int> RedemptionLogicTypes
        {
            get
            {
                if (_redemptionLogicTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        _redemptionLogicTypes = securitizationEngineContext.RedemptionLogicTypeEntities
                            .ToDictionary(e => e.RedemptionLogicTypeDescription,
                                          e => e.RedemptionLogicTypeId);
                    }
                }

                return _redemptionLogicTypes;
            }
        }

        public RedemptionLogicDatabaseSaver(
            List<RedemptionLogic> redemptionLogicList,
            Dictionary<string, int> trancheDetailIdsDictionary,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository,
            DateTime cutOffDate,
            string description)
        : base(cutOffDate, description)
        {
            _redemptionLogicList = redemptionLogicList;
            _trancheDetailIdsDictionary = trancheDetailIdsDictionary;
            _typesAndConventionsDatabaseRepository = typesAndConventionsDatabaseRepository;
        }

        public List<int> SaveRedemptionLogic()
        {
            var redemptionLogicDataSetIds = new List<int>();
            foreach (var redemptionLogic in _redemptionLogicList.Where(r => !r.TreatAsCleanUpCall))
            {
                var redemptionLogicTypeDescription = RedemptionLogicTypeDatabaseConverter
                    .ConvertTypeToDescription(redemptionLogic.GetType());

                var redemptionLogicAllowedMonthsSetId = SaveRedemptionLogicAllowedMonthsSet(redemptionLogic);
                var redemptionTranchesSetId = SaveRedemptionTranchesSet(redemptionLogic);
                var redemptionPriorityOfPaymentsSetId = SaveRedemptionPriorityOfPayments(redemptionLogic.PriorityOfPayments);
                var postRedemptionPriorityOfPaymentsSetId = SaveRedemptionPriorityOfPayments(redemptionLogic.PostRedemptionPriorityOfPayments);

                var redemptionLogicDataSetEntity = new RedemptionLogicDataSetEntity
                {
                    CutOffDate = _CutOffDate,
                    RedemptionLogicTypeId = RedemptionLogicTypes[redemptionLogicTypeDescription],
                    RedemptionLogicAllowedMonthsSetId = redemptionLogicAllowedMonthsSetId,
                    RedemptionTranchesSetId = redemptionTranchesSetId,
                    RedemptionPriorityOfPaymentsSetId = redemptionPriorityOfPaymentsSetId,
                    PostRedemptionPriorityOfPaymentsSetId = postRedemptionPriorityOfPaymentsSetId,
                    RedemptionLogicTriggerValue = redemptionLogic.RedemptionTriggeredThreshold,
                };

                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.RedemptionLogicDataSetEntities.Add(redemptionLogicDataSetEntity);
                    securitizationEngineContext.SaveChanges();
                }

                redemptionLogicDataSetIds.Add(redemptionLogicDataSetEntity.RedemptionLogicDataSetId);
            }

            return redemptionLogicDataSetIds;
        }

        private int? SaveRedemptionLogicAllowedMonthsSet(RedemptionLogic redemptionLogic)
        {
            if (!redemptionLogic.ListOfAllowedMonthsForRedemption.Any()) return null;

            var redemptionLogicAllowedMonthsSetEntity = new RedemptionLogicAllowedMonthsSetEntity();
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.RedemptionLogicAllowedMonthsSetEntities.Add(redemptionLogicAllowedMonthsSetEntity);
                securitizationEngineContext.SaveChanges();
            }

            var listOfRedemptionLogicAllowedMonthsDetailEntities = new List<RedemptionLogicAllowedMonthsDetailEntity>();
            foreach (var allowedMonth in redemptionLogic.ListOfAllowedMonthsForRedemption)
            {
                var redemptionLogicAllowedMonthsDetailEntity = new RedemptionLogicAllowedMonthsDetailEntity
                {
                    RedemptionLogicAllowedMonthsSetId = redemptionLogicAllowedMonthsSetEntity.RedemptionLogicAllowedMonthsSetId,
                    AllowedMonthId = _typesAndConventionsDatabaseRepository.AllowedMonthsReversed[allowedMonth],
                };

                listOfRedemptionLogicAllowedMonthsDetailEntities.Add(redemptionLogicAllowedMonthsDetailEntity);
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.RedemptionLogicAllowedMonthsDetailEntities.AddRange(listOfRedemptionLogicAllowedMonthsDetailEntities);
                securitizationEngineContext.SaveChanges();
            }

            return redemptionLogicAllowedMonthsSetEntity.RedemptionLogicAllowedMonthsSetId;
        }

        private int? SaveRedemptionTranchesSet(RedemptionLogic redemptionLogic)
        {
            if (redemptionLogic is TranchesCanBePaidOutFromAvailableFundsRedemptionLogic tranchesCanBePaidOutFromAvailableFundsRedemptionLogic)
            {
                var redemptionTranchesSetEntity = new RedemptionTranchesSetEntity();
                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.RedemptionTranchesSetEntities.Add(redemptionTranchesSetEntity);
                    securitizationEngineContext.SaveChanges();
                }

                var listOfRedemptionTranchesDetailEntities = new List<RedemptionTranchesDetailEntity>();
                foreach (var trancheToBePaidOut in tranchesCanBePaidOutFromAvailableFundsRedemptionLogic.ListOfTranchesToBePaidOut)
                {
                    var redemptionTranchesDetailEntity = new RedemptionTranchesDetailEntity
                    {
                        RedemptionTranchesSetId = redemptionTranchesSetEntity.RedemptionTranchesSetId,
                        TrancheDetailId = _trancheDetailIdsDictionary[trancheToBePaidOut.TrancheName],
                    };

                    listOfRedemptionTranchesDetailEntities.Add(redemptionTranchesDetailEntity);
                }

                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.RedemptionTranchesDetailEntities.AddRange(listOfRedemptionTranchesDetailEntities);
                    securitizationEngineContext.SaveChanges();
                }

                return redemptionTranchesSetEntity.RedemptionTranchesSetId;
            }

            return null;
        }

        private int? SaveRedemptionPriorityOfPayments(PriorityOfPayments redemptionPriorityOfPayments)
        {
            var priorityOfPaymentsDatabaseSaver = new PriorityOfPaymentsDatabaseSaver(
                redemptionPriorityOfPayments,
                _trancheDetailIdsDictionary,
                _typesAndConventionsDatabaseRepository,
                _CutOffDate,
                _Description);

            var priorityOfPaymentsSetId = priorityOfPaymentsDatabaseSaver.SavePriorityOfPayments();
            return priorityOfPaymentsSetId;
        }
    }
}
