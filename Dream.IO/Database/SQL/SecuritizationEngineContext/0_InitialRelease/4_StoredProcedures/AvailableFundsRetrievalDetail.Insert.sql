SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertAvailableFundsRetrievalDetail')))
BEGIN
	DROP PROCEDURE dream.InsertAvailableFundsRetrievalDetail 
END
GO

CREATE PROCEDURE dream.InsertAvailableFundsRetrievalDetail 
(
	@AvailableFundsRetrievalTypeId int,
	@AvailableFundsRetrievalValue float,
	@AvailableFundsRetrievalInteger int,
	@AvailableFundsRetrievalDate date
)
AS
BEGIN
	INSERT INTO dream.AvailableFundsRetrievalDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , AvailableFundsRetrievalTypeId
			   , AvailableFundsRetrievalValue
			   , AvailableFundsRetrievalInteger
			   , AvailableFundsRetrievalDate
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @AvailableFundsRetrievalTypeId
			   , @AvailableFundsRetrievalValue
			   , @AvailableFundsRetrievalInteger
			   , @AvailableFundsRetrievalDate
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS AvailableFundsRetrievalDetailId
END