SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.UpdateSecuritizationAnalysisOwner')))
BEGIN
	DROP PROCEDURE dream.UpdateSecuritizationAnalysisOwner 
END
GO

-- Note, this update procedure intentionally locks access to all versions under the same shared data set ID
CREATE PROCEDURE dream.UpdateSecuritizationAnalysisOwner 
(
	@SecuritizationAnalysisDataSetId int,
	@IsReadOnlyToOthers bit
)
AS
BEGIN
	UPDATE dream.SecuritizationAnalysisOwner
		 SET   UpdatedDate = GETDATE()
		 	 , UpdatedBy = SUSER_NAME()
			 , IsReadOnlyToOthers = @IsReadOnlyToOthers
		 WHERE SecuritizationAnalysisDataSetId = @SecuritizationAnalysisDataSetId
END