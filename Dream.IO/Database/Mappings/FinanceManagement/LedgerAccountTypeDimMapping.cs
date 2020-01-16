using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LedgerAccountTypeDimMapping : EntityTypeConfiguration<LedgerAccountTypeDimEntity>
    {
        public LedgerAccountTypeDimMapping()
        {
            HasKey(t => t.LedgerAccountTypeKey);

            ToTable("LedgerAccountTypeDim", Constants.FinanceManagementSchemaName);

            Property(t => t.LedgerAccountTypeKey)
                .HasColumnName("LedgerAccountTypeKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LedgerAccountType).HasColumnName("LedgerAccountType");
        }
    }
}
