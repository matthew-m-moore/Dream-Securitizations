SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertDayCountConvention')))
BEGIN
	DROP PROCEDURE dream.InsertDayCountConvention 
END
GO

CREATE PROCEDURE dream.InsertDayCountConvention 
(
	@DayCountConventionDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.DayCountConvention
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , DayCountConventionDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @DayCountConventionDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS DayCountConventionId
END