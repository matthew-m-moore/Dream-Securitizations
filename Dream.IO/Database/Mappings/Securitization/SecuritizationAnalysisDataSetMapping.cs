using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisDataSetMapping : EntityTypeConfiguration<SecuritizationAnalysisDataSetEntity>
    {
        public SecuritizationAnalysisDataSetMapping()
        {
            HasKey(t => t.SecuritizationAnalysisDataSetId);

            ToTable("SecuritizationAnalysisDataSet", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisDataSetId)
                .HasColumnName("SecuritizationAnalysisDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CreatedDate)
                .HasColumnName("CreatedDate")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.IsTemplate).HasColumnName("IsTemplate");
            Property(t => t.IsResecuritization).HasColumnName("IsResecuritization");
            Property(t => t.SecuritizationAnalysisDataSetDescription).HasColumnName("SecuritizationAnalysisDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysisDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.IsTemplate, "IsTemplate")
                    .Parameter(p => p.IsResecuritization, "IsResecuritization")
                    .Parameter(p => p.SecuritizationAnalysisDataSetDescription, "SecuritizationAnalysisDataSetDescription")
                    )));
        }
    }
}
