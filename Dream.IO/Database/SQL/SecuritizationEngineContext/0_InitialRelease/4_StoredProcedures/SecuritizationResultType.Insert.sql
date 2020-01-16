SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationResultType')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationResultType 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationResultType 
(
	@SecuritizationResultTypeDescription varchar(250),
	@StandardDivisor float
)
AS
BEGIN
	INSERT INTO dream.SecuritizationResultType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationResultTypeDescription
			   , StandardDivisor
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationResultTypeDescription
			   , @StandardDivisor
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationResultTypeId
END