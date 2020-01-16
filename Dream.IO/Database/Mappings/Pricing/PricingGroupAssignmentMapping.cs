using Dream.Common;
using Dream.IO.Database.Entities.Pricing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Pricing
{
    public class PricingGroupAssignmentMapping : EntityTypeConfiguration<PricingGroupAssignmentEntity>
    {
        public PricingGroupAssignmentMapping()
        {
            HasKey(t => t.PricingGroupAssignmentId);

            ToTable("PricingGroupAssignment", Constants.DreamSchemaName);

            Property(t => t.PricingGroupAssignmentId)
                .HasColumnName("PricingGroupAssignmentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PricingGroupDataSetId).HasColumnName("PricingGroupDataSetId");

            Property(t => t.InstrumentIdentifier).HasColumnName("InstrumentIdentifier");
            Property(t => t.PricingValue).HasColumnName("PricingValue");
            Property(t => t.PricingTypeId).HasColumnName("PricingTypeId");
            Property(t => t.PricingRateIndexId).HasColumnName("PricingRateIndexId");
            Property(t => t.PricingDayCountConventionId).HasColumnName("PricingDayCountConventionId");
            Property(t => t.PricingCompoundingConventionId).HasColumnName("PricingCompoundingConventionId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPricingGroupAssignment", Constants.DreamSchemaName)
                    .Parameter(p => p.PricingGroupDataSetId, "PricingGroupDataSetId")
                    .Parameter(p => p.InstrumentIdentifier, "InstrumentIdentifier")
                    .Parameter(p => p.PricingValue, "PricingValue")
                    .Parameter(p => p.PricingTypeId, "PricingTypeId")
                    .Parameter(p => p.PricingRateIndexId, "PricingRateIndexId")
                    .Parameter(p => p.PricingDayCountConventionId, "PricingDayCountConventionId")
                    .Parameter(p => p.PricingCompoundingConventionId, "PricingCompoundingConventionId")
                    )));
        }
    }
}
