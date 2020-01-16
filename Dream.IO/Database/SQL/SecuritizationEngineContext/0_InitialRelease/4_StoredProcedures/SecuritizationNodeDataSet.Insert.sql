SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationNodeDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationNodeDataSet 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationNodeDataSet 
(
	@CutOffDate date,
	@SecuritizationNodeDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.SecuritizationNodeDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , SecuritizationNodeDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @SecuritizationNodeDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationNodeDataSetId
END