SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPerformanceAssumptionDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertPerformanceAssumptionDataSet 
END
GO

CREATE PROCEDURE dream.InsertPerformanceAssumptionDataSet 
(
	@CutOffDate date,
	@PerformanceAssumptionDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.PerformanceAssumptionDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , PerformanceAssumptionDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @PerformanceAssumptionDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PerformanceAssumptionDataSetId
END