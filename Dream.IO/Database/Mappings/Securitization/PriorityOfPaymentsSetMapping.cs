using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class PriorityOfPaymentsSetMapping : EntityTypeConfiguration<PriorityOfPaymentsSetEntity>
    {
        public PriorityOfPaymentsSetMapping()
        {
            HasKey(t => t.PriorityOfPaymentsSetId);

            ToTable("PriorityOfPaymentsSet", Constants.DreamSchemaName);

            Property(t => t.PriorityOfPaymentsSetId)
                .HasColumnName("PriorityOfPaymentsSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.PriorityOfPaymentsSetDescription).HasColumnName("PriorityOfPaymentsSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPriorityOfPaymentsSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.PriorityOfPaymentsSetDescription, "PriorityOfPaymentsSetDescription")
                    )));
        }
    }
}
