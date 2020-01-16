using Dream.Common;
using Dream.IO.Database.Entities.InterestRates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.InterestRates
{
    public class MarketDataTypeMapping : EntityTypeConfiguration<MarketDataTypeEntity>
    {
        public MarketDataTypeMapping()
        {
            HasKey(t => t.MarketDataTypeId);

            ToTable("MarketDataType", Constants.DreamSchemaName);

            Property(t => t.MarketDataTypeId)
                .HasColumnName("MarketDataTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.MarketDataTypeDescription).HasColumnName("MarketDataTypeDescription");
            Property(t => t.StandardDivisor).HasColumnName("StandardDivisor");
        }
    }
}
