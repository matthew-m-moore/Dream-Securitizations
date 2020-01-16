using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Converters.Database.Collateral;
using Dream.Core.Interfaces;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Collateral;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.SecuritizationEngine;

namespace Dream.Core.Repositories.Database
{
    public class PaceAssessmentDatabaseRepository : DatabaseRepository, ICollateralRetriever
    {
        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private List<int> _paceAssessmentRecordDataSetIds;

        protected DateTime _CashFlowStartDate;
        protected DateTime _InterestAccrualStartDate;
        protected bool _UsePreFundingStartDate;

        private PropertyStateDatabaseConverter _propertyStateDatabaseConverter;
        private PaceRatePlanDatabaseConverter _paceRatePlanConverter;
        private PrepaymentPenaltyPlanDatabaseConverter _prepaymentPenaltyPlanConverter;

        // This collection can be used if the entity records are altered somehow, say through a UI
        public List<PaceAssessmentRecordEntity> PaceAssessmentRecordEntities = new List<PaceAssessmentRecordEntity>();

        private Dictionary<int, string> _propertyStateAbbreviations;
        public Dictionary<int, string> PropertyStateAbbreviations
        {
            get
            {
                if (_propertyStateAbbreviations == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var propertyStateEntities = securitizationEngineContext.PropertyStateEntities.ToList();
                        _propertyStateAbbreviations = propertyStateEntities.ToDictionary(e => e.PropertyStateId, e => e.PropertyState);
                    }
                }

                return _propertyStateAbbreviations;
            }
        }

        private Dictionary<int, (string Description, List<PaceAssessmentRatePlanEntity> PaceAssessmentRatePlanEntities)> _paceAsssementRatePlans;
        public Dictionary<int, (string Description, List<PaceAssessmentRatePlanEntity> PaceAssessmentRatePlanEntities)> PaceAssessmentRatePlans
        {
            get
            {
                if (_paceAsssementRatePlans == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var ratePlanTermSets = securitizationEngineContext.PaceAssessmentRatePlanTermSetEntities.ToList();
                        var ratePlans = securitizationEngineContext.PaceAssessmentRatePlanEntities.ToList();

                        _paceAsssementRatePlans = ratePlanTermSets
                            .ToDictionary(e => e.PaceAssessmentRatePlanTermSetId,
                                          e => (
                                                    Description: e.PaceAssessmentRatePlanTermSetDescription,
                                                    PaceAssessmentRatePlanEntities: ratePlans.Where(d => d.PaceAssessmentRatePlanTermSetId == e.PaceAssessmentRatePlanTermSetId).ToList()
                                               ));
                    }
                }

                return _paceAsssementRatePlans;
            }
        }

        private Dictionary<int, (string Description, List<PrepaymentPenaltyPlanDetailEntity> PrepaymentPenaltyPlanDetailEntities)> _prepaymentPenaltyPlans;
        public Dictionary<int, (string Description, List<PrepaymentPenaltyPlanDetailEntity> PrepaymentPenaltyPlanDetailEntities)> PrepaymentPenaltyPlans
        {
            get
            {
                if (_prepaymentPenaltyPlans == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        var prepaymentPenaltyPlans = securitizationEngineContext.PrepaymentPenaltyPlanEntities.ToList();
                        var prepaymentPenaltyPlanDetails = securitizationEngineContext.PrepaymentPenaltyPlanDetailEntities.ToList();

                        _prepaymentPenaltyPlans = prepaymentPenaltyPlans
                            .ToDictionary(e => e.PrepaymentPenaltyPlanId, 
                                          e => (
                                                    Description: e.PrepaymentPenaltyPlanDescription,
                                                    PrepaymentPenaltyPlanDetailEntities: prepaymentPenaltyPlanDetails.Where(d => d.PrepaymentPenaltyPlanId == e.PrepaymentPenaltyPlanId).ToList()
                                               ));
                    }
                }

                return _prepaymentPenaltyPlans;
            }
        }

        public PaceAssessmentDatabaseRepository(List<int> paceAssessmentRecordDataSetIds) 
            : this(paceAssessmentRecordDataSetIds, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue) { }

        public PaceAssessmentDatabaseRepository(List<int> paceAssessmentRecordDataSetIds, DateTime collateralCutOffDate, DateTime cashFlowStartDate, DateTime interestAccrualStartDate) 
            : base(collateralCutOffDate)
        {
            _CashFlowStartDate = cashFlowStartDate;
            _InterestAccrualStartDate = interestAccrualStartDate;

            _paceAssessmentRecordDataSetIds = paceAssessmentRecordDataSetIds;
            _propertyStateDatabaseConverter = new PropertyStateDatabaseConverter(PropertyStateAbbreviations);

            SetupPaceRatePlanConverter();
            SetupPrepaymentPenaltyPlanConverter();
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public void SetCollateralDates<T>(T inputs) where T : CashFlowGenerationInput
        {
            _CutOffDate = inputs.CollateralCutOffDate;
            _CashFlowStartDate = inputs.CashFlowStartDate;
            _InterestAccrualStartDate = inputs.InterestAccrualStartDate;

            if (inputs.GetType() == typeof(SecuritizationInput))
            {
                var securitizationInputs = inputs as SecuritizationInput;
                _UsePreFundingStartDate = securitizationInputs.UsePreFundingStartDate.GetValueOrDefault(false);
            }
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public List<Loan> GetCollateral()
        {
            return GetAllPaceAssessments();
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public double GetTotalCollateralBalance()
        {
            return GetCollateral().Sum(l => l.Balance);
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public ICollateralRetriever Copy()
        {
            return new PaceAssessmentDatabaseRepository(
                _paceAssessmentRecordDataSetIds.Select(i => i).ToList(), 
                new DateTime(_CutOffDate.Ticks),
                new DateTime(_CashFlowStartDate.Ticks),
                new DateTime(_InterestAccrualStartDate.Ticks));
        }

        /// <summary>
        /// Retrieves all repline-level, bond-level, and assessment-level PACE assessments from the database.
        /// </summary>
        public List<Loan> GetAllPaceAssessments()
        {
            if (!PaceAssessmentRecordEntities.Any())
            {
                foreach (var paceAssessmentRecordDataSetId in _paceAssessmentRecordDataSetIds)
                {
                    GetPaceAssessments(paceAssessmentRecordDataSetId);
                }
            }

            var paceAssessmentConverter = new PaceAssessmentDatabaseConverter(
                _CutOffDate,
                _CashFlowStartDate,
                _InterestAccrualStartDate,
                _UsePreFundingStartDate,
                _propertyStateDatabaseConverter,
                _paceRatePlanConverter,
                _prepaymentPenaltyPlanConverter);

            var paceAssessments = new List<Loan>();
            paceAssessments.AddRange(paceAssessmentConverter.ConvertListOfPaceTapeRecords(PaceAssessmentRecordEntities));

            return paceAssessments;
        }

        /// <summary>
        /// Retrieves PACE assessments from the database.
        /// </summary>
        public void GetPaceAssessments(int paceAssessmentRecordDataSetId)
        {
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                var paceAssessmentRecordEntities = securitizationEngineContext
                    .PaceAssessmentRecordEntities.Where(e => e.PaceAssessmentRecordDataSetId == paceAssessmentRecordDataSetId).ToList();

                PaceAssessmentRecordEntities.AddRange(paceAssessmentRecordEntities);
            }
        }

        private void SetupPaceRatePlanConverter()
        {
            if (!PaceAssessmentRatePlans.Any()) return;
            _paceRatePlanConverter = new PaceRatePlanDatabaseConverter(PaceAssessmentRatePlans);
        }

        private void SetupPrepaymentPenaltyPlanConverter()
        {
            if (!PrepaymentPenaltyPlans.Any()) return;
            _prepaymentPenaltyPlanConverter = new PrepaymentPenaltyPlanDatabaseConverter(PrepaymentPenaltyPlans);
        }
    }
}
