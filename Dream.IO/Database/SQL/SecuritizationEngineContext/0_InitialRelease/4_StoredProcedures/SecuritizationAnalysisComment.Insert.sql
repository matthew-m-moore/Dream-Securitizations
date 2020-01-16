SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisComment')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisComment 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysisComment 
(
	@SecuritizationAnalysisDataSetId int,
	@SecuritizationAnalysisVersionId int,	
	@CommentText varchar(max),
	@IsVisible bit
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysisComment
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationAnalysisDataSetId
			   , SecuritizationAnalysisVersionId
			   , CommentText
			   , IsVisible
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationAnalysisDataSetId
			   , @SecuritizationAnalysisVersionId
			   , @CommentText
			   , @IsVisible
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisCommentId
END