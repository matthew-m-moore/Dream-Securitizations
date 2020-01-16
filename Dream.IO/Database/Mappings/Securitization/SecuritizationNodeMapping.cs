using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationNodeMapping : EntityTypeConfiguration<SecuritizationNodeEntity>
    {
        public SecuritizationNodeMapping()
        {
            HasKey(t => t.SecuritizationNodeId);

            ToTable("SecuritizationNode", Constants.DreamSchemaName);

            Property(t => t.SecuritizationNodeId)
                .HasColumnName("SecuritizationNodeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationNodeDataSetId).HasColumnName("SecuritizationNodeDataSetId");

            Property(t => t.SecuritizationNodeName).HasColumnName("SecuritizationNodeName");
            Property(t => t.SecuritizationChildNodeName).HasColumnName("SecuritizationChildNodeName");
            Property(t => t.FundsDistributionTypeId).HasColumnName("FundsDistributionTypeId");
            Property(t => t.TrancheDetailId).HasColumnName("TrancheDetailId");
            Property(t => t.TranchePricingTypeId).HasColumnName("TranchePricingTypeId");
            Property(t => t.TranchePricingRateIndexId).HasColumnName("TranchePricingRateIndexId");
            Property(t => t.TranchePricingValue).HasColumnName("TranchePricingValue");
            Property(t => t.TranchePricingDayCountConventionId).HasColumnName("TranchePricingDayCountConventionId");
            Property(t => t.TranchePricingCompoundingConventionId).HasColumnName("TranchePricingCompoundingConventionId");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationNode", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationNodeDataSetId, "SecuritizationNodeDataSetId")
                    .Parameter(p => p.SecuritizationNodeName, "SecuritizationNodeName")
                    .Parameter(p => p.SecuritizationChildNodeName, "SecuritizationChildNodeName")
                    .Parameter(p => p.FundsDistributionTypeId, "FundsDistributionTypeId")
                    .Parameter(p => p.TrancheDetailId, "TrancheDetailId")
                    .Parameter(p => p.TranchePricingTypeId, "TranchePricingTypeId")
                    .Parameter(p => p.TranchePricingRateIndexId, "TranchePricingRateIndexId")
                    .Parameter(p => p.TranchePricingValue, "TranchePricingValue")
                    .Parameter(p => p.TranchePricingDayCountConventionId, "TranchePricingDayCountConventionId")
                    .Parameter(p => p.TranchePricingCompoundingConventionId, "TranchePricingCompoundingConventionId")
                    )));
        }
    }
}
