using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class RedemptionLogicAllowedMonthsDetailMapping : EntityTypeConfiguration<RedemptionLogicAllowedMonthsDetailEntity>
    {
        public RedemptionLogicAllowedMonthsDetailMapping()
        {
            HasKey(t => t.RedemptionLogicAllowedMonthsDetailId);

            ToTable("RedemptionLogicAllowedMonthsDetail", Constants.DreamSchemaName);

            Property(t => t.RedemptionLogicAllowedMonthsDetailId)
                .HasColumnName("RedemptionLogicAllowedMonthsDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RedemptionLogicAllowedMonthsSetId).HasColumnName("RedemptionLogicAllowedMonthsSetId");
            Property(t => t.AllowedMonthId).HasColumnName("AllowedMonthId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRedemptionLogicAllowedMonthsDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.RedemptionLogicAllowedMonthsSetId, "RedemptionLogicAllowedMonthsSetId")
                    .Parameter(p => p.AllowedMonthId, "AllowedMonthId")
                    )));
        }
    }
}
