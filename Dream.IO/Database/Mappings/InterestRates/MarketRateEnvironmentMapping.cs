using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class MarketRateEnvironmentMapping : EntityTypeConfiguration<MarketRateEnvironmentEntity>
    {
        public MarketRateEnvironmentMapping()
        {
            HasKey(t => t.MarketRateEnvironmentId);

            ToTable("MarketRateEnvironment", Constants.DreamSchemaName);

            Property(t => t.MarketRateEnvironmentId)
                .HasColumnName("MarketRateEnvironmentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.MarketRateEnvironmentDescription).HasColumnName("MarketRateEnvironmentDescription");
            Property(t => t.MarketDataSetId).HasColumnName("MarketDataSetId");
            Property(t => t.RateCurveDataSetId).HasColumnName("RateCurveDataSetId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertMarketRateEnvironment", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.MarketRateEnvironmentDescription, "MarketRateEnvironmentDescription")
                    .Parameter(p => p.MarketDataSetId, "MarketDataSetId")
                    .Parameter(p => p.RateCurveDataSetId, "RateCurveDataSetId")
                    )));
        }
    }
}
