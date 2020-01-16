-- HERO 2017-2 Data
-- ================

-- PACE Assessment Records Input

DECLARE @PaceAssessmentRecordDataSetId AS int
SET @PaceAssessmentRecordDataSetId = (
		SELECT MAX(PaceAssessmentRecordDataSetId) FROM dream.PaceAssessmentRecordDataSet
		WHERE PaceAssessmentRecordDataSetDescription = 'July 2017 - PACE Assessment Replines for Securitization'
		  AND CutOffDate = '2017-07-02')

-- Performance Assumption Input

DECLARE @PerformanceAssumptionDataSetId AS int
SET @PerformanceAssumptionDataSetId = (
		SELECT MAX(PerformanceAssumptionDataSetId) FROM dream.PerformanceAssumptionDataSet
		WHERE PerformanceAssumptionDataSetDescription = 'Flat CPR Assumptions Set'
		  AND CutOffDate IS NULL)

-- Market Rate Environment Input

	-- Market Rate Data

	EXEC dream.InsertMarketDataSet '2017-07-26', 'HERO 2017-2 - Pricing - Swap Curve'

	DECLARE @MarketDataSetId AS int
	SET @MarketDataSetId = (SELECT MAX(MarketDataSetId) FROM dream.MarketDataSet)

	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 1.6180, 33, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 1.7410, 34, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 1.8561, 35, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 1.9440, 36, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 2.0390, 37, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 2.1154, 38, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 2.1809, 39, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-07-26', 2.2410, 40, 3

EXEC dream.InsertMarketRateEnvironment '2017-07-26', 'HERO 2017-2 - Pricing', @MarketDataSetId, NULL

DECLARE @MarketRateEnvironmentId AS int
SET @MarketRateEnvironmentId = (SELECT MAX(MarketRateEnvironmentId) FROM dream.MarketRateEnvironment)

-- Securitization Analysis Input @ Flat 12% CPR

EXEC dream.InsertSecuritizationAnalysisInput '12% CPR', '12% CPR', '2017-07-02', '2017-07-02', '2017-08-02', '2017-08-04', '2017-09-20', NULL, '12.0% CPR', NULL, 2, NULL, 1, 0, NULL, NULL, NULL

DECLARE @SecuritizationAnalysisInputId_Flat12CPR AS int
SET @SecuritizationAnalysisInputId_Flat12CPR = (SELECT MAX(SecuritizationAnalysisInputId) FROM dream.SecuritizationAnalysisInput)

-- Securitization Analysis Input @ Flat 15% CPR

EXEC dream.InsertSecuritizationAnalysisInput '15% CPR', '15% CPR', '2017-07-02', '2017-07-02', '2017-08-02', '2017-08-04', '2017-09-20', NULL, '15.0% CPR', NULL, 2, NULL, 1, 0, NULL, NULL, NULL

DECLARE @SecuritizationAnalysisInputId_Flat15CPR AS int
SET @SecuritizationAnalysisInputId_Flat15CPR = (SELECT MAX(SecuritizationAnalysisInputId) FROM dream.SecuritizationAnalysisInput)

-- Securitization Node Data Input

	-- Available Funds Retrieval Details

	DECLARE @AllFundsAvailableFundsRetrievalDetailId AS int
	SET @AllFundsAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail 
													WHERE AvailableFundsRetrievalTypeId = 2 AND AvailableFundsRetrievalValue IS NULL)

	DECLARE @PrincipalAdavancesAvailableFundsRetrievalDetailId AS int
	SET @PrincipalAdavancesAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail
															  WHERE AvailableFundsRetrievalTypeId = 1 AND AvailableFundsRetrievalValue = 0.97)

	EXEC dream.InsertAvailableFundsRetrievalDetail 4, NULL, 6, '2017-09-20'

	DECLARE @IrregularInterestAvailableFundsRetrievalDetailId AS int
	SET @IrregularInterestAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail)

	-- Reserve Tranche

	EXEC dream.InsertBalanceCapAndFloorSet
	DECLARE @BalanceCapAndFloorSetId AS int
	SET @BalanceCapAndFloorSetId = (SELECT MAX(BalanceCapAndFloorSetId) FROM dream.BalanceCapAndFloorSet)

	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Cap', 'Percentage', 0.01, '2017-08-04'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Cap', 'Percentage', 0.02, '2017-09-30'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Cap', 'Percentage', 0.02, '2018-03-31'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Floor', 'Dollars', 0, '2017-08-04'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Floor', 'Dollars', 1000000, '2018-03-20'

	EXEC dream.InsertTrancheDetail 'Liquidity Reserve Account', 1100000, NULL, NULL, 21, NULL, @IrregularInterestAvailableFundsRetrievalDetailId, NULL, NULL, @BalanceCapAndFloorSetId, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, 1, NULL, NULL, NULL, NULL
	DECLARE @LiquidityReserveTrancheDetailId AS int
	SET @LiquidityReserveTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertReserveAccountsSet
	DECLARE @ReserveAccountsSetId AS int
	SET @ReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

	EXEC dream.InsertReserveAccountsDetail @ReserveAccountsSetId, @LiquidityReserveTrancheDetailId

	EXEC dream.InsertReserveAccountsSet
	DECLARE @PreferredSharesReserveAccountsSetId AS int
	SET @PreferredSharesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

	EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, @LiquidityReserveTrancheDetailId

	-- Senior & Residual Tranches

	EXEC dream.InsertTrancheCoupon NULL, 0.03280, NULL, NULL, NULL, NULL, NULL, 1, 2, '2017-09-20', NULL, NULL, NULL
	DECLARE @ClassA1TrancheCouponId AS int
	SET @ClassA1TrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheCoupon NULL, 0.04070, NULL, NULL, NULL, NULL, NULL, 1, 2, '2017-09-20', NULL, NULL, NULL
	DECLARE @ClassA2TrancheCouponId AS int
	SET @ClassA2TrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheCoupon NULL, 0.05120, NULL, NULL, NULL, NULL, NULL, 1, 2, '2017-09-20', NULL, NULL, NULL
	DECLARE @ClassBTrancheCouponId AS int
	SET @ClassBTrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheDetail 'Class A1', 91000000, NULL, NULL, 1, @ClassA1TrancheCouponId, @PrincipalAdavancesAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 1, 6, 1, 1, 1
	DECLARE @ClassA1TrancheDetailId AS int
	SET @ClassA1TrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Class A2', 90837000, NULL, NULL, 1, @ClassA2TrancheCouponId, @PrincipalAdavancesAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 1, 6, 1, 1, 1
	DECLARE @ClassA2TrancheDetailId AS int
	SET @ClassA2TrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Class B', 23000000, NULL, NULL, 4, @ClassBTrancheCouponId, @AllFundsAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 6, 6, 1, 1, 0
	DECLARE @ClassBTrancheDetailId AS int
	SET @ClassBTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Preferred Shares', NULL, NULL, NULL, 7, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @PreferredSharesReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 0
	DECLARE @PreferredSharesTrancheDetailId AS int
	SET @PreferredSharesTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertReserveAccountsDetail @ReserveAccountsSetId, @ClassBTrancheDetailId
	EXEC dream.InsertReserveAccountsDetail @ReserveAccountsSetId, @PreferredSharesTrancheDetailId

	EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, @PreferredSharesTrancheDetailId

	-- Fee Group Details

	EXEC dream.InsertFeeGroupDetail 'Cayman Governmental Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @CaymanGovernmentFeeGroupDetailId AS int
	SET @CaymanGovernmentFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @CaymanGovernmentFeeGroupDetailId, 'Cayman Government', NULL, 853.65, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'ABS Note Trustee Fee', NULL, NULL, 10000.00, NULL, NULL, NULL, NULL, NULL
	DECLARE @AbsNoteTrusteeFeeGroupDetailId AS int
	SET @AbsNoteTrusteeFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @AbsNoteTrusteeFeeGroupDetailId, 'Annual Trustee Fee', NULL, 10000.00, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Pref Shares Paying Agency Fee', 0.000075, NULL, 10000.00, NULL, NULL, 1, NULL, 0
	DECLARE @PrefSharesPayingAgencyFeeGroupDetailId AS int
	SET @PrefSharesPayingAgencyFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeGroupDetail 'SPV Administration Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @SpvAdministrationFeeGroupDetailId AS int
	SET @SpvAdministrationFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @SpvAdministrationFeeGroupDetailId, 'Administration', NULL, 16500.00, NULL, 1
	EXEC dream.InsertFeeDetail @SpvAdministrationFeeGroupDetailId, 'Registered Office', NULL, 1500.00, NULL, 1
	EXEC dream.InsertFeeDetail @SpvAdministrationFeeGroupDetailId, 'FATCA Administrator', NULL, 1500.00, NULL, 1
	EXEC dream.InsertFeeDetail @SpvAdministrationFeeGroupDetailId, 'Preferred Shares Registrar', NULL, 500.00, NULL, 1

	EXEC dream.InsertFeeGroupDetail 'Portfolio Administration Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @PortfolioAdministrationFeeGroupDetailId AS int
	SET @PortfolioAdministrationFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @PortfolioAdministrationFeeGroupDetailId, 'Portfolio Administration', NULL, 15000.00, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'RA Advance Fee', 0.00015, NULL, NULL, NULL, NULL, 12, NULL, NULL
	DECLARE @RaAdvanceFeeGroupDetailId AS int
	SET @RaAdvanceFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	-- Fee Tranches

	EXEC dream.InsertTrancheDetail 'Cayman Governmental Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @CaymanGovernmentFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @CaymanGovernmentTrancheDetailId AS int
	SET @CaymanGovernmentTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'ABS Note Trustee Fee', NULL, NULL, NULL, 14, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @AbsNoteTrusteeFeeGroupDetailId, 3, 1, 1, 2, '2017-09-20', 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @AbsNoteTrusteeTrancheDetailId AS int
	SET @AbsNoteTrusteeTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Pref Shares Paying Agency Fee', NULL, NULL, NULL, 18, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @PrefSharesPayingAgencyFeeGroupDetailId, 3, 1, 1, 2, '2017-09-20', 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @PrefSharesPayingAgencyTrancheDetailId AS int
	SET @PrefSharesPayingAgencyTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'SPV Administration Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @SpvAdministrationFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @SpvAdministrationTrancheDetailId AS int
	SET @SpvAdministrationTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Portfolio Administration Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @PortfolioAdministrationFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @PortfolioAdministrationTrancheDetailId AS int
	SET @PortfolioAdministrationTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'RA Advance Fee', NULL, NULL, NULL, 19, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @RaAdvanceFeeGroupDetailId, 3, 1, 1, 2, '2017-12-31', 1, NULL, 6, NULL, 0, NULL, 0
	DECLARE @RaAdvanceTrancheDetailId AS int
	SET @RaAdvanceTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

EXEC dream.InsertSecuritizationNodeDataSet '2017-08-04', 'HERO 2017-2'

DECLARE @SecuritizationNodeDataSetId AS int
SET @SecuritizationNodeDataSetId = (SELECT MAX(SecuritizationNodeDataSetId) FROM dream.SecuritizationNodeDataSet)

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees A', NULL, 4, @CaymanGovernmentTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 4, @AbsNoteTrusteeTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 4, @PrefSharesPayingAgencyTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees C', NULL, 2, @SpvAdministrationTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees C', NULL, 2, @PortfolioAdministrationTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Subordinated Fees', NULL, 4, @RaAdvanceTrancheDetailId, NULL, NULL, NULL, NULL, NULL

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Reserve Accounts', NULL, 4, @LiquidityReserveTrancheDetailId, NULL, NULL, NULL, NULL, NULL

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Class A', NULL, 1, @ClassA1TrancheDetailId, 2, NULL, 0.03285, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Class A', NULL, 1, @ClassA2TrancheDetailId, 2, NULL, 0.03535, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Residual', NULL, 4, @ClassBTrancheDetailId, 2, NULL, 0.05130, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Residual', NULL, 4, @PreferredSharesTrancheDetailId, 2, NULL, 0.12000, 3, 2

-- Priority of Payments Set Input

EXEC dream.InsertPriorityOfPaymentsSet '2017-08-04', 'HERO 2017-2 - Standard'

DECLARE @PriorityOfPaymentsSetId AS int
SET @PriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 1, @CaymanGovernmentTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 2, @AbsNoteTrusteeTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 3, @PrefSharesPayingAgencyTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 4, @SpvAdministrationTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 4, @PortfolioAdministrationTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @ClassA1TrancheDetailId, 5
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @ClassA2TrancheDetailId, 5
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @ClassA1TrancheDetailId, 3
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @ClassA2TrancheDetailId, 3
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 7, @LiquidityReserveTrancheDetailId, 9
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 8, @CaymanGovernmentTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 9, @AbsNoteTrusteeTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 10, @PrefSharesPayingAgencyTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 11, @SpvAdministrationTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 11, @PortfolioAdministrationTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 12, @RaAdvanceTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 13, @ClassBTrancheDetailId, 5
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 14, @ClassBTrancheDetailId, 3
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 15, @PreferredSharesTrancheDetailId, 1

-- Redemption Logic Input

	-- Allowed Months

	EXEC dream.InsertRedemptionLogicAllowedMonthsSet
	
	DECLARE @RedemptionLogicAllowedMonthsSetId AS int
	SET @RedemptionLogicAllowedMonthsSetId = (SELECT MAX(RedemptionLogicAllowedMonthsSetId) FROM dream.RedemptionLogicAllowedMonthsSet)

	EXEC dream.InsertRedemptionLogicAllowedMonthsDetail @RedemptionLogicAllowedMonthsSetId, 3
	EXEC dream.InsertRedemptionLogicAllowedMonthsDetail @RedemptionLogicAllowedMonthsSetId, 9

	-- Redemption Tranches

	EXEC dream.InsertRedemptionTranchesSet
	
	DECLARE @RedemptionTranchesSetId AS int
	SET @RedemptionTranchesSetId = (SELECT MAX(RedemptionTranchesSetId) FROM dream.RedemptionTranchesSet)

	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @CaymanGovernmentTrancheDetailId
	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @AbsNoteTrusteeTrancheDetailId
	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @PrefSharesPayingAgencyTrancheDetailId
	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @SpvAdministrationTrancheDetailId
	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @PortfolioAdministrationTrancheDetailId
	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @ClassA1TrancheDetailId
	EXEC dream.InsertRedemptionTranchesDetail @RedemptionTranchesSetId, @ClassA2TrancheDetailId

	-- Redemption Priority of Payments

	EXEC dream.InsertPriorityOfPaymentsSet '2017-08-04', 'HERO 2017-2 - Redemption'

	DECLARE @RedemptionPriorityOfPaymentsSetId AS int
	SET @RedemptionPriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 1, @CaymanGovernmentTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 2, @AbsNoteTrusteeTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 3, @PrefSharesPayingAgencyTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 4, @SpvAdministrationTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 4, @PortfolioAdministrationTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @ClassA1TrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @ClassA2TrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @ClassA1TrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @ClassA2TrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 7, @CaymanGovernmentTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 8, @AbsNoteTrusteeTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 9, @PrefSharesPayingAgencyTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 10, @SpvAdministrationTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 10, @PortfolioAdministrationTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 11, @RaAdvanceTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 12, @ClassBTrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 13, @ClassBTrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 14, @PreferredSharesTrancheDetailId, 1

	-- Post-Redemption Priority of Payments

	EXEC dream.InsertPriorityOfPaymentsSet '2017-08-04', 'HERO 2017-2 - Post-Redemption'

	DECLARE @PostRedemptionPriorityOfPaymentsSetId AS int
	SET @PostRedemptionPriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 1, @CaymanGovernmentTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 2, @PrefSharesPayingAgencyTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 3, @SpvAdministrationTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 3, @PortfolioAdministrationTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 4, @ClassA1TrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 4, @ClassA2TrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 5, @ClassA1TrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 5, @ClassA2TrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 6, @CaymanGovernmentTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 7, @PrefSharesPayingAgencyTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 8, @SpvAdministrationTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 8, @PortfolioAdministrationTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 9, @RaAdvanceTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 10, @ClassBTrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 11, @ClassBTrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @PostRedemptionPriorityOfPaymentsSetId, 12, @PreferredSharesTrancheDetailId, 1

EXEC dream.InsertRedemptionLogicDataSet '2017-08-04', 2, @RedemptionLogicAllowedMonthsSetId, @RedemptionTranchesSetId, @RedemptionPriorityOfPaymentsSetId, @PostRedemptionPriorityOfPaymentsSetId, NULL

DECLARE @RedemptionLogicDataSetId AS int
SET @RedemptionLogicDataSetId = (SELECT MAX(RedemptionLogicDataSetId) FROM dream.RedemptionLogicDataSet)

-- Securitization Analysis Data

EXEC dream.InsertSecuritizationAnalysisDataSet '2017-08-04', 'HERO 2017-2', 1, 0

DECLARE @SecuritizationAnalysisDataSetId AS int
SET @SecuritizationAnalysisDataSetId = (SELECT MAX(SecuritizationAnalysisDataSetId) FROM dream.SecuritizationAnalysisDataSet)

EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  5, @PaceAssessmentRecordDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  2, @PerformanceAssumptionDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  4, @MarketRateEnvironmentId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1, 10, @SecuritizationAnalysisInputId_Flat12CPR
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  9, @SecuritizationNodeDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  7, @PriorityOfPaymentsSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  8, @RedemptionLogicDataSetId

EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 2, 10, @SecuritizationAnalysisInputId_Flat15CPR

EXEC dream.InsertSecuritizationAnalysisOwner @SecuritizationAnalysisDataSetId, 1, 1, 1

EXEC dream.InsertSecuritizationAnalysisComment @SecuritizationAnalysisDataSetId, 1, 'This is a template securitization based on HERO 2017-2. Use this securitization to get started if the structure of the securitization is similar to HERO 2017-2. Note that template securitizations cannot be altered, but you can use "Save As" after editing them.', 1

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 1, 5.43470114983608
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 2, 4.75088481791946
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 3, 4.67411251978205
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 12, 42533.4080510262
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 6, 265
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 8, 1.98529660923443
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 7, 129.970339076551
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 10, 3.285
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 9, 90988140.6132787
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 11, 99.9869677068997

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 1, 5.43470114983609
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 2, 4.69703706738794
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 3, 4.61545883252309
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 12, 42972.7600924093
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 6, 265
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 8, 1.98529660923443
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 7, 154.970339076557
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 10, 3.535
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 9, 93096434.4542636
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 11, 102.487350368532

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 1, 2.47346859935508
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 2, 2.31560951352971
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 3, 2.25769952082066
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 12, 5192.66653883137
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 6, 61
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 8, 1.67623663772067
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 7, 345.376336227839
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 10, 5.13
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 9, 22999460.3193967
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 11, 99.9976535625942

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 1, 9.45763731998641
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 2, 7.86560736268891
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 3, 7.02286371668652
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 12, 4985.77327148231
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 5, 61
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 6, 313
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 10, 12.0
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 9, 7098602.27698492

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 1, 4.66649017443308
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 2, 4.14580721729262
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 3, 4.07881271839301
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 12, 37116.635495649
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 6, 241
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 8, 1.91468448633267
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 7, 137.031551366736
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 10, 3.285
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 9, 90990729.4431928
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 11, 99.9898125749371

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 1, 4.66649017443308
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 2, 4.10649374884239
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 3, 4.03517208228795
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 12, 37454.510927448
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 6, 241
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 8, 1.91468448633267
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 7, 162.031551366736
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 10, 3.535
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 9, 92812196.8730282
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 11, 102.174440891958

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 1, 2.51101941891806
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 2, 2.34448438291969
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 3, 2.28585227213932
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 12, 5257.40763798699
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 6, 61
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 8, 1.68085538852692
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 7, 344.914461147234
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 10, 5.13
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 9, 22999395.5478302
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 11, 99.9973719470879

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 1, 9.3834043730702
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 2, 7.89657302078321
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 3, 7.05051162569929
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 12, 3529.08138672717
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 5, 61
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 6, 301
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 10, 12.0
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 9, 5004909.68700133

EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, NULL, 'Class A', 'Senior Notes', 'AA / AAA'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 'Senior Notes, Par', 'AA / AAA'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 'Senior Notes, Premium', 'AA / AAA'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 2, NULL, 'Residual', 'OC / Excess Spread', 'NR'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 'Subordinated Notes', 'BBB / BBB'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 'Back End Residual', 'NR'