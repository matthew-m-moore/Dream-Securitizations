SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'PaceAssessmentRecord'))
BEGIN
    CREATE TABLE dream.PaceAssessmentRecord (
		PaceAssessmentRecordId int IDENTITY(1,1) NOT NULL,
		PaceAssessmentRecordDataSetId int NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		LoanId int NULL,
		BondId varchar(50) NULL,
		ReplineId varchar(250) NULL,		
		PropertyStateId int NULL,
		Balance float NOT NULL,
		ProjectCost float NULL,
		CouponRate float NOT NULL,
		BuyDownRate float NULL,
		TermInYears int NOT NULL,
		FundingDate date NULL,
		BondFirstPaymentDate date NOT NULL,
		BondFirstPrincipalPaymentDate date NULL,
		BondMaturityDate date NULL,
		CashFlowStartDate date NULL,
		InterestAccrualStartDate date NULL,
		InterestAccrualEndMonth int NULL,
		InterestPaymentFrequencyInMonths int NULL,
		PrincipalPaymentFrequencyInMonths int NULL,
		NumberOfUnderlyingBonds int NULL,
		AccruedInterest float NULL,
		ActualPrepaymentsReceived float NULL,
		PrepaymentPenaltyPlanId int NULL,
		PaceAssessmentRatePlanTermSetId int NULL,
	 CONSTRAINT PK_PaceAssessmentRecord PRIMARY KEY CLUSTERED 
		(
			PaceAssessmentRecordId ASC
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