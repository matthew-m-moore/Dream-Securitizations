using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class RedemptionTranchesSetMapping : EntityTypeConfiguration<RedemptionTranchesSetEntity>
    {
        public RedemptionTranchesSetMapping()
        {
            HasKey(t => t.RedemptionTranchesSetId);

            ToTable("RedemptionTranchesSet", Constants.DreamSchemaName);

            Property(t => t.RedemptionTranchesSetId)
                .HasColumnName("RedemptionTranchesSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRedemptionTranchesSet", Constants.DreamSchemaName))));
        }
    }
}
