using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class FeeDetailMapping : EntityTypeConfiguration<FeeDetailEntity>
    {
        public FeeDetailMapping()
        {
            HasKey(t => t.FeeDetailId);

            ToTable("FeeDetail", Constants.DreamSchemaName);

            Property(t => t.FeeDetailId)
                .HasColumnName("FeeDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.FeeGroupDetailId).HasColumnName("FeeGroupDetailId");

            Property(t => t.FeeName).HasColumnName("FeeName");
            Property(t => t.FeeAssociatedTrancheDetailId).HasColumnName("FeeAssociatedTrancheDetailId");
            Property(t => t.FeeAmount).HasColumnName("FeeAmount");
            Property(t => t.IsIncreasingFee).HasColumnName("IsIncreasingFee");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertFeeDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.FeeGroupDetailId, "FeeGroupDetailId")
                    .Parameter(p => p.FeeName, "FeeName")
                    .Parameter(p => p.FeeAssociatedTrancheDetailId, "FeeAssociatedTrancheDetailId")
                    .Parameter(p => p.FeeAmount, "FeeAmount")
                    .Parameter(p => p.IsIncreasingFee, "IsIncreasingFee")
                    )));
        }
    }
}
