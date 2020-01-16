USE CapitalMarkets
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_CollateralizedSecuritizationTranche_CollateralizedSecuritizationDataSet')))
BEGIN
	ALTER TABLE dream.CollateralizedSecuritizationTranche
		ADD CONSTRAINT FK_CollateralizedSecuritizationTranche_CollateralizedSecuritizationDataSet
			FOREIGN KEY (CollateralizedSecuritizationDataSetId) REFERENCES dream.CollateralizedSecuritizationDataSet(CollateralizedSecuritizationDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_CollateralizedSecuritizationTranche_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.CollateralizedSecuritizationTranche
		ADD CONSTRAINT FK_CollateralizedSecuritizationTranche_SecuritizationAnalysisDataSet
			FOREIGN KEY (SecuritizationAnalysisDataSetId) REFERENCES dream.SecuritizationAnalysisDataSet(SecuritizationAnalysisDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_CollateralizedSecuritizationTranche_TrancheDetail')))
BEGIN
	ALTER TABLE dream.CollateralizedSecuritizationTranche
		ADD CONSTRAINT FK_CollateralizedSecuritizationTranche_TrancheDetail
			FOREIGN KEY (SecuritizatizedTrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_PaceAssessmentRecordDataSet')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		ADD CONSTRAINT FK_PaceAssessmentRecord_PaceAssessmentRecordDataSet
			FOREIGN KEY (PaceAssessmentRecordDataSetId) REFERENCES dream.PaceAssessmentRecordDataSet(PaceAssessmentRecordDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_PropertyState')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		ADD CONSTRAINT FK_PaceAssessmentRecord_PropertyState
			FOREIGN KEY (PropertyStateId) REFERENCES dbo.PropertyState(PropertyStateId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRatePlan_PropertyState')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		ADD CONSTRAINT FK_PaceAssessmentRatePlan_PropertyState
			FOREIGN KEY (PropertyStateId) REFERENCES dbo.PropertyState(PropertyStateId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_PrepaymentPenaltyPlan')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		ADD CONSTRAINT FK_PaceAssessmentRecord_PrepaymentPenaltyPlan
			FOREIGN KEY (PrepaymentPenaltyPlanId) REFERENCES dream.PrepaymentPenaltyPlan(PrepaymentPenaltyPlanId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRatePlan_RatePlanTermSet')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		ADD CONSTRAINT FK_PaceAssessmentRatePlan_RatePlanTermSet
			FOREIGN KEY (PaceAssessmentRatePlanTermSetId) REFERENCES dream.PaceAssessmentRatePlanTermSet(PaceAssessmentRatePlanTermSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_RatePlanTermSet')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		ADD CONSTRAINT FK_PaceAssessmentRecord_RatePlanTermSet
			FOREIGN KEY (PaceAssessmentRatePlanTermSetId) REFERENCES dream.PaceAssessmentRatePlanTermSet(PaceAssessmentRatePlanTermSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PrepaymentPenaltyPlanDetail_PrepaymentPenaltyPlan')))
BEGIN
	ALTER TABLE dream.PrepaymentPenaltyPlanDetail
		ADD CONSTRAINT FK_PrepaymentPenaltyPlanDetail_PrepaymentPenaltyPlan
			FOREIGN KEY (PrepaymentPenaltyPlanId) REFERENCES dream.PrepaymentPenaltyPlan(PrepaymentPenaltyPlanId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PerformanceAssumptionAssignment_PerformanceAssumptionDataSet')))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionAssignment
		ADD CONSTRAINT FK_PerformanceAssumptionAssignment_PerformanceAssumptionDataSet
			FOREIGN KEY (PerformanceAssumptionDataSetId) REFERENCES dream.PerformanceAssumptionDataSet(PerformanceAssumptionDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PerformanceAssumptionAssignment_PerformanceAssumptionType')))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionAssignment
		ADD CONSTRAINT FK_PerformanceAssumptionAssignment_PerformanceAssumptionType
			FOREIGN KEY (PerformanceAssumptionTypeId) REFERENCES dream.PerformanceAssumptionType(PerformanceAssumptionTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PerformanceAssumptionAssignment_VectorParent')))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionAssignment
		ADD CONSTRAINT FK_PerformanceAssumptionAssignment_VectorParent
			FOREIGN KEY (VectorParentId) REFERENCES dream.VectorParent(VectorParentId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_Vector_VectorParent')))
BEGIN
	ALTER TABLE dream.Vector
		ADD CONSTRAINT FK_Vector_VectorParent
			FOREIGN KEY (VectorParentId) REFERENCES dream.VectorParent(VectorParentId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_AggregationGroupAssignment_AggregationGroupDataSet')))
BEGIN
	ALTER TABLE dream.AggregationGroupAssignment
		ADD CONSTRAINT FK_AggregationGroupAssignment_AggregationGroupDataSet
			FOREIGN KEY (AggregationGroupDataSetId) REFERENCES dream.AggregationGroupDataSet(AggregationGroupDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_PricingGroupDataSet')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		ADD CONSTRAINT FK_PricingGroupAssignment_PricingGroupDataSet
			FOREIGN KEY (PricingGroupDataSetId) REFERENCES dream.PricingGroupDataSet(PricingGroupDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_PricingType')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		ADD CONSTRAINT FK_PricingGroupAssignment_PricingType
			FOREIGN KEY (PricingTypeId) REFERENCES dream.PricingType(PricingTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_RateIndex')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		ADD CONSTRAINT FK_PricingGroupAssignment_RateIndex
			FOREIGN KEY (PricingRateIndexId) REFERENCES dream.RateIndex(RateIndexId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_DayCountConvention')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		ADD CONSTRAINT FK_PricingGroupAssignment_DayCountConvention
			FOREIGN KEY (PricingDayCountConventionId) REFERENCES dream.DayCountConvention(DayCountConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_CompoundingConvention')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		ADD CONSTRAINT FK_PricingGroupAssignment_CompoundingConvention
			FOREIGN KEY (PricingCompoundingConventionId) REFERENCES dream.CompoundingConvention(CompoundingConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateCurveData_RateCurveDataSet')))
BEGIN
	ALTER TABLE dream.RateCurveData
		ADD CONSTRAINT FK_RateCurveData_RateCurveDataSet
			FOREIGN KEY (RateCurveDataSetId) REFERENCES dream.RateCurveDataSet(RateCurveDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateIndex_RateIndexGroup')))
BEGIN
	ALTER TABLE dream.RateIndex
		ADD CONSTRAINT FK_RateIndex_RateIndexGroup
			FOREIGN KEY (RateIndexGroupId) REFERENCES dream.RateIndexGroup(RateIndexGroupId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateCurveData_RateIndex')))
BEGIN
	ALTER TABLE dream.RateCurveData
		ADD CONSTRAINT FK_RateCurveData_RateIndex
			FOREIGN KEY (RateIndexId) REFERENCES dream.RateIndex(RateIndexId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateCurveData_MarketDataType')))
BEGIN
	ALTER TABLE dream.RateCurveData
		ADD CONSTRAINT FK_RateCurveData_MarketDataType
			FOREIGN KEY (MarketDataTypeId) REFERENCES dream.MarketDataType(MarketDataTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketData_MarketDataSet')))
BEGIN
	ALTER TABLE dream.MarketData
		ADD CONSTRAINT FK_MarketData_MarketDataSet
			FOREIGN KEY (MarketDataSetId) REFERENCES dream.MarketDataSet(MarketDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketData_RateIndex')))
BEGIN
	ALTER TABLE dream.MarketData
		ADD CONSTRAINT FK_MarketData_RateIndex
			FOREIGN KEY (RateIndexId) REFERENCES dream.RateIndex(RateIndexId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketData_MarketDataType')))
BEGIN
	ALTER TABLE dream.MarketData
		ADD CONSTRAINT FK_MarketData_MarketDataType
			FOREIGN KEY (MarketDataTypeId) REFERENCES dream.MarketDataType(MarketDataTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketRateEnvironment_MarketDataSet')))
BEGIN
	ALTER TABLE dream.MarketRateEnvironment
		ADD CONSTRAINT FK_MarketRateEnvironment_MarketDataSet
			FOREIGN KEY (MarketDataSetId) REFERENCES dream.MarketDataSet(MarketDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketRateEnvironment_RateCurveDataSet')))
BEGIN
	ALTER TABLE dream.MarketRateEnvironment
		ADD CONSTRAINT FK_MarketRateEnvironment_RateCurveDataSet
			FOREIGN KEY (RateCurveDataSetId) REFERENCES dream.RateCurveDataSet(RateCurveDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisOwner_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisOwner
		ADD CONSTRAINT FK_SecuritizationAnalysisOwner_SecuritizationAnalysisDataSet
			FOREIGN KEY (SecuritizationAnalysisDataSetId) REFERENCES dream.SecuritizationAnalysisDataSet(SecuritizationAnalysisDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisOwner_ApplicationUser')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisOwner
		ADD CONSTRAINT FK_SecuritizationAnalysisOwner_ApplicationUser
			FOREIGN KEY (ApplicationUserId) REFERENCES dream.ApplicationUser(ApplicationUserId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisResult_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisResult
		ADD CONSTRAINT FK_SecuritizationAnalysisResult_SecuritizationAnalysisDataSet
			FOREIGN KEY (SecuritizationAnalysisDataSetId) REFERENCES dream.SecuritizationAnalysisDataSet(SecuritizationAnalysisDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisResult_TrancheDetail')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisResult
		ADD CONSTRAINT FK_SecuritizationAnalysisResult_TrancheDetail
			FOREIGN KEY (SecuritizationTrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisResult_SecuritizationResultType')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisResult
		ADD CONSTRAINT FK_SecuritizationAnalysisResult_SecuritizationResultType
			FOREIGN KEY (SecuritizationResultTypeId) REFERENCES dream.SecuritizationResultType(SecuritizationResultTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisSummary_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisSummary
		ADD CONSTRAINT FK_SecuritizationAnalysisSummary_SecuritizationAnalysisDataSet
			FOREIGN KEY (SecuritizationAnalysisDataSetId) REFERENCES dream.SecuritizationAnalysisDataSet(SecuritizationAnalysisDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisSummary_TrancheDetail')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisSummary
		ADD CONSTRAINT FK_SecuritizationAnalysisSummary_TrancheDetail
			FOREIGN KEY (SecuritizationTrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisComment_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisComment
		ADD CONSTRAINT FK_SecuritizationAnalysisComment_SecuritizationAnalysisDataSet
			FOREIGN KEY (SecuritizationAnalysisDataSetId) REFERENCES dream.SecuritizationAnalysisDataSet(SecuritizationAnalysisDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisInput_RateIndexGroup')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisInput
		ADD CONSTRAINT FK_SecuritizationAnalysisInput_RateIndexGroup
			FOREIGN KEY (NominalSpreadRateIndexGroupId) REFERENCES dream.RateIndexGroup(RateIndexGroupId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisInput_RateIndex')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisInput
		ADD CONSTRAINT FK_SecuritizationAnalysisInput_RateIndex
			FOREIGN KEY (CurveSpreadRateIndexId) REFERENCES dream.RateIndex(RateIndexId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysis_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysis
		ADD CONSTRAINT FK_SecuritizationAnalysis_SecuritizationAnalysisDataSet
			FOREIGN KEY (SecuritizationAnalysisDataSetId) REFERENCES dream.SecuritizationAnalysisDataSet(SecuritizationAnalysisDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysis_SecuritizationInputType')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysis
		ADD CONSTRAINT FK_SecuritizationAnalysis_SecuritizationInputType
			FOREIGN KEY (SecuritizationInputTypeId) REFERENCES dream.SecuritizationInputType(SecuritizationInputTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_SecuritizationNodeDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_SecuritizationNodeDataSet
			FOREIGN KEY (SecuritizationNodeDataSetId) REFERENCES dream.SecuritizationNodeDataSet(SecuritizationNodeDataSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_FundsDistributionType')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_FundsDistributionType
			FOREIGN KEY (FundsDistributionTypeId) REFERENCES dream.FundsDistributionType(FundsDistributionTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_TrancheDetail')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_TrancheDetail
			FOREIGN KEY (TrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_PricingType')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_PricingType
			FOREIGN KEY (TranchePricingTypeId) REFERENCES dream.PricingType(PricingTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_RateIndex')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_RateIndex
			FOREIGN KEY (TranchePricingRateIndexId) REFERENCES dream.RateIndex(RateIndexId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_DayCountConvention')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_DayCountConvention
			FOREIGN KEY (TranchePricingDayCountConventionId) REFERENCES dream.DayCountConvention(DayCountConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_CompoundingConvention')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT FK_SecuritizationNode_CompoundingConvention
			FOREIGN KEY (TranchePricingCompoundingConventionId) REFERENCES dream.CompoundingConvention(CompoundingConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_TrancheType')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_TrancheType
			FOREIGN KEY (TrancheTypeId) REFERENCES dream.TrancheType(TrancheTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_TrancheCoupon')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_TrancheCoupon
			FOREIGN KEY (TrancheCouponId) REFERENCES dream.TrancheCoupon(TrancheCouponId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_AvailableFundsRetrievalDetail_1')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_AvailableFundsRetrievalDetail_1
			FOREIGN KEY (PaymentAvailableFundsRetrievalDetailId) REFERENCES dream.AvailableFundsRetrievalDetail(AvailableFundsRetrievalDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_AvailableFundsRetrievalDetail_2')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_AvailableFundsRetrievalDetail_2
			FOREIGN KEY (InterestAvailableFundsRetrievalDetailId) REFERENCES dream.AvailableFundsRetrievalDetail(AvailableFundsRetrievalDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_ReserveAccountsSet')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_ReserveAccountsSet
			FOREIGN KEY (ReserveAccountsSetId) REFERENCES dream.ReserveAccountsSet(ReserveAccountsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_BalanceCapAndFloorSet')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_BalanceCapAndFloorSet
			FOREIGN KEY (BalanceCapAndFloorSetId) REFERENCES dream.BalanceCapAndFloorSet(BalanceCapAndFloorSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_FeeGroupDetail')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_FeeGroupDetail
			FOREIGN KEY (FeeGroupDetailId) REFERENCES dream.FeeGroupDetail(FeeGroupDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_PaymentConvention_1')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_PaymentConvention_1
			FOREIGN KEY (PaymentConventionId) REFERENCES dream.PaymentConvention(PaymentConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_PaymentConvention_2')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_PaymentConvention_2
			FOREIGN KEY (InitialPaymentConventionId) REFERENCES dream.PaymentConvention(PaymentConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_DayCountConvention_1')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_DayCountConvention_1
			FOREIGN KEY (AccrualDayCountConventionId) REFERENCES dream.DayCountConvention(DayCountConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_DayCountConvention_2')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		ADD CONSTRAINT FK_TrancheDetail_DayCountConvention_2
			FOREIGN KEY (InitialDayCountConventionId) REFERENCES dream.DayCountConvention(DayCountConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheCoupon_RateIndex')))
BEGIN
	ALTER TABLE dream.TrancheCoupon
		ADD CONSTRAINT FK_TrancheCoupon_RateIndex
			FOREIGN KEY (TrancheCouponRateIndexId) REFERENCES dream.RateIndex(RateIndexId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheCoupon_DayCountConvention_1')))
BEGIN
	ALTER TABLE dream.TrancheCoupon
		ADD CONSTRAINT FK_TrancheCoupon_DayCountConvention_1
			FOREIGN KEY (InterestAccrualDayCountConventionId) REFERENCES dream.DayCountConvention(DayCountConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheCoupon_DayCountConvention_2')))
BEGIN
	ALTER TABLE dream.TrancheCoupon
		ADD CONSTRAINT FK_TrancheCoupon_DayCountConvention_2
			FOREIGN KEY (InitialPeriodInterestAccrualDayCountConventionId) REFERENCES dream.DayCountConvention(DayCountConventionId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_FeeDetail_FeeGroupDetail')))
BEGIN
	ALTER TABLE dream.FeeDetail
		ADD CONSTRAINT FK_FeeDetail_FeeGroupDetail
			FOREIGN KEY (FeeGroupDetailId) REFERENCES dream.FeeGroupDetail(FeeGroupDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_FeeDetail_TrancheDetail')))
BEGIN
	ALTER TABLE dream.FeeDetail
		ADD CONSTRAINT FK_FeeDetail_TrancheDetail
			FOREIGN KEY (FeeAssociatedTrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_RedemptionLogicType')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		ADD CONSTRAINT FK_RedemptionLogicDataSet_RedemptionLogicType
			FOREIGN KEY (RedemptionLogicTypeId) REFERENCES dream.RedemptionLogicType(RedemptionLogicTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_RedemptionLogicAllowedMonthsSet')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		ADD CONSTRAINT FK_RedemptionLogicDataSet_RedemptionLogicAllowedMonthsSet
			FOREIGN KEY (RedemptionLogicAllowedMonthsSetId) REFERENCES dream.RedemptionLogicAllowedMonthsSet(RedemptionLogicAllowedMonthsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_RedemptionTranchesSet')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		ADD CONSTRAINT FK_RedemptionLogicDataSet_RedemptionTranchesSet
			FOREIGN KEY (RedemptionTranchesSetId) REFERENCES dream.RedemptionTranchesSet(RedemptionTranchesSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_RedemptionPriorityOfPaymentsSetId')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		ADD CONSTRAINT FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_RedemptionPriorityOfPaymentsSetId
			FOREIGN KEY (RedemptionPriorityOfPaymentsSetId) REFERENCES dream.PriorityOfPaymentsSet(PriorityOfPaymentsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_PostRedemptionPriorityOfPaymentsSetId')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		ADD CONSTRAINT FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_PostRedemptionPriorityOfPaymentsSetId
			FOREIGN KEY (PostRedemptionPriorityOfPaymentsSetId) REFERENCES dream.PriorityOfPaymentsSet(PriorityOfPaymentsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionTranchesDetail_RedemptionTranchesSet')))
BEGIN
	ALTER TABLE dream.RedemptionTranchesDetail
		ADD CONSTRAINT FK_RedemptionTranchesDetail_RedemptionTranchesSet
			FOREIGN KEY (RedemptionTranchesSetId) REFERENCES dream.RedemptionTranchesSet(RedemptionTranchesSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionTranchesDetail_TrancheDetail')))
BEGIN
	ALTER TABLE dream.RedemptionTranchesDetail
		ADD CONSTRAINT FK_RedemptionTranchesDetail_TrancheDetail
			FOREIGN KEY (TrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicAllowedMonthsDetail_RedemptionLogicAllowedMonthsSet')))
BEGIN
	ALTER TABLE dream.RedemptionLogicAllowedMonthsDetail
		ADD CONSTRAINT FK_RedemptionLogicAllowedMonthsDetail_RedemptionLogicAllowedMonthsSet
			FOREIGN KEY (RedemptionLogicAllowedMonthsSetId) REFERENCES dream.RedemptionLogicAllowedMonthsSet(RedemptionLogicAllowedMonthsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicAllowedMonthsDetail_AllowedMonth')))
BEGIN
	ALTER TABLE dream.RedemptionLogicAllowedMonthsDetail
		ADD CONSTRAINT FK_RedemptionLogicAllowedMonthsDetail_AllowedMonth
			FOREIGN KEY (AllowedMonthId) REFERENCES dream.AllowedMonth(AllowedMonthId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_ReserveAccountsDetail_ReserveAccountsSet')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		ADD CONSTRAINT FK_ReserveAccountsDetail_ReserveAccountsSet
			FOREIGN KEY (ReserveAccountsSetId) REFERENCES dream.ReserveAccountsSet(ReserveAccountsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_ReserveAccountsDetail_TrancheDetail')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		ADD CONSTRAINT FK_ReserveAccountsDetail_TrancheDetail
			FOREIGN KEY (TrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_BalanceCapAndFloorDetail_BalanceCapAndFloorSet')))
BEGIN
	ALTER TABLE dream.BalanceCapAndFloorDetail
		ADD CONSTRAINT FK_BalanceCapAndFloorDetail_BalanceCapAndFloorSet
			FOREIGN KEY (BalanceCapAndFloorSetId) REFERENCES dream.BalanceCapAndFloorSet(BalanceCapAndFloorSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_AvailableFundsRetrievalDetail_AvailableFundsRetrievalType')))
BEGIN
	ALTER TABLE dream.AvailableFundsRetrievalDetail
		ADD CONSTRAINT FK_AvailableFundsRetrievalDetail_AvailableFundsRetrievalType
			FOREIGN KEY (AvailableFundsRetrievalTypeId) REFERENCES dream.AvailableFundsRetrievalType(AvailableFundsRetrievalTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PriorityOfPaymentsAssignment_PriorityOfPaymentsSet')))
BEGIN
	ALTER TABLE dream.PriorityOfPaymentsAssignment
		ADD CONSTRAINT FK_PriorityOfPaymentsAssignment_PriorityOfPaymentsSet
			FOREIGN KEY (PriorityOfPaymentsSetId) REFERENCES dream.PriorityOfPaymentsSet(PriorityOfPaymentsSetId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PriorityOfPaymentsAssignment_TrancheDetail')))
BEGIN
	ALTER TABLE dream.PriorityOfPaymentsAssignment
		ADD CONSTRAINT FK_PriorityOfPaymentsAssignment_TrancheDetail
			FOREIGN KEY (TrancheDetailId) REFERENCES dream.TrancheDetail(TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PriorityOfPaymentsAssignment_TrancheCashFlowType')))
BEGIN
	ALTER TABLE dream.PriorityOfPaymentsAssignment
		ADD CONSTRAINT FK_PriorityOfPaymentsAssignment_TrancheCashFlowType
			FOREIGN KEY (TrancheCashFlowTypeId) REFERENCES dream.TrancheCashFlowType(TrancheCashFlowTypeId)
END