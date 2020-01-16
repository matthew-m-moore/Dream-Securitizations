using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LoanLevelDimMapping : EntityTypeConfiguration<LoanLevelDimEntity>
    {
        public LoanLevelDimMapping()
        {
            HasKey(t => t.LoanLevelKey);

            ToTable("LoanLevelDim", Constants.FinanceManagementSchemaName);

            Property(t => t.LoanLevelKey)
                .HasColumnName("LoanLevelKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LoanLevel).HasColumnName("LoanLevel");
        }
    }
}
