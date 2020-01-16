SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPerformanceAssumptionType')))
BEGIN
	DROP PROCEDURE dream.InsertPerformanceAssumptionType 
END
GO

CREATE PROCEDURE dream.InsertPerformanceAssumptionType 
(
	@PerformanceAssumptionTypeDescription varchar(250),
	@PerformanceAssumptionTypeAbbreviation varchar(10)
)
AS
BEGIN
	INSERT INTO dream.PerformanceAssumptionType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PerformanceAssumptionTypeDescription
			   , PerformanceAssumptionTypeAbbreviation
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PerformanceAssumptionTypeDescription
			   , @PerformanceAssumptionTypeAbbreviation
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PerformanceAssumptionTypeId
END