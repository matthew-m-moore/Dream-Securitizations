using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationResultTypeMapping : EntityTypeConfiguration<SecuritizationResultTypeEntity>
    {
        public SecuritizationResultTypeMapping()
        {
            HasKey(t => t.SecuritizationResultTypeId);

            ToTable("SecuritizationResultType", Constants.DreamSchemaName);

            Property(t => t.SecuritizationResultTypeId)
                .HasColumnName("SecuritizationResultTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationResultTypeDescription).HasColumnName("SecuritizationResultTypeDescription");
            Property(t => t.StandardDivisor).HasColumnName("StandardDivisor");
        }
    }
}
