SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheType')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheType 
END
GO

CREATE PROCEDURE dream.InsertTrancheType 
(
	@TrancheTypeDescription varchar(250),
	@IsVisible bit,
	@IsFeeTranche bit,
	@IsReserveTranche bit,
	@IsInterestPayingTranche bit
)
AS
BEGIN
	INSERT INTO dream.TrancheType
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , TrancheTypeDescription
			   , IsVisible
			   , IsFeeTranche
			   , IsReserveTranche
			   , IsInterestPayingTranche
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @TrancheTypeDescription
			   , @IsVisible
			   , @IsFeeTranche
			   , @IsReserveTranche
			   , @IsInterestPayingTranche
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS TrancheTypeId
END