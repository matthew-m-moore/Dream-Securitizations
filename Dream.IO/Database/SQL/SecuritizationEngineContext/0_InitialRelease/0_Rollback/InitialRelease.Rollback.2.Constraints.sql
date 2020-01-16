USE CapitalMarkets
GO

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_CollateralizedSecuritizationTranche_CollateralizedSecuritizationDataSet')))
BEGIN
	ALTER TABLE dream.CollateralizedSecuritizationTranche
		DROP CONSTRAINT FK_CollateralizedSecuritizationTranche_CollateralizedSecuritizationDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_CollateralizedSecuritizationTranche_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.CollateralizedSecuritizationTranche
		DROP CONSTRAINT FK_CollateralizedSecuritizationTranche_SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_CollateralizedSecuritizationTranche_TrancheDetail')))
BEGIN
	ALTER TABLE dream.CollateralizedSecuritizationTranche
		DROP CONSTRAINT FK_CollateralizedSecuritizationTranche_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_PaceAssessmentRecordDataSet')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		DROP CONSTRAINT FK_PaceAssessmentRecord_PaceAssessmentRecordDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_PropertyState')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		DROP CONSTRAINT FK_PaceAssessmentRecord_PropertyState
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRatePlan_PropertyState')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		DROP CONSTRAINT FK_PaceAssessmentRatePlan_PropertyState
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_PrepaymentPenaltyPlan')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		DROP CONSTRAINT FK_PaceAssessmentRecord_PrepaymentPenaltyPlan
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRecord_RatePlanTermSet')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRecord
		DROP CONSTRAINT FK_PaceAssessmentRecord_RatePlanTermSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PaceAssessmentRatePlan_RatePlanTermSet')))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		DROP CONSTRAINT FK_PaceAssessmentRatePlan_RatePlanTermSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PrepaymentPenaltyPlanDetail_PrepaymentPenaltyPlan')))
BEGIN
	ALTER TABLE dream.PrepaymentPenaltyPlanDetail
		DROP CONSTRAINT FK_PrepaymentPenaltyPlanDetail_PrepaymentPenaltyPlan
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PerformanceAssumptionAssignment_PerformanceAssumptionDataSet')))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionAssignment
		DROP CONSTRAINT FK_PerformanceAssumptionAssignment_PerformanceAssumptionDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PerformanceAssumptionAssignment_PerformanceAssumptionType')))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionAssignment
		DROP CONSTRAINT FK_PerformanceAssumptionAssignment_PerformanceAssumptionType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PerformanceAssumptionAssignment_VectorParent')))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionAssignment
		DROP CONSTRAINT FK_PerformanceAssumptionAssignment_VectorParent
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_Vector_VectorParent')))
BEGIN
	ALTER TABLE dream.Vector
		DROP CONSTRAINT FK_Vector_VectorParent
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_AggregationGroupAssignment_AggregationGroupDataSet')))
BEGIN
	ALTER TABLE dream.AggregationGroupAssignment
		DROP CONSTRAINT FK_AggregationGroupAssignment_AggregationGroupDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_PricingGroupDataSet')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		DROP CONSTRAINT FK_PricingGroupAssignment_PricingGroupDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_PricingType')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		DROP CONSTRAINT FK_PricingGroupAssignment_PricingType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_RateIndex')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		DROP CONSTRAINT FK_PricingGroupAssignment_RateIndex
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_DayCountConvention')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		DROP CONSTRAINT FK_PricingGroupAssignment_DayCountConvention
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PricingGroupAssignment_CompoundingConvention')))
BEGIN
	ALTER TABLE dream.PricingGroupAssignment
		DROP CONSTRAINT FK_PricingGroupAssignment_CompoundingConvention
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateCurveData_RateCurveDataSet')))
BEGIN
	ALTER TABLE dream.RateCurveData
		DROP CONSTRAINT FK_RateCurveData_RateCurveDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateIndex_RateIndexGroup')))
BEGIN
	ALTER TABLE dream.RateIndex
		DROP CONSTRAINT FK_RateIndex_RateIndexGroup
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateCurveData_RateIndex')))
BEGIN
	ALTER TABLE dream.RateCurveData
		DROP CONSTRAINT FK_RateCurveData_RateIndex
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RateCurveData_MarketDataType')))
BEGIN
	ALTER TABLE dream.RateCurveData
		DROP CONSTRAINT FK_RateCurveData_MarketDataType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketData_MarketDataSet')))
BEGIN
	ALTER TABLE dream.MarketData
		DROP CONSTRAINT FK_MarketData_MarketDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketData_RateIndex')))
BEGIN
	ALTER TABLE dream.MarketData
		DROP CONSTRAINT FK_MarketData_RateIndex
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketData_MarketDataType')))
BEGIN
	ALTER TABLE dream.MarketData
		DROP CONSTRAINT FK_MarketData_MarketDataType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketRateEnvironment_MarketDataSet')))
BEGIN
	ALTER TABLE dream.MarketRateEnvironment
		DROP CONSTRAINT FK_MarketRateEnvironment_MarketDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_MarketRateEnvironment_RateCurveDataSet')))
BEGIN
	ALTER TABLE dream.MarketRateEnvironment
		DROP CONSTRAINT FK_MarketRateEnvironment_RateCurveDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisOwner_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisOwner
		DROP CONSTRAINT FK_SecuritizationAnalysisOwner_SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisOwner_ApplicationUser')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisOwner
		DROP CONSTRAINT FK_SecuritizationAnalysisOwner_ApplicationUser
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisComment_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisComment
		DROP CONSTRAINT FK_SecuritizationAnalysisComment_SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisResult_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisResult
		DROP CONSTRAINT FK_SecuritizationAnalysisResult_SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisResult_TrancheDetail')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisResult
		DROP CONSTRAINT FK_SecuritizationAnalysisResult_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisResult_SecuritizationResultType')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisResult
		DROP CONSTRAINT FK_SecuritizationAnalysisResult_SecuritizationResultType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisSummary_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisSummary
		DROP CONSTRAINT FK_SecuritizationAnalysisSummary_SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisSummary_TrancheDetail')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisSummary
		DROP CONSTRAINT FK_SecuritizationAnalysisSummary_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisInput_RateIndexGroup')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisInput
		DROP CONSTRAINT FK_SecuritizationAnalysisInput_RateIndexGroup
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysisInput_RateIndex')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisInput
		DROP CONSTRAINT FK_SecuritizationAnalysisInput_RateIndex
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysis_SecuritizationAnalysisDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysis
		DROP CONSTRAINT FK_SecuritizationAnalysis_SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationAnalysis_SecuritizationInputType')))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysis
		DROP CONSTRAINT FK_SecuritizationAnalysis_SecuritizationInputType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_SecuritizationNodeDataSet')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_SecuritizationNodeDataSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_FundsDistributionType')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_FundsDistributionType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_TrancheDetail')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_PricingType')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_PricingType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_RateIndex')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_RateIndex
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_DayCountConvention')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_DayCountConvention
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_SecuritizationNode_CompoundingConvention')))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT FK_SecuritizationNode_CompoundingConvention
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_TrancheType')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_TrancheType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_TrancheCoupon')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_TrancheCoupon
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_AvailableFundsRetrievalDetail_1')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_AvailableFundsRetrievalDetail_1
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_AvailableFundsRetrievalDetail_2')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_AvailableFundsRetrievalDetail_2
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_ReserveAccountsSet')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_ReserveAccountsSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_BalanceCapAndFloorSet')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_BalanceCapAndFloorSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_FeeGroupDetail')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_FeeGroupDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_PaymentConvention_1')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_PaymentConvention_1
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_PaymentConvention_2')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_PaymentConvention_2
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_DayCountConvention_1')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_DayCountConvention_1
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheDetail_DayCountConvention_2')))
BEGIN
	ALTER TABLE dream.TrancheDetail
		DROP CONSTRAINT FK_TrancheDetail_DayCountConvention_2
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheCoupon_RateIndex')))
BEGIN
	ALTER TABLE dream.TrancheCoupon
		DROP CONSTRAINT FK_TrancheCoupon_RateIndex
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheCoupon_DayCountConvention_1')))
BEGIN
	ALTER TABLE dream.TrancheCoupon
		DROP CONSTRAINT FK_TrancheCoupon_DayCountConvention_1
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_TrancheCoupon_DayCountConvention_2')))
BEGIN
	ALTER TABLE dream.TrancheCoupon
		DROP CONSTRAINT FK_TrancheCoupon_DayCountConvention_2
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_FeeDetail_FeeGroupDetail')))
BEGIN
	ALTER TABLE dream.FeeDetail
		DROP CONSTRAINT FK_FeeDetail_FeeGroupDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_FeeDetail_TrancheDetail')))
BEGIN
	ALTER TABLE dream.FeeDetail
		DROP CONSTRAINT FK_FeeDetail_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_RedemptionLogicType')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		DROP CONSTRAINT FK_RedemptionLogicDataSet_RedemptionLogicType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_RedemptionLogicAllowedMonthsSet')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		DROP CONSTRAINT FK_RedemptionLogicDataSet_RedemptionLogicAllowedMonthsSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_RedemptionTranchesSet')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		DROP CONSTRAINT FK_RedemptionLogicDataSet_RedemptionTranchesSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_RedemptionPriorityOfPaymentsSetId')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		DROP CONSTRAINT FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_RedemptionPriorityOfPaymentsSetId
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_PostRedemptionPriorityOfPaymentsSetId')))
BEGIN
	ALTER TABLE dream.RedemptionLogicDataSet
		DROP CONSTRAINT FK_RedemptionLogicDataSet_PriorityOfPaymentsSet_PostRedemptionPriorityOfPaymentsSetId
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionTranchesDetail_RedemptionTranchesSet')))
BEGIN
	ALTER TABLE dream.RedemptionTranchesDetail
		DROP CONSTRAINT FK_RedemptionTranchesDetail_RedemptionTranchesSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionTranchesDetail_TrancheDetail')))
BEGIN
	ALTER TABLE dream.RedemptionTranchesDetail
		DROP CONSTRAINT FK_RedemptionTranchesDetail_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicAllowedMonthsDetail_RedemptionLogicAllowedMonthsSet')))
BEGIN
	ALTER TABLE dream.RedemptionLogicAllowedMonthsDetail
		DROP CONSTRAINT FK_RedemptionLogicAllowedMonthsDetail_RedemptionLogicAllowedMonthsSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_RedemptionLogicAllowedMonthsDetail_AllowedMonth')))
BEGIN
	ALTER TABLE dream.RedemptionLogicAllowedMonthsDetail
		DROP CONSTRAINT FK_RedemptionLogicAllowedMonthsDetail_AllowedMonth
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_ReserveAccountsDetail_ReserveAccountsSet')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		DROP CONSTRAINT FK_ReserveAccountsDetail_ReserveAccountsSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_ReserveAccountsDetail_TrancheDetail')))
BEGIN
	ALTER TABLE dream.ReserveAccountsDetail
		DROP CONSTRAINT FK_ReserveAccountsDetail_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_BalanceCapAndFloorDetail_BalanceCapAndFloorSet')))
BEGIN
	ALTER TABLE dream.BalanceCapAndFloorDetail
		DROP CONSTRAINT FK_BalanceCapAndFloorDetail_BalanceCapAndFloorSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_AvailableFundsRetrievalDetail_AvailableFundsRetrievalType')))
BEGIN
	ALTER TABLE dream.AvailableFundsRetrievalDetail
		DROP CONSTRAINT FK_AvailableFundsRetrievalDetail_AvailableFundsRetrievalType
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PriorityOfPaymentsAssignment_PriorityOfPaymentsSet')))
BEGIN
	ALTER TABLE dream.PriorityOfPaymentsAssignment
		DROP CONSTRAINT FK_PriorityOfPaymentsAssignment_PriorityOfPaymentsSet
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PriorityOfPaymentsAssignment_TrancheDetail')))
BEGIN
	ALTER TABLE dream.PriorityOfPaymentsAssignment
		DROP CONSTRAINT FK_PriorityOfPaymentsAssignment_TrancheDetail
END

IF (EXISTS (SELECT * 
				FROM SYS.FOREIGN_KEYS 
				WHERE OBJECT_ID = OBJECT_ID('dream.FK_PriorityOfPaymentsAssignment_TrancheCashFlowType')))
BEGIN
	ALTER TABLE dream.PriorityOfPaymentsAssignment
		DROP CONSTRAINT FK_PriorityOfPaymentsAssignment_TrancheCashFlowType
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_ApplicationUser_NetworkUserNameIdentifier'))
BEGIN
	ALTER TABLE dream.ApplicationUser
		DROP CONSTRAINT UC_ApplicationUser_NetworkUserNameIdentifier
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_ApplicationUser_ApplicationDisplayableNickName'))
BEGIN
	ALTER TABLE dream.ApplicationUser
		DROP CONSTRAINT UC_ApplicationUser_ApplicationDisplayableNickName
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationAnalysis_DataSetId_VersionId_ScenarioId_TypeId'))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysis
		DROP CONSTRAINT UC_SecuritizationAnalysis_DataSetId_VersionId_ScenarioId_TypeId
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PaceAssessmentRatePlan_TermSetId_PropertyStateId_CouponRate_BuyDownRate_TermInYears'))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		DROP CONSTRAINT UC_PaceAssessmentRatePlan_TermSetId_PropertyStateId_CouponRate_BuyDownRate_TermInYears
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PaceAssessmentRatePlan_Description'))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		DROP CONSTRAINT UC_PaceAssessmentRatePlan_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PrepaymentPenaltyPlan_Description'))
BEGIN
	ALTER TABLE dream.PrepaymentPenaltyPlan
		DROP CONSTRAINT UC_PrepaymentPenaltyPlan_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationNode_DataSetId_TrancheDetailId'))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		DROP CONSTRAINT UC_SecuritizationNode_DataSetId_TrancheDetailId
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_AllowedMonth_ShortDescription_LongDescription'))
BEGIN
	ALTER TABLE dream.AllowedMonth
		DROP CONSTRAINT UC_AllowedMonth_ShortDescription_LongDescription
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_AvailableFundsRetrievalType_Description'))
BEGIN
	ALTER TABLE dream.AvailableFundsRetrievalType
		DROP CONSTRAINT UC_AvailableFundsRetrievalType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_CompoundingConvention_Description'))
BEGIN
	ALTER TABLE dream.CompoundingConvention
		DROP CONSTRAINT UC_CompoundingConvention_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_DayCountConvention_Description'))
BEGIN
	ALTER TABLE dream.DayCountConvention
		DROP CONSTRAINT UC_DayCountConvention_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_FundsDistributionType_Description'))
BEGIN
	ALTER TABLE dream.FundsDistributionType
		DROP CONSTRAINT UC_FundsDistributionType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_MarketDataType_Description'))
BEGIN
	ALTER TABLE dream.MarketDataType
		DROP CONSTRAINT UC_MarketDataType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PaymentConvention_Description'))
BEGIN
	ALTER TABLE dream.PaymentConvention
		DROP CONSTRAINT UC_PaymentConvention_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PerformanceAssumptionType_Abbreviation'))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionType
		DROP CONSTRAINT UC_PerformanceAssumptionType_Abbreviation
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PricingType_Description'))
BEGIN
	ALTER TABLE dream.PricingType
		DROP CONSTRAINT UC_PricingType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_RateIndex_Description'))
BEGIN
	ALTER TABLE dream.RateIndex
		DROP CONSTRAINT UC_RateIndex_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_RateIndexGroup_Description'))
BEGIN
	ALTER TABLE dream.RateIndexGroup
		DROP CONSTRAINT UC_RateIndexGroup_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_RedemptionLogicType_Description'))
BEGIN
	ALTER TABLE dream.RedemptionLogicType
		DROP CONSTRAINT UC_RedemptionLogicType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationInputType_Description'))
BEGIN
	ALTER TABLE dream.SecuritizationInputType
		DROP CONSTRAINT UC_SecuritizationInputType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationResultType_Description'))
BEGIN
	ALTER TABLE dream.SecuritizationResultType
		DROP CONSTRAINT UC_SecuritizationResultType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_TrancheCashFlowType_Description'))
BEGIN
	ALTER TABLE dream.TrancheCashFlowType
		DROP CONSTRAINT UC_TrancheCashFlowType_Description
END

IF (EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_TrancheType_Description'))
BEGIN
	ALTER TABLE dream.TrancheType
		DROP CONSTRAINT UC_TrancheType_Description
END