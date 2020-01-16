SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dream' 
                 AND  TABLE_NAME = 'SecuritizationAnalysisInput'))
BEGIN
    CREATE TABLE dream.SecuritizationAnalysisInput (
		SecuritizationAnalysisInputId int IDENTITY(1000,1) NOT NULL,
		CreatedDate datetime2(3) NOT NULL,
		CreatedBy varchar(50) NOT NULL,
		UpdatedDate datetime2(3) NOT NULL,
		UpdatedBy varchar(50) NOT NULL,
		SecuritizationAnalysisScenarioDescription varchar(250) NULL,
		AdditionalAnalysisScenarioDescription varchar(250) NULL,
		CollateralCutOffDate date NOT NULL,
		CashFlowStartDate date NOT NULL,
		InterestAccrualStartDate date NOT NULL,
		SecuritizationClosingDate date NOT NULL,
		SecuritizationFirstCashFlowDate date NOT NULL,
		SelectedAggregationGrouping varchar(250) NULL,
		SelectedPerformanceAssumption varchar(250) NULL,
		SelectedPerformanceAssumptionGrouping varchar(250) NULL,
		NominalSpreadRateIndexGroupId int NULL,
		CurveSpreadRateIndexId int NULL,
		UseReplines bit NOT NULL,
		SeparatePrepaymentInterest bit NOT NULL,
		PreFundingAmount float NULL,
		PreFundingBondCount float NULL,
		CleanUpCallPercentage float NULL,
	 CONSTRAINT PK_SecuritizationAnalysisInput PRIMARY KEY CLUSTERED 
		(
			SecuritizationAnalysisInputId ASC
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