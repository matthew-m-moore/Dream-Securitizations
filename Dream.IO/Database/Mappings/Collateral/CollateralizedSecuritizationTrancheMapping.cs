using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class CollateralizedSecuritizationTrancheMapping : EntityTypeConfiguration<CollateralizedSecuritizationTrancheEntity>
    {
        public CollateralizedSecuritizationTrancheMapping()
        {
            HasKey(t => t.CollateralizedSecuritizationTrancheId);

            ToTable("CollateralizedSecuritizationTranche", Constants.DreamSchemaName);

            Property(t => t.CollateralizedSecuritizationTrancheId)
                .HasColumnName("CollateralizedSecuritizationTrancheId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CollateralizedSecuritizationDataSetId).HasColumnName("CollateralizedSecuritizationDataSetId");

            Property(t => t.SecuritizationAnalysisDataSetId).HasColumnName("SecuritizationAnalysisDataSetId");
            Property(t => t.SecuritizationAnalysisVersionId).HasColumnName("SecuritizationAnalysisVersionId");
            Property(t => t.SecuritizatizedTrancheDetailId).HasColumnName("SecuritizatizedTrancheDetailId");
            Property(t => t.SecuritizatizedTranchePercentage).HasColumnName("SecuritizatizedTranchePercentage");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertCollateralizedSecuritizationTranche", Constants.DreamSchemaName)
                    .Parameter(p => p.CollateralizedSecuritizationDataSetId, "CollateralizedSecuritizationDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisVersionId, "SecuritizationAnalysisVersionId")
                    .Parameter(p => p.SecuritizatizedTrancheDetailId, "SecuritizatizedTrancheDetailId")
                    .Parameter(p => p.SecuritizatizedTranchePercentage, "SecuritizatizedTranchePercentage")
                    )));
        }
    }
}
