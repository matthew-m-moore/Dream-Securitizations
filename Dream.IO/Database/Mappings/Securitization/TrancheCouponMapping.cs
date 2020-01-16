using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class TrancheCouponMapping : EntityTypeConfiguration<TrancheCouponEntity>
    {
        public TrancheCouponMapping()
        {
            HasKey(t => t.TrancheCouponId);

            ToTable("TrancheCoupon", Constants.DreamSchemaName);

            Property(t => t.TrancheCouponId)
                .HasColumnName("TrancheCouponId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.TrancheCouponRateIndexId).HasColumnName("TrancheCouponRateIndexId");
            Property(t => t.TrancheCouponValue).HasColumnName("TrancheCouponValue");
            Property(t => t.TrancheCouponFactor).HasColumnName("TrancheCouponFactor");
            Property(t => t.TrancheCouponMargin).HasColumnName("TrancheCouponMargin");
            Property(t => t.TrancheCouponFloor).HasColumnName("TrancheCouponFloor");
            Property(t => t.TrancheCouponCeiling).HasColumnName("TrancheCouponCeiling");
            Property(t => t.TrancheCouponInterimCap).HasColumnName("TrancheCouponInterimCap");
            Property(t => t.InterestAccrualDayCountConventionId).HasColumnName("InterestAccrualDayCountConventionId");
            Property(t => t.InitialPeriodInterestAccrualDayCountConventionId).HasColumnName("InitialPeriodInterestAccrualDayCountConventionId");
            Property(t => t.InitialPeriodInterestAccrualEndDate).HasColumnName("InitialPeriodInterestAccrualEndDate");
            Property(t => t.InterestRateResetFrequencyInMonths).HasColumnName("InterestRateResetFrequencyInMonths");
            Property(t => t.InterestRateResetLookbackMonths).HasColumnName("InterestRateResetLookbackMonths");
            Property(t => t.MonthsToNextInterestRateReset).HasColumnName("MonthsToNextInterestRateReset");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertTrancheCoupon", Constants.DreamSchemaName)
                    .Parameter(p => p.TrancheCouponRateIndexId, "TrancheCouponRateIndexId")
                    .Parameter(p => p.TrancheCouponValue, "TrancheCouponValue")
                    .Parameter(p => p.TrancheCouponFactor, "TrancheCouponFactor")
                    .Parameter(p => p.TrancheCouponMargin, "TrancheCouponMargin")
                    .Parameter(p => p.TrancheCouponFloor, "TrancheCouponFloor")
                    .Parameter(p => p.TrancheCouponCeiling, "TrancheCouponCeiling")
                    .Parameter(p => p.TrancheCouponInterimCap, "TrancheCouponInterimCap")
                    .Parameter(p => p.InterestAccrualDayCountConventionId, "InterestAccrualDayCountConventionId")
                    .Parameter(p => p.InitialPeriodInterestAccrualDayCountConventionId, "InitialPeriodInterestAccrualDayCountConventionId")
                    .Parameter(p => p.InitialPeriodInterestAccrualEndDate, "InitialPeriodInterestAccrualEndDate")
                    .Parameter(p => p.InterestRateResetFrequencyInMonths, "InterestRateResetFrequencyInMonths")
                    .Parameter(p => p.InterestRateResetLookbackMonths, "InterestRateResetLookbackMonths")
                    .Parameter(p => p.MonthsToNextInterestRateReset, "MonthsToNextInterestRateReset")
                    )));
        }
    }
}

