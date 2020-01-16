using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class CostCenterAggregationGroupMapping : EntityTypeConfiguration<CostCenterAggregationGroupEntity>
    {
        public CostCenterAggregationGroupMapping()
        {
            HasKey(t => t.CostCenterAggregationGroupId);

            ToTable("CostCenterAggregationGroup", Constants.FinanceManagementSchemaName);

            Property(t => t.CostCenterAggregationGroupId)
                .HasColumnName("CostCenterAggregationGroupId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CostCenterAggregationGroupDescription).HasColumnName("CostCenterAggregationGroupDescription");
            Property(t => t.IsAggregationGroupActive).HasColumnName("IsAggregationGroupActive");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertCostCenterAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.CostCenterAggregationGroupDescription, "CostCenterAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));

            MapToStoredProcedures(s =>
                s.Update((u => u.HasName("UpdateCostCenterAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.CostCenterAggregationGroupId, "CostCenterAggregationGroupId")
                    .Parameter(p => p.CostCenterAggregationGroupDescription, "CostCenterAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));
        }
    }
}
