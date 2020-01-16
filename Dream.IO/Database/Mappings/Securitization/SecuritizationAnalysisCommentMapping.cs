using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisCommentMapping : EntityTypeConfiguration<SecuritizationAnalysisCommentEntity>
    {
        public SecuritizationAnalysisCommentMapping()
        {
            HasKey(t => t.SecuritizationAnalysisCommentId);

            ToTable("SecuritizationAnalysisComment", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisCommentId)
                .HasColumnName("SecuritizationAnalysisCommentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationAnalysisDataSetId).HasColumnName("SecuritizationAnalysisDataSetId");
            Property(t => t.SecuritizationAnalysisVersionId).HasColumnName("SecuritizationAnalysisVersionId");

            Property(t => t.IsVisible).HasColumnName("IsVisible");
            Property(t => t.CommentText).HasColumnName("CommentText");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysisComment", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisVersionId, "SecuritizationAnalysisVersionId")
                    .Parameter(p => p.IsVisible, "IsVisible")
                    .Parameter(p => p.CommentText, "CommentText")
                    )));

            MapToStoredProcedures(s =>
                s.Update((i => i.HasName("UpdateSecuritizationAnalysisComment", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisCommentId, "SecuritizationAnalysisCommentId")
                    .Parameter(p => p.IsVisible, "IsVisible")
                    )));
        }
    }
}
