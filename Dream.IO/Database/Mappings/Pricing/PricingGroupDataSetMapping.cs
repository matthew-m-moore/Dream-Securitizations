using Dream.Common;
using Dream.IO.Database.Entities.Pricing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Pricing
{
    public class PricingGroupDataSetMapping : EntityTypeConfiguration<PricingGroupDataSetEntity>
    {
        public PricingGroupDataSetMapping()
        {
            HasKey(t => t.PricingGroupDataSetId);

            ToTable("PricingGroupDataSet", Constants.DreamSchemaName);

            Property(t => t.PricingGroupDataSetId)
                .HasColumnName("PricingGroupDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.PricingGroupDataSetDescription).HasColumnName("PricingGroupDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPricingGroupDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.PricingGroupDataSetDescription, "PricingGroupDataSetDescription")
                    )));
        }
    }
}
