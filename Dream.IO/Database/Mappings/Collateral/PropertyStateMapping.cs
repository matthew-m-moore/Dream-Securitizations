using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class PropertyStateMapping : EntityTypeConfiguration<PropertyStateEntity>
    {
        public PropertyStateMapping()
        {
            HasKey(t => t.PropertyStateId);

            ToTable("PropertyState", Constants.DatabaseOwnerSchemaName);

            Property(t => t.PropertyStateId)
                .HasColumnName("PropertyStateId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PropertyState).HasColumnName("PropertyState");
        }
    }
}
