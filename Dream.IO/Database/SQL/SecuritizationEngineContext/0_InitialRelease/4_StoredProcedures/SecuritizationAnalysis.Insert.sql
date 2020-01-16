SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysis')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysis 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysis 
(
	@SecuritizationAnalysisDataSetId int,
	@SecuritizationAnalysisVersionId int,
	@SecuritizationAnalysisScenarioId int,
	@SecuritizationInputTypeId int,
	@SecuritizationInputTypeDataSetId int
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysis
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationAnalysisDataSetId
			   , SecuritizationAnalysisVersionId
			   , SecuritizationAnalysisScenarioId
			   , SecuritizationInputTypeId
			   , SecuritizationInputTypeDataSetId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationAnalysisDataSetId
			   , @SecuritizationAnalysisVersionId
			   , @SecuritizationAnalysisScenarioId
			   , @SecuritizationInputTypeId
			   , @SecuritizationInputTypeDataSetId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisId
END