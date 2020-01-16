SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicAllowedMonthsDetail')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicAllowedMonthsDetail 
END
GO

CREATE PROCEDURE dream.InsertRedemptionLogicAllowedMonthsDetail 
(
	@RedemptionLogicAllowedMonthsSetId int,
	@AllowedMonthId int
)
AS
BEGIN
	INSERT INTO dream.RedemptionLogicAllowedMonthsDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RedemptionLogicAllowedMonthsSetId
			   , AllowedMonthId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RedemptionLogicAllowedMonthsSetId
			   , @AllowedMonthId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RedemptionLogicAllowedMonthsDetailId
END