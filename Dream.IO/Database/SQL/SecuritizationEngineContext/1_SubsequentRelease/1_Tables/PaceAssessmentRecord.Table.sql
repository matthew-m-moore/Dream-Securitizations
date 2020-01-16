SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF COL_LENGTH('dream.PaceAssessmentRecord', 'IsPreFundingRepline') IS NULL
BEGIN
    ALTER TABLE dream.PaceAssessmentRecord
		ADD IsPreFundingRepline bit NULL
END

IF COL_LENGTH('dream.PaceAssessmentRecord', 'LastPreFundDate') IS NULL
BEGIN
    ALTER TABLE dream.PaceAssessmentRecord
		ADD LastPreFundDate date NULL
END

IF COL_LENGTH('dream.PaceAssessmentRecord', 'PreFundingStartDate') IS NULL
BEGIN
    ALTER TABLE dream.PaceAssessmentRecord
		ADD PreFundingStartDate date NULL
END