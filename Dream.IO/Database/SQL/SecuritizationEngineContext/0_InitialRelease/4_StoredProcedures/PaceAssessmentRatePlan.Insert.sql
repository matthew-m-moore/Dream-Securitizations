SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRatePlan')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRatePlan 
END
GO

CREATE PROCEDURE dream.InsertPaceAssessmentRatePlan 
(
	@PaceAssessmentRatePlanTermSetId int,
	@PropertyStateId int,
	@CouponRate float,
	@BuyDownRate float,
	@TermInYears int
)
AS
BEGIN
	INSERT INTO dream.PaceAssessmentRatePlan
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PaceAssessmentRatePlanTermSetId
			   , PropertyStateId
			   , CouponRate
			   , BuyDownRate
			   , TermInYears
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PaceAssessmentRatePlanTermSetId
			   , @PropertyStateId
			   , @CouponRate
			   , @BuyDownRate
			   , @TermInYears
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PaceAssessmentRatePlanId
END