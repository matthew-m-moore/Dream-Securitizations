-- HERO Funding II Data
-- ================
-- WARNING: This script would FAIL if the required Excel securitizations in "Excel-Securitizations-To-Save.bat" were not saved beforehand!!
-- =====================================================================================================================================
-- Collateralized Securitization Input

EXEC dream.InsertCollateralizedSecuritizationDataSet '2017-08-24', 'HERO Funding II - Collateralized Securitizations'

DECLARE @CollateralizedSecuritizationDataSetId AS int
SET @CollateralizedSecuritizationDataSetId = (SELECT MAX(CollateralizedSecuritizationDataSetId) FROM dream.CollateralizedSecuritizationDataSet)

	-- HERO 2017-1 Tranches
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1002, 1, 26, 0.0500023909335799 --Class A1
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1002, 1, 27, 0.05				 --Class A2
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1002, 1, 25, 0.05				 --Class B
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1002, 1, 24, 0.05				 --Pref Shares

	-- HERO 2017-2 Tranches
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1003, 1, 37, 0.05				 --Class A1
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1003, 1, 38, 0.0500016513094884 --Class A2
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1003, 1, 36, 0.05				 --Class B
	EXEC dream.InsertCollateralizedSecuritizationTranche @CollateralizedSecuritizationDataSetId, 1003, 1, 35, 0.05				 --Pref Shares

-- Performance Assumption Input

DECLARE @PerformanceAssumptionDataSetId AS int
SET @PerformanceAssumptionDataSetId = (
		SELECT MAX(PerformanceAssumptionDataSetId) FROM dream.PerformanceAssumptionDataSet
		WHERE PerformanceAssumptionDataSetDescription = 'Flat CPR Assumptions Set'
		  AND CutOffDate IS NULL)

-- Market Rate Environment Input

	-- Market Rate Data

	EXEC dream.InsertMarketDataSet '2017-08-24', 'HERO Funding II - Pricing - Swap Curve'

	DECLARE @MarketDataSetId AS int
	SET @MarketDataSetId = (SELECT MAX(MarketDataSetId) FROM dream.MarketDataSet)

	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 1.5748505592, 33, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 1.6765003204, 34, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 1.7660498619, 35, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 1.8513002396, 36, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 1.9310002327, 37, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 2.005300045, 38, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 2.0694999695, 39, 3
	EXEC dream.InsertMarketData @MarketDataSetId, '2017-08-24', 2.1273503304, 40, 3

EXEC dream.InsertMarketRateEnvironment '2017-08-24', 'HERO Funding II - Pricing', @MarketDataSetId, NULL

DECLARE @MarketRateEnvironmentId AS int
SET @MarketRateEnvironmentId = (SELECT MAX(MarketRateEnvironmentId) FROM dream.MarketRateEnvironment)

-- Securitization Analysis Input @ Flat 12% CPR

EXEC dream.InsertSecuritizationAnalysisInput '12% CPR', '12% CPR', '2017-07-31', '2017-07-31', '2017-07-31', '2017-08-24', '2017-09-21', NULL, '12.0% CPR', NULL, 2, NULL, 1, 0, NULL, NULL, NULL

DECLARE @SecuritizationAnalysisInputId_Flat12CPR AS int
SET @SecuritizationAnalysisInputId_Flat12CPR = (SELECT MAX(SecuritizationAnalysisInputId) FROM dream.SecuritizationAnalysisInput)

-- Securitization Analysis Input @ Flat 20% CPR

EXEC dream.InsertSecuritizationAnalysisInput '20% CPR', '15% CPR', '2017-07-31', '2017-07-31', '2017-07-31', '2017-08-24', '2017-09-21', NULL, '20.0% CPR', NULL, 2, NULL, 1, 0, NULL, NULL, NULL

DECLARE @SecuritizationAnalysisInputId_Flat20CPR AS int
SET @SecuritizationAnalysisInputId_Flat20CPR = (SELECT MAX(SecuritizationAnalysisInputId) FROM dream.SecuritizationAnalysisInput)

-- Securitization Node Data Input

	-- Available Funds Retrieval Details

	DECLARE @AllFundsAvailableFundsRetrievalDetailId AS int
	SET @AllFundsAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail 
													WHERE AvailableFundsRetrievalTypeId = 2 AND AvailableFundsRetrievalValue IS NULL)

	DECLARE @PrincipalAdavancesAvailableFundsRetrievalDetailId AS int
	SET @PrincipalAdavancesAvailableFundsRetrievalDetailId = (SELECT MAX(AvailableFundsRetrievalDetailId) FROM dream.AvailableFundsRetrievalDetail
															  WHERE AvailableFundsRetrievalTypeId = 1 AND AvailableFundsRetrievalValue = 1.00)

	-- Reserve Tranche

	EXEC dream.InsertBalanceCapAndFloorSet
	DECLARE @BalanceCapAndFloorSetId AS int
	SET @BalanceCapAndFloorSetId = (SELECT MAX(BalanceCapAndFloorSetId) FROM dream.BalanceCapAndFloorSet)

	EXEC dream.InsertBalanceCapAndFloorDetail @BalanceCapAndFloorSetId, 'Cap', 'Dollars', 230000, '2017-08-24'

	EXEC dream.InsertTrancheDetail 'Reserve Account', 0, NULL, NULL, 20, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, NULL, @BalanceCapAndFloorSetId, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, 1, NULL, NULL, NULL, NULL
	DECLARE @ReserveAccountTrancheDetailId AS int
	SET @ReserveAccountTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertReserveAccountsSet
	DECLARE @ReserveAccountsSetId AS int
	SET @ReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

	EXEC dream.InsertReserveAccountsDetail @ReserveAccountsSetId, @ReserveAccountTrancheDetailId

	EXEC dream.InsertReserveAccountsSet
	DECLARE @EquityReserveAccountsSetId AS int
	SET @EquityReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

	EXEC dream.InsertReserveAccountsDetail @EquityReserveAccountsSetId, @ReserveAccountTrancheDetailId

	-- Senior & Residual Tranches

	EXEC dream.InsertTrancheCoupon NULL, 0.03750, NULL, NULL, NULL, NULL, NULL, 1, 1, '2017-09-21', NULL, NULL, NULL
	DECLARE @NoteTrancheCouponId AS int
	SET @NoteTrancheCouponId = (SELECT MAX(TrancheCouponId) FROM dream.TrancheCoupon)

	EXEC dream.InsertTrancheDetail 'Risk Retention Note', 23251178, NULL, NULL, 1, @NoteTrancheCouponId, @PrincipalAdavancesAvailableFundsRetrievalDetailId, @AllFundsAvailableFundsRetrievalDetailId, @ReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 1, 6, 1, 1, 1
	DECLARE @NoteTrancheDetailId AS int
	SET @NoteTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Risk Retention Equity', NULL, NULL, NULL, 7, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @EquityReserveAccountsSetId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 0
	DECLARE @EquityTrancheDetailId AS int
	SET @EquityTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertReserveAccountsDetail @ReserveAccountsSetId, @EquityTrancheDetailId
	EXEC dream.InsertReserveAccountsDetail @EquityReserveAccountsSetId, @EquityTrancheDetailId

	-- Fee Group Details

	EXEC dream.InsertFeeGroupDetail 'Cayman Govt Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @CaymanGovernmentFeeGroupDetailId AS int
	SET @CaymanGovernmentFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @CaymanGovernmentFeeGroupDetailId, 'Cayman Govt Fee', NULL, 853.66, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Custodian Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @CustodianFeeGroupDetailId AS int
	SET @CustodianFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @CustodianFeeGroupDetailId, 'Custodian Fee', NULL, 10000, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Trustee Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @TrusteeFeeGroupDetailId AS int
	SET @TrusteeFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @TrusteeFeeGroupDetailId, 'Trustee Fee', NULL, 10000, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Admin Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @AdminFeeGroupDetailId AS int
	SET @AdminFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @AdminFeeGroupDetailId, 'Admin Fee', NULL, 11500, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Reg Office Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @RegOfficeFeeGroupDetailId AS int
	SET @RegOfficeFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @RegOfficeFeeGroupDetailId, 'Reg Office Fee', NULL, 2000, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'FATCA Fee', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @FatcaFeeGroupDetailId AS int
	SET @FatcaFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @FatcaFeeGroupDetailId, 'FATCA Fee', NULL, 1500, NULL, NULL

	EXEC dream.InsertFeeGroupDetail 'Out of Pocket Provision', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	DECLARE @OutOfPocketFeeGroupDetailId AS int
	SET @OutOfPocketFeeGroupDetailId = (SELECT MAX(FeeGroupDetailId) FROM dream.FeeGroupDetail)

	EXEC dream.InsertFeeDetail @OutOfPocketFeeGroupDetailId, 'Out of Pocket Provision', NULL, 500, NULL, NULL

	-- Fee Tranches

	EXEC dream.InsertTrancheDetail 'Cayman Govt Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @CaymanGovernmentFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @CaymanGovernmentTrancheDetailId AS int
	SET @CaymanGovernmentTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Custodian Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @CustodianFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @CustodianTrancheDetailId AS int
	SET @CustodianTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Trustee Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @TrusteeFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @TrusteeTrancheDetailId AS int
	SET @TrusteeTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Admin Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @AdminFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @AdminTrancheDetailId AS int
	SET @AdminTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Reg Office Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @RegOfficeFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @RegOfficeTrancheDetailId AS int
	SET @RegOfficeTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'FATCA Fee', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @FatcaFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @FatcaTrancheDetailId AS int
	SET @FatcaTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

	EXEC dream.InsertTrancheDetail 'Out of Pocket Provision', NULL, NULL, NULL, 13, NULL, @AllFundsAvailableFundsRetrievalDetailId, NULL, @ReserveAccountsSetId, NULL, @OutOfPocketFeeGroupDetailId, 3, 1, NULL, NULL, NULL, 1, NULL, 6, NULL, 0, NULL, 1
	DECLARE @OutOfPocketTrancheDetailId AS int
	SET @OutOfPocketTrancheDetailId = (SELECT MAX(TrancheDetailId) FROM dream.TrancheDetail)

EXEC dream.InsertSecuritizationNodeDataSet '2017-08-24', 'HERO Funding II'

DECLARE @SecuritizationNodeDataSetId AS int
SET @SecuritizationNodeDataSetId = (SELECT MAX(SecuritizationNodeDataSetId) FROM dream.SecuritizationNodeDataSet)

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees A', NULL, 2, @CaymanGovernmentTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees A', NULL, 2, @CustodianTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees A', NULL, 2, @TrusteeTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 2, @AdminTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 2, @RegOfficeTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 2, @FatcaTrancheDetailId, NULL, NULL, NULL, NULL, NULL
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Fees B', NULL, 2, @OutOfPocketTrancheDetailId, NULL, NULL, NULL, NULL, NULL

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Reserve Accounts', NULL, 4, @ReserveAccountTrancheDetailId, NULL, NULL, NULL, NULL, NULL

EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Senior Note', NULL, 4, @NoteTrancheDetailId, 2, NULL, 0.05338, 1, 3
EXEC dream.InsertSecuritizationNode @SecuritizationNodeDataSetId, 'Pass-Thru', NULL, 4, @EquityTrancheDetailId, 2, NULL, 0.12000, 3, 2

-- Priority of Payments Set Input

EXEC dream.InsertPriorityOfPaymentsSet '2017-08-24', 'HERO Funding II - Standard'

DECLARE @PriorityOfPaymentsSetId AS int
SET @PriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 1, @NoteTrancheDetailId, 3
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 2, @CaymanGovernmentTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 2, @CustodianTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 2, @TrusteeTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 3, @AdminTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 3, @RegOfficeTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 3, @FatcaTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 3, @OutOfPocketTrancheDetailId, 7
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 4, @NoteTrancheDetailId, 5
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @CaymanGovernmentTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @CustodianTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 5, @TrusteeTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @AdminTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @RegOfficeTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @FatcaTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 6, @OutOfPocketTrancheDetailId, 8
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 7, @ReserveAccountTrancheDetailId, 9
EXEC dream.InsertPriorityOfPaymentsAssignment @PriorityOfPaymentsSetId, 8, @EquityTrancheDetailId, 1

-- Redemption Logic Input

	-- Redemption Priority of Payments

	EXEC dream.InsertPriorityOfPaymentsSet '2017-08-24', 'HERO Funding II - Redemption'

	DECLARE @RedemptionPriorityOfPaymentsSetId AS int
	SET @RedemptionPriorityOfPaymentsSetId = (SELECT MAX(PriorityOfPaymentsSetId) FROM dream.PriorityOfPaymentsSet)

	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 1, @NoteTrancheDetailId, 3
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 2, @CaymanGovernmentTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 2, @CustodianTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 2, @TrusteeTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 3, @AdminTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 3, @RegOfficeTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 3, @FatcaTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 3, @OutOfPocketTrancheDetailId, 7
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 4, @NoteTrancheDetailId, 5
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @CaymanGovernmentTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @CustodianTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 5, @TrusteeTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @AdminTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @RegOfficeTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @FatcaTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 6, @OutOfPocketTrancheDetailId, 8
	EXEC dream.InsertPriorityOfPaymentsAssignment @RedemptionPriorityOfPaymentsSetId, 7, @EquityTrancheDetailId, 1

EXEC dream.InsertRedemptionLogicDataSet '2017-08-24', 3, NULL, NULL, @RedemptionPriorityOfPaymentsSetId, NULL, 0.0

DECLARE @RedemptionLogicDataSetId AS int
SET @RedemptionLogicDataSetId = (SELECT MAX(RedemptionLogicDataSetId) FROM dream.RedemptionLogicDataSet)

-- Securitization Analysis Data

EXEC dream.InsertSecuritizationAnalysisDataSet '2017-08-24', 'HERO Funding II', 1, 1

DECLARE @SecuritizationAnalysisDataSetId AS int
SET @SecuritizationAnalysisDataSetId = (SELECT MAX(SecuritizationAnalysisDataSetId) FROM dream.SecuritizationAnalysisDataSet)

EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  6, @CollateralizedSecuritizationDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  2, @PerformanceAssumptionDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  4, @MarketRateEnvironmentId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1, 10, @SecuritizationAnalysisInputId_Flat12CPR
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  9, @SecuritizationNodeDataSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  7, @PriorityOfPaymentsSetId
EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 1,  8, @RedemptionLogicDataSetId

EXEC dream.InsertSecuritizationAnalysis @SecuritizationAnalysisDataSetId, 1, 2, 10, @SecuritizationAnalysisInputId_Flat20CPR

EXEC dream.InsertSecuritizationAnalysisOwner @SecuritizationAnalysisDataSetId, 1, 1, 1

EXEC dream.InsertSecuritizationAnalysisComment @SecuritizationAnalysisDataSetId, 1, 'This is a template resecuritization based on HERO Funding II. Use this resecuritization to get started if the structure of the resecuritization is similar to HERO Funding II. Note that template securitizations cannot be altered, but you can use "Save As" after editing them.', 1

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 1, 5.03086499784568
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 2, 4.12820728286798
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 3, 4.02088973581897
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 12, 8754.50756031074
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 6, 265
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 8, 1.85376017971533
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 7, 348.423982028472
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 10, 5.338
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 9, 21770640.4992222
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 11, 93.6324193949321

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 1, 9.45620173958617
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 2, 8.06774795712969
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 3, 7.20334639029436
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 12, 387.387118868977
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 5, 61
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 6, 265
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 10, 12.0
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 1, @EquityTrancheDetailId, 'Pass-Thru', 9, 537732.386826306

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 1, 3.60677577357013
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 2, 3.10873015332743
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 3, 3.0279150993264
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 12, 6702.1318353644
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 5, 1
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 6, 223
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 8, 1.73083681271651
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 7, 360.71631872835
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 10, 5.338
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 9, 22133255.2693218
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @NoteTrancheDetailId, 'Senior Note', 11, 95.1919737972921

EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 1, 9.23078250730665
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 2, 8.31682964231908
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 3, 7.4257407520706
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 12, 10.2407170796771
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 5, 85
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 6, 223
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 10, 12.0
EXEC dream.InsertSecuritizationAnalysisResult @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 9, 13789.4940748374

EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 1, @NoteTrancheDetailId, 'Senior Note', 'Senior Note', 'NR'
EXEC dream.InsertSecuritizationAnalysisSummary @SecuritizationAnalysisDataSetId, 1, 2, @EquityTrancheDetailId, 'Pass-Thru', 'Pass-Thru', 'NR'