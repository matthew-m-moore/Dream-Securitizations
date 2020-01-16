SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPerformanceAssumptionAssignment')))
BEGIN
	DROP PROCEDURE dream.InsertPerformanceAssumptionAssignment 
END
GO

CREATE PROCEDURE dream.InsertPerformanceAssumptionAssignment 
(
	@PerformanceAssumptionDataSetId int,
	@InstrumentIdentifier varchar(250),
	@PerformanceAssumptionGrouping varchar(250),
	@PerformanceAssumptionIdentifier varchar(250),
	@PerformanceAssumptionTypeId int,
	@VectorParentId int
)
AS
BEGIN
	INSERT INTO dream.PerformanceAssumptionAssignment
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PerformanceAssumptionDataSetId
			   , InstrumentIdentifier
			   , PerformanceAssumptionGrouping
			   , PerformanceAssumptionIdentifier
			   , PerformanceAssumptionTypeId
			   , VectorParentId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PerformanceAssumptionDataSetId
			   , @InstrumentIdentifier
			   , @PerformanceAssumptionGrouping
			   , @PerformanceAssumptionIdentifier
			   , @PerformanceAssumptionTypeId
			   , @VectorParentId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PerformanceAssumptionAssignmentId
END


