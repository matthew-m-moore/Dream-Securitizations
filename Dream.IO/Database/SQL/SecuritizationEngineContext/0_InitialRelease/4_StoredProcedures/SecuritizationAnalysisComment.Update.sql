SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.UpdateSecuritizationAnalysisComment')))
BEGIN
	DROP PROCEDURE dream.UpdateSecuritizationAnalysisComment 
END
GO

CREATE PROCEDURE dream.UpdateSecuritizationAnalysisComment 
(
	@SecuritizationAnalysisCommentId int,
	@IsVisible bit
)
AS
BEGIN
	UPDATE dream.SecuritizationAnalysisComment
		 SET   UpdatedDate = GETDATE()
		 	 , UpdatedBy = SUSER_NAME()
			 , IsVisible = @IsVisible
		 WHERE SecuritizationAnalysisCommentId = @SecuritizationAnalysisCommentId
END