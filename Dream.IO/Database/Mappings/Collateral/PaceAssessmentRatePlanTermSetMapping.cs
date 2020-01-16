using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PaceAssessmentRatePlanTermSetMapping : EntityTypeConfiguration<PaceAssessmentRatePlanTermSetEntity>
    {
        public PaceAssessmentRatePlanTermSetMapping()
        {
            HasKey(t => t.PaceAssessmentRatePlanTermSetId);

            ToTable("PaceAssessmentRatePlanTermSet", Constants.DreamSchemaName);

            Property(t => t.PaceAssessmentRatePlanTermSetId)
                .HasColumnName("PaceAssessmentRatePlanTermSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.PaceAssessmentRatePlanTermSetDescription).HasColumnName("PaceAssessmentRatePlanTermSetDescription");
        }
    }
}
