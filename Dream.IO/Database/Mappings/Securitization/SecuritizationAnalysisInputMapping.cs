using Dream.Common;
using Dream.IO.Database.Entities.Securitization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Securitization
{
    public class SecuritizationAnalysisInputMapping : EntityTypeConfiguration<SecuritizationAnalysisInputEntity>
    {
        public SecuritizationAnalysisInputMapping()
        {
            HasKey(t => t.SecuritizationAnalysisInputId);

            ToTable("SecuritizationAnalysisInput", Constants.DreamSchemaName);

            Property(t => t.SecuritizationAnalysisInputId)
                .HasColumnName("SecuritizationAnalysisInputId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SecuritizationAnalysisScenarioDescription).HasColumnName("SecuritizationAnalysisScenarioDescription");
            Property(t => t.AdditionalAnalysisScenarioDescription).HasColumnName("AdditionalAnalysisScenarioDescription");

            Property(t => t.CollateralCutOffDate).HasColumnName("CollateralCutOffDate");
            Property(t => t.CashFlowStartDate).HasColumnName("CashFlowStartDate");
            Property(t => t.InterestAccrualStartDate).HasColumnName("InterestAccrualStartDate");
            Property(t => t.SecuritizationClosingDate).HasColumnName("SecuritizationClosingDate");
            Property(t => t.SecuritizationFirstCashFlowDate).HasColumnName("SecuritizationFirstCashFlowDate");

            Property(t => t.SelectedAggregationGrouping).HasColumnName("SelectedAggregationGrouping");
            Property(t => t.SelectedPerformanceAssumption).HasColumnName("SelectedPerformanceAssumption");
            Property(t => t.SelectedPerformanceAssumptionGrouping).HasColumnName("SelectedPerformanceAssumptionGrouping");

            Property(t => t.NominalSpreadRateIndexGroupId).HasColumnName("NominalSpreadRateIndexGroupId");
            Property(t => t.CurveSpreadRateIndexId).HasColumnName("CurveSpreadRateIndexId");

            Property(t => t.UseReplines).HasColumnName("UseReplines");
            Property(t => t.SeparatePrepaymentInterest).HasColumnName("SeparatePrepaymentInterest");

            Property(t => t.PreFundingAmount).HasColumnName("PreFundingAmount");
            Property(t => t.PreFundingBondCount).HasColumnName("PreFundingBondCount");
            Property(t => t.CleanUpCallPercentage).HasColumnName("CleanUpCallPercentage");

            Property(t => t.UsePreFundingStartDate).HasColumnName("UsePreFundingStartDate");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertSecuritizationAnalysisInput", Constants.DreamSchemaName)
                    .Parameter(p => p.SecuritizationAnalysisScenarioDescription, "SecuritizationAnalysisScenarioDescription")
                    .Parameter(p => p.AdditionalAnalysisScenarioDescription, "AdditionalAnalysisScenarioDescription")
                    .Parameter(p => p.CollateralCutOffDate, "CollateralCutOffDate")
                    .Parameter(p => p.CashFlowStartDate, "CashFlowStartDate")
                    .Parameter(p => p.InterestAccrualStartDate, "InterestAccrualStartDate")
                    .Parameter(p => p.SecuritizationClosingDate, "SecuritizationClosingDate")
                    .Parameter(p => p.SecuritizationFirstCashFlowDate, "SecuritizationFirstCashFlowDate")
                    .Parameter(p => p.SelectedAggregationGrouping, "SelectedAggregationGrouping")
                    .Parameter(p => p.SelectedPerformanceAssumption, "SelectedPerformanceAssumption")
                    .Parameter(p => p.SelectedPerformanceAssumptionGrouping, "SelectedPerformanceAssumptionGrouping")
                    .Parameter(p => p.NominalSpreadRateIndexGroupId, "NominalSpreadRateIndexGroupId")
                    .Parameter(p => p.CurveSpreadRateIndexId, "CurveSpreadRateIndexId")
                    .Parameter(p => p.UseReplines, "UseReplines")
                    .Parameter(p => p.SeparatePrepaymentInterest, "SeparatePrepaymentInterest")
                    .Parameter(p => p.PreFundingAmount, "PreFundingAmount")
                    .Parameter(p => p.PreFundingBondCount, "PreFundingBondCount")
                    .Parameter(p => p.CleanUpCallPercentage, "CleanUpCallPercentage")
                    .Parameter(p => p.UsePreFundingStartDate, "UsePreFundingStartDate")
                    )));
        }
    }
}
