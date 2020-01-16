using Dream.Common.Enums;
using Dream.ConsoleApp.Interfaces;
using Dream.Core.BusinessLogic.InterestRates;
using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Redemption;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic.FundsDistribution;
using Dream.Core.Reporting.Excel;
using Dream.Core.Repositories.Excel;
using Dream.IO.Excel;
using System;
using System.Collections.Generic;

namespace Dream.ConsoleApp.Scripts.Miscellaneous
{
    public class RunSampleCommercialPaceSecuritization : IScript
    {
        public List<string> GetArgumentsList()
        {
            return new List<string>
            {
                "[1] Valid file path to Excel inputs file",
            };
        }

        public string GetFriendlyName()
        {
            return "Run C-PACE Securitization";
        }

        public bool GetVisibilityStatus()
        {
            return false;
        }

        public void RunScript(string[] args)
        {
            var inputsFilePath = args[1];

            var paceSecuritization = GetSecuritization(inputsFilePath);
            var securitizationResult = paceSecuritization.RunSecuritizationAnalysis();

            var openFileOnSave = true;
            var excelFileWriter = new ExcelFileWriter(openFileOnSave);
            SecuritizationTranchesSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResult);
            SecuritizationCashFlowsExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResult);
            CollateralSummaryExcelReport.AddReportTab(excelFileWriter.ExcelWorkbook, securitizationResult);
            CollateralCashFlowsExcelReport.AddReportTabs(excelFileWriter.ExcelWorkbook, securitizationResult);
            excelFileWriter.ExportWorkbook();
        }

        private static Securitization GetSecuritization(string inputsFilePath)
        {
            var securitizationDataRepository = new SecuritizationExcelDataRepository(inputsFilePath);
            var paceSecuritization = GetSecuritization(securitizationDataRepository);
            return paceSecuritization;
        }

        private static Securitization GetSecuritization(SecuritizationExcelDataRepository securitizationDataRepository)
        {
            var paceSecuritization = securitizationDataRepository.GetPaceSecuritizationWithoutNodesDefined();

            RedemptionLogic redemptionLogic;
            var securitizationNodes = GetSecuritizationNodesAndRedemptionLogic(
                paceSecuritization.Inputs.MarketRateEnvironment, 
                out redemptionLogic);

            var priorityOfPayments = GetPriorityOfPayments();
            var redemptionPriorityOfPayments = GetRedemptionPriorityOfPayments();

            redemptionLogic.PriorityOfPayments = redemptionPriorityOfPayments;
            paceSecuritization.PriorityOfPayments = priorityOfPayments;
            paceSecuritization.SecuritizationNodes = securitizationNodes;

            paceSecuritization.RedemptionLogicList.Add(redemptionLogic);
            paceSecuritization.Inputs.MarketDataGroupingForNominalSpread = MarketDataGrouping.Swaps;

            return paceSecuritization;
        }

        private static PriorityOfPayments GetPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "Trustee Admin Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "Portfolio Admin Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "Issuer Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 5, "Class A", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 6, "Class A", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 7, "Reserve Account", TrancheCashFlowType.Reserves),
                    new PriorityOfPaymentsEntry( 8, "Trustee Admin Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry( 9, "Portfolio Admin Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(10, "Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(11, "Issuer Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(12, "Residual", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static PriorityOfPayments GetRedemptionPriorityOfPayments()
        {
            var priorityOfPayments = new PriorityOfPayments(
                new List<PriorityOfPaymentsEntry>
                {
                    new PriorityOfPaymentsEntry( 1, "Trustee Admin Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 2, "Portfolio Admin Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 3, "Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 4, "Issuer Trustee Fee", TrancheCashFlowType.Fees),
                    new PriorityOfPaymentsEntry( 5, "Class A", TrancheCashFlowType.Interest),
                    new PriorityOfPaymentsEntry( 6, "Class A", TrancheCashFlowType.Payment),
                    new PriorityOfPaymentsEntry( 7, "Trustee Admin Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry( 8, "Portfolio Admin Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry( 9, "Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(10, "Issuer Trustee Fee", TrancheCashFlowType.FeesShortfall),
                    new PriorityOfPaymentsEntry(11, "Residual", TrancheCashFlowType.Payment)
                });

            return priorityOfPayments;
        }

        private static List<SecuritizationNodeTree> GetSecuritizationNodesAndRedemptionLogic(
            MarketRateEnvironment rateEnvironment, 
            out RedemptionLogic redemptionLogic)
        {
            var principalRemittancesAvailableFundsRetriever = new PrincipalRemittancesAvailableFundsRetriever(0.97);
            var allFundsAvailableFundsRetriever = new AllFundsAvailableFundsRetriever();

            #region Reserves Definition Region
            // Reserve Account
            var reserveAccount = new PercentOfCollateralBalanceCappedReserveFundTranche("Reserve Account", 0,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1
            };

            reserveAccount.AddReserveFundBalanceCap(new DateTime(2017, 6, 1), 0.02);
            reserveAccount.AddReserveFundBalanceFloor(new DateTime(2017, 6, 1), 0);
            reserveAccount.AddReserveFundBalanceFloor(new DateTime(2017, 7, 1), 100000);
            #endregion Reserves
            #region Notes & Residual Definition Region
            // Residual Certificates
            var residualCertificate = new ResidualTranche("Residual",
                                new YieldBasedPricingStrategy(DayCountConvention.Actual365, CompoundingConvention.Annually, 0.12000),
                                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 6,
                PaymentFrequencyInMonths = 6,
            };

            residualCertificate.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Payment);
            residualCertificate.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.PaymentShortfall);

            // Senior Notes
            var classANote = new FixedRateTranche("Class A",
                                new NominalSpreadBasedPricingStrategy(
                                    DayCountConvention.Thirty360, 
                                    CompoundingConvention.SemiAnnually, 
                                    rateEnvironment, 
                                    MarketDataGrouping.Swaps, 
                                    0.0180),
                                principalRemittancesAvailableFundsRetriever,
                                allFundsAvailableFundsRetriever,
                                new FixedRateCoupon(0.04090))
            {
                CurrentBalance = 6481000,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,
                InterestAccrualStartDate = new DateTime(2017, 6, 1),
                InitialPeriodInterestAccrualDayCountConvention = DayCountConvention.Actual360,
                InitialPeriodInterestAccrualEndDate = new DateTime(2017, 12, 31),
                MonthsToNextInterestPayment = 6,
                InterestPaymentFrequencyInMonths = 6,
                MonthsToNextPayment = 1,
                PaymentFrequencyInMonths = 1,
                IncludePaymentShortfall = true,
                IncludeInterestShortfall = true
            };

            classANote.InitialBalance = classANote.CurrentBalance.Value;
            classANote.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Interest);
            classANote.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.InterestShortfall);
            classANote.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Principal);
            classANote.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.PrincipalShortfall);
            classANote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Interest);
            classANote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.InterestShortfall);
            classANote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Principal);
            classANote.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.PrincipalShortfall);
            #endregion Notes & Residual Definition Region
            #region Fees Definition Region
            // Senior Fees
            var trusteeAdminFee = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche("Trustee Admin Fee", 0.0004, 6,
                new DateTime(2017, 12, 31),
                DayCountConvention.Actual360,
                DayCountConvention.Thirty360,
                PaymentConvention.SemiAnnual,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 6,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            trusteeAdminFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Fees);
            trusteeAdminFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.FeesShortfall);
            trusteeAdminFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            trusteeAdminFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);

            var portfolioAdminFee = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche("Portfolio Admin Fee", 0.0002, 6,
                new DateTime(2017, 12, 31),
                DayCountConvention.Actual360,
                DayCountConvention.Thirty360,
                PaymentConvention.SemiAnnual,
                allFundsAvailableFundsRetriever)
                        {
                            MonthsToNextPayment = 6,
                            PaymentFrequencyInMonths = 6,
                            IncludePaymentShortfall = true
                        };

            portfolioAdminFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Fees);
            portfolioAdminFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.FeesShortfall);
            portfolioAdminFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            portfolioAdminFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);

            var trusteeFee = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche("Trustee Fee", 0.0003, 6,
                new DateTime(2017, 12, 31),
                DayCountConvention.Actual360,
                DayCountConvention.Thirty360,
                PaymentConvention.SemiAnnual,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 6,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            trusteeFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Fees);
            trusteeFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.FeesShortfall);
            trusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            trusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);

            var issuerTrusteeFee = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche("Issuer Trustee Fee", 0.0001, 6,
                new DateTime(2017, 12, 31),
                DayCountConvention.Actual360,
                DayCountConvention.Thirty360,
                PaymentConvention.SemiAnnual,
                allFundsAvailableFundsRetriever)
            {
                MonthsToNextPayment = 6,
                PaymentFrequencyInMonths = 6,
                IncludePaymentShortfall = true
            };

            issuerTrusteeFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.Fees);
            issuerTrusteeFee.AddAssociatedReserveAccount(reserveAccount, TrancheCashFlowType.FeesShortfall);
            issuerTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.Fees);
            issuerTrusteeFee.AddAssociatedReserveAccount(residualCertificate, TrancheCashFlowType.FeesShortfall);
            #endregion Fees Definition Region

            // Securitization Node Structure
            var securitizationNodes = new List<SecuritizationNodeTree>
            {
                new SecuritizationNodeTree(new InitialBalanceProRataDistributionRule())
                {
                    SecuritizationNodeName = "Senior",
                    SecuritizationTranches = new List<Tranche>
                    {
                        classANote,
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Excess Spread",
                    SecuritizationTranches = new List<Tranche>
                    {
                        residualCertificate
                    }
                },
                new SecuritizationNodeTree(new SequentialDistributionRule())
                {
                    SecuritizationNodeName = "Reserve Accounts",
                    SecuritizationTranches = new List<Tranche>
                    {
                       reserveAccount,
                    }
                },
                new SecuritizationNodeTree(new NextFeePaymentProRataDistributionRule())
                {
                    SecuritizationNodeName = "Senior Fees",
                    SecuritizationTranches = new List<Tranche>
                    {
                       trusteeAdminFee,
                       portfolioAdminFee,
                       trusteeFee,
                       issuerTrusteeFee,
                    }
                },
            };

            // Redemption Logic
            redemptionLogic = new TranchesCanBePaidOutFromAvailableFundsRedemptionLogic()
            {
                ListOfTranchesToBePaidOut = new List<Tranche>
                {
                    trusteeAdminFee,
                    portfolioAdminFee,
                    trusteeFee,
                    issuerTrusteeFee,

                    classANote,
                }
            };

            return securitizationNodes;
        }
    }
}
