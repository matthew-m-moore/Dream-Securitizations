SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertRateCurveDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertRateCurveDataSet 
END
GO

CREATE PROCEDURE dream.InsertRateCurveDataSet 
(
	@CutOffDate date,
	@RateCurveDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.RateCurveDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , RateCurveDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @RateCurveDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS RateCurveDataSetId
END