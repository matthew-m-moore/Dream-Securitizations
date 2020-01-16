SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheCoupon')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheCoupon 
END
GO

CREATE PROCEDURE dream.InsertTrancheCoupon 
(
	@TrancheCouponRateIndexId int,
	@TrancheCouponValue float,
	@TrancheCouponFactor float,
	@TrancheCouponMargin float,
	@TrancheCouponFloor float,
	@TrancheCouponCeiling float,
	@TrancheCouponInterimCap float,
	@InterestAccrualDayCountConventionId int,
	@InitialPeriodInterestAccrualDayCountConventionId int,
	@InitialPeriodInterestAccrualEndDate date,
	@InterestRateResetFrequencyInMonths int,
	@InterestRateResetLookbackMonths int,
	@MonthsToNextInterestRateReset int
)
AS
BEGIN
	INSERT INTO dream.TrancheCoupon
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , TrancheCouponRateIndexId
			   , TrancheCouponValue
			   , TrancheCouponFactor
			   , TrancheCouponMargin
			   , TrancheCouponFloor
			   , TrancheCouponCeiling
			   , TrancheCouponInterimCap
			   , InterestAccrualDayCountConventionId
			   , InitialPeriodInterestAccrualDayCountConventionId
			   , InitialPeriodInterestAccrualEndDate
			   , InterestRateResetFrequencyInMonths
			   , InterestRateResetLookbackMonths
			   , MonthsToNextInterestRateReset
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @TrancheCouponRateIndexId
			   , @TrancheCouponValue
			   , @TrancheCouponFactor
			   , @TrancheCouponMargin
			   , @TrancheCouponFloor
			   , @TrancheCouponCeiling
			   , @TrancheCouponInterimCap
			   , @InterestAccrualDayCountConventionId
			   , @InitialPeriodInterestAccrualDayCountConventionId
			   , @InitialPeriodInterestAccrualEndDate
			   , @InterestRateResetFrequencyInMonths
			   , @InterestRateResetLookbackMonths
			   , @MonthsToNextInterestRateReset
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS TrancheCouponId
END