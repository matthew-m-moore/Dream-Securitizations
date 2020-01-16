SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisDataSet 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysisDataSet 
(
	@CutOffDate date,
	@SecuritizationAnalysisDataSetDescription varchar(250),
	@IsTemplate bit,
	@IsResecuritization bit
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysisDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , SecuritizationAnalysisDataSetDescription
			   , IsTemplate
			   , IsResecuritization
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @SecuritizationAnalysisDataSetDescription
			   , @IsTemplate
			   , @IsResecuritization
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisDataSetId,
		   GETDATE() AS CreatedDate
END