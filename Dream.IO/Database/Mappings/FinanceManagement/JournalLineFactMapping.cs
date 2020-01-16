using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class JournalLineFactMapping : EntityTypeConfiguration<JournalLineFactEntity>
    {
        public JournalLineFactMapping()
        {
            HasKey(t => t.JournalLineFactDWId);

            ToTable("JournalLineFact", Constants.FinanceManagementSchemaName);

            Property(t => t.JournalLineFactDWId)
                .HasColumnName("JournalLineFactDWId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.JournalLineFactLastUpdated).HasColumnName("JournalLineFactLastUpdated");
            Property(t => t.ReportDate).HasColumnName("ReportDate");
            Property(t => t.AccountingDate).HasColumnName("AccountingDate");
            Property(t => t.NetAmount).HasColumnName("NetAmount");

            Property(t => t.CompanyKey).HasColumnName("CompanyKey");
            Property(t => t.EmployeeKey).HasColumnName("EmployeeKey");
            Property(t => t.LedgerAccountKey).HasColumnName("LedgerAccountKey");
            Property(t => t.LedgerAccountTypeKey).HasColumnName("LedgerAccountTypeKey");
            Property(t => t.SpendCategoryKey).HasColumnName("SpendCategoryKey");
            Property(t => t.RevenueCategoryKey).HasColumnName("RevenueCategoryKey");
            Property(t => t.ProductLineKey).HasColumnName("ProductLineKey");
            Property(t => t.RegionKey).HasColumnName("RegionKey");

            Property(t => t.CFSupplier).HasColumnName("CFSupplier");
            Property(t => t.SupplierInvoice).HasColumnName("SupplierInvoice");
            Property(t => t.LineMemo).HasColumnName("LineMemo");
        }
    }
}
