SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertCompoundingConvention')))
BEGIN
	DROP PROCEDURE dream.InsertCompoundingConvention 
END
GO

CREATE PROCEDURE dream.InsertCompoundingConvention 
(
	@CompoundingConventionDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.CompoundingConvention
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CompoundingConventionDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CompoundingConventionDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS CompoundingConventionId
END