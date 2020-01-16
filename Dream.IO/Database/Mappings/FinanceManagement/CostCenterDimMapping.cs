using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class CostCenterDimMapping : EntityTypeConfiguration<CostCenterDimEntity>
    {
        public CostCenterDimMapping()
        {
            HasKey(t => t.CostCenterKey);

            ToTable("CostCenterDim", Constants.FinanceManagementSchemaName);

            Property(t => t.CostCenterKey)
                .HasColumnName("CostCenterKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CostCenterId).HasColumnName("CostCenterId");
            Property(t => t.CostCenter).HasColumnName("CostCenter");
        }
    }
}
