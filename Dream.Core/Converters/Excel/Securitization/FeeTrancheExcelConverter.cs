using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.IO.Excel.Entities.SecuritizationRecords;
using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;

namespace Dream.Core.Converters.Excel.Securitization
{
    public class FeeTrancheExcelConverter
    {
        private const string _flatFee = "Flat Fee";
        private const string _increasingFee = "Increasing Fee";
        private const string _bondCountFee = "Bond Count Fee";
        private const string _trancheBasedFee = "% Tranche Fee";
        private const string _collateralBasedFee = "% Collateral Fee";

        private const string _beginningBalance = "Beginning Balance";
        private const string _endingBalance = "Ending Balance";

        private DateTime _securitizationFirstCashFlowDate;
        private PriorityOfPayments _priorityOfPayments;

        public FeeTrancheExcelConverter(
            DateTime securitizationFirstCashFlowDate,
            PriorityOfPayments priorityOfPayments)
        {
            _securitizationFirstCashFlowDate = securitizationFirstCashFlowDate;
            _priorityOfPayments = priorityOfPayments;
        }

        /// <summary>
        /// Returns a list of fee tranches based on lists of fee group and fee detail records from Excel.
        /// </summary>
        public List<Tranche> ConvertListOfFeeGroupRecordsAndListOfFeeDetailRecords(
            List<FeeGroupRecord> listOfFeeGroupRecords,
            List<FeeDetailRecord> listOfFeeDetailRecords)
        {
            var listOfFeeTranches = listOfFeeGroupRecords
                .Select(r => ConvertFeeGroupRecordAndListOfFeeDetailRecords(r, listOfFeeDetailRecords)).ToList();
            return listOfFeeTranches;
        }

        private Tranche ConvertFeeGroupRecordAndListOfFeeDetailRecords(FeeGroupRecord feeGroupRecord, List<FeeDetailRecord> listOfFeeDetailRecords)
        {
            if (!_priorityOfPayments.OrderedListOfEntries.Any(e => e.TrancheName == feeGroupRecord.FeeGroupName))
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' was not listed in the standard priority of payments.",
                    feeGroupRecord.FeeGroupName));
            }

            FeeTranche feeTranche;
            var feeType = feeGroupRecord.FeeType;
            switch(feeType)
            {
                case _flatFee:
                    feeTranche = CreateFlatFeeTrancheFromFeeGroupRecord(feeGroupRecord);
                    break;

                case _increasingFee:
                    feeTranche = CreateIncreasingFeeTrancheFromFeeGroupRecord(feeGroupRecord);
                    break;

                case _bondCountFee:
                    feeTranche = CreateBondCountBasedFeeTrancheFromFeeGroupRecord(feeGroupRecord);
                    break;

                case _trancheBasedFee:
                    feeTranche = CreateTrancheBasedFeeTrancheFromFeeGroupRecord(feeGroupRecord);
                    break;

                case _collateralBasedFee:
                    feeTranche = CreateCollateralBasedFeeTrancheFromFeeGroupRecord(feeGroupRecord);
                    break;
                    
                default:
                    throw new Exception((feeType == null)
                        ? string.Format("ERROR: Fee type was not provided for fee group named '{0}'.", feeGroupRecord.FeeGroupName)
                        : string.Format("ERROR: Fee type of '{0}' is not yet supported.", feeGroupRecord.FeeType));
            }

            // Set next payment month and payment frequency based off of fee payment convention
            feeTranche.MonthsToNextPayment = DateUtility.MonthsBetweenTwoDates(_securitizationFirstCashFlowDate, feeGroupRecord.FirstFeePaymentDate) + 1;
            feeTranche.PaymentFrequencyInMonths = ExtractPaymentFrequencyFromFeeGroupRecord(feeGroupRecord);

            // Set whether or not to include fee shortfall if it is recoverable and not in priority of payments
            var isFeeShortFallInPriorityOfPayments = _priorityOfPayments.OrderedListOfEntries
                .Any(e => e.TrancheName == feeGroupRecord.FeeGroupName && e.TrancheCashFlowType == TrancheCashFlowType.FeesShortfall);

            if (!feeGroupRecord.IsShortfallRecoverable && isFeeShortFallInPriorityOfPayments)
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' has short-fall that is set to be not recoverable on the fee groups overview," + 
                    " but the priority of payments waterfall includes an entry for the shortfall of these fees.", 
                    feeGroupRecord.FeeGroupName));
            }

            if (!feeGroupRecord.IsShortfallRecoverable && feeGroupRecord.IsShortfallPaidFromReserves)
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' has short-fall that is set to be not recoverable on the fee groups overview," +
                    " but on the same tab it is indicated that short-fall for that fee can be paid from reserves.",
                    feeGroupRecord.FeeGroupName));
            }

            if (feeGroupRecord.IsShortfallRecoverable && !isFeeShortFallInPriorityOfPayments)
            {
                feeTranche.IncludePaymentShortfall = true;
            }

            feeTranche.IsShortfallPaidFromReserves = feeGroupRecord.IsShortfallPaidFromReserves;

            // Add relevant list of fee details
            AddRelevantFeeDetails(feeTranche, listOfFeeDetailRecords);
            return feeTranche;
        }

        private void AddRelevantFeeDetails(FeeTranche feeTranche, List<FeeDetailRecord> listOfFeeDetailRecords)
        {
            var relevantFeeDetailRecords = listOfFeeDetailRecords.Where(f => f.FeeGroupName == feeTranche.TrancheName);
            foreach(var feeDetailRecord in relevantFeeDetailRecords)
            {
                if (feeDetailRecord.ApplyFeeIncreaseRate.GetValueOrDefault())
                {
                    var increasingFeeTranche = feeTranche as PeriodicallyIncreasingFeeTranche;
                    if (increasingFeeTranche != null)
                    {
                        increasingFeeTranche.AddIncreasingFee(feeDetailRecord.FeeName, feeDetailRecord.AnnualFeeAmount);
                        feeTranche = increasingFeeTranche;
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: The fee group name '{0}' has a fee detail named '{1}' that is indicated to be an increasing fee," +
                            " but this fee group is not of a type that supports increasing fees.", 
                            feeTranche.TrancheName,
                            feeDetailRecord.FeeName));
                    }
                }
                else
                {
                    // Note, there is not any support for delayed fees written in here just yet
                    feeTranche.AddBaseFee(feeDetailRecord.FeeName, feeDetailRecord.AnnualFeeAmount);
                }
            }
        }

        private int ExtractPaymentFrequencyFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var feePaymentConvention = FeePaymentConventionExcelConverter.ConvertString(feeGroupRecord.PaymentConvention);
            var paymentFrequencyInMonths = (int) feePaymentConvention;
            return paymentFrequencyInMonths;
        }

        private FeeTranche CreateFlatFeeTrancheFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var feePaymentConvention = FeePaymentConventionExcelConverter.ConvertString(feeGroupRecord.PaymentConvention);
            var proRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.Accrual);
            var availableFundsRetriever = AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromFeeGroupRecord(feeGroupRecord);
            var initialProRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.InitialFeeAccrual);

            if (feeGroupRecord.InitialAccrualEndDate.HasValue)
            {
                if (initialProRatingDayCountConvention == default(DayCountConvention))
                {
                    throw new Exception(string.Format("ERROR: The fee group named '{0}' has only part of its initial accrual period set up. Please double-check the inputs file.",
                        feeGroupRecord.FeeGroupName));
                }

                return new InitialPeriodSpecialAccrualFeeTranche(
                    feeGroupRecord.FeeGroupName,
                    feeGroupRecord.InitialAccrualEndDate.Value,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    PaymentConvention.ProRated,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever)
                {
                    AccruedPayment = feeGroupRecord.AccruedPayment
                };
            }

            if (initialProRatingDayCountConvention != default(DayCountConvention))
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' has only part of its initial accrual period set up. Please double-check the inputs file.",
                    feeGroupRecord.FeeGroupName));
            }

            return new FlatFeeTranche(
                feeGroupRecord.FeeGroupName,
                feePaymentConvention,
                proRatingDayCountConvention,
                availableFundsRetriever)
            {
                AccruedPayment = feeGroupRecord.AccruedPayment
            };
        }

        private FeeTranche CreateIncreasingFeeTrancheFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var feePaymentConvention = FeePaymentConventionExcelConverter.ConvertString(feeGroupRecord.PaymentConvention);
            var proRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.Accrual);
            var availableFundsRetriever = AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromFeeGroupRecord(feeGroupRecord);

            if (!feeGroupRecord.IncreaseRate.HasValue || !feeGroupRecord.IncreaseFrequency.HasValue)
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' is designated as an increasing fee, but no increase rate and/or frequency was provided.",
                    feeGroupRecord.FeeGroupName));
            }

            return new PeriodicallyIncreasingFeeTranche(
                feeGroupRecord.FeeGroupName,
                feeGroupRecord.IncreaseRate.Value,
                feeGroupRecord.IncreaseFrequency.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                availableFundsRetriever);
        }

        private FeeTranche CreateBondCountBasedFeeTrancheFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var feePaymentConvention = FeePaymentConventionExcelConverter.ConvertString(feeGroupRecord.PaymentConvention);
            var proRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.Accrual);
            var availableFundsRetriever = AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromFeeGroupRecord(feeGroupRecord);
            var initialProRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.InitialFeeAccrual);

            if (!feeGroupRecord.AnnualFeePerUnit.HasValue || !feeGroupRecord.AnnualMinFeeAmount.HasValue || !feeGroupRecord.AnnualMaxFeeAmount.HasValue)
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' is designated as a bond count fee, but no fee per unit and/or min/max fee amount was provided.",
                    feeGroupRecord.FeeGroupName));
            }

            if (!(initialProRatingDayCountConvention != default(DayCountConvention) && feeGroupRecord.InitialAccrualEndDate.HasValue))
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' has only part of its initial accrual period set up. Please double-check the inputs file.",
                    feeGroupRecord.FeeGroupName));
            }

            return new BondCountBasedFeeTranche(
                feeGroupRecord.FeeGroupName,
                feeGroupRecord.AnnualFeePerUnit.Value,
                feeGroupRecord.AnnualMinFeeAmount.Value,
                feeGroupRecord.AnnualMaxFeeAmount.Value,
                feeGroupRecord.InitialAccrualEndDate.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                PaymentConvention.ProRated,
                initialProRatingDayCountConvention,
                availableFundsRetriever)
            {
                AccruedPayment = feeGroupRecord.AccruedPayment
            };
        }

        private FeeTranche CreateTrancheBasedFeeTrancheFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var feePaymentConvention = FeePaymentConventionExcelConverter.ConvertString(feeGroupRecord.PaymentConvention);
            var proRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.Accrual);
            var availableFundsRetriever = AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromFeeGroupRecord(feeGroupRecord);
            var initialProRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.InitialFeeAccrual);

            if (!feeGroupRecord.PercentOfTrancheBalance.HasValue || !feeGroupRecord.RollingAverageMonths.HasValue)
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' is designated as a collateral based fee, but no percentage of tranche balance and/or rolling average months was provided.",
                    feeGroupRecord.FeeGroupName));
            }

            if (!(initialProRatingDayCountConvention != default(DayCountConvention) && feeGroupRecord.InitialAccrualEndDate.HasValue))
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' has only part of its initial accrual period set up. Please double-check the inputs file.",
                    feeGroupRecord.FeeGroupName));
            }

            var percentOfTrancheBalanceFee = new PercentOfTrancheBalanceFeeTranche(
                feeGroupRecord.FeeGroupName,
                feeGroupRecord.AnnualMinFeeAmount.GetValueOrDefault(0.0),
                feeGroupRecord.PercentOfTrancheBalance.Value,
                feeGroupRecord.RollingAverageMonths.Value,
                feeGroupRecord.InitialAccrualEndDate.Value,
                feePaymentConvention,
                proRatingDayCountConvention,
                PaymentConvention.ProRated,
                initialProRatingDayCountConvention,
                availableFundsRetriever)
            {
                AccruedPayment = feeGroupRecord.AccruedPayment
            };

            // Three associated tranches is pretty arbitrary, but the inputs file format doesn't lend itself to a more elegant solution
            if (feeGroupRecord.AssociatedTrancheName1 != null && feeGroupRecord.AssociatedTrancheName1 != string.Empty)
            {
                percentOfTrancheBalanceFee.AssociatedTrancheNames.Add(feeGroupRecord.AssociatedTrancheName1);
            }
            if (feeGroupRecord.AssociatedTrancheName2 != null && feeGroupRecord.AssociatedTrancheName2 != string.Empty)
            {
                percentOfTrancheBalanceFee.AssociatedTrancheNames.Add(feeGroupRecord.AssociatedTrancheName2);
            }
            if (feeGroupRecord.AssociatedTrancheName3 != null && feeGroupRecord.AssociatedTrancheName3 != string.Empty)
            {
                percentOfTrancheBalanceFee.AssociatedTrancheNames.Add(feeGroupRecord.AssociatedTrancheName3);
            }

            return percentOfTrancheBalanceFee;
        }

        private FeeTranche CreateCollateralBasedFeeTrancheFromFeeGroupRecord(FeeGroupRecord feeGroupRecord)
        {
            var feePaymentConvention = FeePaymentConventionExcelConverter.ConvertString(feeGroupRecord.PaymentConvention);
            var proRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.Accrual);
            var availableFundsRetriever = AvailableFundsRetrieverExcelConverter.ExtractAvailableFundsRetrieverFromFeeGroupRecord(feeGroupRecord);
            var initialProRatingDayCountConvention = DayCountConventionExcelConverter.ConvertString(feeGroupRecord.InitialFeeAccrual);

            if (!feeGroupRecord.PercentOfCollateral.HasValue || !feeGroupRecord.BalanceUpdateFreq.HasValue)
            {
                throw new Exception(string.Format("ERROR: The fee group named '{0}' is designated as a collateral based fee, but no percentage of collateral and/or balance update frequency was provided.",
                    feeGroupRecord.FeeGroupName));
            }

            if (feeGroupRecord.BeginningOrEndingBalance != null && 
               (feeGroupRecord.BeginningOrEndingBalance != _beginningBalance && feeGroupRecord.BeginningOrEndingBalance != _endingBalance))
            {
                throw new Exception(string.Format("ERROR: The choice of '{0}' is not a valid choice of 'Beginning Balance' or 'Ending Balance' for fee group '{1}'.",
                    feeGroupRecord.BeginningOrEndingBalance,
                    feeGroupRecord.FeeGroupName));
            }

            PercentOfCollateralBalanceFeeTranche percentOfCollateralBalanceFeeTranche;

            if (feeGroupRecord.AnnualMinFeeAmount.HasValue)
            {
                percentOfCollateralBalanceFeeTranche = new PercentOfCollateralBalanceFeeTranche(
                    feeGroupRecord.FeeGroupName,
                    feeGroupRecord.AnnualMinFeeAmount.Value,
                    feeGroupRecord.PercentOfCollateral.Value,
                    feeGroupRecord.BalanceUpdateFreq.Value,
                    feeGroupRecord.InitialAccrualEndDate.Value,
                    feePaymentConvention,
                    proRatingDayCountConvention,
                    PaymentConvention.ProRated,
                    initialProRatingDayCountConvention,
                    availableFundsRetriever)
                {
                    AccruedPayment = feeGroupRecord.AccruedPayment
                };
            }
            else
            {
                if (!(initialProRatingDayCountConvention != default(DayCountConvention) && feeGroupRecord.InitialAccrualEndDate.HasValue))
                {
                    throw new Exception(string.Format("ERROR: The fee group named '{0}' has only part of its initial accrual period set up. Please double-check the inputs file.",
                        feeGroupRecord.FeeGroupName));
                }

                percentOfCollateralBalanceFeeTranche = new SpecialInitialYearAccrualPercentOfCollateralBalanceFeeTranche(
                    feeGroupRecord.FeeGroupName,
                    feeGroupRecord.PercentOfCollateral.Value,
                    feeGroupRecord.BalanceUpdateFreq.Value,
                    feeGroupRecord.InitialAccrualEndDate.Value,
                    initialProRatingDayCountConvention,
                    proRatingDayCountConvention,
                    feePaymentConvention,
                    availableFundsRetriever)
                {
                    AccruedPayment = feeGroupRecord.AccruedPayment
                };
            }

            if (feeGroupRecord.BeginningOrEndingBalance == _beginningBalance)
            {
                percentOfCollateralBalanceFeeTranche.UseStartingBalance = true;
            }

            return percentOfCollateralBalanceFeeTranche;
        }
    }
}
