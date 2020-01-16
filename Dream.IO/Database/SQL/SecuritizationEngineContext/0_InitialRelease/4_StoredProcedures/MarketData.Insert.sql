SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketData')))
BEGIN
	DROP PROCEDURE dream.InsertMarketData 
END
GO

CREATE PROCEDURE dream.InsertMarketData 
(
	@MarketDataSetId int,
	@MarketDateTime datetime,
	@DataValue float,
	@RateIndexId int,
	@MarketDataTypeId int
)
AS
BEGIN
	INSERT INTO dream.MarketData
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , MarketDataSetId
			   , MarketDateTime
			   , DataValue
			   , RateIndexId
			   , MarketDataTypeId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @MarketDataSetId
			   , @MarketDateTime
			   , @DataValue
			   , @RateIndexId
			   , @MarketDataTypeId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS MarketDataId
END