using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class RedemptionTranchesDetailMapping : EntityTypeConfiguration<RedemptionTranchesDetailEntity>
    {
        public RedemptionTranchesDetailMapping()
        {
            HasKey(t => t.RedemptionTranchesDetailId);

            ToTable("RedemptionTranchesDetail", Constants.DreamSchemaName);

            Property(t => t.RedemptionTranchesDetailId)
                .HasColumnName("RedemptionTranchesDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RedemptionTranchesSetId).HasColumnName("RedemptionTranchesSetId");

            Property(t => t.TrancheDetailId).HasColumnName("TrancheDetailId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRedemptionTranchesDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.RedemptionTranchesSetId, "RedemptionTranchesSetId")
                    .Parameter(p => p.TrancheDetailId, "TrancheDetailId")
                    )));
        }
    }
}
