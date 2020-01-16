SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAvailableFundsRetrievalType')))
BEGIN
	DROP PROCEDURE dream.InsertAvailableFundsRetrievalType 
END
GO

CREATE PROCEDURE dream.InsertAvailableFundsRetrievalType 
(
	@AvailableFundsRetrievalTypeDescription varchar(250)
)
AS
BEGIN
	INSERT INTO dream.AvailableFundsRetrievalType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , AvailableFundsRetrievalTypeDescription
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @AvailableFundsRetrievalTypeDescription
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS AvailableFundsRetrievalTypeId
END