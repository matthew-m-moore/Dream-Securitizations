SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRedemptionLogicType')))
BEGIN
	DROP PROCEDURE dream.InsertRedemptionLogicType 
END
GO

CREATE PROCEDURE dream.InsertRedemptionLogicType 
(
	@RedemptionLogicTypeDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.RedemptionLogicType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RedemptionLogicTypeDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RedemptionLogicTypeDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RedemptionLogicTypeId
END