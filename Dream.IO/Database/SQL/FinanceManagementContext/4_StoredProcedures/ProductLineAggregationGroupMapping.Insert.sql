SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertProductLineAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertProductLineAggregationGroupMapping 
END
GO

CREATE PROCEDURE FinanceManagement.InsertProductLineAggregationGroupMapping 
(
	@ProductLineAggregationGroupId int,
	@ProductLineKey int,
	@ProductLineAggregationGroupIdentifier varchar(250)
)
AS
BEGIN
	INSERT INTO FinanceManagement.ProductLineAggregationGroupMapping
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , ProductLineAggregationGroupId
			   , ProductLineKey
			   , ProductLineAggregationGroupIdentifier
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @ProductLineAggregationGroupId
			   , @ProductLineKey
			   , @ProductLineAggregationGroupIdentifier
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS ProductLineAggregationGroupMappingId
END


