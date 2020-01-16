SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateIndexGroup')))
BEGIN
	DROP PROCEDURE dream.InsertRateIndexGroup 
END
GO

CREATE PROCEDURE dream.InsertRateIndexGroup 
(
	@RateIndexGroupDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.RateIndexGroup
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RateIndexGroupDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RateIndexGroupDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RateIndexGroupId
END