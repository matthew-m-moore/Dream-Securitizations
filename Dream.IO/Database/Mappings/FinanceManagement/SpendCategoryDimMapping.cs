using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class SpendCategoryDimMapping : EntityTypeConfiguration<SpendCategoryDimEntity>
    {
        public SpendCategoryDimMapping()
        {
            HasKey(t => t.SpendCategoryKey);

            ToTable("SpendCategoryDim", Constants.FinanceManagementSchemaName);

            Property(t => t.SpendCategoryKey)
                .HasColumnName("SpendCategoryKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SpendCategory).HasColumnName("SpendCategory");
        }
    }
}
