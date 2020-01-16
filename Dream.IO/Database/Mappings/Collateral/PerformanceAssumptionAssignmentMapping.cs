using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PerformanceAssumptionAssignmentMapping : EntityTypeConfiguration<PerformanceAssumptionAssignmentEntity>
    {
        public PerformanceAssumptionAssignmentMapping()
        {
            HasKey(t => t.PerformanceAssumptionAssignmentId);

            ToTable("PerformanceAssumptionAssignment", Constants.DreamSchemaName);

            Property(t => t.PerformanceAssumptionAssignmentId)
                .HasColumnName("PerformanceAssumptionAssignmentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PerformanceAssumptionDataSetId).HasColumnName("PerformanceAssumptionDataSetId");

            Property(t => t.InstrumentIdentifier).HasColumnName("InstrumentIdentifier");
            Property(t => t.PerformanceAssumptionGrouping).HasColumnName("PerformanceAssumptionGrouping");
            Property(t => t.PerformanceAssumptionIdentifier).HasColumnName("PerformanceAssumptionIdentifier");
            Property(t => t.PerformanceAssumptionTypeId).HasColumnName("PerformanceAssumptionTypeId");
            Property(t => t.VectorParentId).HasColumnName("VectorParentId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPerformanceAssumptionAssignment", Constants.DreamSchemaName)
                    .Parameter(p => p.PerformanceAssumptionDataSetId, "PerformanceAssumptionDataSetId")
                    .Parameter(p => p.InstrumentIdentifier, "InstrumentIdentifier")
                    .Parameter(p => p.PerformanceAssumptionGrouping, "PerformanceAssumptionGrouping")
                    .Parameter(p => p.PerformanceAssumptionIdentifier, "PerformanceAssumptionIdentifier")
                    .Parameter(p => p.PerformanceAssumptionTypeId, "PerformanceAssumptionTypeId")
                    .Parameter(p => p.VectorParentId, "VectorParentId")
                    )));
        }
    }
}
