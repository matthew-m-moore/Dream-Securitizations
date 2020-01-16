using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class ProductLineDimMapping : EntityTypeConfiguration<ProductLineDimEntity>
    {
        public ProductLineDimMapping()
        {
            HasKey(t => t.ProductLineKey);

            ToTable("ProductLineDim", Constants.FinanceManagementSchemaName);

            Property(t => t.ProductLineKey)
                .HasColumnName("ProductLineKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ProductLine).HasColumnName("ProductLine");
        }
    }
}
