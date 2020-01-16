SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.UpdateRegionAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.UpdateRegionAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.UpdateRegionAggregationGroup 
(
	@RegionAggregationGroupId int,
	@RegionAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	UPDATE FinanceManagement.RegionAggregationGroup
		 SET     UpdatedDate = GETDATE()
			   , UpdatedBy = SUSER_NAME()
			   , RegionAggregationGroupDescription = @RegionAggregationGroupDescription
			   , IsAggregationGroupActive = @IsAggregationGroupActive
		 WHERE RegionAggregationGroupId = @RegionAggregationGroupId
END

