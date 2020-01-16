SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRatePlanTermSet')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRatePlanTermSet 
END
GO

CREATE PROCEDURE dream.InsertPaceAssessmentRatePlanTermSet 
(
	@CutOffDate date,
	@PaceAssessmentRatePlanTermSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PaceAssessmentRatePlanTermSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , PaceAssessmentRatePlanTermSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @PaceAssessmentRatePlanTermSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PaceAssessmentRatePlanTermSetId
END