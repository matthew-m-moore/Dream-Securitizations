using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class AggregationGroupDataSetMapping : EntityTypeConfiguration<AggregationGroupDataSetEntity>
    {
        public AggregationGroupDataSetMapping()
        {
            HasKey(t => t.AggregationGroupDataSetId);

            ToTable("AggregationGroupDataSet", Constants.DreamSchemaName);

            Property(t => t.AggregationGroupDataSetId)
                .HasColumnName("AggregationGroupDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.AggregationGroupDataSetDescription).HasColumnName("AggregationGroupDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertAggregationGroupDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.AggregationGroupDataSetDescription, "AggregationGroupDataSetDescription")
                    )));
        }
    }
}
