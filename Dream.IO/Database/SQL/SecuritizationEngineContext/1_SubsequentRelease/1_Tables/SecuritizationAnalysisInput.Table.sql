SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF COL_LENGTH('dream.SecuritizationAnalysisInput', 'UsePreFundingStartDate') IS NULL
BEGIN
    ALTER TABLE dream.SecuritizationAnalysisInput
		ADD UsePreFundingStartDate bit NULL
END