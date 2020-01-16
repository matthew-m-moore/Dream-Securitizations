SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertFundsDistributionType')))
BEGIN
	DROP PROCEDURE dream.InsertFundsDistributionType 
END
GO

CREATE PROCEDURE dream.InsertFundsDistributionType 
(
	@FundsDistributionTypeDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.FundsDistributionType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , FundsDistributionTypeDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @FundsDistributionTypeDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS FundsDistributionTypeId
END