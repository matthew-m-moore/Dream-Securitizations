using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PaceAssessmentRecordDataSetMapping : EntityTypeConfiguration<PaceAssessmentRecordDataSetEntity>
    {
        public PaceAssessmentRecordDataSetMapping()
        {
            HasKey(t => t.PaceAssessmentRecordDataSetId);

            ToTable("PaceAssessmentRecordDataSet", Constants.DreamSchemaName);

            Property(t => t.PaceAssessmentRecordDataSetId)
                .HasColumnName("PaceAssessmentRecordDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.PaceAssessmentRecordDataSetDescription).HasColumnName("PaceAssessmentRecordDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPaceAssessmentRecordDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.PaceAssessmentRecordDataSetDescription, "PaceAssessmentRecordDataSetDescription")
                    )));
        }
    }
}
