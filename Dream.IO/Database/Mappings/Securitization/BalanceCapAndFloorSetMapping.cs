using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class BalanceCapAndFloorSetMapping : EntityTypeConfiguration<BalanceCapAndFloorSetEntity>
    {
        public BalanceCapAndFloorSetMapping()
        {
            HasKey(t => t.BalanceCapAndFloorSetId);

            ToTable("BalanceCapAndFloorSet", Constants.DreamSchemaName);

            Property(t => t.BalanceCapAndFloorSetId)
                .HasColumnName("BalanceCapAndFloorSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertBalanceCapAndFloorSet", Constants.DreamSchemaName))));
        }
    }
}
