using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class RegionAggregationGroupMappingMapping : EntityTypeConfiguration<RegionAggregationGroupMappingEntity>
    {
        public RegionAggregationGroupMappingMapping()
        {
            HasKey(t => t.RegionAggregationGroupMappingId);

            ToTable("RegionAggregationGroupMapping", Constants.FinanceManagementSchemaName);

            Property(t => t.RegionAggregationGroupMappingId)
                .HasColumnName("RegionAggregationGroupMappingId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RegionAggregationGroupId).HasColumnName("RegionAggregationGroupId");
            Property(t => t.RegionKey).HasColumnName("RegionKey");
            Property(t => t.RegionAggregationGroupIdentifier).HasColumnName("RegionAggregationGroupIdentifier");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRegionAggregationGroupMapping", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.RegionAggregationGroupId, "RegionAggregationGroupId")
                    .Parameter(p => p.RegionKey, "RegionKey")
                    .Parameter(p => p.RegionAggregationGroupIdentifier, "RegionAggregationGroupIdentifier")
                    )));
        }
    }
}
