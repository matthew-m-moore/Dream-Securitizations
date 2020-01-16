using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class RevenueCategoryDimMapping : EntityTypeConfiguration<RevenueCategoryDimEntity>
    {
        public RevenueCategoryDimMapping()
        {
            HasKey(t => t.RevenueCategoryKey);

            ToTable("RevenueCategoryDim", Constants.FinanceManagementSchemaName);

            Property(t => t.RevenueCategoryKey)
                .HasColumnName("RevenueCategoryKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RevenueCategory).HasColumnName("RevenueCategory");
        }
    }
}
