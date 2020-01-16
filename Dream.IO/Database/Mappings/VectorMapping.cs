using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class VectorMapping : EntityTypeConfiguration<VectorEntity>
    {
        public VectorMapping()
        {
            HasKey(t => t.VectorId);

            ToTable("Vector", Constants.DreamSchemaName);

            Property(t => t.VectorId)
                .HasColumnName("VectorId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.VectorParentId).HasColumnName("VectorParentId");

            Property(t => t.VectorPeriod).HasColumnName("VectorPeriod");
            Property(t => t.VectorValue).HasColumnName("VectorValue");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertVector", Constants.DreamSchemaName)
                    .Parameter(p => p.VectorParentId, "VectorParentId")
                    .Parameter(p => p.VectorPeriod, "VectorPeriod")
                    .Parameter(p => p.VectorValue, "VectorValue")
                    )));
        }
    }
}
