SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheDetail'))
BEGIN
    CREATE TABLE dream.TrancheDetail (
		TrancheDetailId int IDENTITY(1,1) NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		TrancheName varchar(150) NOT NULL,
		TrancheBalance float NULL,
		TrancheAccruedPayment float NULL,
		TrancheAccruedInterest float NULL,
		TrancheTypeId int NOT NULL,
		TrancheCouponId int NULL,
		PaymentAvailableFundsRetrievalDetailId int NOT NULL,
		InterestAvailableFundsRetrievalDetailId int NULL,
		ReserveAccountsSetId int NULL,
		BalanceCapAndFloorSetId int NULL,
		FeeGroupDetailId int NULL,
		PaymentConventionId int NULL,
		AccrualDayCountConventionId int NULL,
		InitialPaymentConventionId int NULL,
		InitialDayCountConventionId int NULL,
		InitialPeriodEnd date NULL,
		MonthsToNextPayment int NOT NULL,
		MonthsToNextInterestPayment int NULL,
		PaymentFrequencyInMonths int NOT NULL,
		InterestPaymentFrequencyInMonths int NULL,
		IncludePaymentShortfall bit NULL,
		IncludeInterestShortfall bit NULL,
		IsShortfallPaidFromReserves bit NULL,
	 CONSTRAINT PK_TrancheDetail PRIMARY KEY CLUSTERED 
		(
			TrancheDetailId ASC
		)
			WITH 
			(
				PAD_INDEX = OFF, 
				STATISTICS_NORECOMPUTE = OFF, 
				IGNORE_DUP_KEY = OFF, 
				ALLOW_ROW_LOCKS = ON, 
				ALLOW_PAGE_LOCKS = ON
			)
		) ON [PRIMARY]
END