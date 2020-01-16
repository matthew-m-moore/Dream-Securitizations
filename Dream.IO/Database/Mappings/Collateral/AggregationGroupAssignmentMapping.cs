using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class AggregationGroupAssignmentMapping : EntityTypeConfiguration<AggregationGroupAssignmentEntity>
    {
        public AggregationGroupAssignmentMapping()
        {
            HasKey(t => t.AggregationGroupAssignmentId);

            ToTable("AggregationGroupAssignment", Constants.DreamSchemaName);

            Property(t => t.AggregationGroupAssignmentId)
                .HasColumnName("AggregationGroupAssignmentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.AggregationGroupDataSetId).HasColumnName("AggregationGroupDataSetId");

            Property(t => t.InstrumentIdentifier).HasColumnName("InstrumentIdentifier");
            Property(t => t.AggregationGroupingIdentifier).HasColumnName("AggregationGroupingIdentifier");
            Property(t => t.AggregationGroupName).HasColumnName("AggregationGroupName");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertAggregationGroupAssignment", Constants.DreamSchemaName)
                    .Parameter(p => p.AggregationGroupDataSetId, "AggregationGroupAssignmentDataSetId")
                    .Parameter(p => p.InstrumentIdentifier, "InstrumentIdentifier")
                    .Parameter(p => p.AggregationGroupingIdentifier, "AggregationGroupingIdentifier")
                    .Parameter(p => p.AggregationGroupName, "AggregationGroupName")
                    )));
        }
    }
}
