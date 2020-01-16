SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertFeeGroupDetail')))
BEGIN
	DROP PROCEDURE dream.InsertFeeGroupDetail 
END
GO

CREATE PROCEDURE dream.InsertFeeGroupDetail 
(
	@FeeGroupName varchar(150),
	@FeeRate float,
	@FeePerUnit float,
	@FeeMinimum float,
	@FeeMaximum float,
	@FeeIncreaseRate float,
	@FeeRateUpdateFrequencyInMonths int,
	@FeeRollingAverageInMonths int,
	@UseStartingBalanceToDetermineFee bit
)
AS
BEGIN
	INSERT INTO dream.FeeGroupDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , FeeGroupName
			   , FeeRate
			   , FeePerUnit
			   , FeeMinimum
			   , FeeMaximum
			   , FeeIncreaseRate
			   , FeeRateUpdateFrequencyInMonths
			   , FeeRollingAverageInMonths
			   , UseStartingBalanceToDetermineFee
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @FeeGroupName
			   , @FeeRate
			   , @FeePerUnit
			   , @FeeMinimum
			   , @FeeMaximum
			   , @FeeIncreaseRate
			   , @FeeRateUpdateFrequencyInMonths
			   , @FeeRollingAverageInMonths
			   , @UseStartingBalanceToDetermineFee
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS FeeGroupDetailId
END