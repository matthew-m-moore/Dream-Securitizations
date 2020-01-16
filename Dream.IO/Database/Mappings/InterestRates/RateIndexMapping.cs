using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class RateIndexMapping : EntityTypeConfiguration<RateIndexEntity>
    {
        public RateIndexMapping()
        {
            HasKey(t => t.RateIndexId);

            ToTable("RateIndex", Constants.DreamSchemaName);

            Property(t => t.RateIndexId)
                .HasColumnName("RateIndexId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RateIndexDescription).HasColumnName("RateIndexDescription");
            Property(t => t.TenorInMonths).HasColumnName("TenorInMonths");
            Property(t => t.RateIndexGroupId).HasColumnName("RateIndexGroupId");
        }
    }
}
