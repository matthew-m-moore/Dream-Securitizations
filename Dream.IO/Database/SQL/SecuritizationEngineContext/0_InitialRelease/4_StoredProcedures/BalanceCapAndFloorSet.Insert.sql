SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertBalanceCapAndFloorSet')))
BEGIN
	DROP PROCEDURE dream.InsertBalanceCapAndFloorSet 
END
GO

CREATE PROCEDURE dream.InsertBalanceCapAndFloorSet 
AS
BEGIN
	INSERT INTO dream.BalanceCapAndFloorSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS BalanceCapAndFloorSetId
END