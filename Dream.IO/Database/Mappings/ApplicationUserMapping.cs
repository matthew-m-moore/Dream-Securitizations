using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class ApplicationUserMapping : EntityTypeConfiguration<ApplicationUserEntity>
    {
        public ApplicationUserMapping()
        {
            HasKey(t => t.ApplicationUserId);

            ToTable("ApplicationUser", Constants.DreamSchemaName);

            Property(t => t.ApplicationUserId)
                .HasColumnName("ApplicationUserId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.NetworkUserNameIdentifier).HasColumnName("NetworkUserNameIdentifier");
            Property(t => t.ApplicationDisplayableNickName).HasColumnName("ApplicationDisplayableNickName");
            Property(t => t.IsReadOnlyUser).HasColumnName("IsReadOnlyUser");
        }
    }
}
