SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPricingGroupDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertPricingGroupDataSet 
END
GO

CREATE PROCEDURE dream.InsertPricingGroupDataSet 
(
	@CutOffDate date,
	@PricingGroupDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PricingGroupDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , PricingGroupDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @PricingGroupDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PricingGroupDataSetId
END