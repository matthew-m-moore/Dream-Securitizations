using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisMapping : EntityTypeConfiguration<SecuritizationAnalysisEntity>
    {
        public SecuritizationAnalysisMapping()
        {
            HasKey(t => t.SecuritizationAnalysisId);

            ToTable("SecuritizationAnalysis", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisId)
                .HasColumnName("SecuritizationAnalysisId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationAnalysisDataSetId).HasColumnName("SecuritizationAnalysisDataSetId");
            Property(t => t.SecuritizationAnalysisVersionId).HasColumnName("SecuritizationAnalysisVersionId");
            Property(t => t.SecuritizationAnalysisScenarioId).HasColumnName("SecuritizationAnalysisScenarioId");

            Property(t => t.SecuritizationInputTypeId).HasColumnName("SecuritizationInputTypeId");
            Property(t => t.SecuritizationInputTypeDataSetId).HasColumnName("SecuritizationInputTypeDataSetId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysis", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisVersionId, "SecuritizationAnalysisVersionId")
                    .Parameter(p => p.SecuritizationAnalysisScenarioId, "SecuritizationAnalysisScenarioId")
                    .Parameter(p => p.SecuritizationInputTypeId, "SecuritizationInputTypeId")
                    .Parameter(p => p.SecuritizationInputTypeDataSetId, "SecuritizationInputTypeDataSetId")
                    )));
        }
    }
}
