using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class ProductLineAggregationGroupMappingMapping : EntityTypeConfiguration<ProductLineAggregationGroupMappingEntity>
    {
        public ProductLineAggregationGroupMappingMapping()
        {
            HasKey(t => t.ProductLineAggregationGroupMappingId);

            ToTable("ProductLineAggregationGroupMapping", Constants.FinanceManagementSchemaName);

            Property(t => t.ProductLineAggregationGroupMappingId)
                .HasColumnName("ProductLineAggregationGroupMappingId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ProductLineAggregationGroupId).HasColumnName("ProductLineAggregationGroupId");
            Property(t => t.ProductLineKey).HasColumnName("ProductLineKey");
            Property(t => t.ProductLineAggregationGroupIdentifier).HasColumnName("ProductLineAggregationGroupIdentifier");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertProductLineAggregationGroupMapping", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.ProductLineAggregationGroupId, "ProductLineAggregationGroupId")
                    .Parameter(p => p.ProductLineKey, "ProductLineKey")
                    .Parameter(p => p.ProductLineAggregationGroupIdentifier, "ProductLineAggregationGroupIdentifier")
                    )));
        }
    }
}
