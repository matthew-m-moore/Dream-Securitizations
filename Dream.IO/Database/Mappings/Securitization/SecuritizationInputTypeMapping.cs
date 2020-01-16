using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationInputTypeMapping : EntityTypeConfiguration<SecuritizationInputTypeEntity>
    {
        public SecuritizationInputTypeMapping()
        {
            HasKey(t => t.SecuritizationInputTypeId);

            ToTable("SecuritizationInputType", Constants.DreamSchemaName);

            Property(t => t.SecuritizationInputTypeId)
                .HasColumnName("SecuritizationInputTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationInputTypeDescription).HasColumnName("SecuritizationInputTypeDescription");
        }
    }
}
