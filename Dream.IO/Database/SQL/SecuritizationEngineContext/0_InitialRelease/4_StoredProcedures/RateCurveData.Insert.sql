SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateCurveData')))
BEGIN
	DROP PROCEDURE dream.InsertRateCurveData 
END
GO

CREATE PROCEDURE dream.InsertRateCurveData 
(
	@RateCurveDataSetId int,
	@MarketDateTime datetime,
	@ForwardDateTime datetime,
	@RateCurveValue float,
	@RateIndexId int,
	@MarketDataTypeId int
)
AS
BEGIN
	INSERT INTO dream.RateCurveData
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RateCurveDataSetId
			   , MarketDateTime
			   , ForwardDateTime
			   , RateCurveValue
			   , RateIndexId
			   , MarketDataTypeId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RateCurveDataSetId
			   , @MarketDateTime
			   , @ForwardDateTime
			   , @RateCurveValue
			   , @RateIndexId
			   , @MarketDataTypeId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RateCurveDataId
END