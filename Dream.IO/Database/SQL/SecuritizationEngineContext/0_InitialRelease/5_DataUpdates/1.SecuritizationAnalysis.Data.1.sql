-- HERO 2017-1 Data
-- ================

-- PACE Assessment Records Input

DECLARE @PaceAssessmentRecordDataSetId AS int
SET @PaceAssessmentRecordDataSetId = (
		SELECT MAX(PaceAssessmentRecordDataSetId) FROM dream.PaceAssessmentRecordDataSet
		WHERE PaceAssessmentRecordDataSetDescription = 'April 2017 - PACE Assessments for Securitization'
		  AND CutOffDate = '2017-04-02')

-- Performance Assumption Input

DECLARE @PerformanceAssumptionDataSetId AS int
SET @PerformanceAssumptionDataSetId = (
		SELECT MAX(PerformanceAssumptionDataSetId) FROM dream.PerformanceAssumptionDataSet
		WHERE PerformanceAssumptionDataSetDescription = 'Flat CPR Assumptions Set'
		  AND CutOffDate IS NULL)

-- Market Rate Environment Input

	-- Market Rate Data

	EXEC dream.InsertMarketDataSet '2017-04-21', 'HERO 2017-1 - Pricing - Swap Curve'

	DECLARE @MarketDataSetId AS int
	SET @MarketDataSetId = (SELECT MAX(MarketDataSetId) FROM dream.MarketDataSet)

	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 1.525, 33, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 1.671, 34, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 1.786, 35, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 1.886, 36, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 1.965, 37, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 2.041, 38, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 2.106, 39, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-04-21', 2.159, 40, 3

EXEC dream.InsertMarketRateEnvironment '2017-04-21', 'HERO 2017-1 - Pricing', @MarketDataSetId, NULL

DECLARE @MarketRateEnvironmentId AS int
SET @MarketRateEnvironmentId = (SELECT MAX(MarketRateEnvironmentId) FROM dream.MarketRateEnvironment)

-- Securitization Analysis Input @ Flat 10% CPR

EXEC dream.InsertSecuritizationAnalysisInput '10% CPR', '10% CPR', '2017-04-02', '2017-04-02', '2017-04-02', '2017-04-28', '2017-05-20', NULL, '10.0% CPR', NULL, 2, NULL, 1, 0, 0.20, 12, NULL

DECLARE @SecuritizationAnalysisInputId_Flat10CPR AS int
SET @SecuritizationAnalysisInputId_Flat10CPR = (SELECT MAX(SecuritizationAnalysisInputId) FROM dream.SecuritizationAnalysisInput)

-- Securitization Analysis Input @ Flat 12% CPR

EXEC dream.InsertSecuritizationAnalysisInput '12% CPR', '12% CPR', '2017-04-02', '2017-04-02', '2017-04-02', '2017-04-28', '2017-05-20', NULL, '12.0% CPR', NULL, 2, NULL, 1, 0, 0.20, 12, NULL

DECLARE @SecuritizationAnalysisInputId_Flat12CPR AS int
SET @SecuritizationAnalysisInputId_Flat12CPR = (SELECT MAX(SecuritizationAnalysisInputId) FROM dream.SecuritizationAnalysisInput)

-- Securitization Node Data Input

	-- Available Funds Retrieval Details

	EXEC dream.InsertAvailableFundsRetrievalDetail 3, NULL, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 2, NULL, NULL, NULL

	DECLARE @AllFundsAvailableFundsRetrievalDetailId AS int
	SET @AllFundsAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail)

	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 1.00, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.99, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.98, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.97, NULL, NULL

	DECLARE @PrincipalAdavancesAvailableFundsRetrievalDetailId AS int
	SET @PrincipalAdavancesAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail)

	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.96, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.95, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.94, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.93, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.92, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.91, NULL, NULL
	EXEC dream.InsertAvailableFundsRetrievalDetail 1, 0.90, NULL, NULL

	EXEC dream.InsertAvailableFundsRetrievalDetail 4, NULL, 6, '2017-09-20'

	DECLARE @IrregularInterestAvailableFundsRetrievalDetailId AS int
	SET @IrregularInterestAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail)

	-- Reserve Tranche

	EXEC dream.InsertBalanceCapAndFloorSet
	DECLARE @BalanceCapAndFloorSetId AS int
	SET @BalanceCapAndFloorSetId = (SELECT MAX(BalanceCapAndFloorSetId) FROM dream.BalanceCapAndFloorSet)

	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Cap', 'Percentage', 0.01, '2017-04-28'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Cap', 'Percentage', 0.02, '2017-09-30'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Floor', 'Dollars', 0, '2017-04-28'
	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Floor', 'Dollars', 1400000, '2018-03-20'

	EXEC dream.InsertTrancheDetail 'Liquidity Reserve Account', 0.0, NULL, NULL, 21, NULL, @IrregularInterestAvailableFundsRetrievalDetailId, NULL, NULL, @BalanceCapAndFloorSetId, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, 1, NULL, NULL, NULL, NULL
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

	EXEC dream.InsertTrancheCoupon NULL, 0.03710, NULL, NULL, NULL, NULL, NULL, 1, 2, '2017-09-20', NULL, NULL, NULL
	DECLARE @ClassA1TrancheCouponId AS int
	SET @ClassA1TrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheCoupon NULL, 0.04460, NULL, NULL, NULL, NULL, NULL, 1, 2, '2017-09-20', NULL, NULL, NULL
	DECLARE @ClassA2TrancheCouponId AS int
	SET @ClassA2TrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheCoupon NULL, 0.04990, NULL, NULL, NULL, NULL, NULL, 1, 2, '2017-09-20', NULL, NULL, NULL
	DECLARE @ClassBTrancheCouponId AS int
	SET @ClassBTrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheDetail 'Class A1', 125474000, NULL, NULL, 1, @ClassA1TrancheCouponId, @PrincipalAdavancesAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 5, 1, 6, 1, 1, 1
	DECLARE @ClassA1TrancheDetailId AS int
	SET @ClassA1TrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Class A2', 107000000, NULL, NULL, 1, @ClassA2TrancheCouponId, @PrincipalAdavancesAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 5, 1, 6, 1, 1, 1
	DECLARE @ClassA2TrancheDetailId AS int
	SET @ClassA2TrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Class B', 36000000, NULL, NULL, 4, @ClassBTrancheCouponId, @AllFundsAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, 5, 6, 6, 1, 1, 0
	DECLARE @ClassBTrancheDetailId AS int
	SET @ClassBTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Preferred Shares', NULL, NULL, NULL, 7, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @PreferredSharesReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, 6, NULL, 0, NULL, 0
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

	EXEC dream.InsertFeeGroupDetail 'ABS Note Trustee Fee', NULL, 300.00, 6000.00, 24000.00, NULL, NULL, NULL, NULL
	DECLARE @AbsNoteTrusteeFeeGroupDetailId AS int
	SET @AbsNoteTrusteeFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @AbsNoteTrusteeFeeGroupDetailId, 'Administration', NULL, 15000.00, NULL, NULL
	EXEC dream.InsertFeeDetail @AbsNoteTrusteeFeeGroupDetailId, 'Reporting', NULL, 5000.00, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Pref Shares Paying Agency Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @PrefSharesPayingAgencyFeeGroupDetailId AS int
	SET @PrefSharesPayingAgencyFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @PrefSharesPayingAgencyFeeGroupDetailId, 'Preferred Shares Paying Agency', NULL, 3000.00, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'SPV Administration Fee', NULL, NULL, NULL, NULL, 0.03, 60, NULL, NULL
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

	EXEC dream.InsertTrancheDetail 'Cayman Governmental Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @CaymanGovernmentFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 5, NULL, 6, NULL, 0, NULL, 1
	DECLARE @CaymanGovernmentTrancheDetailId AS int
	SET @CaymanGovernmentTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'ABS Note Trustee Fee', NULL, NULL, NULL, 16, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @AbsNoteTrusteeFeeGroupDetailId, 3, 1, 1, 2, '2017-09-20', 5, NULL, 6, NULL, 0, NULL, 1
	DECLARE @AbsNoteTrusteeTrancheDetailId AS int
	SET @AbsNoteTrusteeTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Pref Shares Paying Agency Fee', NULL, NULL, NULL, 14, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @PrefSharesPayingAgencyFeeGroupDetailId, 3, 1, 1, 2, '2017-09-20', 5, NULL, 6, NULL, 0, NULL, 1
	DECLARE @PrefSharesPayingAgencyTrancheDetailId AS int
	SET @PrefSharesPayingAgencyTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'SPV Administration Fee', NULL, NULL, NULL, 15, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @SpvAdministrationFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 5, NULL, 6, NULL, 0, NULL, 1
	DECLARE @SpvAdministrationTrancheDetailId AS int
	SET @SpvAdministrationTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Portfolio Administration Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @PortfolioAdministrationFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 5, NULL, 6, NULL, 0, NULL, 1
	DECLARE @PortfolioAdministrationTrancheDetailId AS int
	SET @PortfolioAdministrationTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'RA Advance Fee', NULL, NULL, NULL, 19, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @RaAdvanceFeeGroupDetailId, 3, 1, 1, 2, '2017-12-31', 5, NULL, 6, NULL, 1, NULL, 0
	DECLARE @RaAdvanceTrancheDetailId AS int
	SET @RaAdvanceTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

EXEC dream.InsertSecuritizationNodeDataSet '2017-04-28', 'HERO 2017-1'

DECLARE @SecuritizationNodeDataSetId AS int
SET @SecuritizationNodeDataSetId = (SELECT MAX(SecuritizationNodeDataSetId) FROM dream.SecuritizationNodeDataSet)

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees A', NULL, 4, @CaymanGovernmentTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 2, @AbsNoteTrusteeTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 2, @PrefSharesPayingAgencyTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees C', NULL, 4, @SpvAdministrationTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees C', NULL, 4, @PortfolioAdministrationTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Subordinated Fees', NULL, 4, @RaAdvanceTrancheDetailId, NULL, NULL, NULL, NULL, NULL

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Reserve Accounts', NULL, 4, @LiquidityReserveTrancheDetailId, NULL, NULL, NULL, NULL, NULL

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Class A', NULL, 1, @ClassA1TrancheDetailId, 2, NULL, 0.03724, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Class A', NULL, 1, @ClassA2TrancheDetailId, 2, NULL, 0.03974, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Residual', NULL, 4, @ClassBTrancheDetailId, 2, NULL, 0.05250, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Residual', NULL, 4, @PreferredSharesTrancheDetailId, 2, NULL, 0.12000, 3, 2

-- Priority of Payments Set Input

EXEC dream.InsertPriorityOfPaymentsSet '2017-04-28', 'HERO 2017-1 - Standard'

DECLARE @PriorityOfPaymentsSetId AS int
SET @PriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 1, @CaymanGovernmentTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 2, @AbsNoteTrusteeTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 2, @PrefSharesPayingAgencyTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 3, @SpvAdministrationTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 4, @PortfolioAdministrationTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @ClassA1TrancheDetailId, 5
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @ClassA2TrancheDetailId, 5
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @ClassA1TrancheDetailId, 3
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @ClassA2TrancheDetailId, 3
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 7, @LiquidityReserveTrancheDetailId, 9
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 8, @CaymanGovernmentTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 9, @AbsNoteTrusteeTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 9, @PrefSharesPayingAgencyTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 10, @SpvAdministrationTrancheDetailId, 8
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

	EXEC dream.InsertPriorityOfPaymentsSet '2017-04-28', 'HERO 2017-1 - Redemption'

	DECLARE @RedemptionPriorityOfPaymentsSetId AS int
	SET @RedemptionPriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 1, @CaymanGovernmentTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 2, @AbsNoteTrusteeTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 2, @PrefSharesPayingAgencyTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 3, @SpvAdministrationTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 4, @PortfolioAdministrationTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @ClassA1TrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @ClassA2TrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @ClassA1TrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @ClassA2TrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 7, @CaymanGovernmentTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 8, @AbsNoteTrusteeTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 8, @PrefSharesPayingAgencyTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 9, @SpvAdministrationTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 10, @PortfolioAdministrationTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 11, @RaAdvanceTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 12, @ClassBTrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 13, @ClassBTrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 14, @PreferredSharesTrancheDetailId, 1

	-- Post-Redemption Priority of Payments (N/A)

EXEC dream.InsertRedemptionLogicDataSet '2017-04-28', 2, @RedemptionLogicAllowedMonthsSetId, @RedemptionTranchesSetId, @RedemptionPriorityOfPaymentsSetId, NULL, NULL

DECLARE @RedemptionLogicDataSetId AS int
SET @RedemptionLogicDataSetId = (SELECT MAX(RedemptionLogicDataSetId) FROM dream.RedemptionLogicDataSet)

-- Securitization Analysis Data

EXEC dream.InsertSecuritizationAnalysisDataSet '2017-04-28', 'HERO 2017-1', 1, 0

DECLARE @SecuritizationAnalysisDataSetId AS int
SET @SecuritizationAnalysisDataSetId = (SELECT MAX(SecuritizationAnalysisDataSetId) FROM dream.SecuritizationAnalysisDataSet)

EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  5, @PaceAssessmentRecordDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  2, @PerformanceAssumptionDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  4, @MarketRateEnvironmentId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1, 10, @SecuritizationAnalysisInputId_Flat10CPR
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  9, @SecuritizationNodeDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  7, @PriorityOfPaymentsSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  8, @RedemptionLogicDataSetId

EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 2, 10, @SecuritizationAnalysisInputId_Flat12CPR

EXEC dream.InsertSecuritizationAnalysisOwner @SecuritizationAnalysisDataSetId, 1, 1, 1

EXEC dream.InsertSecuritizationAnalysisComment @SecuritizationAnalysisDataSetId, 1, 'This is a template securitization based on HERO 2017-1. Use this securitization to get started if the structure of the securitization is similar to HERO 2017-1. Note that template securitizations cannot be altered, but you can use "Save As" after editing them.', 1

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 1, 6.12127310862979
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 2, 5.17826583765744
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 3, 5.0836090373814
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 12, 63768.1773405635
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 6, 281
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 8, 1.97421675625586
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 7, 174.978324374419
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 10, 3.724
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 9, 125423553.141308
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 11, 99.9597949705186

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 1, 6.1212731086298
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 2, 5.11385566329161
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 3, 5.01422305126301
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 12, 54989.5275879788
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 6, 281
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 8, 1.97421675625586
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 7, 199.978324374396
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 10, 3.974
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 9, 109654048.299396
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 11, 102.480418971398

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 1, 2.72199429092839
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 2, 2.52494800529721
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 3, 2.46036346435781
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 12, 8804.92781637996
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 5, 6
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 6, 65
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 8, 1.63041116647555
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 7, 361.958883352447
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 10, 5.25
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 9, 35786447.657373
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 11, 99.4067990482583

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 1, 10.2451012626986
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 2, 8.57803352440281
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 3, 7.65895850393108
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 12, 7687.78499229062
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 5, 65
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 6, 305
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 10, 12.0
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @PreferredSharesTrancheDetailId, 'Residual', 9, 10036458.9102894

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 1, 5.47637466601118
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 2, 4.69985054942005
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 3, 4.61393900514426
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 12, 57879.3861831915
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 6, 257
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 8, 1.92363359861488
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 7, 180.036640138515
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 10, 3.724
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 9, 125431429.29598
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA1TrancheDetailId, 'Class A', 11, 99.9660720914131

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 1, 5.47637466601119
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 2, 4.64812625016926
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 3, 4.55756738620536
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 12, 49871.3970559844
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 6, 257
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 8, 1.92363359861488
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 7, 205.036640138513
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 10, 3.974
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 9, 109414183.04439
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassA2TrancheDetailId, 'Class A', 11, 102.256245835878

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 1, 2.76137447411209
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 2, 2.55573655754743
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 3, 2.49036448969299
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 12, 8911.60007110894
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 5, 5
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 6, 71
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 8, 1.63616067322037
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 7, 361.383932677966
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 10, 5.25
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 9, 35783636.917259
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @ClassBTrancheDetailId, 'Residual', 11, 99.3989914368305

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 1, 10.2796592853713
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 2, 8.67830739746096
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 3, 7.748488747733
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 12, 5954.52071945811
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 5, 71
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 6, 305
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 10, 12.0
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 9, 7683848.37413029

EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, NULL, 'Class A', 'Senior Notes', 'AA / AAA'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @ClassA1TrancheDetailId, 'Class A', 'Senior Notes, Par', 'AA / AAA'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @ClassA2TrancheDetailId, 'Class A', 'Senior Notes, Premium', 'AA / AAA'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 2, NULL, 'Residual', 'OC / Excess Spread', 'NR'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @ClassBTrancheDetailId, 'Residual', 'Subordinated Notes', 'BBB / BBB'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 2, @PreferredSharesTrancheDetailId, 'Residual', 'Back End Residual', 'NR'