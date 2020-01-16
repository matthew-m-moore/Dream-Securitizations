using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class TrancheCashFlowTypeMapping : EntityTypeConfiguration<TrancheCashFlowTypeEntity>
    {
        public TrancheCashFlowTypeMapping()
        {
            HasKey(t => t.TrancheCashFlowTypeId);

            ToTable("TrancheCashFlowType", Constants.DreamSchemaName);

            Property(t => t.TrancheCashFlowTypeId)
                .HasColumnName("TrancheCashFlowTypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.TrancheCashFlowTypeDescription).HasColumnName("TrancheCashFlowTypeDescription");
        }
    }
}
