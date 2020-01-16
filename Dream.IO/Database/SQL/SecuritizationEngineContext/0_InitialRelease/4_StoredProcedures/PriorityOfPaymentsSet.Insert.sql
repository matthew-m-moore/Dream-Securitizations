SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPriorityOfPaymentsSet')))
BEGIN
	DROP PROCEDURE dream.InsertPriorityOfPaymentsSet 
END
GO

CREATE PROCEDURE dream.InsertPriorityOfPaymentsSet 
(
	@CutOffDate date,
	@PriorityOfPaymentsSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PriorityOfPaymentsSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , PriorityOfPaymentsSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @PriorityOfPaymentsSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PriorityOfPaymentsSetId
END