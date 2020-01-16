using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class LedgerAccountAggregationGroupMapping : EntityTypeConfiguration<LedgerAccountAggregationGroupEntity>
    {
        public LedgerAccountAggregationGroupMapping()
        {
            HasKey(t => t.LedgerAccountAggregationGroupId);

            ToTable("LedgerAccountAggregationGroup", Constants.FinanceManagementSchemaName);

            Property(t => t.LedgerAccountAggregationGroupId)
                .HasColumnName("LedgerAccountAggregationGroupId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LedgerAccountAggregationGroupDescription).HasColumnName("LedgerAccountAggregationGroupDescription");
            Property(t => t.IsAggregationGroupActive).HasColumnName("IsAggregationGroupActive");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertLedgerAccountAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.LedgerAccountAggregationGroupDescription, "LedgerAccountAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));

            MapToStoredProcedures(s =>
                s.Update((u => u.HasName("UpdateLedgerAccountAggregationGroup", Constants.FinanceManagementSchemaName)
                    .Parameter(p => p.LedgerAccountAggregationGroupId, "LedgerAccountAggregationGroupId")
                    .Parameter(p => p.LedgerAccountAggregationGroupDescription, "LedgerAccountAggregationGroupDescription")
                    .Parameter(p => p.IsAggregationGroupActive, "IsAggregationGroupActive")
                    )));
        }
    }
}
