using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class TrancheTypeMapping : EntityTypeConfiguration<TrancheTypeEntity>
    {
        public TrancheTypeMapping()
        {
            HasKey(t => t.TrancheTypeId);

            ToTable("TrancheType", Constants.DreamSchemaName);

            Property(t => t.TrancheTypeId)
                .HasColumnName("TrancheTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.TrancheTypeDescription).HasColumnName("TrancheTypeDescription");
            Property(t => t.IsVisible).HasColumnName("IsVisible");
            Property(t => t.IsFeeTranche).HasColumnName("IsFeeTranche");
            Property(t => t.IsReserveTranche).HasColumnName("IsReserveTranche");
            Property(t => t.IsInterestPayingTranche).HasColumnName("IsInterestPayingTranche");
        }
    }
}
