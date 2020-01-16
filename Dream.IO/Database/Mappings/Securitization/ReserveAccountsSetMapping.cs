using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class ReserveAccountsSetMapping : EntityTypeConfiguration<ReserveAccountsSetEntity>
    {
        public ReserveAccountsSetMapping()
        {
            HasKey(t => t.ReserveAccountsSetId);

            ToTable("ReserveAccountsSet", Constants.DreamSchemaName);

            Property(t => t.ReserveAccountsSetId)
                .HasColumnName("ReserveAccountsSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertReserveAccountsSet", Constants.DreamSchemaName))));
        }
    }
}
