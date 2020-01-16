using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class FundsDistributionTypeMapping : EntityTypeConfiguration<FundsDistributionTypeEntity>
    {
        public FundsDistributionTypeMapping()
        {
            HasKey(t => t.FundsDistributionTypeId);

            ToTable("FundsDistributionType", Constants.DreamSchemaName);

            Property(t => t.FundsDistributionTypeId)
                .HasColumnName("FundsDistributionTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.FundsDistributionTypeDescription).HasColumnName("FundsDistributionTypeDescription");
        }
    }
}
