using Dream.Common.Enums;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Converters.Database;
using Dream.Core.Converters.Database.Securitization;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Repositories.Database
{
    public class RedemptionLogicDatabaseRepository : DatabaseRepository
    {
        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private List<RedemptionLogicDataSetEntity> _redemptionLogicDataSetEntities;

        private PriorityOfPayments _basePriorityOfPayments;
        private Dictionary<int, Tranche> _securitizationTranchesDictionary;
        private Dictionary<int, TrancheCashFlowType> _trancheCashFlowTypes;
        private Dictionary<int, TrancheDetailEntity> _trancheDetails;

        private Dictionary<int, (Type Type, string Description)> _redemptionLogicTypes;
        public Dictionary<int, (Type Type, string Description)> RedemptionLogicTypes
        {
            get
            {
                if (_redemptionLogicTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var redemptionLogicTypeEntities = securitizationEngineContext.RedemptionLogicTypeEntities.ToList();
                        _redemptionLogicTypes = redemptionLogicTypeEntities.ToDictionary(
                            e => e.RedemptionLogicTypeId,
                            e => (Type: RedemptionLogicTypeDatabaseConverter.ConvertString(e.RedemptionLogicTypeDescription),
                                  Description: e.RedemptionLogicTypeDescription));
                    }
                }

                return _redemptionLogicTypes;
            }
        }

        private Dictionary<int, (string Abbreviation, string FullName)> _allowedMonths;
        public Dictionary<int, (string Abbreviation, string FullName)> AllowedMonths
        {
            get
            {
                if (_allowedMonths == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var allowedMonthEntities = securitizationEngineContext.AllowedMonthEntities.ToList();
                        _allowedMonths = allowedMonthEntities.ToDictionary(
                            e => e.AllowedMonthId,
                            e => (Abbreviation: e.AllowedMonthShortDescription, FullName: e.AllowedMonthLongDescription));
                    }
                }

                return _allowedMonths;
            }
        }

        private Dictionary<int, List<Month>> _redemptionAllowedMonths;
        public Dictionary<int, List<Month>> RedemptionAllowedMonths
        {
            get
            {
                if (_redemptionAllowedMonths == null)
                {
                    var redemptionAllowedMonthSetIds = _redemptionLogicDataSetEntities.Select(e => e.RedemptionLogicAllowedMonthsSetId)
                        .Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var redemptionLogicAllowedMonthsDetailEntities = securitizationEngineContext.RedemptionLogicAllowedMonthsDetailEntities
                            .Where(e => redemptionAllowedMonthSetIds.Contains(e.RedemptionLogicAllowedMonthsSetId)).ToList();

                        _redemptionAllowedMonths = redemptionAllowedMonthSetIds.ToDictionary(
                            allowedMonthDataSetId => allowedMonthDataSetId,
                            allowedMonthDataSetId => redemptionLogicAllowedMonthsDetailEntities
                                                        .Where(e => e.RedemptionLogicAllowedMonthsSetId == allowedMonthDataSetId)
                                                        .Select(m => MonthDatabaseConverter.ConvertString(AllowedMonths[m.AllowedMonthId].FullName)).ToList());
                    }
                }

                return _redemptionAllowedMonths;
            }
        }

        private Dictionary<int, List<int>> _redemptionTrancheDetailIds;
        public Dictionary<int, List<int>> RedemptionTrancheDetailIds
        {
            get
            {
                if (_redemptionTrancheDetailIds == null)
                {
                    var redemptionTrancheDataSetIds = _redemptionLogicDataSetEntities.Select(e => e.RedemptionTranchesSetId)
                        .Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var redemptionTranchesDetailEntities = securitizationEngineContext.RedemptionTranchesDetailEntities
                            .Where(e => redemptionTrancheDataSetIds.Contains(e.RedemptionTranchesSetId)).ToList();

                        _redemptionTrancheDetailIds = redemptionTrancheDataSetIds.ToDictionary(
                            tranchesDataSetId => tranchesDataSetId,
                            tranchesDataSetId => redemptionTranchesDetailEntities
                                                    .Where(e => e.RedemptionTranchesSetId == tranchesDataSetId)
                                                    .Select(d => d.TrancheDetailId).ToList());
                    }
                }

                return _redemptionTrancheDetailIds;
            }
        }

        private Dictionary<int, List<PriorityOfPaymentsAssignmentEntity>> _priorityOfPaymentsSets;
        public Dictionary<int, List<PriorityOfPaymentsAssignmentEntity>> PriorityOfPaymentsSets
        {
            get
            {
                if (_priorityOfPaymentsSets == null)
                {
                    var redemptionPriorityOfPaymentsSetIds = _redemptionLogicDataSetEntities.Select(e => e.RedemptionPriorityOfPaymentsSetId)
                        .Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();

                    var postRedemptionPriorityOfPaymentsSetIds = _redemptionLogicDataSetEntities.Select(e => e.PostRedemptionPriorityOfPaymentsSetId)
                        .Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();

                    redemptionPriorityOfPaymentsSetIds.AddRange(postRedemptionPriorityOfPaymentsSetIds);

                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var priorityOfPaymentsAssignmentEntities = securitizationEngineContext.PriorityOfPaymentsAssignmentEntities
                            .Where(e => redemptionPriorityOfPaymentsSetIds.Contains(e.PriorityOfPaymentsSetId)).ToList();

                        _priorityOfPaymentsSets = redemptionPriorityOfPaymentsSetIds.ToDictionary(
                            priorityOfPaymentsDataSetId => priorityOfPaymentsDataSetId,
                            priorityOfPaymentsDataSetId => priorityOfPaymentsAssignmentEntities.Where(e => e.PriorityOfPaymentsSetId == priorityOfPaymentsDataSetId).ToList());
                    }
                }

                return _priorityOfPaymentsSets;
            }
        }

        public RedemptionLogicDatabaseRepository(
            List<int> redemptionLogicDataSetIds, 
            PriorityOfPayments basePriorityOfPayments,
            Dictionary<int, Tranche> securitizationTranchesDictionary,
            Dictionary<int, TrancheCashFlowType> trancheCashFlowTypes,
            Dictionary<int, TrancheDetailEntity> trancheDetails)
        {
            _basePriorityOfPayments = basePriorityOfPayments;
            _securitizationTranchesDictionary = securitizationTranchesDictionary;
            _trancheCashFlowTypes = trancheCashFlowTypes;
            _trancheDetails = trancheDetails;

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                _redemptionLogicDataSetEntities = securitizationEngineContext.RedemptionLogicDataSetEntities
                    .Where(e => redemptionLogicDataSetIds.Contains(e.RedemptionLogicDataSetId)).ToList();
            }
        }

        public List<RedemptionLogic> PrepareListOfRedemptionLogic()
        {
            var listOfRedemptionLogic = new List<RedemptionLogic>();
            foreach (var redemptionLogicDataSetEntity in _redemptionLogicDataSetEntities)
            {
                var redemptionLogicTypeInformation = RedemptionLogicTypes[redemptionLogicDataSetEntity.RedemptionLogicTypeId];
                var redemptionLogicType = redemptionLogicTypeInformation.Type;

                RedemptionLogic redemptionLogic = null;
                if (redemptionLogicType == typeof(TranchesCanBePaidOutFromAvailableFundsRedemptionLogic))
                    redemptionLogic = CreateTranchesCanBePaidOutFromAvailableFundsRedemptionLogic(redemptionLogicDataSetEntity);

                else if (redemptionLogicType == typeof(LessThanPercentOfInitalCollateralBalanceRedemptionLogic))
                    redemptionLogic = new LessThanPercentOfInitalCollateralBalanceRedemptionLogic(redemptionLogicDataSetEntity.RedemptionLogicTriggerValue.Value);

                else if (redemptionLogicType == typeof(DoNothingRedemptionLogic))
                    redemptionLogic = new DoNothingRedemptionLogic();

                if (redemptionLogic == null)
                {
                    throw new Exception(string.Format("INTERNAL ERROR: The redemption logic type '{0}' is not supported. Please report this error.",
                        redemptionLogicTypeInformation.Description));
                }

                redemptionLogic.PriorityOfPayments = _basePriorityOfPayments.Copy();
                if (redemptionLogicDataSetEntity.RedemptionPriorityOfPaymentsSetId.HasValue)
                    redemptionLogic.PriorityOfPayments = CreatePriorityOfPayments(redemptionLogicDataSetEntity.RedemptionPriorityOfPaymentsSetId.Value);

                if (redemptionLogicDataSetEntity.PostRedemptionPriorityOfPaymentsSetId.HasValue)
                    redemptionLogic.PostRedemptionPriorityOfPayments = CreatePriorityOfPayments(redemptionLogicDataSetEntity.PostRedemptionPriorityOfPaymentsSetId.Value);

                if (redemptionLogicDataSetEntity.RedemptionLogicAllowedMonthsSetId.HasValue)
                    redemptionLogic.AddAllowedMonthsForRedemption(RedemptionAllowedMonths[redemptionLogicDataSetEntity.RedemptionLogicAllowedMonthsSetId.Value]);

                listOfRedemptionLogic.Add(redemptionLogic);
            }

            return listOfRedemptionLogic;
        }

        private RedemptionLogic CreateTranchesCanBePaidOutFromAvailableFundsRedemptionLogic(RedemptionLogicDataSetEntity redemptionLogicDataSetEntity)
        {
            var redemptionLogic = new TranchesCanBePaidOutFromAvailableFundsRedemptionLogic();

            if (redemptionLogicDataSetEntity.RedemptionTranchesSetId.HasValue)
            {
                var listOfTrancheDetailIds = RedemptionTrancheDetailIds[redemptionLogicDataSetEntity.RedemptionTranchesSetId.Value];
                foreach (var trancheDetailId in listOfTrancheDetailIds)
                {
                    redemptionLogic.ListOfTranchesToBePaidOut.Add(_securitizationTranchesDictionary[trancheDetailId]);
                }
            }

            return redemptionLogic;
        }

        private PriorityOfPayments CreatePriorityOfPayments(int priorityOfPaymentsDataSetId)
        {
            var priorityOfPaymentsAssignmentEntities = PriorityOfPaymentsSets[priorityOfPaymentsDataSetId];

            var priorityOfPayments = PriorityOfPaymentsDatabaseConverter
                .ConvertToPriorityOfPayments(priorityOfPaymentsAssignmentEntities, _trancheCashFlowTypes, _trancheDetails);

            return priorityOfPayments;
        }
    }
}
