using Dream.Common;
using Dream.IO.Database.Entities.FinanceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.FinanceManagement
{
    public class RegionDimMapping : EntityTypeConfiguration<RegionDimEntity>
    {
        public RegionDimMapping()
        {
            HasKey(t => t.RegionKey);

            ToTable("RegionDim", Constants.FinanceManagementSchemaName);

            Property(t => t.RegionKey)
                .HasColumnName("RegionKey")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Region).HasColumnName("Region");
        }
    }
}
