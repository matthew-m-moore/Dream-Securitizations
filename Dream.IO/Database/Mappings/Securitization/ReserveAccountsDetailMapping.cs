using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class ReserveAccountsDetailMapping : EntityTypeConfiguration<ReserveAccountsDetailEntity>
    {
        public ReserveAccountsDetailMapping()
        {
            HasKey(t => t.ReserveAccountsDetailId);

            ToTable("ReserveAccountsDetail", Constants.DreamSchemaName);

            Property(t => t.ReserveAccountsDetailId)
                .HasColumnName("ReserveAccountsDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ReserveAccountsSetId).HasColumnName("ReserveAccountsSetId");

            Property(t => t.TrancheDetailId).HasColumnName("TrancheDetailId");
            Property(t => t.TrancheCashFlowTypeId).HasColumnName("TrancheCashFlowTypeId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertReserveAccountsDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.ReserveAccountsSetId, "ReserveAccountsSetId")
                    .Parameter(p => p.TrancheDetailId, "TrancheDetailId")
                    .Parameter(p => p.TrancheCashFlowTypeId, "TrancheCashFlowTypeId")
                    )));
        }
    }
}
