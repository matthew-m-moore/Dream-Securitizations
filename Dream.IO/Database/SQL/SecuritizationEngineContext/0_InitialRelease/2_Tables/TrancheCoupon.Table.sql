SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'TrancheCoupon'))
BEGIN
    CREATE TABLE dream.TrancheCoupon (
		TrancheCouponId int IDENTITY(1,1) NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		TrancheCouponRateIndexId int NULL,
		TrancheCouponValue float NOT NULL,
		TrancheCouponFactor float NULL,
		TrancheCouponMargin float NULL,
		TrancheCouponFloor float NULL,
		TrancheCouponCeiling float NULL,
		TrancheCouponInterimCap float NULL,
		InterestAccrualDayCountConventionId int NOT NULL,
		InitialPeriodInterestAccrualDayCountConventionId int NULL,
		InitialPeriodInterestAccrualEndDate date NULL,
		InterestRateResetFrequencyInMonths int NULL,
		InterestRateResetLookbackMonths int NULL,
		MonthsToNextInterestRateReset int NULL,
	 CONSTRAINT PK_TrancheCoupon PRIMARY KEY CLUSTERED 
		(
			TrancheCouponId ASC
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