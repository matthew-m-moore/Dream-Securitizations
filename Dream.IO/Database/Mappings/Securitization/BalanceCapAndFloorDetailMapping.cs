using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class BalanceCapAndFloorDetailMapping : EntityTypeConfiguration<BalanceCapAndFloorDetailEntity>
    {
        public BalanceCapAndFloorDetailMapping()
        {
            HasKey(t => t.BalanceCapAndFloorDetailId);

            ToTable("BalanceCapAndFloorDetail", Constants.DreamSchemaName);

            Property(t => t.BalanceCapAndFloorDetailId)
                .HasColumnName("BalanceCapAndFloorDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.BalanceCapAndFloorSetId).HasColumnName("BalanceCapAndFloorSetId");

            Property(t => t.BalanceCapOrFloor).HasColumnName("BalanceCapOrFloor");
            Property(t => t.PercentageOrDollarAmount).HasColumnName("PercentageOrDollarAmount");
            Property(t => t.CapOrFloorValue).HasColumnName("CapOrFloorValue");
            Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertBalanceCapAndFloorDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.BalanceCapAndFloorSetId, "BalanceCapAndFloorSetId")
                    .Parameter(p => p.BalanceCapOrFloor, "BalanceCapOrFloor")
                    .Parameter(p => p.PercentageOrDollarAmount, "PercentageOrDollarAmount")
                    .Parameter(p => p.CapOrFloorValue, "CapOrFloorValue")
                    .Parameter(p => p.EffectiveDate, "EffectiveDate")
                    )));
        }
    }
}
