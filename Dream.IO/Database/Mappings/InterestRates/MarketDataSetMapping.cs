using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class MarketDataSetMapping : EntityTypeConfiguration<MarketDataSetEntity>
    {
        public MarketDataSetMapping()
        {
            HasKey(t => t.MarketDataSetId);

            ToTable("MarketDataSet", Constants.DreamSchemaName);

            Property(t => t.MarketDataSetId)
                .HasColumnName("MarketDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.MarketDataSetDescription).HasColumnName("MarketDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertMarketDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.MarketDataSetDescription, "MarketDataSetDescription")
                    )));
        }
    }
}
