using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LoanProductDimMapping : EntityTypeConfiguration<LoanProductDimEntity>
    {
        public LoanProductDimMapping()
        {
            HasKey(t => t.LoanProductKey);

            ToTable("LoanProductDim", Constants.FinanceManagementSchemaName);

            Property(t => t.LoanProductKey)
                .HasColumnName("LoanProductKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LoanProduct).HasColumnName("LoanProduct");
        }
    }
}
