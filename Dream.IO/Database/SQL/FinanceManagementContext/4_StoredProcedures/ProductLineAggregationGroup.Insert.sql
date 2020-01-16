SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertProductLineAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertProductLineAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.InsertProductLineAggregationGroup 
(
	@ProductLineAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	INSERT INTO FinanceManagement.ProductLineAggregationGroup
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , ProductLineAggregationGroupDescription
			   , IsAggregationGroupActive
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @ProductLineAggregationGroupDescription
			   , @IsAggregationGroupActive
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS ProductLineAggregationGroupId
END


