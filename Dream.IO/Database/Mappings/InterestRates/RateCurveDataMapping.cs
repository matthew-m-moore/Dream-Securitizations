using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class RateCurveDataMapping : EntityTypeConfiguration<RateCurveDataEntity>
    {
        public RateCurveDataMapping()
        {
            HasKey(t => t.RateCurveDataId);

            ToTable("RateCurveData", Constants.DreamSchemaName);

            Property(t => t.RateCurveDataId)
                .HasColumnName("RateCurveDataId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.RateCurveDataSetId).HasColumnName("RateCurveDataSetId");

            Property(t => t.MarketDateTime).HasColumnName("MarketDateTime");
            Property(t => t.ForwardDateTime).HasColumnName("ForwardDateTime");
            Property(t => t.RateCurveValue).HasColumnName("RateCurveValue");
            Property(t => t.RateIndexId).HasColumnName("RateIndexId");
            Property(t => t.MarketDataTypeId).HasColumnName("MarketDataTypeId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRateCurveData", Constants.DreamSchemaName)
                    .Parameter(p => p.RateCurveDataSetId, "RateCurveDataSetId")
                    .Parameter(p => p.MarketDateTime, "MarketDateTime")
                    .Parameter(p => p.ForwardDateTime, "ForwardDateTime")
                    .Parameter(p => p.RateCurveValue, "RateCurveValue")
                    .Parameter(p => p.RateIndexId, "RateIndexId")
                    .Parameter(p => p.MarketDataTypeId, "MarketDataTypeId")
                    )));
        }
    }
}
