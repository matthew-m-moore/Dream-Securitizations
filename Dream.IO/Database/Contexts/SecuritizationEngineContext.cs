using Dream.Common;
using Dream.IO.Database.Entities;
using Dream.IO.Database.Entities.Collateral;
using Dream.IO.Database.Entities.InterestRates;
using Dream.IO.Database.Entities.Pricing;
using Dream.IO.Database.Entities.Securitization;
using Dream.IO.Database.Mappings;
using Dream.IO.Database.Mappings.Collateral;
using Dream.IO.Database.Mappings.InterestRates;
using Dream.IO.Database.Mappings.Pricing;
using Dream.IO.Database.Mappings.Securitization;
using System.Data.Entity;

namespace Dream.IO.Database.Contexts
{
    public class SecuritizationEngineContext : DbContext
    {
        public SecuritizationEngineContext(string connectionString) : base(connectionString)
        {
            System.Data.Entity.Database.SetInitializer<SecuritizationEngineContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Constants.DreamSchemaName);

            #region Collateral
            modelBuilder.Configurations.Add(new AggregationGroupAssignmentMapping());
            modelBuilder.Configurations.Add(new AggregationGroupDataSetMapping());
            modelBuilder.Configurations.Add(new CollateralizedSecuritizationDataSetMapping());
            modelBuilder.Configurations.Add(new CollateralizedSecuritizationTrancheMapping());
            modelBuilder.Configurations.Add(new PaceAssessmentRatePlanMapping());
            modelBuilder.Configurations.Add(new PaceAssessmentRatePlanTermSetMapping());
            modelBuilder.Configurations.Add(new PaceAssessmentRecordMapping());
            modelBuilder.Configurations.Add(new PaceAssessmentRecordDataSetMapping());
            modelBuilder.Configurations.Add(new PerformanceAssumptionAssignmentMapping());
            modelBuilder.Configurations.Add(new PerformanceAssumptionDataSetMapping());
            modelBuilder.Configurations.Add(new PerformanceAssumptionTypeMapping());
            modelBuilder.Configurations.Add(new PrepaymentPenaltyPlanMapping());
            modelBuilder.Configurations.Add(new PrepaymentPenaltyPlanDetailMapping());
            modelBuilder.Configurations.Add(new PropertyStateMapping());
            #endregion Collateral

            #region Interest Rates
            modelBuilder.Configurations.Add(new MarketDataMapping());
            modelBuilder.Configurations.Add(new MarketDataSetMapping());
            modelBuilder.Configurations.Add(new MarketDataTypeMapping());
            modelBuilder.Configurations.Add(new MarketRateEnvironmentMapping());
            modelBuilder.Configurations.Add(new RateCurveDataMapping());
            modelBuilder.Configurations.Add(new RateCurveDataSetMapping());
            modelBuilder.Configurations.Add(new RateIndexMapping());
            modelBuilder.Configurations.Add(new RateIndexGroupMapping());
            #endregion Interest Rates

            #region Pricing
            modelBuilder.Configurations.Add(new PricingGroupAssignmentMapping());
            modelBuilder.Configurations.Add(new PricingGroupDataSetMapping());
            modelBuilder.Configurations.Add(new PricingTypeMapping());
            #endregion Pricing

            #region Securitization
            modelBuilder.Configurations.Add(new AvailableFundsRetrievalDetailMapping());
            modelBuilder.Configurations.Add(new AvailableFundsRetrievalTypeMapping());
            modelBuilder.Configurations.Add(new BalanceCapAndFloorDetailMapping());
            modelBuilder.Configurations.Add(new BalanceCapAndFloorSetMapping());
            modelBuilder.Configurations.Add(new FeeDetailMapping());
            modelBuilder.Configurations.Add(new FeeGroupDetailMapping());
            modelBuilder.Configurations.Add(new FundsDistributionTypeMapping());
            modelBuilder.Configurations.Add(new PriorityOfPaymentsAssignmentMapping());
            modelBuilder.Configurations.Add(new PriorityOfPaymentsSetMapping());
            modelBuilder.Configurations.Add(new RedemptionLogicAllowedMonthsDetailMapping());
            modelBuilder.Configurations.Add(new RedemptionLogicAllowedMonthsSetMapping());
            modelBuilder.Configurations.Add(new RedemptionLogicDataSetMapping());
            modelBuilder.Configurations.Add(new RedemptionLogicTypeMapping());
            modelBuilder.Configurations.Add(new RedemptionTranchesDetailMapping());
            modelBuilder.Configurations.Add(new RedemptionTranchesSetMapping());
            modelBuilder.Configurations.Add(new ReserveAccountsDetailMapping());
            modelBuilder.Configurations.Add(new ReserveAccountsSetMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisDataSetMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisCommentMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisOwnerMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisResultMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisSummaryMapping());
            modelBuilder.Configurations.Add(new SecuritizationAnalysisInputMapping());
            modelBuilder.Configurations.Add(new SecuritizationInputTypeMapping());
            modelBuilder.Configurations.Add(new SecuritizationResultTypeMapping());
            modelBuilder.Configurations.Add(new SecuritizationNodeMapping());
            modelBuilder.Configurations.Add(new SecuritizationNodeDataSetMapping());
            modelBuilder.Configurations.Add(new TrancheCashFlowTypeMapping());
            modelBuilder.Configurations.Add(new TrancheCouponMapping());
            modelBuilder.Configurations.Add(new TrancheDetailMapping());
            modelBuilder.Configurations.Add(new TrancheTypeMapping());
            #endregion Securitization

            #region Other
            modelBuilder.Configurations.Add(new ApplicationUserMapping());
            modelBuilder.Configurations.Add(new AllowedMonthMapping());
            modelBuilder.Configurations.Add(new CompoundingConventionMapping());
            modelBuilder.Configurations.Add(new DayCountConventionMapping());
            modelBuilder.Configurations.Add(new PaymentConventionMapping());
            modelBuilder.Configurations.Add(new VectorMapping());
            modelBuilder.Configurations.Add(new VectorParentMapping());
            #endregion Other
        }

        #region Collateral
        public virtual DbSet<AggregationGroupAssignmentEntity> AggregationGroupAssignmentEntities { get; set; }
        public virtual DbSet<AggregationGroupDataSetEntity> AggregationGroupDataSetEntities { get; set; }
        public virtual DbSet<CollateralizedSecuritizationDataSetEntity> CollateralizedSecuritizationDataSeEntities { get; set; }
        public virtual DbSet<CollateralizedSecuritizationTrancheEntity> CollateralizedSecuritizationTrancheEntities { get; set; }
        public virtual DbSet<PaceAssessmentRatePlanEntity> PaceAssessmentRatePlanEntities { get; set; }
        public virtual DbSet<PaceAssessmentRatePlanTermSetEntity> PaceAssessmentRatePlanTermSetEntities { get; set; }
        public virtual DbSet<PaceAssessmentRecordEntity> PaceAssessmentRecordEntities { get; set; }
        public virtual DbSet<PaceAssessmentRecordDataSetEntity> PaceAssessmentRecordDataSetEntities { get; set; }
        public virtual DbSet<PerformanceAssumptionAssignmentEntity> PerformanceAssumptionAssignmentEntities { get; set; }
        public virtual DbSet<PerformanceAssumptionDataSetEntity> PerformanceAssumptionDataSetEntities { get; set; }
        public virtual DbSet<PerformanceAssumptionTypeEntity> PerformanceAssumptionTypeEntities { get; set; }
        public virtual DbSet<PrepaymentPenaltyPlanEntity> PrepaymentPenaltyPlanEntities { get; set; }
        public virtual DbSet<PrepaymentPenaltyPlanDetailEntity> PrepaymentPenaltyPlanDetailEntities { get; set; }
        public virtual DbSet<PropertyStateEntity> PropertyStateEntities { get; set; }
        #endregion Collateral

        #region Interest Rates
        public virtual DbSet<MarketDataEntity> MarketDataEntities { get; set; }
        public virtual DbSet<MarketDataSetEntity> MarketDataSetEntities { get; set; }
        public virtual DbSet<MarketDataTypeEntity> MarketDataTypeEntities { get; set; }
        public virtual DbSet<MarketRateEnvironmentEntity> MarketRateEnvironmentEntities { get; set; }
        public virtual DbSet<RateCurveDataEntity> RateCurveDataEntities { get; set; }
        public virtual DbSet<RateCurveDataSetEntity> RateCurveDataSetEntities { get; set; }
        public virtual DbSet<RateIndexEntity> RateIndexEntities { get; set; }
        public virtual DbSet<RateIndexGroupEntity> RateIndexGroupEntities { get; set; }
        #endregion Interest Rates

        #region Pricing
        public virtual DbSet<PricingGroupAssignmentEntity> PricingGroupAssignmentEntities { get; set; }
        public virtual DbSet<PricingGroupDataSetEntity> PricingGroupDataSetEntities { get; set; }
        public virtual DbSet<PricingTypeEntity> PricingTypeEntities { get; set; }
        #endregion Pricing

        #region Securitization
        public virtual DbSet<AvailableFundsRetrievalDetailEntity> AvailableFundsRetrievalDetailEntities { get; set; }
        public virtual DbSet<AvailableFundsRetrievalTypeEntity> AvailableFundsRetrievalTypeEntities { get; set; }
        public virtual DbSet<BalanceCapAndFloorDetailEntity> BalanceCapAndFloorDetailEntities { get; set; }
        public virtual DbSet<BalanceCapAndFloorSetEntity> BalanceCapAndFloorSetEntities { get; set; }
        public virtual DbSet<FeeDetailEntity> FeeDetailEntities { get; set; }
        public virtual DbSet<FeeGroupDetailEntity> FeeGroupDetailEntities { get; set; }
        public virtual DbSet<FundsDistributionTypeEntity> FundsDistributionTypeEntities { get; set; }
        public virtual DbSet<PriorityOfPaymentsAssignmentEntity> PriorityOfPaymentsAssignmentEntities { get; set; }
        public virtual DbSet<PriorityOfPaymentsSetEntity> PriorityOfPaymentsSetEntities { get; set; }
        public virtual DbSet<RedemptionLogicAllowedMonthsDetailEntity> RedemptionLogicAllowedMonthsDetailEntities { get; set; }
        public virtual DbSet<RedemptionLogicAllowedMonthsSetEntity> RedemptionLogicAllowedMonthsSetEntities { get; set; }
        public virtual DbSet<RedemptionLogicDataSetEntity> RedemptionLogicDataSetEntities { get; set; }
        public virtual DbSet<RedemptionLogicTypeEntity> RedemptionLogicTypeEntities { get; set; }
        public virtual DbSet<RedemptionTranchesDetailEntity> RedemptionTranchesDetailEntities { get; set; }
        public virtual DbSet<RedemptionTranchesSetEntity> RedemptionTranchesSetEntities { get; set; }
        public virtual DbSet<ReserveAccountsDetailEntity> ReserveAccountsDetailEntities { get; set; }
        public virtual DbSet<ReserveAccountsSetEntity> ReserveAccountsSetEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisEntity> SecuritizationAnalysisEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisDataSetEntity> SecuritizationAnalysisDataSetEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisCommentEntity> SecuritizationAnalysisCommentEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisOwnerEntity> SecuritizationAnalysisOwnerEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisResultEntity> SecuritizationAnalysisResultEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisSummaryEntity> SecuritizationAnalysisSummaryEntities { get; set; }
        public virtual DbSet<SecuritizationAnalysisInputEntity> SecuritizationAnalysisInputEntities { get; set; }
        public virtual DbSet<SecuritizationInputTypeEntity> SecuritizationInputTypeEntities { get; set; }
        public virtual DbSet<SecuritizationResultTypeEntity> SecuritizationResultTypeEntities { get; set; }
        public virtual DbSet<SecuritizationNodeEntity> SecuritizationNodeEntities { get; set; }
        public virtual DbSet<SecuritizationNodeDataSetEntity> SecuritizationNodeDataSetEntities { get; set; }
        public virtual DbSet<TrancheCashFlowTypeEntity> TrancheCashFlowTypeEntities { get; set; }
        public virtual DbSet<TrancheCouponEntity> TrancheCouponEntities { get; set; }
        public virtual DbSet<TrancheDetailEntity> TrancheDetailEntities { get; set; }
        public virtual DbSet<TrancheTypeEntity> TrancheTypeEntities { get; set; }
        #endregion Securitization

        #region Other
        public virtual DbSet<ApplicationUserEntity> ApplicationUserEntities { get; set; }
        public virtual DbSet<AllowedMonthEntity> AllowedMonthEntities { get; set; }
        public virtual DbSet<CompoundingConventionEntity> CompoundingConventionEntities { get; set; }
        public virtual DbSet<DayCountConventionEntity> DayCountConventionEntities { get; set; }
        public virtual DbSet<PaymentConventionEntity> PaymentConventionEntities { get; set; }
        public virtual DbSet<VectorEntity> VectorEntities { get; set; }
        public virtual DbSet<VectorParentEntity> VectorParentEntities { get; set; }
        #endregion Other
    }
}
