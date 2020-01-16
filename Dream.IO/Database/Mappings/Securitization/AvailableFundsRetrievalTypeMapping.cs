using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class AvailableFundsRetrievalTypeMapping : EntityTypeConfiguration<AvailableFundsRetrievalTypeEntity>
    {
        public AvailableFundsRetrievalTypeMapping()
        {
            HasKey(t => t.AvailableFundsRetrievalTypeId);

            ToTable("AvailableFundsRetrievalType", Constants.DreamSchemaName);

            Property(t => t.AvailableFundsRetrievalTypeId)
                .HasColumnName("AvailableFundsRetrievalTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.AvailableFundsRetrievalTypeDescription).HasColumnName("AvailableFundsRetrievalTypeDescription");
        }
    }
}
