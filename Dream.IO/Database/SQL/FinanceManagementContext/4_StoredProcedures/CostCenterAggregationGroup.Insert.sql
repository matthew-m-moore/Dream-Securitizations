SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertCostCenterAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertCostCenterAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.InsertCostCenterAggregationGroup 
(
	@CostCenterAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	INSERT INTO FinanceManagement.CostCenterAggregationGroup
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CostCenterAggregationGroupDescription
			   , IsAggregationGroupActive
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CostCenterAggregationGroupDescription
			   , @IsAggregationGroupActive
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS CostCenterAggregationGroupId
END


