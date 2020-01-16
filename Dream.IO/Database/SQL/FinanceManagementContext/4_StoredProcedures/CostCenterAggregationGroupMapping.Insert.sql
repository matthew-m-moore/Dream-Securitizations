SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertCostCenterAggregationGroupMapping')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertCostCenterAggregationGroupMapping 
END
GO

CREATE PROCEDURE FinanceManagement.InsertCostCenterAggregationGroupMapping 
(
	@CostCenterAggregationGroupId int,
	@CostCenterKey int,
	@CostCenterAggregationGroupIdentifier varchar(250)
)
AS
BEGIN
	INSERT INTO FinanceManagement.CostCenterAggregationGroupMapping
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CostCenterAggregationGroupId
			   , CostCenterKey
			   , CostCenterAggregationGroupIdentifier
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CostCenterAggregationGroupId
			   , @CostCenterKey
			   , @CostCenterAggregationGroupIdentifier
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS CostCenterAggregationGroupMappingId
END


