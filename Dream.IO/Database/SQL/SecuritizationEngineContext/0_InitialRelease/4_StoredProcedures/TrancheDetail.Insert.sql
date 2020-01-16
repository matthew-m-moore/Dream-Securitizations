SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertTrancheDetail')))
BEGIN
	DROP PROCEDURE dream.InsertTrancheDetail 
END
GO

CREATE PROCEDURE dream.InsertTrancheDetail 
(
	@TrancheName varchar(150),
	@TrancheBalance float,
	@TrancheAccruedPayment float,
	@TrancheAccruedInterest float,
	@TrancheTypeId int,
	@TrancheCouponId int,
	@PaymentAvailableFundsRetrievalDetailId int,
	@InterestAvailableFundsRetrievalDetailId int,
	@ReserveAccountsSetId int,
	@BalanceCapAndFloorSetId int,
	@FeeGroupDetailId int,
	@PaymentConventionId int,
	@AccrualDayCountConventionId int,
	@InitialPaymentConventionId int,
	@InitialDayCountConventionId int,
	@InitialPeriodEnd date,
	@MonthsToNextPayment int,
	@MonthsToNextInterestPayment int,
	@PaymentFrequencyInMonths int,
	@InterestPaymentFrequencyInMonths int,
	@IncludePaymentShortfall bit,
	@IncludeInterestShortfall bit,
	@IsShortfallPaidFromReserves bit
)
AS
BEGIN
	INSERT INTO dream.TrancheDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , TrancheName
			   , TrancheBalance
			   , TrancheAccruedPayment
			   , TrancheAccruedInterest
			   , TrancheTypeId
			   , TrancheCouponId
			   , PaymentAvailableFundsRetrievalDetailId
			   , InterestAvailableFundsRetrievalDetailId
			   , ReserveAccountsSetId
			   , BalanceCapAndFloorSetId
			   , FeeGroupDetailId
			   , PaymentConventionId
			   , AccrualDayCountConventionId
			   , InitialPaymentConventionId
			   , InitialDayCountConventionId
			   , InitialPeriodEnd
			   , MonthsToNextPayment
			   , MonthsToNextInterestPayment
			   , PaymentFrequencyInMonths
			   , InterestPaymentFrequencyInMonths
			   , IncludePaymentShortfall
			   , IncludeInterestShortfall
			   , IsShortfallPaidFromReserves
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @TrancheName
			   , @TrancheBalance
			   , @TrancheAccruedPayment
			   , @TrancheAccruedInterest
			   , @TrancheTypeId
			   , @TrancheCouponId
			   , @PaymentAvailableFundsRetrievalDetailId
			   , @InterestAvailableFundsRetrievalDetailId
			   , @ReserveAccountsSetId
			   , @BalanceCapAndFloorSetId
			   , @FeeGroupDetailId
			   , @PaymentConventionId
			   , @AccrualDayCountConventionId
			   , @InitialPaymentConventionId
			   , @InitialDayCountConventionId
			   , @InitialPeriodEnd
			   , @MonthsToNextPayment
			   , @MonthsToNextInterestPayment
			   , @PaymentFrequencyInMonths
			   , @InterestPaymentFrequencyInMonths
			   , @IncludePaymentShortfall
			   , @IncludeInterestShortfall
			   , @IsShortfallPaidFromReserves
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS TrancheDetailId
END