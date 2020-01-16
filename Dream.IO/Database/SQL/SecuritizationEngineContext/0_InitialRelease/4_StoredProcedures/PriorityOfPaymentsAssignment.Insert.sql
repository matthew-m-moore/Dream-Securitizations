SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPriorityOfPaymentsAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertPriorityOfPaymentsAssignment 
END
GO

CREATE PROCEDURE dream.InsertPriorityOfPaymentsAssignment 
(
	@PriorityOfPaymentsSetId int,
	@SeniorityRanking int,
	@TrancheDetailId int,
	@TrancheCashFlowTypeId int
)
AS
BEGIN
	INSERT INTO dream.PriorityOfPaymentsAssignment
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PriorityOfPaymentsSetId
			   , SeniorityRanking
			   , TrancheDetailId
			   , TrancheCashFlowTypeId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PriorityOfPaymentsSetId
			   , @SeniorityRanking
			   , @TrancheDetailId
			   , @TrancheCashFlowTypeId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PriorityOfPaymentsAssignmentId
END


