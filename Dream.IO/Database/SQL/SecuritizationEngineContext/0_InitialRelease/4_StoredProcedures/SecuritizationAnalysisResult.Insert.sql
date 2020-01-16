SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisResult')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisResult 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysisResult 
(
	@SecuritizationAnalysisDataSetId int,
	@SecuritizationAnalysisVersionId int,
	@SecuritizationAnalysisScenarioId int,
	@SecuritizationTrancheDetailId int,
	@SecuritizationNodeName varchar(250),
	@SecuritizationResultTypeId int,
	@SecuritizationResultValue float
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysisResult
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationAnalysisDataSetId
			   , SecuritizationAnalysisVersionId
			   , SecuritizationAnalysisScenarioId
			   , SecuritizationTrancheDetailId
			   , SecuritizationNodeName
			   , SecuritizationResultTypeId
			   , SecuritizationResultValue
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
			   , @SecuritizationTrancheDetailId
			   , @SecuritizationNodeName
			   , @SecuritizationResultTypeId
			   , @SecuritizationResultValue
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisResultId
END