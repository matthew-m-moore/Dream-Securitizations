using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.IO.Excel.Entities.CollateralTapeRecords;
using System.Collections.Generic;
using System;
using System.Linq;
using Dream.Core.BusinessLogic.Containers;

namespace Dream.Core.Converters.Excel.Collateral
{
    /// <summary>
    /// Converts various PACE assessment entity objects into PACE assessment business objects.
    /// </summary>
    public class PaceAssessmentExcelConverter : LoanExcelConverter
    {
        private int _integerIdCounter;
        private readonly DateTime _interestAccrualStartDate;
        private readonly DateTime? _lastPreFundingDate;
        private Dictionary<string, int> _assessmentsPerBondDictionary;

        private PaceRatePlanExcelConverter _paceRatePlanConverter;
        private PrepaymentPenaltyPlanExcelConverter _prepaymentPenaltyPlanConverter;

        private bool _usePreFundingStartDate;

        public PaceAssessmentExcelConverter(
            DateTime collateralCutOffDate, 
            DateTime? cashFlowStartDate, 
            DateTime? interestAccrualStartDate,
            DateTime? lastPreFundingDate,
            bool usePreFundingStartDate,
            PaceRatePlanExcelConverter paceRatePlanConverter,
            PrepaymentPenaltyPlanExcelConverter prepaymentPenaltyPlanConverter) 
            : base (collateralCutOffDate, cashFlowStartDate)
        {
            // Note that the interest accrual date cannot come before the collateral cut-off date, so the value is floored
            if (interestAccrualStartDate.HasValue)
            {
                var flooredDate = new DateTime(Math.Max(collateralCutOffDate.Ticks, interestAccrualStartDate.Value.Ticks));
                _interestAccrualStartDate = flooredDate;
            }

            _lastPreFundingDate = lastPreFundingDate;
            _usePreFundingStartDate = usePreFundingStartDate;
            _paceRatePlanConverter = paceRatePlanConverter;
            _prepaymentPenaltyPlanConverter = prepaymentPenaltyPlanConverter;
        }

        /// <summary>
        /// Converts a collection of Excel-based PaceTapeRecord objects into a collection of PaceAssessment business objects.
        /// </summary>
        public List<PaceAssessment> ConvertListOfPaceTapeRecords<T>(List<T> listOfPaceTapeRecords) where T : PaceTapeRecord
        {
            _assessmentsPerBondDictionary = listOfPaceTapeRecords
                .Where(r => r.RunFlag.GetValueOrDefault(true))
                .OfType<AssessmentLevelPaceTapeRecord>()
                .GroupBy(t => t.MunicipalBondId)
                .ToDictionary(g => g.Key, g => g.Count());

            var listOfPaceAssessments = listOfPaceTapeRecords
                .AsParallel()
                .WithDegreeOfParallelism(Constants.ProcessorCount)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .Where(r => r.RunFlag.GetValueOrDefault(true))
                .Select(ConvertPaceTapeRecord).ToList();

            return listOfPaceAssessments;
        }

        /// <summary>
        /// Converts an Excel-based PaceTapeRecord object into a PaceAssessment business object.
        /// </summary>
        public PaceAssessment ConvertPaceTapeRecord(PaceTapeRecord paceTapeRecord)
        {
            var firstPaymentDate = GetFirstPaymentDate(paceTapeRecord);
            var monthsToNextInterestPayment = DateUtility.MonthsBetweenTwoDates(
                _CollateralCutOffDate, 
                firstPaymentDate);

            var firstPrincipalPaymentDate = GetFirstPrincipalPaymentDate(paceTapeRecord);
            var monthsToNextPrincipalPayment = DateUtility.MonthsBetweenTwoDates(
                _CollateralCutOffDate,
                firstPrincipalPaymentDate);

            var principalPaymentFrequency = paceTapeRecord.PrincipalPaymentFrequency ?? Constants.MonthsInOneYear;
            var additionalMonthsToMaturity = monthsToNextPrincipalPayment - principalPaymentFrequency;
            var totalMonthsToMaturity = (paceTapeRecord.TermInYears * Constants.MonthsInOneYear) + additionalMonthsToMaturity;

            var paceAssessment = new PaceAssessment()
            {
                StartDate = paceTapeRecord.StartDate ?? _CashFlowStartDate,
                FirstPaymentDate = firstPaymentDate,

                LastPreFundingDate = paceTapeRecord.LastPreFundingDate ?? _lastPreFundingDate,
                PreFundingStartDate = (paceTapeRecord.IsPrefundingRepline.GetValueOrDefault(false) && _usePreFundingStartDate)
                    ? paceTapeRecord.PreFundingStartDate
                    : null,

                PropertyState = (paceTapeRecord.PropertyState != null) 
                    ? (PropertyState) Enum.Parse(typeof(PropertyState), paceTapeRecord.PropertyState)
                    : default(PropertyState),

                Balance = paceTapeRecord.Balance,
                ActualPrepayments = paceTapeRecord.ActualPrepayments,
                InitialCouponRate = paceTapeRecord.CouponRate,
                InterestAccrualStartDate = paceTapeRecord.InterestStartDate ?? _interestAccrualStartDate,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,

                MonthsToNextInterestPayment = monthsToNextInterestPayment,
                MonthsToNextPrincipalPayment = monthsToNextPrincipalPayment,

                InterestPaymentFrequencyInMonths = paceTapeRecord.InterestPaymentFrequency ?? Constants.MonthsInOneYear / 2,
                PrincipalPaymentFrequencyInMonths = paceTapeRecord.PrincipalPaymentFrequency ?? Constants.MonthsInOneYear,

                AmortizationTermInMonths = Math.Min(paceTapeRecord.TermInYears * Constants.MonthsInOneYear, totalMonthsToMaturity),
                MaturityTermInMonths = totalMonthsToMaturity,               
            };

            // These first two methods might be kind of slow, since they rely on reflection
            SetPaceAssessmentIdAndBondCount(paceTapeRecord, paceAssessment);
            SetPreAnalysisAccruedInterest(paceTapeRecord, paceAssessment);
            SetPaceAssessmentRatePlan(paceTapeRecord, paceAssessment);
            SetPrepaymentPenalties(paceTapeRecord, paceAssessment);

            // This will roll forward the collateral appropriately to a new start date
            paceAssessment.AdjustLoanToCashFlowStartDate(_CollateralCutOffDate);
            paceAssessment.SetInterestAccrualEndMonths((Month) paceTapeRecord.InterestAccrualEndMonth);

            if (paceAssessment.InterestAccrualStartDate.Ticks < paceAssessment.StartDate.Ticks)
            {
                throw new Exception(string.Format("ERROR: For PACE assessment '{0}', interest start date '{1}' cannot occur before cash flow start date '{2}'.",
                    paceAssessment.StringId, paceAssessment.InterestAccrualStartDate, paceAssessment.StartDate));
            }

            return paceAssessment;
        }

        private DateTime GetFirstPaymentDate(PaceTapeRecord paceTapeRecord)
        {
            if (paceTapeRecord is ReplineLevelPaceTapeRecord)
            {
                var replineLevelPaceTapeRecord = paceTapeRecord as ReplineLevelPaceTapeRecord;
                var firstPaymentDate = replineLevelPaceTapeRecord.FirstPaymentDate;
                return firstPaymentDate;
            }

            if (paceTapeRecord is MunicipalBondLevelPaceTapeRecord)
            {
                var municipalBondLevelPaceTapeRecord = paceTapeRecord as MunicipalBondLevelPaceTapeRecord;
                var firstPaymentDate = municipalBondLevelPaceTapeRecord.FirstPaymentDate;
                return firstPaymentDate;
            }

            if (paceTapeRecord is AssessmentLevelPaceTapeRecord)
            {
                var assesmentLevelPaceTapeRecord = paceTapeRecord as AssessmentLevelPaceTapeRecord;
                var firstPaymentDate = assesmentLevelPaceTapeRecord.BondFirstPaymentDate;
                return firstPaymentDate;
            }

            if (paceTapeRecord is CommercialPaceTapeRecord)
            {
                var commercialPaceTapeRecord = paceTapeRecord as CommercialPaceTapeRecord;
                var firstPaymentDate = commercialPaceTapeRecord.FirstPaymentDate;
                return firstPaymentDate;
            }

            throw new Exception("ERROR: The PACE assessment tape record provided is not yet supported");
        }

        private DateTime GetFirstPrincipalPaymentDate(PaceTapeRecord paceTapeRecord)
        {
            if (paceTapeRecord is ReplineLevelPaceTapeRecord)
            {
                var replineLevelPaceTapeRecord = paceTapeRecord as ReplineLevelPaceTapeRecord;
                var firstPrincipalPaymentDate = replineLevelPaceTapeRecord.FirstPrincipalPaymentDate;
                return firstPrincipalPaymentDate;
            }

            if (paceTapeRecord is MunicipalBondLevelPaceTapeRecord)
            {
                var municipalBondLevelPaceTapeRecord = paceTapeRecord as MunicipalBondLevelPaceTapeRecord;
                var termInYears = municipalBondLevelPaceTapeRecord.TermInYears;
                var maturityDate = municipalBondLevelPaceTapeRecord.MaturityDate;       

                var firstPrincipalPaymentDate = maturityDate.AddYears(1 - termInYears);
                return firstPrincipalPaymentDate;
            }

            if (paceTapeRecord is AssessmentLevelPaceTapeRecord)
            {
                var assesmentLevelPaceTapeRecord = paceTapeRecord as AssessmentLevelPaceTapeRecord;
                var termInYears = assesmentLevelPaceTapeRecord.TermInYears;
                var maturityDate = assesmentLevelPaceTapeRecord.BondMaturityDate;             

                var firstPrincipalPaymentDate = maturityDate.AddYears(1 - termInYears);
                return firstPrincipalPaymentDate;
            }

            if (paceTapeRecord is CommercialPaceTapeRecord)
            {
                var commercialPaceTapeRecord = paceTapeRecord as CommercialPaceTapeRecord;
                var firstPaymentDate = commercialPaceTapeRecord.FirstPrincipalPaymentDate;
                return firstPaymentDate;
            }

            throw new Exception("ERROR: The PACE assessment tape record provided is not yet supported");
        }

        private void SetPaceAssessmentIdAndBondCount(PaceTapeRecord paceTapeRecord, PaceAssessment paceAssessment)
        {
            if (paceTapeRecord is ReplineLevelPaceTapeRecord)
            {
                _integerIdCounter++;

                var replineLevelPaceTapeRecord = paceTapeRecord as ReplineLevelPaceTapeRecord;
                paceAssessment.StringId = replineLevelPaceTapeRecord.ReplineId;
                paceAssessment.IntegerId = _integerIdCounter;
                paceAssessment.BondCount = replineLevelPaceTapeRecord.NumberOfBonds;
                return;
            }

            if (paceTapeRecord is MunicipalBondLevelPaceTapeRecord)
            {
                _integerIdCounter++;

                var municipalBondLevelPaceTapeRecord = paceTapeRecord as MunicipalBondLevelPaceTapeRecord;
                paceAssessment.StringId = municipalBondLevelPaceTapeRecord.MunicipalBondId;
                paceAssessment.IntegerId = _integerIdCounter;
                paceAssessment.MunicipalBondId = municipalBondLevelPaceTapeRecord.MunicipalBondId;
                paceAssessment.BondCount = 1;
                return;
            }

            if (paceTapeRecord is AssessmentLevelPaceTapeRecord)
            {
                var assesmentLevelPaceTapeRecord = paceTapeRecord as AssessmentLevelPaceTapeRecord;
                paceAssessment.StringId = assesmentLevelPaceTapeRecord.LoanId.ToString();
                paceAssessment.IntegerId = assesmentLevelPaceTapeRecord.LoanId;
                paceAssessment.MunicipalBondId = assesmentLevelPaceTapeRecord.MunicipalBondId;
                paceAssessment.BondCount = 1.0 / _assessmentsPerBondDictionary[paceAssessment.MunicipalBondId];
                return;
            }

            if (paceTapeRecord is CommercialPaceTapeRecord)
            {
                var commercialPaceTapeRecord = paceTapeRecord as CommercialPaceTapeRecord;
                paceAssessment.StringId = commercialPaceTapeRecord.Description;
                paceAssessment.IntegerId = _integerIdCounter;
                paceAssessment.BondCount = 1.0;
                return;
            }
        }

        private void SetPreAnalysisAccruedInterest(PaceTapeRecord paceTapeRecord, PaceAssessment paceAssessment)
        {
            if (paceTapeRecord is ReplineLevelPaceTapeRecord)
            {
                var replineLevelPaceTapeRecord = paceTapeRecord as ReplineLevelPaceTapeRecord;
                paceAssessment.AccruedInterest = replineLevelPaceTapeRecord.PreAnalysisInterest.GetValueOrDefault(0.0);
                return;
            }

            if (paceTapeRecord is MunicipalBondLevelPaceTapeRecord)
            {
                var municipalBondLevelPaceTapeRecord = paceTapeRecord as MunicipalBondLevelPaceTapeRecord;
                var fundingDate = municipalBondLevelPaceTapeRecord.FundingDate;
                var startDate = municipalBondLevelPaceTapeRecord.StartDate;

                var accruedInterest = municipalBondLevelPaceTapeRecord.AccruedInterest.GetValueOrDefault(0.0);
                if (!municipalBondLevelPaceTapeRecord.AccruedInterest.HasValue)
                {
                    var interestAccrualStartDate = municipalBondLevelPaceTapeRecord.InterestStartDate ?? _interestAccrualStartDate;
                    var yearsOfAccruedInterest = DateUtility.CalculateTimePeriodInYears(DayCountConvention.Thirty360, fundingDate, interestAccrualStartDate);
                    var assessmentBalance = municipalBondLevelPaceTapeRecord.Balance + municipalBondLevelPaceTapeRecord.ActualPrepayments;
                    var couponRate = municipalBondLevelPaceTapeRecord.CouponRate;

                    accruedInterest = yearsOfAccruedInterest * assessmentBalance * couponRate;
                }

                paceAssessment.AccruedInterest = accruedInterest;
                paceAssessment.FundingDate = fundingDate;
                return;
            }

            if (paceTapeRecord is AssessmentLevelPaceTapeRecord)
            {
                var assesmentLevelPaceTapeRecord = paceTapeRecord as AssessmentLevelPaceTapeRecord;
                var fundingDate = assesmentLevelPaceTapeRecord.FundingDate;
                var startDate = assesmentLevelPaceTapeRecord.StartDate;

                var accruedInterest = assesmentLevelPaceTapeRecord.AccruedInterest.GetValueOrDefault(0.0);
                if (!assesmentLevelPaceTapeRecord.AccruedInterest.HasValue)
                {
                    var interestAccrualStartDate = assesmentLevelPaceTapeRecord.InterestStartDate ?? _interestAccrualStartDate;
                    var yearsOfAccruedInterest = DateUtility.CalculateTimePeriodInYears(DayCountConvention.Thirty360, fundingDate, interestAccrualStartDate);
                    var assessmentBalance = assesmentLevelPaceTapeRecord.Balance + assesmentLevelPaceTapeRecord.ActualPrepayments;
                    var couponRate = assesmentLevelPaceTapeRecord.CouponRate;

                    accruedInterest = yearsOfAccruedInterest * assessmentBalance * couponRate;
                }

                paceAssessment.AccruedInterest = accruedInterest;
                paceAssessment.FundingDate = fundingDate;
                return;
            }
        }

        private void SetPaceAssessmentRatePlan(PaceTapeRecord paceTapeRecord, PaceAssessment paceAssessment)
        {
            paceAssessment.RatePlan = new PaceRatePlan { BuyDownRate = paceTapeRecord.BuyDownRate };

            if (_paceRatePlanConverter == null) return;

            var termInYears = paceTapeRecord.TermInYears;
            var ratePlanName = paceTapeRecord.RatePlan;

            _paceRatePlanConverter.AssignPaceRatePlan(paceAssessment, termInYears, ratePlanName);
        }

        private void SetPrepaymentPenalties(PaceTapeRecord paceTapeRecord, PaceAssessment paceAssessment)
        {
            var commercialPaceTapeRecord = paceTapeRecord as CommercialPaceTapeRecord;
            if (commercialPaceTapeRecord == null) return;
            if (commercialPaceTapeRecord.PrepaymentPenaltyPlan == null) return;

            var prepaymentPenaltyPlan = commercialPaceTapeRecord.PrepaymentPenaltyPlan;
            _prepaymentPenaltyPlanConverter.AssignPrepaymentPenaltyPlan(paceAssessment, prepaymentPenaltyPlan);
        }
    }
}
