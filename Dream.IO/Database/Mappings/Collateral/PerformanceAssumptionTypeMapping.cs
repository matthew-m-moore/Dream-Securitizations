using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PerformanceAssumptionTypeMapping : EntityTypeConfiguration<PerformanceAssumptionTypeEntity>
    {
        public PerformanceAssumptionTypeMapping()
        {
            HasKey(t => t.PerformanceAssumptionTypeId);

            ToTable("PerformanceAssumptionType", Constants.DreamSchemaName);

            Property(t => t.PerformanceAssumptionTypeId)
                .HasColumnName("PerformanceAssumptionTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PerformanceAssumptionTypeDescription).HasColumnName("PerformanceAssumptionTypeDescription");
            Property(t => t.PerformanceAssumptionTypeAbbreviation).HasColumnName("PerformanceAssumptionTypeAbbreviation");
        }
    }
}
