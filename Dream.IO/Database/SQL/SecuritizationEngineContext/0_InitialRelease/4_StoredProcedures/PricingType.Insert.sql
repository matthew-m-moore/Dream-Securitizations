SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPricingType')))
BEGIN
	DROP PROCEDURE dream.InsertPricingType 
END
GO

CREATE PROCEDURE dream.InsertPricingType 
(
	@PricingTypeDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PricingType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PricingTypeDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PricingTypeDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PricingTypeId
END