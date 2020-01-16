using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisSummaryMapping : EntityTypeConfiguration<SecuritizationAnalysisSummaryEntity>
    {
        public SecuritizationAnalysisSummaryMapping()
        {
            HasKey(t => t.SecuritizationAnalysisSummaryId);

            ToTable("SecuritizationAnalysisSummary", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisSummaryId)
                .HasColumnName("SecuritizationAnalysisSummaryId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationAnalysisDataSetId).HasColumnName("SecuritizationAnalysisDataSetId");
            Property(t => t.SecuritizationAnalysisVersionId).HasColumnName("SecuritizationAnalysisVersionId");
            Property(t => t.SecuritizationAnalysisScenarioId).HasColumnName("SecuritizationAnalysisScenarioId");

            Property(t => t.SecuritizationNodeName).HasColumnName("SecuritizationNodeName");

            Property(t => t.SecuritizationTrancheDetailId).HasColumnName("SecuritizationTrancheDetailId");
            Property(t => t.SecuritizationTrancheRating).HasColumnName("SecuritizationTrancheRating");
            Property(t => t.SecuritizationTrancheType).HasColumnName("SecuritizationTrancheType");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysisSummary", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisVersionId, "SecuritizationAnalysisVersionId")
                    .Parameter(p => p.SecuritizationAnalysisScenarioId, "SecuritizationAnalysisScenarioId")
                    .Parameter(p => p.SecuritizationNodeName, "SecuritizationNodeName")
                    .Parameter(p => p.SecuritizationTrancheDetailId, "SecuritizationTrancheDetailId")
                    .Parameter(p => p.SecuritizationTrancheRating, "SecuritizationTrancheRating")
                    .Parameter(p => p.SecuritizationTrancheType, "SecuritizationTrancheType")
                    )));
        }
    }
}
