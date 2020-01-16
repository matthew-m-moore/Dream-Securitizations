using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationNodeDataSetMapping : EntityTypeConfiguration<SecuritizationNodeDataSetEntity>
    {
        public SecuritizationNodeDataSetMapping()
        {
            HasKey(t => t.SecuritizationNodeDataSetId);

            ToTable("SecuritizationNodeDataSet", Constants.DreamSchemaName);

            Property(t => t.SecuritizationNodeDataSetId)
                .HasColumnName("SecuritizationNodeDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.SecuritizationNodeDataSetDescription).HasColumnName("SecuritizationNodeDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationNodeDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.SecuritizationNodeDataSetDescription, "SecuritizationNodeDataSetDescription")
                    )));
        }
    }
}
