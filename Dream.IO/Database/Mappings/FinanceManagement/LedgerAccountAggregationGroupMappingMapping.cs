using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LedgerAccountAggregationGroupMappingMapping : EntityTypeConfiguration<LedgerAccountAggregationGroupMappingEntity>
    {
        public LedgerAccountAggregationGroupMappingMapping()
        {
            HasKey(t => t.LedgerAccountAggregationGroupMappingId);

            ToTable("LedgerAccountAggregationGroupMapping", Constants.FinanceManagementSchemaName);

            Property(t => t.LedgerAccountAggregationGroupMappingId)
                .HasColumnName("LedgerAccountAggregationGroupMappingId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LedgerAccountAggregationGroupId).HasColumnName("LedgerAccountAggregationGroupId");
            Property(t => t.LedgerAccountKey).HasColumnName("LedgerAccountKey");
            Property(t => t.LedgerAccountAggregationGroupIdentifier).HasColumnName("LedgerAccountAggregationGroupIdentifier");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertLedgerAccountAggregationGroupMapping", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.LedgerAccountAggregationGroupId, "LedgerAccountAggregationGroupId")
                    .Parameter(p => p.LedgerAccountKey, "LedgerAccountKey")
                    .Parameter(p => p.LedgerAccountAggregationGroupIdentifier, "LedgerAccountAggregationGroupIdentifier")
                    )));
        }
    }
}
