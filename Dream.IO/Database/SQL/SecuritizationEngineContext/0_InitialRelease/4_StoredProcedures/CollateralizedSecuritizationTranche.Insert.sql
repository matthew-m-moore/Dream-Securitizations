SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertCollateralizedSecuritizationTranche')))
BEGIN
	DROP PROCEDURE dream.InsertCollateralizedSecuritizationTranche 
END
GO

CREATE PROCEDURE dream.InsertCollateralizedSecuritizationTranche 
(
	@CollateralizedSecuritizationDataSetId int,
	@SecuritizationAnalysisDataSetId int,
	@SecuritizationAnalysisVersionId int,
	@SecuritizatizedTrancheDetailId int,
	@SecuritizatizedTranchePercentage float
)
AS
BEGIN
	INSERT INTO dream.CollateralizedSecuritizationTranche
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CollateralizedSecuritizationDataSetId
			   , SecuritizationAnalysisDataSetId
			   , SecuritizationAnalysisVersionId
			   , SecuritizatizedTrancheDetailId
			   , SecuritizatizedTranchePercentage
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CollateralizedSecuritizationDataSetId
			   , @SecuritizationAnalysisDataSetId
			   , @SecuritizationAnalysisVersionId
			   , @SecuritizatizedTrancheDetailId
			   , @SecuritizatizedTranchePercentage
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS CollateralizedSecuritizationTrancheId
END