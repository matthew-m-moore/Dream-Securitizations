using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class RateCurveDataSetMapping : EntityTypeConfiguration<RateCurveDataSetEntity>
    {
        public RateCurveDataSetMapping()
        {
            HasKey(t => t.RateCurveDataSetId);

            ToTable("RateCurveDataSet", Constants.DreamSchemaName);

            Property(t => t.RateCurveDataSetId)
                .HasColumnName("RateCurveDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.RateCurveDataSetDescription).HasColumnName("RateCurveDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRateCurveDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.RateCurveDataSetDescription, "RateCurveDataSetDescription")
                    )));
        }
    }
}
