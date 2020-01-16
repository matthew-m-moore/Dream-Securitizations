SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationInputType')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationInputType 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationInputType 
(
	@SecuritizationInputTypeDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.SecuritizationInputType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationInputTypeDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationInputTypeDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationInputTypeId
END