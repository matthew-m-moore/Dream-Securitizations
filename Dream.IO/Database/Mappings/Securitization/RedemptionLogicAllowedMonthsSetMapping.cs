using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class RedemptionLogicAllowedMonthsSetMapping : EntityTypeConfiguration<RedemptionLogicAllowedMonthsSetEntity>
    {
        public RedemptionLogicAllowedMonthsSetMapping()
        {
            HasKey(t => t.RedemptionLogicAllowedMonthsSetId);

            ToTable("RedemptionLogicAllowedMonthsSetId", Constants.DreamSchemaName);

            Property(t => t.RedemptionLogicAllowedMonthsSetId)
                .HasColumnName("RedemptionLogicAllowedMonthsSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRedemptionLogicAllowedMonthsSet", Constants.DreamSchemaName))));
        }
    }
}
