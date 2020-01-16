SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertReserveAccountsDetail')))
BEGIN
	DROP PROCEDURE dream.InsertReserveAccountsDetail 
END
GO

CREATE PROCEDURE dream.InsertReserveAccountsDetail 
(
	@ReserveAccountsSetId int,
	@TrancheDetailId int,
	@TrancheCashFlowTypeId int
)
AS
BEGIN
	INSERT INTO dream.ReserveAccountsDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , ReserveAccountsSetId
			   , TrancheDetailId
			   , TrancheCashFlowTypeId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @ReserveAccountsSetId
			   , @TrancheDetailId
			   , @TrancheCashFlowTypeId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS ReserveAccountsDetailId
END