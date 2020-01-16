using Dream.Common;
using Dream.Common.Enums;
using Dream.Common.Utilities;
using Dream.Core.BusinessLogic.Containers;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.IO.Database.Entities.Collateral;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.Core.Converters.Database.Collateral
{
    /// <summary>
    /// Converts various PACE assessment entity objects into PACE assessment business objects.
    /// </summary>
    public class PaceAssessmentDatabaseConverter : LoanDatabaseConverter
    {
        private int _integerIdCounter;
        private readonly DateTime _interestAccrualStartDate;
        private Dictionary<string, int> _assessmentsPerBondDictionary;

        private PropertyStateDatabaseConverter _propertyStateDatabaseConverter;
        private PaceRatePlanDatabaseConverter _paceRatePlanConverter;
        private PrepaymentPenaltyPlanDatabaseConverter _prepaymentPenaltyPlanConverter;

        private bool _usePreFundingStartDate;

        public PaceAssessmentDatabaseConverter(
            DateTime collateralCutOffDate,
            DateTime? cashFlowStartDate,
            DateTime? interestAccrualStartDate,
            bool usePreFundingStartDate,
            PropertyStateDatabaseConverter propertyStateDatabaseConverter,
            PaceRatePlanDatabaseConverter paceRatePlanConverter,
            PrepaymentPenaltyPlanDatabaseConverter prepaymentPenaltyPlanConverter) 
            : base (collateralCutOffDate, cashFlowStartDate)
        {
            // Note that the interest accrual date cannot come before the collateral cut-off date, so the value is floored
            if (interestAccrualStartDate.HasValue)
            {
                var flooredDate = new DateTime(Math.Max(collateralCutOffDate.Ticks, interestAccrualStartDate.Value.Ticks));
                _interestAccrualStartDate = flooredDate;
            }

            _usePreFundingStartDate = usePreFundingStartDate;
            _propertyStateDatabaseConverter = propertyStateDatabaseConverter;
            _paceRatePlanConverter = paceRatePlanConverter;
            _prepaymentPenaltyPlanConverter = prepaymentPenaltyPlanConverter;
        }

        /// <summary>
        /// Converts a collection of database-sourced PACE assessement records into a collection of PaceAssessment business objects.
        /// </summary>
        public List<PaceAssessment> ConvertListOfPaceTapeRecords(List<PaceAssessmentRecordEntity> listOfPaceAssessmentRecordEntities)
        {
            _assessmentsPerBondDictionary = listOfPaceAssessmentRecordEntities
                .Where(IsAssessmentLevelEntity)
                .GroupBy(t => t.BondId)
                .ToDictionary(g => g.Key, g => g.Count());

            var listOfPaceAssessments = listOfPaceAssessmentRecordEntities
                .AsParallel()
                .WithDegreeOfParallelism(Constants.ProcessorCount)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .Select(ConvertPaceAssessmentRecordEntity).ToList();

            return listOfPaceAssessments;
        }

        /// <summary>
        /// Converts a database-sourced PACE assessement record into a PaceAssessment business object.
        /// </summary>
        public PaceAssessment ConvertPaceAssessmentRecordEntity(PaceAssessmentRecordEntity paceAssessmentRecordEntity)
        {
            var firstPaymentDate = paceAssessmentRecordEntity.BondFirstPaymentDate;
            var monthsToNextInterestPayment = DateUtility.MonthsBetweenTwoDates(
                _CollateralCutOffDate,
                firstPaymentDate);

            var firstPrincipalPaymentDate = GetFirstPrincipalPaymentDate(paceAssessmentRecordEntity);
            var monthsToNextPrincipalPayment = DateUtility.MonthsBetweenTwoDates(
                _CollateralCutOffDate,
                firstPrincipalPaymentDate);

            var principalPaymentFrequency = paceAssessmentRecordEntity.PrincipalPaymentFrequencyInMonths ?? Constants.MonthsInOneYear;
            var additionalMonthsToMaturity = monthsToNextPrincipalPayment - principalPaymentFrequency;
            var totalMonthsToMaturity = (paceAssessmentRecordEntity.TermInYears * Constants.MonthsInOneYear) + additionalMonthsToMaturity;

            var paceAssessment = new PaceAssessment()
            {
                StartDate = paceAssessmentRecordEntity.CashFlowStartDate ?? _CashFlowStartDate,
                FirstPaymentDate = firstPaymentDate,
                
                LastPreFundingDate = paceAssessmentRecordEntity.LastPreFundDate,
                PreFundingStartDate = (paceAssessmentRecordEntity.IsPreFundingRepline.GetValueOrDefault(false) && _usePreFundingStartDate)
                    ? paceAssessmentRecordEntity.PreFundingStartDate
                    : null,

                PropertyState = _propertyStateDatabaseConverter.ConvertId(paceAssessmentRecordEntity.PropertyStateId),

                Balance = paceAssessmentRecordEntity.Balance,
                ActualPrepayments = paceAssessmentRecordEntity.ActualPrepaymentsReceived.GetValueOrDefault(0.0),
                InitialCouponRate = paceAssessmentRecordEntity.CouponRate,
                InterestAccrualStartDate = paceAssessmentRecordEntity.InterestAccrualStartDate ?? _interestAccrualStartDate,
                InterestAccrualDayCountConvention = DayCountConvention.Thirty360,

                MonthsToNextInterestPayment = monthsToNextInterestPayment,
                MonthsToNextPrincipalPayment = monthsToNextPrincipalPayment,

                InterestPaymentFrequencyInMonths = paceAssessmentRecordEntity.InterestPaymentFrequencyInMonths ?? Constants.MonthsInOneYear / 2,
                PrincipalPaymentFrequencyInMonths = paceAssessmentRecordEntity.PrincipalPaymentFrequencyInMonths ?? Constants.MonthsInOneYear,

                AmortizationTermInMonths = Math.Min(paceAssessmentRecordEntity.TermInYears * Constants.MonthsInOneYear, totalMonthsToMaturity),
                MaturityTermInMonths = totalMonthsToMaturity,
            };

            SetPaceAssessmentIdAndBondCount(paceAssessmentRecordEntity, paceAssessment);
            SetPreAnalysisAccruedInterest(paceAssessmentRecordEntity, paceAssessment);
            SetPaceAssessmentRatePlan(paceAssessmentRecordEntity, paceAssessment);
            SetPrepaymentPenalties(paceAssessmentRecordEntity, paceAssessment);

            // This will roll-forward the collateral appropriately to a new start date
            paceAssessment.AdjustLoanToCashFlowStartDate(_CollateralCutOffDate);

            paceAssessment.SetInterestAccrualEndMonths((Month)paceAssessmentRecordEntity.InterestAccrualEndMonth.GetValueOrDefault());
            return paceAssessment;
        }

        private bool IsAssessmentLevelEntity(PaceAssessmentRecordEntity paceAssessmentRecordEntity)
        {
            if (paceAssessmentRecordEntity.LoanId != null) return true;
            else return false;
        }

        private DateTime GetFirstPrincipalPaymentDate(PaceAssessmentRecordEntity paceAssessmentRecordEntity)
        {
            if (paceAssessmentRecordEntity.BondFirstPrincipalPaymentDate.HasValue)
            {
                return paceAssessmentRecordEntity.BondFirstPrincipalPaymentDate.Value;
            }

            if (paceAssessmentRecordEntity.BondMaturityDate.HasValue)
            {
                var termInYears = paceAssessmentRecordEntity.TermInYears;
                var maturityDate = paceAssessmentRecordEntity.BondMaturityDate.Value;

                var firstPrincipalPaymentDate = maturityDate.AddYears(1 - termInYears);
                return firstPrincipalPaymentDate;
            }

            throw new Exception(string.Format("ERROR: The PACE assessment record for LoanId: '{0}', BondId: '{1}', ReplineId: '{2}' contains neither a first principal payment date, "
                + "nor a maturity date, and thus cannot be processed. Please check the data and try again.",
                paceAssessmentRecordEntity.LoanId,
                paceAssessmentRecordEntity.BondId,
                paceAssessmentRecordEntity.ReplineId));
        }

        private void SetPaceAssessmentIdAndBondCount(PaceAssessmentRecordEntity paceAssessmentRecordEntity, PaceAssessment paceAssessment)
        {
            if (paceAssessmentRecordEntity.LoanId.HasValue)
            {
                paceAssessment.IntegerId = paceAssessmentRecordEntity.LoanId.Value;
                paceAssessment.StringId = paceAssessmentRecordEntity.LoanId.Value.ToString();
            }
            else
            {
                _integerIdCounter++;
                paceAssessment.IntegerId = _integerIdCounter;
                paceAssessment.StringId = paceAssessmentRecordEntity.BondId
                                       ?? paceAssessmentRecordEntity.ReplineId
                                       ?? string.Empty;
            }

            paceAssessment.MunicipalBondId = paceAssessmentRecordEntity.BondId ?? string.Empty;

            if (paceAssessmentRecordEntity.NumberOfUnderlyingBonds.HasValue)
            {
                paceAssessment.BondCount = paceAssessmentRecordEntity.NumberOfUnderlyingBonds.Value;
                return;
            }

            if (!string.IsNullOrEmpty(paceAssessment.MunicipalBondId) 
                && _assessmentsPerBondDictionary.ContainsKey(paceAssessment.MunicipalBondId))
            {
                paceAssessment.BondCount = 1.0 / _assessmentsPerBondDictionary[paceAssessment.MunicipalBondId];
            }
        }

        private void SetPreAnalysisAccruedInterest(PaceAssessmentRecordEntity paceAssessmentRecordEntity, PaceAssessment paceAssessment)
        {
            if (paceAssessmentRecordEntity.AccruedInterest.HasValue)
            {
                paceAssessment.AccruedInterest = paceAssessmentRecordEntity.AccruedInterest.Value;
                return;
            }

            // Note that if no funding date is provided, accrued interest will always be zero, so it may be more appropriate to throw an error for the else bracket
            if (paceAssessmentRecordEntity.FundingDate.HasValue)
            {
                var fundingDate = paceAssessmentRecordEntity.FundingDate.Value;
                var interestAccrualStartDate = paceAssessmentRecordEntity.InterestAccrualStartDate ?? _interestAccrualStartDate;

                var yearsOfAccruedInterest = DateUtility.CalculateTimePeriodInYears(DayCountConvention.Thirty360, fundingDate, interestAccrualStartDate);
                var assessmentBalance = paceAssessmentRecordEntity.Balance + paceAssessmentRecordEntity.ActualPrepaymentsReceived.GetValueOrDefault(0.0);
                var couponRate = paceAssessmentRecordEntity.CouponRate;

                paceAssessment.AccruedInterest = yearsOfAccruedInterest * assessmentBalance * couponRate;
                paceAssessment.FundingDate = fundingDate;
            }
        }

        private void SetPaceAssessmentRatePlan(PaceAssessmentRecordEntity paceAssessmentRecordEntity, PaceAssessment paceAssessment)
        {
            if (!paceAssessmentRecordEntity.BuyDownRate.HasValue) return;

            paceAssessment.RatePlan = new PaceRatePlan { BuyDownRate = paceAssessmentRecordEntity.BuyDownRate.Value };

            if (_paceRatePlanConverter == null) return;

            if (paceAssessmentRecordEntity.PaceAssessmentRatePlanTermSetId.HasValue)
            {
                var termInYears = paceAssessmentRecordEntity.TermInYears;
                var ratePlanTermSetId = paceAssessmentRecordEntity.PaceAssessmentRatePlanTermSetId.Value;

                _paceRatePlanConverter.AssignPaceRatePlan(paceAssessment, termInYears, ratePlanTermSetId);
            }
        }

        private void SetPrepaymentPenalties(PaceAssessmentRecordEntity paceAssessmentRecordEntity, PaceAssessment paceAssessment)
        {
            if (paceAssessmentRecordEntity.PrepaymentPenaltyPlanId.HasValue)
            {
                var prepaymentPenaltyPlanId = paceAssessmentRecordEntity.PrepaymentPenaltyPlanId.Value;
                _prepaymentPenaltyPlanConverter.AssignPrepaymentPenaltyPlan(paceAssessment, prepaymentPenaltyPlanId);
            }
        }
    }
}
