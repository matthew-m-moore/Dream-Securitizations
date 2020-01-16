using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class RedemptionLogicDataSetMapping : EntityTypeConfiguration<RedemptionLogicDataSetEntity>
    {
        public RedemptionLogicDataSetMapping()
        {
            HasKey(t => t.RedemptionLogicDataSetId);

            ToTable("RedemptionLogicDataSet", Constants.DreamSchemaName);

            Property(t => t.RedemptionLogicDataSetId)
                .HasColumnName("RedemptionLogicDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.RedemptionLogicTypeId).HasColumnName("RedemptionLogicTypeId");
            Property(t => t.RedemptionLogicAllowedMonthsSetId).HasColumnName("RedemptionLogicAllowedMonthsSetId");
            Property(t => t.RedemptionTranchesSetId).HasColumnName("RedemptionTranchesSetId");
            Property(t => t.RedemptionPriorityOfPaymentsSetId).HasColumnName("RedemptionPriorityOfPaymentsSetId");
            Property(t => t.PostRedemptionPriorityOfPaymentsSetId).HasColumnName("PostRedemptionPriorityOfPaymentsSetId");
            Property(t => t.RedemptionLogicTriggerValue).HasColumnName("RedemptionLogicTriggerValue");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertRedemptionLogicDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.RedemptionLogicTypeId, "RedemptionLogicTypeId")
                    .Parameter(p => p.RedemptionLogicAllowedMonthsSetId, "RedemptionLogicAllowedMonthsSetId")
                    .Parameter(p => p.RedemptionTranchesSetId, "RedemptionTranchesSetId")
                    .Parameter(p => p.RedemptionPriorityOfPaymentsSetId, "RedemptionPriorityOfPaymentsSetId")
                    .Parameter(p => p.PostRedemptionPriorityOfPaymentsSetId, "PostRedemptionPriorityOfPaymentsSetId")
                    .Parameter(p => p.RedemptionLogicTriggerValue, "RedemptionLogicTriggerValue")
                    )));
        }
    }
}
