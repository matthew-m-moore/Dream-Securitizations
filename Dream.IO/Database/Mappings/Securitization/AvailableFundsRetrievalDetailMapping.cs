using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class AvailableFundsRetrievalDetailMapping : EntityTypeConfiguration<AvailableFundsRetrievalDetailEntity>
    {
        public AvailableFundsRetrievalDetailMapping()
        {
            HasKey(t => t.AvailableFundsRetrievalDetailId);

            ToTable("AvailableFundsRetrievalDetail", Constants.DreamSchemaName);

            Property(t => t.AvailableFundsRetrievalDetailId)
                .HasColumnName("AvailableFundsRetrievalDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.AvailableFundsRetrievalTypeId).HasColumnName("AvailableFundsRetrievalTypeId");
            Property(t => t.AvailableFundsRetrievalValue).HasColumnName("AvailableFundsRetrievalValue");
            Property(t => t.AvailableFundsRetrievalInteger).HasColumnName("AvailableFundsRetrievalInteger");
            Property(t => t.AvailableFundsRetrievalDate).HasColumnName("AvailableFundsRetrievalDate");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertAvailableFundsRetrievalDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.AvailableFundsRetrievalTypeId, "AvailableFundsRetrievalTypeId")
                    .Parameter(p => p.AvailableFundsRetrievalValue, "AvailableFundsRetrievalValue")
                    .Parameter(p => p.AvailableFundsRetrievalInteger, "AvailableFundsRetrievalInteger")
                    .Parameter(p => p.AvailableFundsRetrievalDate, "AvailableFundsRetrievalDate")
                    )));
        }
    }
}
