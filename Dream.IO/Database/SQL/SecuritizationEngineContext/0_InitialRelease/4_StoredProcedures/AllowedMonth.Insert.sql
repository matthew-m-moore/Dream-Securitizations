SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAllowedMonth')))
BEGIN
	DROP PROCEDURE dream.InsertAllowedMonth 
END
GO

CREATE PROCEDURE dream.InsertAllowedMonth 
(
	@AllowedMonthShortDescription varchar(3),
	@AllowedMonthLongDescription varchar(25)
)
AS
BEGIN
	INSERT INTO dream.AllowedMonth
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , AllowedMonthShortDescription
			   , AllowedMonthLongDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @AllowedMonthShortDescription
			   , @AllowedMonthLongDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS AllowedMonthId
END