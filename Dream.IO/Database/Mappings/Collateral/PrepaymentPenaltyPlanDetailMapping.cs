using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PrepaymentPenaltyPlanDetailMapping : EntityTypeConfiguration<PrepaymentPenaltyPlanDetailEntity>
    {
        public PrepaymentPenaltyPlanDetailMapping()
        {
            HasKey(t => t.PrepaymentPenaltyPlanDetailId);

            ToTable("PrepaymentPenaltyPlanDetail", Constants.DreamSchemaName);

            Property(t => t.PrepaymentPenaltyPlanDetailId)
                .HasColumnName("PrepaymentPenaltyPlanDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PrepaymentPenaltyPlanId).HasColumnName("PrepaymentPenaltyPlanId");

            Property(t => t.EndingMonthlyPeriodOfPenalty).HasColumnName("EndingMonthlyPeriodOfPenalty");
            Property(t => t.PenaltyAmount).HasColumnName("PenaltyAmount");
            Property(t => t.PenaltyType).HasColumnName("PenaltyType");
        }
    }
}
