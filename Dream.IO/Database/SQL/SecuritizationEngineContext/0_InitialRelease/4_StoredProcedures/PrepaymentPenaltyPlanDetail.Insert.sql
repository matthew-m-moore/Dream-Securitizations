SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPrepaymentPenaltyPlanDetail')))
BEGIN
	DROP PROCEDURE dream.InsertPrepaymentPenaltyPlanDetail 
END
GO

CREATE PROCEDURE dream.InsertPrepaymentPenaltyPlanDetail 
(
	@PrepaymentPenaltyPlanId int,
	@EndingMonthlyPeriodOfPenalty int,
	@PenaltyAmount float,
	@PenaltyType varchar(50)
)
AS
BEGIN
	INSERT INTO dream.PrepaymentPenaltyPlanDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PrepaymentPenaltyPlanId
			   , EndingMonthlyPeriodOfPenalty
			   , PenaltyAmount
			   , PenaltyType
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PrepaymentPenaltyPlanId
			   , @EndingMonthlyPeriodOfPenalty
			   , @PenaltyAmount
			   , @PenaltyType
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PrepaymentPenaltyPlanDetailId
END