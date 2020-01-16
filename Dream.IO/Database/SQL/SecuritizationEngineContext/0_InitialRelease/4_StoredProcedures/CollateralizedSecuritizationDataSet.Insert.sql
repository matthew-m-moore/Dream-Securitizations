SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertCollateralizedSecuritizationDataSet')))
BEGIN
	DROP PROCEDURE dream.InsertCollateralizedSecuritizationDataSet 
END
GO

CREATE PROCEDURE dream.InsertCollateralizedSecuritizationDataSet 
(
	@CutOffDate date,
	@CollateralizedSecuritizationDataSetDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.CollateralizedSecuritizationDataSet
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , CutOffDate
			   , CollateralizedSecuritizationDataSetDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @CutOffDate
			   , @CollateralizedSecuritizationDataSetDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS CollateralizedSecuritizationDataSetId
END