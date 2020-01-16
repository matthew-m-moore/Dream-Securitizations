using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PrepaymentPenaltyPlanMapping : EntityTypeConfiguration<PrepaymentPenaltyPlanEntity>
    {
        public PrepaymentPenaltyPlanMapping()
        {
            HasKey(t => t.PrepaymentPenaltyPlanId);

            ToTable("PrepaymentPenaltyPlan", Constants.DreamSchemaName);

            Property(t => t.PrepaymentPenaltyPlanId)
                .HasColumnName("PrepaymentPenaltyPlanId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PrepaymentPenaltyPlanDescription).HasColumnName("PrepaymentPenaltyPlanDescription");
        }
    }
}
