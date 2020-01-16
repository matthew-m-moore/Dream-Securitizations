SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateProductLineAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateProductLineAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.UpdateProductLineAggregationGroup 
(
	@ProductLineAggregationGroupId int,
	@ProductLineAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	UPDATE FinanceManagement.ProductLineAggregationGroup
		 SET     UpdatedDate = GETDATE()
			   , UpdatedBy = SUSER_NAME()
			   , ProductLineAggregationGroupDescription = @ProductLineAggregationGroupDescription
			   , IsAggregationGroupActive = @IsAggregationGroupActive
		 WHERE ProductLineAggregationGroupId = @ProductLineAggregationGroupId
END

