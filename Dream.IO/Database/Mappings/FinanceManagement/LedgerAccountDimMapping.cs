using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LedgerAccountDimMapping : EntityTypeConfiguration<LedgerAccountDimEntity>
    {
        public LedgerAccountDimMapping()
        {
            HasKey(t => t.LedgerAccountKey);

            ToTable("LedgerAccountDim", Constants.FinanceManagementSchemaName);

            Property(t => t.LedgerAccountKey)
                .HasColumnName("LedgerAccountKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LedgerAccountNumber).HasColumnName("LedgerAccountNumber");
            Property(t => t.LedgerAccount).HasColumnName("LedgerAccount");
        }
    }
}
