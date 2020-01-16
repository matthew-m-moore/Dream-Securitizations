using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class AllowedMonthMapping : EntityTypeConfiguration<AllowedMonthEntity>
    {
        public AllowedMonthMapping()
        {
            HasKey(t => t.AllowedMonthId);

            ToTable("AllowedMonth", Constants.DreamSchemaName);

            Property(t => t.AllowedMonthId)
                .HasColumnName("AllowedMonthId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.AllowedMonthShortDescription).HasColumnName("AllowedMonthShortDescription");
            Property(t => t.AllowedMonthLongDescription).HasColumnName("AllowedMonthLongDescription");
        }
    }
}
