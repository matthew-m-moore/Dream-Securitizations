SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPricingGroupAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertPricingGroupAssignment 
END
GO

CREATE PROCEDURE dream.InsertPricingGroupAssignment 
(
	@PricingGroupDataSetId int,
	@InstrumentIdentifier varchar(250),
	@PricingValue float,
	@PricingTypeId int,
	@PricingRateIndexId int,
	@PricingDayCountConventionId int,
	@PricingCompoundingConventionId int
)
AS
BEGIN
	INSERT INTO dream.PricingGroupAssignment
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PricingGroupDataSetId
			   , InstrumentIdentifier
			   , PricingValue
			   , PricingTypeId
			   , PricingRateIndexId
			   , PricingDayCountConventionId
			   , PricingCompoundingConventionId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PricingGroupDataSetId
			   , @InstrumentIdentifier
			   , @PricingValue
			   , @PricingTypeId
			   , @PricingRateIndexId
			   , @PricingDayCountConventionId
			   , @PricingCompoundingConventionId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PricingGroupAssignmentId
END


