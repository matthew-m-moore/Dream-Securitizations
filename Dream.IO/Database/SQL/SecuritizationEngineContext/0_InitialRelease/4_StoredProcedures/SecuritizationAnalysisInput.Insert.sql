SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
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