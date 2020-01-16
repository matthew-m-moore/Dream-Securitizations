SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionTranchesDetail')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionTranchesDetail 
END
GO

CREATE PROCEDURE dream.InsertRedemptionTranchesDetail 
(
	@RedemptionTranchesSetId int,
	@TrancheDetailId int
)
AS
BEGIN
	INSERT INTO dream.RedemptionTranchesDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RedemptionTranchesSetId
			   , TrancheDetailId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RedemptionTranchesSetId
			   , @TrancheDetailId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RedemptionTranchesDetailId
END