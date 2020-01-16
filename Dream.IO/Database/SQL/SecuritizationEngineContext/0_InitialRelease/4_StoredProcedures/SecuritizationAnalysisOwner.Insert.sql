SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisOwner')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisOwner 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysisOwner 
(
	@SecuritizationAnalysisDataSetId int,
	@SecuritizationAnalysisVersionId int,	
	@ApplicationUserId int,
	@IsReadOnlyToOthers bit
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysisOwner
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationAnalysisDataSetId
			   , SecuritizationAnalysisVersionId
			   , ApplicationUserId
			   , IsReadOnlyToOthers
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationAnalysisDataSetId
			   , @SecuritizationAnalysisVersionId
			   , @ApplicationUserId
			   , @IsReadOnlyToOthers
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisOwnerId
END