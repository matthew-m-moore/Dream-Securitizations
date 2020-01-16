using System;
using System.Linq;
using System.Collections.Generic;
using Dream.Core.Interfaces;
using Dream.Core.BusinessLogic.Valuation;
using Dream.Core.BusinessLogic.ProductTypes;
using Dream.Core.Converters.Excel.Collateral;
using Dream.IO.Excel;
using Dream.IO.Excel.Entities.CollateralTapeRecords;
using Dream.Core.BusinessLogic.SecuritizationEngine;

namespace Dream.Core.Repositories.Excel
{
    public class PaceAssessmentExcelDataRepository : ExcelDataRepository, ICollateralRetriever
    {
        private const string _replineDataTape = "ReplineDataTape";
        private const string _municipalBondDataTape = "MunicipalBondDataTape";
        private const string _assessmentDataTape = "AssessmentDataTape";
        private const string _commercialDataTape = "CommercialDataTape";

        private const string _ratePlanMapping = "RatePlanMapping";
        private const string _prepaymentPenalties= "PrepaymentPenalties";

        protected DateTime _CashFlowStartDate;
        protected DateTime _InterestAccrualStartDate;
        protected DateTime? _LastPreFundingDate;
        protected bool _UsePreFundingStartDate;

        private PaceRatePlanExcelConverter _paceRatePlanConverter;
        private PrepaymentPenaltyPlanExcelConverter _prepaymentPenaltyPlanConverter;

        public List<PaceTapeRecord> PaceAssessmentTapeRecords = new List<PaceTapeRecord>();

        public PaceAssessmentExcelDataRepository(ExcelFileReader excelFileReader) : this(excelFileReader, DateTime.MinValue) { }

        public PaceAssessmentExcelDataRepository(ExcelFileReader excelFileReader, DateTime collateralCutOffDate) : base(excelFileReader, collateralCutOffDate)
        {
            SetupPaceRatePlanConverter();
            SetupPrepaymentPenaltyPlanConverter();
        }

        public PaceAssessmentExcelDataRepository(ExcelFileReader excelFileReader, DateTime collateralCutOffDate, DateTime cashFlowStartDate, DateTime interestAccrualStartDate,
            DateTime? lastPreFundingDate = null) 
            : base(excelFileReader, collateralCutOffDate)
        {
            _CashFlowStartDate = cashFlowStartDate;
            _InterestAccrualStartDate = interestAccrualStartDate;
            SetupPaceRatePlanConverter();
            SetupPrepaymentPenaltyPlanConverter();
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public void SetCollateralDates<T>(T inputs) where T : CashFlowGenerationInput
        {
            _CutOffDate = inputs.CollateralCutOffDate;
            _CashFlowStartDate = inputs.CashFlowStartDate;
            _InterestAccrualStartDate = inputs.InterestAccrualStartDate;

            if (inputs.GetType() == typeof(SecuritizationInput))
            {
                var securitizationInputs = inputs as SecuritizationInput;
                _UsePreFundingStartDate = securitizationInputs.UsePreFundingStartDate.GetValueOrDefault(false);
                _LastPreFundingDate = securitizationInputs.LastPreFundingDate;
            }
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public List<Loan> GetCollateral()
        {
            return GetAllPaceAssessments();
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public double GetTotalCollateralBalance()
        {
            return GetCollateral().Sum(l => l.Balance);
        }

        /// <summary>
        /// Implementation of ICollateralRetriever
        /// </summary>
        public ICollateralRetriever Copy()
        {
            return new PaceAssessmentExcelDataRepository(
                _ExcelFileReader.Copy(),
                new DateTime(_CutOffDate.Ticks),
                new DateTime(_CashFlowStartDate.Ticks),
                new DateTime(_InterestAccrualStartDate.Ticks),
                _LastPreFundingDate);
        }

        /// <summary>
        /// Retrieves all repline-level, bond-level, and assessment-level PACE assessments from an Excel workbook.
        /// </summary>
        public List<Loan> GetAllPaceAssessments()
        {
            if (!PaceAssessmentTapeRecords.Any())
            {
                GetAssessmentLevelPaceAssessments();
                GetMunicipalBondLevelPaceAssessments();
                GetReplineLevelPaceAssessments();
                GetCommercialPaceAssessments();
            }

            var paceAssessmentConverter = new PaceAssessmentExcelConverter(
                _CutOffDate,
                _CashFlowStartDate,
                _InterestAccrualStartDate,
                _LastPreFundingDate,
                _UsePreFundingStartDate,
                _paceRatePlanConverter,
                _prepaymentPenaltyPlanConverter);

            var paceAssessments = new List<Loan>();
            paceAssessments.AddRange(paceAssessmentConverter.ConvertListOfPaceTapeRecords(PaceAssessmentTapeRecords));
            return paceAssessments;
        }

        public void GetReplineLevelPaceAssessments()
        {
            var paceTapeRecords = _ExcelFileReader.GetDataFromSpecificTab<ReplineLevelPaceTapeRecord>(_replineDataTape);
            PaceAssessmentTapeRecords.AddRange(paceTapeRecords);
        }

        private void GetMunicipalBondLevelPaceAssessments()
        {
            var paceTapeRecords = _ExcelFileReader.GetDataFromSpecificTab<MunicipalBondLevelPaceTapeRecord>(_municipalBondDataTape);
            PaceAssessmentTapeRecords.AddRange(paceTapeRecords);
        }

        private void GetAssessmentLevelPaceAssessments()
        {
            var paceTapeRecords = _ExcelFileReader.GetDataFromSpecificTab<AssessmentLevelPaceTapeRecord>(_assessmentDataTape);
            PaceAssessmentTapeRecords.AddRange(paceTapeRecords);
        }

        private void GetCommercialPaceAssessments()
        {
            var paceTapeRecords = _ExcelFileReader.GetDataFromSpecificTab<CommercialPaceTapeRecord>(_commercialDataTape);
            PaceAssessmentTapeRecords.AddRange(paceTapeRecords);
        }

        private void SetupPaceRatePlanConverter()
        {
            var ratePlanMappingRecords = _ExcelFileReader.GetDataFromSpecificTab<RatePlanMappingRecord>(_ratePlanMapping);
            if (!ratePlanMappingRecords.Any()) return;
            _paceRatePlanConverter = new PaceRatePlanExcelConverter(ratePlanMappingRecords);
        }

        private void SetupPrepaymentPenaltyPlanConverter()
        {
            var prepaymentPenaltyRecords = _ExcelFileReader.GetDataFromSpecificTab<PrepaymentPenaltyRecord>(_prepaymentPenalties);
            if (!prepaymentPenaltyRecords.Any()) return;
            _prepaymentPenaltyPlanConverter = new PrepaymentPenaltyPlanExcelConverter(prepaymentPenaltyRecords);
        }
    }
}
