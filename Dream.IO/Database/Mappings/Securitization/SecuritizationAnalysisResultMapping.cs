using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisResultMapping : EntityTypeConfiguration<SecuritizationAnalysisResultEntity>
    {
        public SecuritizationAnalysisResultMapping()
        {
            HasKey(t => t.SecuritizationAnalysisResultId);

            ToTable("SecuritizationAnalysisResult", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisResultId)
                .HasColumnName("SecuritizationAnalysisResultId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationAnalysisDataSetId).HasColumnName("SecuritizationAnalysisDataSetId");
            Property(t => t.SecuritizationAnalysisVersionId).HasColumnName("SecuritizationAnalysisVersionId");
            Property(t => t.SecuritizationAnalysisScenarioId).HasColumnName("SecuritizationAnalysisScenarioId");

            Property(t => t.SecuritizationTrancheDetailId).HasColumnName("SecuritizationTrancheDetailId");
            Property(t => t.SecuritizationNodeName).HasColumnName("SecuritizationNodeName");

            Property(t => t.SecuritizationResultTypeId).HasColumnName("SecuritizationResultTypeId");
            Property(t => t.SecuritizationResultValue).HasColumnName("SecuritizationResultValue");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysisResult", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisVersionId, "SecuritizationAnalysisVersionId")
                    .Parameter(p => p.SecuritizationAnalysisScenarioId, "SecuritizationAnalysisScenarioId")
                    .Parameter(p => p.SecuritizationTrancheDetailId, "SecuritizationTrancheDetailId")
                    .Parameter(p => p.SecuritizationNodeName, "SecuritizationNodeName")
                    .Parameter(p => p.SecuritizationResultTypeId, "SecuritizationResultTypeId")
                    .Parameter(p => p.SecuritizationResultValue, "SecuritizationResultValue")
                    )));
        }
    }
}
