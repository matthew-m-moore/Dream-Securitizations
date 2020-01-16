using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class DayCountConventionMapping : EntityTypeConfiguration<DayCountConventionEntity>
    {
        public DayCountConventionMapping()
        {
            HasKey(t => t.DayCountConventionId);

            ToTable("DayCountConvention", Constants.DreamSchemaName);

            Property(t => t.DayCountConventionId)
                .HasColumnName("DayCountConventionId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.DayCountConventionDescription).HasColumnName("DayCountConventionDescription");
        }
    }
}
