SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRecordDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRecordDataSet 
END
GO

CREATE PROCEDURE dream.InsertPaceAssessmentRecordDataSet 
(
	@CutOffDate date,
	@PaceAssessmentRecordDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PaceAssessmentRecordDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , PaceAssessmentRecordDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @PaceAssessmentRecordDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PaceAssessmentRecordDataSetId
END