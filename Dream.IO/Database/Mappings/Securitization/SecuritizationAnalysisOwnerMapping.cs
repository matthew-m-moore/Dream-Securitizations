using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisOwnerMapping : EntityTypeConfiguration<SecuritizationAnalysisOwnerEntity>
    {
        public SecuritizationAnalysisOwnerMapping()
        {
            HasKey(t => t.SecuritizationAnalysisOwnerId);

            ToTable("SecuritizationAnalysisOwner", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisOwnerId)
                .HasColumnName("SecuritizationAnalysisOwnerId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationAnalysisDataSetId).HasColumnName("SecuritizationAnalysisDataSetId");
            Property(t => t.SecuritizationAnalysisVersionId).HasColumnName("SecuritizationAnalysisVersionId");

            Property(t => t.ApplicationUserId).HasColumnName("ApplicationUserId");
            Property(t => t.IsReadOnlyToOthers).HasColumnName("IsReadOnlyToOthers");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysisOwner", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.SecuritizationAnalysisVersionId, "SecuritizationAnalysisVersionId")
                    .Parameter(p => p.ApplicationUserId, "ApplicationUserId")
                    .Parameter(p => p.IsReadOnlyToOthers, "IsReadOnlyToOthers")
                    )));

            MapToStoredProcedures(s =>
                s.Update((i => i.HasName("UpdateSecuritizationAnalysisOwner", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisDataSetId, "SecuritizationAnalysisDataSetId")
                    .Parameter(p => p.IsReadOnlyToOthers, "IsReadOnlyToOthers")
                    )));
        }
    }
}
