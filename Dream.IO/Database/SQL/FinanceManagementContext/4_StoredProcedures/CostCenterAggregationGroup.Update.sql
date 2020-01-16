SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateCostCenterAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateCostCenterAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.UpdateCostCenterAggregationGroup 
(
	@CostCenterAggregationGroupId int,
	@CostCenterAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	UPDATE FinanceManagement.CostCenterAggregationGroup
		 SET     UpdatedDate = GETDATE()
			   , UpdatedBy = SUSER_NAME()
			   , CostCenterAggregationGroupDescription = @CostCenterAggregationGroupDescription
			   , IsAggregationGroupActive = @IsAggregationGroupActive
		 WHERE CostCenterAggregationGroupId = @CostCenterAggregationGroupId
END

