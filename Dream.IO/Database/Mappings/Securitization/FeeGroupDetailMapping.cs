using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class FeeGroupDetailMapping : EntityTypeConfiguration<FeeGroupDetailEntity>
    {
        public FeeGroupDetailMapping()
        {
            HasKey(t => t.FeeGroupDetailId);

            ToTable("FeeGroupDetail", Constants.DreamSchemaName);

            Property(t => t.FeeGroupDetailId)
                .HasColumnName("FeeGroupDetailId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.FeeGroupName).HasColumnName("FeeGroupName");
            Property(t => t.FeeRate).HasColumnName("FeeRate");
            Property(t => t.FeePerUnit).HasColumnName("FeePerUnit");
            Property(t => t.FeeMinimum).HasColumnName("FeeMinimum");
            Property(t => t.FeeMaximum).HasColumnName("FeeMaximum");
            Property(t => t.FeeIncreaseRate).HasColumnName("FeeIncreaseRate");
            Property(t => t.FeeRateUpdateFrequencyInMonths).HasColumnName("FeeRateUpdateFrequencyInMonths");
            Property(t => t.FeeRollingAverageInMonths).HasColumnName("FeeRollingAverageInMonths");
            Property(t => t.UseStartingBalanceToDetermineFee).HasColumnName("UseStartingBalanceToDetermineFee");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertFeeGroupDetail", Constants.DreamSchemaName)
                    .Parameter(p => p.FeeGroupName, "FeeGroupName")
                    .Parameter(p => p.FeeRate, "FeeRate")
                    .Parameter(p => p.FeePerUnit, "FeePerUnit")
                    .Parameter(p => p.FeeMinimum, "FeeMinimum")
                    .Parameter(p => p.FeeMaximum, "FeeMaximum")
                    .Parameter(p => p.FeeIncreaseRate, "FeeIncreaseRate")
                    .Parameter(p => p.FeeRateUpdateFrequencyInMonths, "FeeRateUpdateFrequencyInMonths")
                    .Parameter(p => p.FeeRollingAverageInMonths, "FeeRollingAverageInMonths")
                    .Parameter(p => p.UseStartingBalanceToDetermineFee, "UseStartingBalanceToDetermineFee")
                    )));
        }
    }
}
