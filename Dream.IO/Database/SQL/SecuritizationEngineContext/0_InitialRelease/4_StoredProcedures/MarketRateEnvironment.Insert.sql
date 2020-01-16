SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketRateEnvironment')))
BEGIN
	DROP PROCEDURE dream.InsertMarketRateEnvironment 
END
GO

CREATE PROCEDURE dream.InsertMarketRateEnvironment 
(
	@CutOffDate date,
	@MarketRateEnvironmentDescription varchar(250),
	@MarketDataSetId int,
	@RateCurveDataSetId int
)
AS
BEGIN
	INSERT INTO dream.MarketRateEnvironment
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , MarketRateEnvironmentDescription
			   , MarketDataSetId
			   , RateCurveDataSetId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @MarketRateEnvironmentDescription
			   , @MarketDataSetId
			   , @RateCurveDataSetId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS MarketRateEnvironmentId
END