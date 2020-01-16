SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketDataType')))
BEGIN
	DROP PROCEDURE dream.InsertMarketDataType 
END
GO

CREATE PROCEDURE dream.InsertMarketDataType 
(
	@MarketDataTypeDescription varchar(250),
	@StandardDivisor float
)
AS
BEGIN
	INSERT INTO dream.MarketDataType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , MarketDataTypeDescription
			   , StandardDivisor
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @MarketDataTypeDescription
			   , @StandardDivisor
			   )
	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS MarketDataTypeId
END