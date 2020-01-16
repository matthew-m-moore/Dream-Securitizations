SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheCashFlowType')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheCashFlowType 
END
GO

CREATE PROCEDURE dream.InsertTrancheCashFlowType 
(
	@TrancheCashFlowTypeDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.TrancheCashFlowType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , TrancheCashFlowTypeDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @TrancheCashFlowTypeDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS TrancheCashFlowTypeId
END