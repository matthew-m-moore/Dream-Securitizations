using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class ProductLineAggregationGroupMapping : EntityTypeConfiguration<ProductLineAggregationGroupEntity>
    {
        public ProductLineAggregationGroupMapping()
        {
            HasKey(t => t.ProductLineAggregationGroupId);

            ToTable("ProductLineAggregationGroup", Constants.FinanceManagementSchemaName);

            Property(t => t.ProductLineAggregationGroupId)
                .HasColumnName("ProductLineAggregationGroupId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ProductLineAggregationGroupDescription).HasColumnName("ProductLineAggregationGroupDescription");
            Property(t => t.IsAggregationGroupActive).HasColumnName("IsAggregationGroupActive");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertProductLineAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.ProductLineAggregationGroupDescription, "ProductLineAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));

            MapToStoredProcedures(s =>
                s.Update((u => u.HasName("UpdateProductLineAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.ProductLineAggregationGroupId, "ProductLineAggregationGroupId")
                    .Parameter(p => p.ProductLineAggregationGroupDescription, "ProductLineAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));
        }
    }
}
