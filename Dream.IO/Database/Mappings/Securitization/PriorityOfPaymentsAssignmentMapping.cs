using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class PriorityOfPaymentsAssignmentMapping : EntityTypeConfiguration<PriorityOfPaymentsAssignmentEntity>
    {
        public PriorityOfPaymentsAssignmentMapping()
        {
            HasKey(t => t.PriorityOfPaymentsAssignmentId);

            ToTable("PriorityOfPaymentsAssignment", Constants.DreamSchemaName);

            Property(t => t.PriorityOfPaymentsAssignmentId)
                .HasColumnName("PriorityOfPaymentsAssignmentId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.PriorityOfPaymentsSetId).HasColumnName("PriorityOfPaymentsSetId");

            Property(t => t.SeniorityRanking).HasColumnName("SeniorityRanking");
            Property(t => t.TrancheDetailId).HasColumnName("TrancheDetailId");
            Property(t => t.TrancheCashFlowTypeId).HasColumnName("TrancheCashFlowTypeId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertPriorityOfPaymentsAssignment", Constants.DreamSchemaName)
                    .Parameter(p => p.PriorityOfPaymentsSetId, "PriorityOfPaymentsSetId")
                    .Parameter(p => p.SeniorityRanking, "SeniorityRanking")
                    .Parameter(p => p.TrancheDetailId, "TrancheDetailId")
                    .Parameter(p => p.TrancheCashFlowTypeId, "TrancheCashFlowTypeId")
                    )));
        }
    }
}
