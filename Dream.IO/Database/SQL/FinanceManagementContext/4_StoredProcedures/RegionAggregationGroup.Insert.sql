SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('FinanceManagement.InsertRegionAggregationGroup')))
BEGIN
	DROP PROCEDURE FinanceManagement.InsertRegionAggregationGroup 
END
GO

CREATE PROCEDURE FinanceManagement.InsertRegionAggregationGroup 
(
	@RegionAggregationGroupDescription varchar(250),
	@IsAggregationGroupActive bit
)
AS
BEGIN
	INSERT INTO FinanceManagement.RegionAggregationGroup
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RegionAggregationGroupDescription
			   , IsAggregationGroupActive
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RegionAggregationGroupDescription
			   , @IsAggregationGroupActive
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RegionAggregationGroupId
END


