SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertPaceAssessmentRecord')))
BEGIN
	DROP PROCEDURE dream.InsertPaceAssessmentRecord 
END
GO

CREATE PROCEDURE dream.InsertPaceAssessmentRecord 
(
	@PaceAssessmentRecordDataSetId int,
	@LoanId int,
	@BondId varchar(50),
	@ReplineId varchar(250),
	@PropertyStateId int,
	@Balance float,
	@ProjectCost float,
	@CouponRate float,
	@BuyDownRate float,
	@TermInYears int,
	@FundingDate date,
	@BondFirstPaymentDate date,
	@BondFirstPrincipalPaymentDate date,
	@BondMaturityDate date,
	@CashFlowStartDate date,
	@InterestAccrualStartDate date,
	@InterestAccrualEndMonth int,
	@InterestPaymentFrequencyInMonths int,
	@PrincipalPaymentFrequencyInMonths int,
	@NumberOfUnderlyingBonds int,
	@AccruedInterest float,
	@ActualPrepaymentsReceived float,
	@PrepaymentPenaltyPlanId int,
	@PaceAssessmentRatePlanTermSetId int,
	@IsPreFundingRepline bit,
	@LastPreFundDate date,
	@PreFundingStartDate date
)
AS
BEGIN
	INSERT INTO dream.PaceAssessmentRecord
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , PaceAssessmentRecordDataSetId
			   , LoanId
			   , BondId
			   , ReplineId
			   , PropertyStateId
			   , Balance
			   , ProjectCost
			   , CouponRate
			   , BuyDownRate
			   , TermInYears
			   , FundingDate
			   , BondFirstPaymentDate
			   , BondFirstPrincipalPaymentDate
			   , BondMaturityDate
			   , CashFlowStartDate
			   , InterestAccrualStartDate
			   , InterestAccrualEndMonth
			   , InterestPaymentFrequencyInMonths
			   , PrincipalPaymentFrequencyInMonths
			   , NumberOfUnderlyingBonds
			   , AccruedInterest
			   , ActualPrepaymentsReceived
			   , PrepaymentPenaltyPlanId
			   , PaceAssessmentRatePlanTermSetId
			   , IsPreFundingRepline
			   , LastPreFundDate
			   , PreFundingStartDate
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @PaceAssessmentRecordDataSetId
			   , @LoanId
			   , @BondId
			   , @ReplineId
			   , @PropertyStateId
			   , @Balance
			   , @ProjectCost
			   , @CouponRate
			   , @BuyDownRate
			   , @TermInYears
			   , @FundingDate
			   , @BondFirstPaymentDate
			   , @BondFirstPrincipalPaymentDate
			   , @BondMaturityDate
			   , @CashFlowStartDate
			   , @InterestAccrualStartDate
			   , @InterestAccrualEndMonth
			   , @InterestPaymentFrequencyInMonths
			   , @PrincipalPaymentFrequencyInMonths
			   , @NumberOfUnderlyingBonds
			   , @AccruedInterest
			   , @ActualPrepaymentsReceived
			   , @PrepaymentPenaltyPlanId
			   , @PaceAssessmentRatePlanTermSetId
			   , @IsPreFundingRepline
			   , @LastPreFundDate
			   , @PreFundingStartDate
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PaceAssessmentRecordId
END