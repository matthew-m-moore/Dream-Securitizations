SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAggregationGroupAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertAggregationGroupAssignment 
END
GO

CREATE PROCEDURE dream.InsertAggregationGroupAssignment 
(
	@AggregationGroupDataSetId int,
	@InstrumentIdentifier varchar(250),
	@AggregationGroupingIdentifier varchar(250),
	@AggregationGroupName varchar(250)
)
AS
BEGIN
	INSERT INTO dream.AggregationGroupAssignment
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , AggregationGroupDataSetId
			   , InstrumentIdentifier
			   , AggregationGroupingIdentifier
			   , AggregationGroupName
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @AggregationGroupDataSetId
			   , @InstrumentIdentifier
			   , @AggregationGroupingIdentifier
			   , @AggregationGroupName
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS AggregationGroupAssignmentId
END


