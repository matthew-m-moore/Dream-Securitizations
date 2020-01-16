USE CapitalMarkets
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAggregationGroupAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertAggregationGroupAssignment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAggregationGroupDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertAggregationGroupDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAllowedMonth')))
BEGIN
	DROP PROCEDURE dream.InsertAllowedMonth 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertApplicationUser')))
BEGIN
	DROP PROCEDURE dream.InsertApplicationUser 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.UpdateApplicationUser')))
BEGIN
	DROP PROCEDURE dream.UpdateApplicationUser 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAvailableFundsRetrievalDetail')))
BEGIN
	DROP PROCEDURE dream.InsertAvailableFundsRetrievalDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAvailableFundsRetrievalType')))
BEGIN
	DROP PROCEDURE dream.InsertAvailableFundsRetrievalType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertBalanceCapAndFloorDetail')))
BEGIN
	DROP PROCEDURE dream.InsertBalanceCapAndFloorDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertBalanceCapAndFloorSet')))
BEGIN
	DROP PROCEDURE dream.InsertBalanceCapAndFloorSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertCollateralizedSecuritizationDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertCollateralizedSecuritizationDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertCollateralizedSecuritizationTranche')))
BEGIN
	DROP PROCEDURE dream.InsertCollateralizedSecuritizationTranche 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertCompoundingConvention')))
BEGIN
	DROP PROCEDURE dream.InsertCompoundingConvention 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertDayCountConvention')))
BEGIN
	DROP PROCEDURE dream.InsertDayCountConvention 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertFeeDetail')))
BEGIN
	DROP PROCEDURE dream.InsertFeeDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertFeeGroupDetail')))
BEGIN
	DROP PROCEDURE dream.InsertFeeGroupDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertFundsDistributionType')))
BEGIN
	DROP PROCEDURE dream.InsertFundsDistributionType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketData')))
BEGIN
	DROP PROCEDURE dream.InsertMarketData 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertMarketDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketDataType')))
BEGIN
	DROP PROCEDURE dream.InsertMarketDataType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketRateEnvironment')))
BEGIN
	DROP PROCEDURE dream.InsertMarketRateEnvironment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRatePlanTermSet')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRatePlanTermSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRatePlan')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRatePlan 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRecord')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRecord 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRecordDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRecordDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaymentConvention')))
BEGIN
	DROP PROCEDURE dream.InsertPaymentConvention 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPerformanceAssumptionAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertPerformanceAssumptionAssignment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPerformanceAssumptionDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertPerformanceAssumptionDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPerformanceAssumptionType')))
BEGIN
	DROP PROCEDURE dream.InsertPerformanceAssumptionType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPrepaymentPenaltyPlan')))
BEGIN
	DROP PROCEDURE dream.InsertPrepaymentPenaltyPlan 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPrepaymentPenaltyPlanDetail')))
BEGIN
	DROP PROCEDURE dream.InsertPrepaymentPenaltyPlanDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPricingGroupAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertPricingGroupAssignment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPricingGroupDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertPricingGroupDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPricingType')))
BEGIN
	DROP PROCEDURE dream.InsertPricingType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPriorityOfPaymentsAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertPriorityOfPaymentsAssignment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPriorityOfPaymentsSet')))
BEGIN
	DROP PROCEDURE dream.InsertPriorityOfPaymentsSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateCurveData')))
BEGIN
	DROP PROCEDURE dream.InsertRateCurveData 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateCurveDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertRateCurveDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateIndex')))
BEGIN
	DROP PROCEDURE dream.InsertRateIndex 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateIndexGroup')))
BEGIN
	DROP PROCEDURE dream.InsertRateIndexGroup
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicAllowedMonthsDetail')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicAllowedMonthsDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicAllowedMonthsSet')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicAllowedMonthsSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicType')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionTranchesDetail')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionTranchesDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionTranchesSet')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionTranchesSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertReserveAccountsDetail')))
BEGIN
	DROP PROCEDURE dream.InsertReserveAccountsDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertReserveAccountsSet')))
BEGIN
	DROP PROCEDURE dream.InsertReserveAccountsSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysis')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysis 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisOwner')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisOwner 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.UpdateSecuritizationAnalysisOwner')))
BEGIN
	DROP PROCEDURE dream.UpdateSecuritizationAnalysisOwner 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisComment')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisComment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.UpdateSecuritizationAnalysisComment')))
BEGIN
	DROP PROCEDURE dream.UpdateSecuritizationAnalysisComment 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisResult')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisResult 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisSummary')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisSummary
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisInput')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisInput 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationInputType')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationInputType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationResultType')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationResultType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationNode')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationNode 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationNodeDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationNodeDataSet 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheCashFlowType')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheCashFlowType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheCoupon')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheCoupon 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheDetail')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheDetail 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheType')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheType 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertVector')))
BEGIN
	DROP PROCEDURE dream.InsertVector 
END

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertVectorParent')))
BEGIN
	DROP PROCEDURE dream.InsertVectorParent 
END