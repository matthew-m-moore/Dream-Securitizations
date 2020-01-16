SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAggregationGroupDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertAggregationGroupDataSet 
END
GO

CREATE PROCEDURE dream.InsertAggregationGroupDataSet 
(
	@CutOffDate date,
	@AggregationGroupDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.AggregationGroupDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , AggregationGroupDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @AggregationGroupDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS AggregationGroupDataSetId
END