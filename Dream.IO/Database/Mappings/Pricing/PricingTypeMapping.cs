using Dream.Common;
using Dream.IO.Database.Entities.Pricing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Pricing
{
    public class PricingTypeMapping : EntityTypeConfiguration<PricingTypeEntity>
    {
        public PricingTypeMapping()
        {
            HasKey(t => t.PricingTypeId);

            ToTable("PricingType", Constants.DreamSchemaName);

            Property(t => t.PricingTypeId)
                .HasColumnName("PricingTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PricingTypeDescription).HasColumnName("PricingTypeDescription");
        }
    }
}
