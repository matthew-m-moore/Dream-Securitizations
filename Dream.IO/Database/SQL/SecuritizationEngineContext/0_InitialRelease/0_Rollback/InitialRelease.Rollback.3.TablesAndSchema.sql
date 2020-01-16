USE CapitalMarkets
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'ApplicationUser'))
BEGIN
	DROP TABLE dream.ApplicationUser
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PaceAssessmentRatePlanTermSet'))
BEGIN
	DROP TABLE dream.PaceAssessmentRatePlanTermSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PaceAssessmentRatePlan'))
BEGIN
	DROP TABLE dream.PaceAssessmentRatePlan
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PaceAssessmentRecord'))
BEGIN
	DROP TABLE dream.PaceAssessmentRecord
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PaceAssessmentRecordDataSet'))
BEGIN
	DROP TABLE dream.PaceAssessmentRecordDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'CollateralizedSecuritizationTranche'))
BEGIN
	DROP TABLE dream.CollateralizedSecuritizationTranche
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'CollateralizedSecuritizationDataSet'))
BEGIN
	DROP TABLE dream.CollateralizedSecuritizationDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PrepaymentPenaltyPlan'))
BEGIN
	DROP TABLE dream.PrepaymentPenaltyPlan
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PrepaymentPenaltyPlanDetail'))
BEGIN
	DROP TABLE dream.PrepaymentPenaltyPlanDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PerformanceAssumptionType'))
BEGIN
	DROP TABLE dream.PerformanceAssumptionType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PerformanceAssumptionAssignment'))
BEGIN
	DROP TABLE dream.PerformanceAssumptionAssignment
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PerformanceAssumptionDataSet'))
BEGIN
	DROP TABLE dream.PerformanceAssumptionDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'Vector'))
BEGIN
	DROP TABLE dream.Vector
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'VectorParent'))
BEGIN
	DROP TABLE dream.VectorParent
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AggregationGroupAssignment'))
BEGIN
	DROP TABLE dream.AggregationGroupAssignment
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AggregationGroupDataSet'))
BEGIN
	DROP TABLE dream.AggregationGroupDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PricingType'))
BEGIN
	DROP TABLE dream.PricingType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PricingGroupAssignment'))
BEGIN
	DROP TABLE dream.PricingGroupAssignment
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PricingGroupDataSet'))
BEGIN
	DROP TABLE dream.PricingGroupDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RateCurveData'))
BEGIN
	DROP TABLE dream.RateCurveData
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RateCurveDataSet'))
BEGIN
	DROP TABLE dream.RateCurveDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'MarketDataType'))
BEGIN
	DROP TABLE dream.MarketDataType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'MarketData'))
BEGIN
	DROP TABLE dream.MarketData
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'MarketDataSet'))
BEGIN
	DROP TABLE dream.MarketDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'MarketRateEnvironment'))
BEGIN
	DROP TABLE dream.MarketRateEnvironment
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'CompoundingConvention'))
BEGIN
	DROP TABLE dream.CompoundingConvention
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'DayCountConvention'))
BEGIN
	DROP TABLE dream.DayCountConvention
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PaymentConvention'))
BEGIN
	DROP TABLE dream.PaymentConvention
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RateIndex'))
BEGIN
	DROP TABLE dream.RateIndex
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RateIndexGroup'))
BEGIN
	DROP TABLE dream.RateIndexGroup
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisDataSet'))
BEGIN
	DROP TABLE dream.SecuritizationAnalysisDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysis'))
BEGIN	
	DROP TABLE dream.SecuritizationAnalysis
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisOwner'))
BEGIN	
	DROP TABLE dream.SecuritizationAnalysisOwner
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisComment'))
BEGIN	
	DROP TABLE dream.SecuritizationAnalysisComment
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisResult'))
BEGIN	
	DROP TABLE dream.SecuritizationAnalysisResult
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisSummary'))
BEGIN	
	DROP TABLE dream.SecuritizationAnalysisSummary
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisInput'))
BEGIN	
	DROP TABLE dream.SecuritizationAnalysisInput
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationInputType'))
BEGIN
	DROP TABLE dream.SecuritizationInputType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationResultType'))
BEGIN
	DROP TABLE dream.SecuritizationResultType
END


IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'FundsDistributionType'))
BEGIN
	DROP TABLE dream.FundsDistributionType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationNode'))
BEGIN
	DROP TABLE dream.SecuritizationNode
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationNodeDataSet'))
BEGIN
	DROP TABLE dream.SecuritizationNodeDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheDetail'))
BEGIN
	DROP TABLE dream.TrancheDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheCoupon'))
BEGIN
	DROP TABLE dream.TrancheCoupon
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheType'))
BEGIN
	DROP TABLE dream.TrancheType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'FeeGroupDetail'))
BEGIN
	DROP TABLE dream.FeeGroupDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'FeeDetail'))
BEGIN
	DROP TABLE dream.FeeDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RedemptionLogicDataSet'))
BEGIN
	DROP TABLE dream.RedemptionLogicDataSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RedemptionLogicType'))
BEGIN
	DROP TABLE dream.RedemptionLogicType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RedemptionTranchesSet'))
BEGIN
	DROP TABLE dream.RedemptionTranchesSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RedemptionTranchesDetail'))
BEGIN
	DROP TABLE dream.RedemptionTranchesDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RedemptionLogicAllowedMonthsSet'))
BEGIN
	DROP TABLE dream.RedemptionLogicAllowedMonthsSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'RedemptionLogicAllowedMonthsDetail'))
BEGIN
	DROP TABLE dream.RedemptionLogicAllowedMonthsDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AllowedMonth'))
BEGIN
	DROP TABLE dream.AllowedMonth
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'ReserveAccountsSet'))
BEGIN
	DROP TABLE dream.ReserveAccountsSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'ReserveAccountsDetail'))
BEGIN
	DROP TABLE dream.ReserveAccountsDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'BalanceCapAndFloorSet'))
BEGIN
	DROP TABLE dream.BalanceCapAndFloorSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'BalanceCapAndFloorDetail'))
BEGIN
	DROP TABLE dream.BalanceCapAndFloorDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AvailableFundsRetrievalDetail'))
BEGIN
	DROP TABLE dream.AvailableFundsRetrievalDetail
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'AvailableFundsRetrievalType'))
BEGIN
	DROP TABLE dream.AvailableFundsRetrievalType
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PriorityOfPaymentsSet'))
BEGIN
	DROP TABLE dream.PriorityOfPaymentsSet
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PriorityOfPaymentsAssignment'))
BEGIN
	DROP TABLE dream.PriorityOfPaymentsAssignment
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheCashFlowType'))
BEGIN
	DROP TABLE dream.TrancheCashFlowType
END

-- Note, schema drop has to be the final command in a rollback
IF (EXISTS (SELECT * FROM SYS.SCHEMAS WHERE NAME = 'dream'))
BEGIN
	DROP SCHEMA dream
END