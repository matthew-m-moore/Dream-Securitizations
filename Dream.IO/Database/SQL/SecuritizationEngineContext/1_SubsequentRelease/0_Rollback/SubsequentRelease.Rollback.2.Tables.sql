USE CapitalMarkets
GO

IF COL_LENGTH('dream.ReserveAccountsDetail', 'TrancheCashFlowTypeId') IS NOT NULL
BEGIN
    ALTER TABLE dream.ReserveAccountsDetail
		DROP COLUMN TrancheCashFlowTypeId
END

IF COL_LENGTH('dream.PaceAssessmentRecord', 'IsPreFundingRepline') IS NOT NULL
BEGIN
    ALTER TABLE dream.PaceAssessmentRecord
		DROP COLUMN IsPreFundingRepline
END

IF COL_LENGTH('dream.PaceAssessmentRecord', 'LastPreFundDate') IS NOT NULL
BEGIN
    ALTER TABLE dream.PaceAssessmentRecord
		DROP COLUMN LastPreFundDate
END

IF COL_LENGTH('dream.PaceAssessmentRecord', 'PreFundingStartDate') IS NOT NULL
BEGIN
    ALTER TABLE dream.PaceAssessmentRecord
		DROP COLUMN PreFundingStartDate
END

IF COL_LENGTH('dream.SecuritizationAnalysisInput', 'UsePreFundingStartDate') IS NOT NULL
BEGIN
    ALTER TABLE dream.SecuritizationAnalysisInput
		DROP COLUMN UsePreFundingStartDate
END