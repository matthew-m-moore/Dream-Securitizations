using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PerformanceAssumptionDataSetMapping : EntityTypeConfiguration<PerformanceAssumptionDataSetEntity>
    {
        public PerformanceAssumptionDataSetMapping()
        {
            HasKey(t => t.PerformanceAssumptionDataSetId);

            ToTable("PerformanceAssumptionDataSet", Constants.DreamSchemaName);

            Property(t => t.PerformanceAssumptionDataSetId)
                .HasColumnName("PerformanceAssumptionDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.PerformanceAssumptionDataSetDescription).HasColumnName("PerformanceAssumptionDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPerformanceAssumptionDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.PerformanceAssumptionDataSetDescription, "PerformanceAssumptionDataSetDescription")
                    )));
        }
    }
}
