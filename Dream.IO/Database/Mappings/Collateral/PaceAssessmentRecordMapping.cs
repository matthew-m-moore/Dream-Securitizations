using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PaceAssessmentRecordMapping : EntityTypeConfiguration<PaceAssessmentRecordEntity>
    {
        public PaceAssessmentRecordMapping()
        {
            HasKey(t => t.PaceAssessmentRecordId);

            ToTable("PaceAssessmentRecord", Constants.DreamSchemaName);

            Property(t => t.PaceAssessmentRecordId)
                .HasColumnName("PaceAssessmentRecordId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PaceAssessmentRecordDataSetId).HasColumnName("PaceAssessmentRecordDataSetId");

            Property(t => t.LoanId).HasColumnName("LoanId");
            Property(t => t.BondId).HasColumnName("BondId");
            Property(t => t.ReplineId).HasColumnName("ReplineId");
            Property(t => t.PropertyStateId).HasColumnName("PropertyStateId");

            Property(t => t.Balance).HasColumnName("Balance");
            Property(t => t.ProjectCost).HasColumnName("ProjectCost");
            Property(t => t.CouponRate).HasColumnName("CouponRate");
            Property(t => t.BuyDownRate).HasColumnName("BuyDownRate");
            Property(t => t.TermInYears).HasColumnName("TermInYears");
            Property(t => t.FundingDate).HasColumnName("FundingDate");

            Property(t => t.BondFirstPaymentDate).HasColumnName("BondFirstPaymentDate");
            Property(t => t.BondFirstPrincipalPaymentDate).HasColumnName("BondFirstPrincipalPaymentDate");
            Property(t => t.BondMaturityDate).HasColumnName("BondMaturityDate");
            Property(t => t.CashFlowStartDate).HasColumnName("CashFlowStartDate");

            Property(t => t.InterestAccrualStartDate).HasColumnName("InterestAccrualStartDate");
            Property(t => t.InterestAccrualEndMonth).HasColumnName("InterestAccrualEndMonth");

            Property(t => t.InterestPaymentFrequencyInMonths).HasColumnName("InterestPaymentFrequencyInMonths");
            Property(t => t.PrincipalPaymentFrequencyInMonths).HasColumnName("PrincipalPaymentFrequencyInMonths");

            Property(t => t.NumberOfUnderlyingBonds).HasColumnName("NumberOfUnderlyingBonds");
            Property(t => t.AccruedInterest).HasColumnName("AccruedInterest");
            Property(t => t.ActualPrepaymentsReceived).HasColumnName("ActualPrepaymentsReceived");
            Property(t => t.PrepaymentPenaltyPlanId).HasColumnName("PrepaymentPenaltyPlanId");
            Property(t => t.PaceAssessmentRatePlanTermSetId).HasColumnName("PaceAssessmentRatePlanTermSetId");

            Property(t => t.IsPreFundingRepline).HasColumnName("IsPreFundingRepline");
            Property(t => t.LastPreFundDate).HasColumnName("LastPreFundDate");
            Property(t => t.PreFundingStartDate).HasColumnName("PreFundingStartDate");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPaceAssessmentRecord", Constants.DreamSchemaName)
                    .Parameter(p => p.PaceAssessmentRecordDataSetId, "PaceAssessmentRecordDataSetId")
                    .Parameter(p => p.LoanId, "LoanId")
                    .Parameter(p => p.BondId, "BondId")
                    .Parameter(p => p.ReplineId, "ReplineId")
                    .Parameter(p => p.PropertyStateId, "PropertyStateId")

                    .Parameter(p => p.Balance, "Balance")
                    .Parameter(p => p.ProjectCost, "ProjectCost")
                    .Parameter(p => p.CouponRate, "CouponRate")
                    .Parameter(p => p.BuyDownRate, "BuyDownRate")
                    .Parameter(p => p.TermInYears, "TermInYears")
                    .Parameter(p => p.FundingDate, "FundingDate")

                    .Parameter(p => p.BondFirstPaymentDate, "BondFirstPaymentDate")
                    .Parameter(p => p.BondFirstPrincipalPaymentDate, "BondFirstPrincipalPaymentDate")
                    .Parameter(p => p.BondMaturityDate, "BondMaturityDate")
                    .Parameter(p => p.CashFlowStartDate, "CashFlowStartDate")

                    .Parameter(p => p.InterestAccrualStartDate, "InterestAccrualStartDate")
                    .Parameter(p => p.InterestAccrualEndMonth, "InterestAccrualEndMonth")

                    .Parameter(p => p.InterestPaymentFrequencyInMonths, "InterestPaymentFrequencyInMonths")
                    .Parameter(p => p.PrincipalPaymentFrequencyInMonths, "PrincipalPaymentFrequencyInMonths")

                    .Parameter(p => p.NumberOfUnderlyingBonds, "NumberOfUnderlyingBonds")
                    .Parameter(p => p.AccruedInterest, "AccruedInterest")
                    .Parameter(p => p.ActualPrepaymentsReceived, "ActualPrepaymentsReceived")
                    .Parameter(p => p.PrepaymentPenaltyPlanId, "PrepaymentPenaltyPlanId")
                    .Parameter(p => p.PaceAssessmentRatePlanTermSetId, "PaceAssessmentRatePlanTermSetId")

                    .Parameter(p => p.IsPreFundingRepline, "IsPreFundingRepline")
                    .Parameter(p => p.LastPreFundDate, "LastPreFundDate")
                    .Parameter(p => p.PreFundingStartDate, "PreFundingStartDate")
                    )));
        }
    }
}
