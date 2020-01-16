SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertRegionAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertRegionAggregationGroupMapping 
END
GO

CREATE PROCEDURE FinanceManagement.InsertRegionAggregationGroupMapping 
(
	@RegionAggregationGroupId int,
	@RegionKey int,
	@RegionAggregationGroupIdentifier varchar(250)
)
AS
BEGIN
	INSERT INTO FinanceManagement.RegionAggregationGroupMapping
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RegionAggregationGroupId
			   , RegionKey
			   , RegionAggregationGroupIdentifier
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RegionAggregationGroupId
			   , @RegionKey
			   , @RegionAggregationGroupIdentifier
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RegionAggregationGroupMappingId
END


