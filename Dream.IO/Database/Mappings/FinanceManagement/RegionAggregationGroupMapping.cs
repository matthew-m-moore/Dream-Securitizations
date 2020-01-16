using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class RegionAggregationGroupMapping : EntityTypeConfiguration<RegionAggregationGroupEntity>
    {
        public RegionAggregationGroupMapping()
        {
            HasKey(t => t.RegionAggregationGroupId);

            ToTable("RegionAggregationGroup", Constants.FinanceManagementSchemaName);

            Property(t => t.RegionAggregationGroupId)
                .HasColumnName("RegionAggregationGroupId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RegionAggregationGroupDescription).HasColumnName("RegionAggregationGroupDescription");
            Property(t => t.IsAggregationGroupActive).HasColumnName("IsAggregationGroupActive");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRegionAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.RegionAggregationGroupDescription, "RegionAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));

            MapToStoredProcedures(s =>
                s.Update((u => u.HasName("UpdateRegionAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.RegionAggregationGroupId, "RegionAggregationGroupId")
                    .Parameter(p => p.RegionAggregationGroupDescription, "RegionAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));
        }
    }
}
