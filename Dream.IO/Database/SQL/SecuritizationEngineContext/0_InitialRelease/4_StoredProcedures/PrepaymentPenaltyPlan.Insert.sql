SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPrepaymentPenaltyPlan')))
BEGIN
	DROP PROCEDURE dream.InsertPrepaymentPenaltyPlan 
END
GO

CREATE PROCEDURE dream.InsertPrepaymentPenaltyPlan 
(
	@PrepaymentPenaltyPlanDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PrepaymentPenaltyPlan
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PrepaymentPenaltyPlanDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PrepaymentPenaltyPlanDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PrepaymentPenaltyPlanId
END