using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class EmployeeDimMapping : EntityTypeConfiguration<EmployeeDimEntity>
    {
        public EmployeeDimMapping()
        {
            HasKey(t => t.EmployeeKey);

            ToTable("EmployeeDim", Constants.FinanceManagementSchemaName);

            Property(t => t.EmployeeKey)
                .HasColumnName("EmployeeKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            Property(t => t.Employee).HasColumnName("Employee");
            Property(t => t.EmployeeFirstName).HasColumnName("EmployeeFirstName");
            Property(t => t.EmployeeLastName).HasColumnName("EmployeeLastName");
            Property(t => t.BusinessTitle).HasColumnName("BusinessTitle");
            Property(t => t.WorkerType).HasColumnName("WorkerType");
        }
    }
}
