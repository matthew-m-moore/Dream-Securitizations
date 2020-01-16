using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class RateIndexGroupMapping : EntityTypeConfiguration<RateIndexGroupEntity>
    {
        public RateIndexGroupMapping()
        {
            HasKey(t => t.RateIndexGroupId);

            ToTable("RateIndexGroup", Constants.DreamSchemaName);

            Property(t => t.RateIndexGroupId)
                .HasColumnName("RateIndexGroupId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RateIndexGroupDescription).HasColumnName("RateIndexGroupDescription");
        }
    }
}
