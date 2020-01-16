SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisSummary')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisSummary 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysisSummary 
(
	@SecuritizationAnalysisDataSetId int,
	@SecuritizationAnalysisVersionId int,
	@SecuritizationAnalysisScenarioId int,
	@SecuritizationTrancheDetailId int,
	@SecuritizationNodeName varchar(250),
	@SecuritizationTrancheType varchar(250),
	@SecuritizationTrancheRating varchar(250)
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysisSummary
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
			   , SecuritizationTrancheType
			   , SecuritizationTrancheRating
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
			   , @SecuritizationTrancheType
			   , @SecuritizationTrancheRating
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisSummaryId
END