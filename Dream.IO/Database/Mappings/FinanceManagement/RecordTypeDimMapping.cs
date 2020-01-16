using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class RecordTypeDimMapping : EntityTypeConfiguration<RecordTypeDimEntity>
    {
        public RecordTypeDimMapping()
        {
            HasKey(t => t.RecordTypeKey);

            ToTable("RecordTypeDim", Constants.FinanceManagementSchemaName);

            Property(t => t.RecordTypeKey)
                .HasColumnName("RecordTypeKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RecordType).HasColumnName("RecordType");
        }
    }
}
