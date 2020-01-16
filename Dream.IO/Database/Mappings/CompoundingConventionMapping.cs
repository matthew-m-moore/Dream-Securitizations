using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class CompoundingConventionMapping : EntityTypeConfiguration<CompoundingConventionEntity>
    {
        public CompoundingConventionMapping()
        {
            HasKey(t => t.CompoundingConventionId);

            ToTable("CompoundingConvention", Constants.DreamSchemaName);

            Property(t => t.CompoundingConventionId)
                .HasColumnName("CompoundingConventionId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CompoundingConventionDescription).HasColumnName("CompoundingConventionDescription");
        }
    }
}
