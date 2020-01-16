using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class CompanyDimMapping : EntityTypeConfiguration<CompanyDimEntity>
    {
        public CompanyDimMapping()
        {
            HasKey(t => t.CompanyKey);

            ToTable("CompanyDim", Constants.FinanceManagementSchemaName);

            Property(t => t.CompanyKey)
                .HasColumnName("CompanyKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CompanyId).HasColumnName("CompanyId");
            Property(t => t.Company).HasColumnName("Company");
        }
    }
}
