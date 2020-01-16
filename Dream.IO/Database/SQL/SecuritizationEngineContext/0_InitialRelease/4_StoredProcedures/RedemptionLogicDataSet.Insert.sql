SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicDataSet 
END
GO

CREATE PROCEDURE dream.InsertRedemptionLogicDataSet 
(
	@CutOffDate date,
	@RedemptionLogicTypeId int,
	@RedemptionLogicAllowedMonthsSetId int,
	@RedemptionTranchesSetId int,
	@RedemptionPriorityOfPaymentsSetId int,
	@PostRedemptionPriorityOfPaymentsSetId int,
	@RedemptionLogicTriggerValue float
)
AS
BEGIN
	INSERT INTO dream.RedemptionLogicDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , RedemptionLogicTypeId
			   , RedemptionLogicAllowedMonthsSetId
			   , RedemptionTranchesSetId
			   , RedemptionPriorityOfPaymentsSetId
			   , PostRedemptionPriorityOfPaymentsSetId
			   , RedemptionLogicTriggerValue
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @RedemptionLogicTypeId
			   , @RedemptionLogicAllowedMonthsSetId
			   , @RedemptionTranchesSetId
			   , @RedemptionPriorityOfPaymentsSetId
			   , @PostRedemptionPriorityOfPaymentsSetId
			   , @RedemptionLogicTriggerValue
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RedemptionLogicDataSetId
END