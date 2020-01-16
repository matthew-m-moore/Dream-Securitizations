using Dream.Core.Interfaces;
using Dream.Core.Repositories.Database;
using Dream.Core.Repositories.Excel;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Collateral;
using Dream.IO.Excel.Entities.CollateralTapeRecords;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Savers
{
    public class PaceAssessmentDatabaseSaver : DatabaseSaver
    {
        private PaceAssessmentDatabaseRepository _paceAssessmentDatabaseRepository;
        private PaceAssessmentExcelDataRepository _paceAssessmentExcelDataRepository;

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        public PaceAssessmentDatabaseSaver(
            ICollateralRetriever paceAssessmentCollateralRetriever,
            DateTime cutOffDate,
            string description)
        : base (cutOffDate, description)
        {
            if (paceAssessmentCollateralRetriever is PaceAssessmentDatabaseRepository paceAssessmentDatabaseRepository)
            {
                if (!paceAssessmentDatabaseRepository.PaceAssessmentRecordEntities.Any())
                {
                    paceAssessmentDatabaseRepository.GetAllPaceAssessments();
                }

                _paceAssessmentDatabaseRepository = paceAssessmentDatabaseRepository;

            }
            else if (paceAssessmentCollateralRetriever is PaceAssessmentExcelDataRepository paceAssessmentExcelDataRepository)
            {
                if (!paceAssessmentExcelDataRepository.PaceAssessmentTapeRecords.Any())
                {
                    paceAssessmentExcelDataRepository.GetAllPaceAssessments();
                }

                _paceAssessmentExcelDataRepository = paceAssessmentExcelDataRepository;
            }
            else
            {
                throw new Exception("INTERNAL ERROR: The appropriate collateral retrieiver for PACE assessments could not be identified for saving. Please report this error.");
            }
        }

        public int SavePaceAssessments()
        {
            var paceAssessmentRecordDataSetEntity = new PaceAssessmentRecordDataSetEntity
            {
                CutOffDate = _CutOffDate,
                PaceAssessmentRecordDataSetDescription = _Description
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.PaceAssessmentRecordDataSetEntities.Add(paceAssessmentRecordDataSetEntity);
                securitizationEngineContext.SaveChanges();
            }

            SavePaceAssessmentRecords(paceAssessmentRecordDataSetEntity.PaceAssessmentRecordDataSetId);
            return paceAssessmentRecordDataSetEntity.PaceAssessmentRecordDataSetId;
        }

        private void SavePaceAssessmentRecords(int paceAssessmentRecordDataSetId)
        {
            var paceAssessmentRecordEntities = new List<PaceAssessmentRecordEntity>();

            if (_paceAssessmentDatabaseRepository != null)
            {
                paceAssessmentRecordEntities = _paceAssessmentDatabaseRepository.PaceAssessmentRecordEntities
                    .Select(p => CopyPaceAssessmentRecordEntity(p, paceAssessmentRecordDataSetId)).ToList();
            }

            if (_paceAssessmentExcelDataRepository != null)
            {
                // Note, I'm ignoring the rate plan term set and prepayment penalty plan term sets, since there is no knowing
                // what those might look like in the database right now
                paceAssessmentRecordEntities.AddRange(GetAssessmentLevelPaceTapeRecords(paceAssessmentRecordDataSetId));
                paceAssessmentRecordEntities.AddRange(GetMunicipalBondLevelPaceTapeRecords(paceAssessmentRecordDataSetId));
                paceAssessmentRecordEntities.AddRange(GetReplineLevelPaceTapeRecords(paceAssessmentRecordDataSetId));
                paceAssessmentRecordEntities.AddRange(GetCommercialPaceTapeRecords(paceAssessmentRecordDataSetId));
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.PaceAssessmentRecordEntities.AddRange(paceAssessmentRecordEntities);
                securitizationEngineContext.SaveChanges();
            }
        }

        private PaceAssessmentRecordEntity CopyPaceAssessmentRecordEntity(
            PaceAssessmentRecordEntity paceAssessmentRecordEntity, 
            int paceAssessmentRecordDataSetId)
        {
            return new PaceAssessmentRecordEntity
            {
                PaceAssessmentRecordDataSetId = paceAssessmentRecordDataSetId,
                LoanId = paceAssessmentRecordEntity.LoanId,
                BondId = paceAssessmentRecordEntity.BondId,
                ReplineId = paceAssessmentRecordEntity.ReplineId,
                PropertyStateId = paceAssessmentRecordEntity.PropertyStateId,
                Balance = paceAssessmentRecordEntity.Balance,
                ProjectCost = paceAssessmentRecordEntity.ProjectCost,
                CouponRate = paceAssessmentRecordEntity.CouponRate,
                BuyDownRate = paceAssessmentRecordEntity.BuyDownRate,
                TermInYears = paceAssessmentRecordEntity.TermInYears,
                FundingDate = paceAssessmentRecordEntity.FundingDate,
                BondFirstPaymentDate = paceAssessmentRecordEntity.BondFirstPaymentDate,
                BondFirstPrincipalPaymentDate = paceAssessmentRecordEntity.BondFirstPrincipalPaymentDate,
                BondMaturityDate = paceAssessmentRecordEntity.BondMaturityDate,
                CashFlowStartDate = paceAssessmentRecordEntity.CashFlowStartDate,
                InterestAccrualStartDate = paceAssessmentRecordEntity.InterestAccrualStartDate,
                InterestAccrualEndMonth = paceAssessmentRecordEntity.InterestAccrualEndMonth,
                InterestPaymentFrequencyInMonths = paceAssessmentRecordEntity.InterestPaymentFrequencyInMonths,
                PrincipalPaymentFrequencyInMonths = paceAssessmentRecordEntity.PrincipalPaymentFrequencyInMonths,
                NumberOfUnderlyingBonds = paceAssessmentRecordEntity.NumberOfUnderlyingBonds,
                AccruedInterest = paceAssessmentRecordEntity.AccruedInterest,
                ActualPrepaymentsReceived = paceAssessmentRecordEntity.ActualPrepaymentsReceived,
                PrepaymentPenaltyPlanId = paceAssessmentRecordEntity.PrepaymentPenaltyPlanId,
                PaceAssessmentRatePlanTermSetId = paceAssessmentRecordEntity.PaceAssessmentRatePlanTermSetId,
            };
        }

        private List<PaceAssessmentRecordEntity> GetAssessmentLevelPaceTapeRecords(int paceAssessmentRecordDataSetId)
        {
            return _paceAssessmentExcelDataRepository.PaceAssessmentTapeRecords.OfType<AssessmentLevelPaceTapeRecord>()
                .Select(p => 
                {
                    return new PaceAssessmentRecordEntity
                    {
                        PaceAssessmentRecordDataSetId = paceAssessmentRecordDataSetId,

                        Balance = p.Balance,
                        CouponRate = p.CouponRate,
                        BuyDownRate = p.BuyDownRate,
                        TermInYears = p.TermInYears,
                        CashFlowStartDate = p.StartDate,
                        InterestAccrualStartDate = p.InterestStartDate,
                        InterestPaymentFrequencyInMonths = p.InterestPaymentFrequency,
                        PrincipalPaymentFrequencyInMonths = p.PrincipalPaymentFrequency,
                        ActualPrepaymentsReceived = p.ActualPrepayments,
                        InterestAccrualEndMonth = p.InterestAccrualEndMonth,

                        LoanId = p.LoanId,
                        BondId = p.MunicipalBondId,
                        ReplineId = p.ReplineId,
                        FundingDate = p.FundingDate,
                        BondFirstPaymentDate = p.BondFirstPaymentDate,
                        BondMaturityDate = p.BondMaturityDate,
                        AccruedInterest = p.AccruedInterest,
                    };
                }).ToList();
        }

        private List<PaceAssessmentRecordEntity> GetMunicipalBondLevelPaceTapeRecords(int paceAssessmentRecordDataSetId)
        {
            return _paceAssessmentExcelDataRepository.PaceAssessmentTapeRecords.OfType<MunicipalBondLevelPaceTapeRecord>()
                .Select(p =>
                {
                    return new PaceAssessmentRecordEntity
                    {
                        PaceAssessmentRecordDataSetId = paceAssessmentRecordDataSetId,

                        Balance = p.Balance,
                        CouponRate = p.CouponRate,
                        BuyDownRate = p.BuyDownRate,
                        TermInYears = p.TermInYears,
                        CashFlowStartDate = p.StartDate,
                        InterestAccrualStartDate = p.InterestStartDate,
                        InterestPaymentFrequencyInMonths = p.InterestPaymentFrequency,
                        PrincipalPaymentFrequencyInMonths = p.PrincipalPaymentFrequency,
                        ActualPrepaymentsReceived = p.ActualPrepayments,
                        InterestAccrualEndMonth = p.InterestAccrualEndMonth,

                        BondId = p.MunicipalBondId,
                        ReplineId = p.ReplineId,
                        FundingDate = p.FundingDate,
                        BondFirstPaymentDate = p.FirstPaymentDate,
                        BondMaturityDate = p.MaturityDate,
                        AccruedInterest = p.AccruedInterest,

                        NumberOfUnderlyingBonds = 1,
                    };
                }).ToList();
        }

        private List<PaceAssessmentRecordEntity> GetReplineLevelPaceTapeRecords(int paceAssessmentRecordDataSetId)
        {
            return _paceAssessmentExcelDataRepository.PaceAssessmentTapeRecords.OfType<ReplineLevelPaceTapeRecord>()
                .Select(p =>
                {
                    return new PaceAssessmentRecordEntity
                    {
                        PaceAssessmentRecordDataSetId = paceAssessmentRecordDataSetId,

                        Balance = p.Balance,
                        CouponRate = p.CouponRate,
                        BuyDownRate = p.BuyDownRate,
                        TermInYears = p.TermInYears,
                        CashFlowStartDate = p.StartDate,
                        InterestAccrualStartDate = p.InterestStartDate,
                        InterestPaymentFrequencyInMonths = p.InterestPaymentFrequency,
                        PrincipalPaymentFrequencyInMonths = p.PrincipalPaymentFrequency,
                        ActualPrepaymentsReceived = p.ActualPrepayments,
                        InterestAccrualEndMonth = p.InterestAccrualEndMonth,

                        ReplineId = p.ReplineId,
                        AccruedInterest = p.PreAnalysisInterest,
                        BondFirstPaymentDate = p.FirstPaymentDate,
                        BondFirstPrincipalPaymentDate = p.FirstPrincipalPaymentDate,
                        NumberOfUnderlyingBonds = p.NumberOfBonds,
                    };
                }).ToList();
        }

        private List<PaceAssessmentRecordEntity> GetCommercialPaceTapeRecords(int paceAssessmentRecordDataSetId)
        {
            return _paceAssessmentExcelDataRepository.PaceAssessmentTapeRecords.OfType<CommercialPaceTapeRecord>()
                .Select(p =>
                {
                    return new PaceAssessmentRecordEntity
                    {
                        PaceAssessmentRecordDataSetId = paceAssessmentRecordDataSetId,

                        Balance = p.Balance,
                        CouponRate = p.CouponRate,
                        BuyDownRate = p.BuyDownRate,
                        TermInYears = p.TermInYears,
                        CashFlowStartDate = p.StartDate,
                        InterestAccrualStartDate = p.InterestStartDate,
                        InterestPaymentFrequencyInMonths = p.InterestPaymentFrequency,
                        PrincipalPaymentFrequencyInMonths = p.PrincipalPaymentFrequency,
                        ActualPrepaymentsReceived = p.ActualPrepayments,
                        InterestAccrualEndMonth = p.InterestAccrualEndMonth,

                        ReplineId = p.Description,
                        BondFirstPaymentDate = p.FirstPaymentDate,
                        BondFirstPrincipalPaymentDate = p.FirstPrincipalPaymentDate,
                    };
                }).ToList();
        }
    }
}
