using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class RedemptionLogicTypeMapping : EntityTypeConfiguration<RedemptionLogicTypeEntity>
    {
        public RedemptionLogicTypeMapping()
        {
            HasKey(t => t.RedemptionLogicTypeId);

            ToTable("RedemptionLogicType", Constants.DreamSchemaName);

            Property(t => t.RedemptionLogicTypeId)
                .HasColumnName("RedemptionLogicTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RedemptionLogicTypeDescription).HasColumnName("RedemptionLogicTypeDescription");
        }
    }
}
