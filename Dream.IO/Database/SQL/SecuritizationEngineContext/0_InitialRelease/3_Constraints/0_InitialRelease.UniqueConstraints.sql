USE CapitalMarkets
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT *
				FROM SYS.INDEXES
				WHERE NAME = 'UIDX_SecuritizationAnalysisComment'))
BEGIN
	CREATE UNIQUE INDEX UIDX_SecuritizationAnalysisComment
	ON dream.SecuritizationAnalysisComment(SecuritizationAnalysisDataSetId, SecuritizationAnalysisVersionId)
	WHERE IsVisible = 1
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationAnalysisOwner_DataSetId_VersionId'))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysisOwner
		ADD CONSTRAINT UC_SecuritizationAnalysisOwner_DataSetId_VersionId
			UNIQUE (SecuritizationAnalysisDataSetId, SecuritizationAnalysisVersionId)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_ApplicationUser_NetworkUserNameIdentifier'))
BEGIN
	ALTER TABLE dream.ApplicationUser
		ADD CONSTRAINT UC_ApplicationUser_NetworkUserNameIdentifier
			UNIQUE (NetworkUserNameIdentifier)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_ApplicationUser_ApplicationDisplayableNickName'))
BEGIN
	ALTER TABLE dream.ApplicationUser
		ADD CONSTRAINT UC_ApplicationUser_ApplicationDisplayableNickName
			UNIQUE (ApplicationDisplayableNickName)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationAnalysis_DataSetId_VersionId_ScenarioId_TypeId'))
BEGIN
	ALTER TABLE dream.SecuritizationAnalysis
		ADD CONSTRAINT UC_SecuritizationAnalysis_DataSetId_VersionId_ScenarioId_TypeId
			UNIQUE (SecuritizationAnalysisDataSetId, SecuritizationAnalysisVersionId, SecuritizationAnalysisScenarioId, SecuritizationInputTypeId)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PaceAssessmentRatePlan_TermSetId_PropertyStateId_CouponRate_BuyDownRate_TermInYears'))
BEGIN
	ALTER TABLE dream.PaceAssessmentRatePlan
		ADD CONSTRAINT UC_PaceAssessmentRatePlan_TermSetId_PropertyStateId_CouponRate_BuyDownRate_TermInYears
			UNIQUE (PaceAssessmentRatePlanTermSetId, PropertyStateId, CouponRate, BuyDownRate, TermInYears)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PrepaymentPenaltyPlan_Description'))
BEGIN
	ALTER TABLE dream.PrepaymentPenaltyPlan
		ADD CONSTRAINT UC_PrepaymentPenaltyPlan_Description
			UNIQUE (PrepaymentPenaltyPlanDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationNode_DataSetId_TrancheDetailId'))
BEGIN
	ALTER TABLE dream.SecuritizationNode
		ADD CONSTRAINT UC_SecuritizationNode_DataSetId_TrancheDetailId
			UNIQUE (SecuritizationNodeDataSetId, TrancheDetailId)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_AllowedMonth_ShortDescription_LongDescription'))
BEGIN
	ALTER TABLE dream.AllowedMonth
		ADD CONSTRAINT UC_AllowedMonth_ShortDescription_LongDescription
			UNIQUE (AllowedMonthShortDescription, AllowedMonthLongDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_AvailableFundsRetrievalType_Description'))
BEGIN
	ALTER TABLE dream.AvailableFundsRetrievalType
		ADD CONSTRAINT UC_AvailableFundsRetrievalType_Description
			UNIQUE (AvailableFundsRetrievalTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_CompoundingConvention_Description'))
BEGIN
	ALTER TABLE dream.CompoundingConvention
		ADD CONSTRAINT UC_CompoundingConvention_Description
			UNIQUE (CompoundingConventionDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_DayCountConvention_Description'))
BEGIN
	ALTER TABLE dream.DayCountConvention
		ADD CONSTRAINT UC_DayCountConvention_Description
			UNIQUE (DayCountConventionDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_FundsDistributionType_Description'))
BEGIN
	ALTER TABLE dream.FundsDistributionType
		ADD CONSTRAINT UC_FundsDistributionType_Description
			UNIQUE (FundsDistributionTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_MarketDataType_Description'))
BEGIN
	ALTER TABLE dream.MarketDataType
		ADD CONSTRAINT UC_MarketDataType_Description
			UNIQUE (MarketDataTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PaymentConvention_Description'))
BEGIN
	ALTER TABLE dream.PaymentConvention
		ADD CONSTRAINT UC_PaymentConvention_Description
			UNIQUE (PaymentConventionDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PerformanceAssumptionType_Abbreviation'))
BEGIN
	ALTER TABLE dream.PerformanceAssumptionType
		ADD CONSTRAINT UC_PerformanceAssumptionType_Abbreviation
			UNIQUE (PerformanceAssumptionTypeAbbreviation)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_PricingType_Description'))
BEGIN
	ALTER TABLE dream.PricingType
		ADD CONSTRAINT UC_PricingType_Description
			UNIQUE (PricingTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_RateIndex_Description'))
BEGIN
	ALTER TABLE dream.RateIndex
		ADD CONSTRAINT UC_RateIndex_Description
			UNIQUE (RateIndexDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_RateIndexGroup_Description'))
BEGIN
	ALTER TABLE dream.RateIndexGroup
		ADD CONSTRAINT UC_RateIndexGroup_Description
			UNIQUE (RateIndexGroupDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_RedemptionLogicType_Description'))
BEGIN
	ALTER TABLE dream.RedemptionLogicType
		ADD CONSTRAINT UC_RedemptionLogicType_Description
			UNIQUE (RedemptionLogicTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationInputType_Description'))
BEGIN
	ALTER TABLE dream.SecuritizationInputType
		ADD CONSTRAINT UC_SecuritizationInputType_Description
			UNIQUE (SecuritizationInputTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_SecuritizationResultType_Description'))
BEGIN
	ALTER TABLE dream.SecuritizationResultType
		ADD CONSTRAINT UC_SecuritizationResultType_Description
			UNIQUE (SecuritizationResultTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_TrancheCashFlowType_Description'))
BEGIN
	ALTER TABLE dream.TrancheCashFlowType
		ADD CONSTRAINT UC_TrancheCashFlowType_Description
			UNIQUE (TrancheCashFlowTypeDescription)
END

IF (NOT EXISTS (SELECT * 
				FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
				WHERE CONSTRAINT_TYPE = 'UNIQUE'
				  AND CONSTRAINT_SCHEMA = 'dream'
				  AND CONSTRAINT_NAME = 'UC_TrancheType_Description'))
BEGIN
	ALTER TABLE dream.TrancheType
		ADD CONSTRAINT UC_TrancheType_Description
			UNIQUE (TrancheTypeDescription)
END