USE CapitalMarkets
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertReserveAccountsDetail')))
BEGIN
	DROP PROCEDURE dream.InsertReserveAccountsDetail 
END
GO

CREATE PROCEDURE dream.InsertReserveAccountsDetail 
(
	@ReserveAccountsSetId int,
	@TrancheDetailId int
)
AS
BEGIN
	INSERT INTO dream.ReserveAccountsDetail
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , ReserveAccountsSetId
			   , TrancheDetailId
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @ReserveAccountsSetId
			   , @TrancheDetailId
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS ReserveAccountsDetailId
END
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
	@PaceAssessmentRatePlanTermSetId int
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
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS PaceAssessmentRecordId
END
GO

IF (EXISTS (SELECT * 
				FROM SYS.OBJECTS WHERE TYPE = 'P' 
				AND OBJECT_ID = OBJECT_ID('dream.InsertSecuritizationAnalysisInput')))
BEGIN
	DROP PROCEDURE dream.InsertSecuritizationAnalysisInput 
END
GO

CREATE PROCEDURE dream.InsertSecuritizationAnalysisInput 
(
	@SecuritizationAnalysisScenarioDescription varchar(250),
	@AdditionalAnalysisScenarioDescription varchar(250),
	@CollateralCutOffDate date,
	@CashFlowStartDate date,
	@InterestAccrualStartDate date,
	@SecuritizationClosingDate date,
	@SecuritizationFirstCashFlowDate date,
	@SelectedAggregationGrouping varchar(250),
	@SelectedPerformanceAssumption varchar(250),
	@SelectedPerformanceAssumptionGrouping varchar(250),
	@NominalSpreadRateIndexGroupId int,
	@CurveSpreadRateIndexId int,
	@UseReplines bit,
	@SeparatePrepaymentInterest bit,
	@PreFundingAmount float,
	@PreFundingBondCount float,
	@CleanUpCallPercentage float
)
AS
BEGIN
	INSERT INTO dream.SecuritizationAnalysisInput
			   (
			     CreatedDate
			   , CreatedBy
			   , UpdatedDate
			   , UpdatedBy
			   , SecuritizationAnalysisScenarioDescription
			   , AdditionalAnalysisScenarioDescription
			   , CollateralCutOffDate
			   , CashFlowStartDate
			   , InterestAccrualStartDate
			   , SecuritizationClosingDate
			   , SecuritizationFirstCashFlowDate
			   , SelectedAggregationGrouping
			   , SelectedPerformanceAssumption
			   , SelectedPerformanceAssumptionGrouping
			   , NominalSpreadRateIndexGroupId
			   , CurveSpreadRateIndexId
			   , UseReplines
			   , SeparatePrepaymentInterest
			   , PreFundingAmount
			   , PreFundingBondCount
			   , CleanUpCallPercentage
			   )
		 VALUES
			   (
				 GETDATE()
			   , SUSER_NAME()
			   , GETDATE()
			   , SUSER_NAME()
			   , @SecuritizationAnalysisScenarioDescription
			   , @AdditionalAnalysisScenarioDescription
			   , @CollateralCutOffDate
			   , @CashFlowStartDate
			   , @InterestAccrualStartDate
			   , @SecuritizationClosingDate
			   , @SecuritizationFirstCashFlowDate
			   , @SelectedAggregationGrouping
			   , @SelectedPerformanceAssumption
			   , @SelectedPerformanceAssumptionGrouping
			   , @NominalSpreadRateIndexGroupId
			   , @CurveSpreadRateIndexId
			   , @UseReplines
			   , @SeparatePrepaymentInterest
			   , @PreFundingAmount
			   , @PreFundingBondCount
			   , @CleanUpCallPercentage
			   )

	-- Note the line below is required to get EF6 fluent API to behave
	SELECT SCOPE_IDENTITY() AS SecuritizationAnalysisInputId
END
GO