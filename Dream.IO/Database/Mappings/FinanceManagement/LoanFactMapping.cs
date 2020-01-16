using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LoanFactMapping : EntityTypeConfiguration<LoanFactEntity>
    {
        public LoanFactMapping()
        {
            HasKey(t => t.LoanFactId);

            ToTable("LoanFact", Constants.FinanceManagementSchemaName);

            Property(t => t.LoanFactId)
                .HasColumnName("LoanFactId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SourceSystemKey).HasColumnName("SourceSystemKey");
            Property(t => t.RecordTypeKey).HasColumnName("RecordTypeKey");
            Property(t => t.LoanLevelKey).HasColumnName("LoanLevelKey");
            Property(t => t.LoanProductKey).HasColumnName("LoanProductKey");

            Property(t => t.StateFullName).HasColumnName("StateFullName");
            Property(t => t.SourceRecordId).HasColumnName("SourceRecordId");
            Property(t => t.TransactionDate).HasColumnName("TransactionDate");
            Property(t => t.TransactionDateKey).HasColumnName("TransactionDateKey");

            Property(t => t.TermInYears).HasColumnName("TermInYears");
            Property(t => t.Rate).HasColumnName("Rate");
            Property(t => t.TransactionAmount).HasColumnName("TransactionAmount");
        }
    }
}
