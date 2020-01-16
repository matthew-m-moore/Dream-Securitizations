SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertMarketDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertMarketDataSet 
END
GO

CREATE PROCEDURE dream.InsertMarketDataSet 
(
	@CutOffDate date,
	@MarketDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.MarketDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , MarketDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @MarketDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS MarketDataSetId
END