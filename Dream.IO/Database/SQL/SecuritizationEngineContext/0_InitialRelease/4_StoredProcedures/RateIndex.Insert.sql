SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateIndex')))
BEGIN
	DROP PROCEDURE dream.InsertRateIndex 
END
GO

CREATE PROCEDURE dream.InsertRateIndex 
(
	@RateIndexDescription varchar(250),
	@TenorInMonths int,
	@RateIndexGroupId int
)
AS
BEGIN
	INSERT INTO dream.RateIndex
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , RateIndexDescription
			   , TenorInMonths
			   , RateIndexGroupId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @RateIndexDescription
			   , @TenorInMonths
			   , @RateIndexGroupId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RateIndexId
END