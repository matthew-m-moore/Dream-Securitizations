using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class VectorParentMapping : EntityTypeConfiguration<VectorParentEntity>
    {
        public VectorParentMapping()
        {
            HasKey(t => t.VectorParentId);

            ToTable("VectorParent", Constants.DreamSchemaName);

            Property(t => t.VectorParentId)
                .HasColumnName("VectorParentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.IsFlatVector).HasColumnName("IsFlatVector");
            Property(t => t.VectorParentDescription).HasColumnName("VectorParentDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertVectorParent", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.IsFlatVector, "IsFlatVector")
                    .Parameter(p => p.VectorParentDescription, "VectorParentDescription")
                    )));
        }
    }
}
