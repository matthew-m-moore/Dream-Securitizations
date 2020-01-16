using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class CostCenterAggregationGroupMappingMapping : EntityTypeConfiguration<CostCenterAggregationGroupMappingEntity>
    {
        public CostCenterAggregationGroupMappingMapping()
        {
            HasKey(t => t.CostCenterAggregationGroupMappingId);

            ToTable("CostCenterAggregationGroupMapping", Constants.FinanceManagementSchemaName);

            Property(t => t.CostCenterAggregationGroupMappingId)
                .HasColumnName("CostCenterAggregationGroupMappingId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CostCenterAggregationGroupId).HasColumnName("CostCenterAggregationGroupId");
            Property(t => t.CostCenterKey).HasColumnName("CostCenterKey");
            Property(t => t.CostCenterAggregationGroupIdentifier).HasColumnName("CostCenterAggregationGroupIdentifier");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertCostCenterAggregationGroupMapping", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.CostCenterAggregationGroupId, "CostCenterAggregationGroupId")
                    .Parameter(p => p.CostCenterKey, "CostCenterKey")
                    .Parameter(p => p.CostCenterAggregationGroupIdentifier, "CostCenterAggregationGroupIdentifier")
                    )));
        }
    }
}
