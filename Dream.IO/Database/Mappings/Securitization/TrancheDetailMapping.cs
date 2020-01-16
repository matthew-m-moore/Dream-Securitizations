using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class TrancheDetailMapping : EntityTypeConfiguration<TrancheDetailEntity>
    {
        public TrancheDetailMapping()
        {
            HasKey(t => t.TrancheDetailId);

            ToTable("TrancheDetail", Constants.DreamSchemaName);

            Property(t => t.TrancheDetailId)
                .HasColumnName("TrancheDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.TrancheName).HasColumnName("TrancheName");
            Property(t => t.TrancheBalance).HasColumnName("TrancheBalance");
            Property(t => t.TrancheAccruedPayment).HasColumnName("TrancheAccruedPayment");
            Property(t => t.TrancheAccruedInterest).HasColumnName("TrancheAccruedInterest");
            Property(t => t.TrancheTypeId).HasColumnName("TrancheTypeId");
            Property(t => t.TrancheCouponId).HasColumnName("TrancheCouponId");

            Property(t => t.PaymentAvailableFundsRetrievalDetailId).HasColumnName("PaymentAvailableFundsRetrievalDetailId");
            Property(t => t.InterestAvailableFundsRetrievalDetailId).HasColumnName("InterestAvailableFundsRetrievalDetailId");

            Property(t => t.ReserveAccountsSetId).HasColumnName("ReserveAccountsSetId");
            Property(t => t.BalanceCapAndFloorSetId).HasColumnName("BalanceCapAndFloorSetId");
            Property(t => t.FeeGroupDetailId).HasColumnName("FeeGroupDetailId");
            Property(t => t.PaymentConventionId).HasColumnName("PaymentConventionId");
            Property(t => t.AccrualDayCountConventionId).HasColumnName("AccrualDayCountConventionId");

            Property(t => t.InitialPaymentConventionId).HasColumnName("InitialPaymentConventionId");
            Property(t => t.InitialDayCountConventionId).HasColumnName("InitialDayCountConventionId");
            Property(t => t.InitialPeriodEnd).HasColumnName("InitialPeriodEnd");

            Property(t => t.MonthsToNextPayment).HasColumnName("MonthsToNextPayment");
            Property(t => t.MonthsToNextInterestPayment).HasColumnName("MonthsToNextInterestPayment");
            Property(t => t.PaymentFrequencyInMonths).HasColumnName("PaymentFrequencyInMonths");
            Property(t => t.InterestPaymentFrequencyInMonths).HasColumnName("InterestPaymentFrequencyInMonths");

            Property(t => t.IncludePaymentShortfall).HasColumnName("IncludePaymentShortfall");
            Property(t => t.IncludeInterestShortfall).HasColumnName("IncludeInterestShortfall");
            Property(t => t.IsShortfallPaidFromReserves).HasColumnName("IsShortfallPaidFromReserves");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertTrancheDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.TrancheName, "TrancheName")
                    .Parameter(p => p.TrancheBalance, "TrancheBalance")
                    .Parameter(p => p.TrancheTypeId, "TrancheTypeId")
                    .Parameter(p => p.TrancheCouponId, "TrancheCouponId")
                    .Parameter(p => p.PaymentAvailableFundsRetrievalDetailId, "PaymentAvailableFundsRetrievalDetailId")
                    .Parameter(p => p.InterestAvailableFundsRetrievalDetailId, "InterestAvailableFundsRetrievalDetailId")
                    .Parameter(p => p.ReserveAccountsSetId, "ReserveAccountsSetId")
                    .Parameter(p => p.BalanceCapAndFloorSetId, "BalanceCapAndFloorSetId")
                    .Parameter(p => p.FeeGroupDetailId, "FeeGroupDetailId")
                    .Parameter(p => p.PaymentConventionId, "PaymentConventionId")
                    .Parameter(p => p.AccrualDayCountConventionId, "AccrualDayCountConventionId")
                    .Parameter(p => p.InitialPaymentConventionId, "InitialPaymentConventionId")
                    .Parameter(p => p.InitialDayCountConventionId, "InitialDayCountConventionId")
                    .Parameter(p => p.InitialPeriodEnd, "InitialPeriodEnd")
                    .Parameter(p => p.MonthsToNextPayment, "MonthsToNextPayment")
                    .Parameter(p => p.MonthsToNextInterestPayment, "MonthsToNextInterestPayment")
                    .Parameter(p => p.PaymentFrequencyInMonths, "PaymentFrequencyInMonths")
                    .Parameter(p => p.InterestPaymentFrequencyInMonths, "InterestPaymentFrequencyInMonths")
                    .Parameter(p => p.IncludePaymentShortfall, "IncludePaymentShortfall")
                    .Parameter(p => p.IncludeInterestShortfall, "IncludeInterestShortfall")
                    .Parameter(p => p.IsShortfallPaidFromReserves, "IsShortfallPaidFromReserves")
                    )));
        }
    }
}

