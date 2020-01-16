using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class SourceSystemDimMapping : EntityTypeConfiguration<SourceSystemDimEntity>
    {
        public SourceSystemDimMapping()
        {
            HasKey(t => t.SourceSystemKey);

            ToTable("SourceSystemDim", Constants.FinanceManagementSchemaName);

            Property(t => t.SourceSystemKey)
                .HasColumnName("SourceSystemKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SourceSystem).HasColumnName("SourceSystem");
        }
    }
}
