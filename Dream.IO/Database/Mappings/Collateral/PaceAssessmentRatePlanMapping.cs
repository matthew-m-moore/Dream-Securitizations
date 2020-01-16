using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PaceAssessmentRatePlanMapping : EntityTypeConfiguration<PaceAssessmentRatePlanEntity>
    {
        public PaceAssessmentRatePlanMapping()
        {
            HasKey(t => t.PaceAssessmentRatePlanId);

            ToTable("PaceAssessmentRatePlan", Constants.DreamSchemaName);

            Property(t => t.PaceAssessmentRatePlanId)
                .HasColumnName("PaceAssessmentRatePlanId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PaceAssessmentRatePlanTermSetId).HasColumnName("PaceAssessmentRatePlanTermSetId");
            Property(t => t.PropertyStateId).HasColumnName("PropertyStateId");
            Property(t => t.CouponRate).HasColumnName("CouponRate");
            Property(t => t.BuyDownRate).HasColumnName("BuyDownRate");
            Property(t => t.TermInYears).HasColumnName("TermInYears");
        }
    }
}
