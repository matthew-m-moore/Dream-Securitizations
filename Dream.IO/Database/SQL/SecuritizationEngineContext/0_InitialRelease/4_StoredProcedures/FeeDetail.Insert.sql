SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertFeeDetail')))
BEGIN
	DROP PROCEDURE dream.InsertFeeDetail 
END
GO

CREATE PROCEDURE dream.InsertFeeDetail 
(
	@FeeGroupDetailId int,
	@FeeName varchar(150),
	@FeeAssociatedTrancheDetailId int NULL,
	@FeeAmount float NULL,
	@FeeEffectiveDate date NULL,
	@IsIncreasingFee bit NULL
)
AS
BEGIN
	INSERT INTO dream.FeeDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , FeeGroupDetailId
			   , FeeName
			   , FeeAssociatedTrancheDetailId
			   , FeeAmount
			   , FeeEffectiveDate
			   , IsIncreasingFee
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @FeeGroupDetailId
			   , @FeeName
			   , @FeeAssociatedTrancheDetailId
			   , @FeeAmount
			   , @FeeEffectiveDate
			   , @IsIncreasingFee
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS FeeDetailId
END