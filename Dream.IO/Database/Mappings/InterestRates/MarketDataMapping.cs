using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class MarketDataMapping : EntityTypeConfiguration<MarketDataEntity>
    {
        public MarketDataMapping()
        {
            HasKey(t => t.MarketDataId);

            ToTable("MarketData", Constants.DreamSchemaName);

            Property(t => t.MarketDataId)
                .HasColumnName("MarketDataId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.MarketDataSetId).HasColumnName("MarketDataSetId");          

            Property(t => t.MarketDateTime).HasColumnName("MarketDateTime");
            Property(t => t.DataValue).HasColumnName("DataValue");
            Property(t => t.RateIndexId).HasColumnName("RateIndexId");
            Property(t => t.MarketDataTypeId).HasColumnName("MarketDataTypeId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertMarketData", Constants.DreamSchemaName)
                    .Parameter(p => p.MarketDataSetId, "MarketDataSetId")
                    .Parameter(p => p.MarketDateTime, "MarketDateTime")
                    .Parameter(p => p.DataValue, "DataValue")
                    .Parameter(p => p.RateIndexId, "RateIndexId")
                    .Parameter(p => p.MarketDataTypeId, "MarketDataTypeId")
                    )));
        }
    }
}
