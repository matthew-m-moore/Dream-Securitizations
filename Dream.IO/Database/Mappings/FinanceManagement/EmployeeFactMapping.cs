using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class EmployeeFactMapping : EntityTypeConfiguration<EmployeeFactEntity>
    {
        public EmployeeFactMapping()
        {
            HasKey(t => t.EmployeeFactDWId);

            ToTable("EmployeeFact", Constants.FinanceManagementSchemaName);

            Property(t => t.EmployeeFactDWId)
                .HasColumnName("EmployeeFactDWId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ReportEffectiveDate).HasColumnName("ReportEffectiveDate");
            Property(t => t.HireDate).HasColumnName("HireDate");
            Property(t => t.TerminationDate).HasColumnName("TerminationDate");

            Property(t => t.EmployeeKey).HasColumnName("EmployeeKey");
            Property(t => t.RegionKey).HasColumnName("RegionKey");
            Property(t => t.ProductLineKey).HasColumnName("ProductLineKey");
            Property(t => t.CostCenterKey).HasColumnName("CostCenterKey");
            Property(t => t.ActiveStatus).HasColumnName("ActiveStatus");
        }
    }
}
