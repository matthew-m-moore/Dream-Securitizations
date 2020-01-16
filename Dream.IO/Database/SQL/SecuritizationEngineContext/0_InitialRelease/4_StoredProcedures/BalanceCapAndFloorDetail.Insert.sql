SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertBalanceCapAndFloorDetail')))
BEGIN
	DROP PROCEDURE dream.InsertBalanceCapAndFloorDetail 
END
GO

CREATE PROCEDURE dream.InsertBalanceCapAndFloorDetail 
(
	@BalanceCapAndFloorSetId int,
	@BalanceCapOrFloor varchar(10),
	@PercentageOrDollarAmount varchar(15),
	@CapOrFloorValue float,
	@EffectiveDate date
)
AS
BEGIN
	INSERT INTO dream.BalanceCapAndFloorDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , BalanceCapAndFloorSetId
			   , BalanceCapOrFloor
			   , PercentageOrDollarAmount
			   , CapOrFloorValue
			   , EffectiveDate
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @BalanceCapAndFloorSetId
			   , @BalanceCapOrFloor
			   , @PercentageOrDollarAmount
			   , @CapOrFloorValue
			   , @EffectiveDate
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS BalanceCapAndFloorDetailId
END