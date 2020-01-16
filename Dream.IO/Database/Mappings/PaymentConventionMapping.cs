using Dream.Common;
using Dream.IO.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings
{
    public class PaymentConventionMapping : EntityTypeConfiguration<PaymentConventionEntity>
    {
        public PaymentConventionMapping()
        {
            HasKey(t => t.PaymentConventionId);

            ToTable("PaymentConvention", Constants.DreamSchemaName);

            Property(t => t.PaymentConventionId)
                .HasColumnName("PaymentConventionId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PaymentConventionDescription).HasColumnName("PaymentConventionDescription");
        }
    }
}
