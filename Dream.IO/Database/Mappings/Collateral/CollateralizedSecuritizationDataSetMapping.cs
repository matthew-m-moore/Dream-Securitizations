using Dream.Common;
using Dream.IO.Database.Entities.Collateral;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Dream.IO.Database.Mappings.Collateral
{
    public class CollateralizedSecuritizationDataSetMapping : EntityTypeConfiguration<CollateralizedSecuritizationDataSetEntity>
    {
        public CollateralizedSecuritizationDataSetMapping()
        {
            HasKey(t => t.CollateralizedSecuritizationDataSetId);

            ToTable("CollateralizedSecuritizationDataSet", Constants.DreamSchemaName);

            Property(t => t.CollateralizedSecuritizationDataSetId)
                .HasColumnName("CollateralizedSecuritizationDataSetId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CutOffDate).HasColumnName("CutOffDate");
            Property(t => t.CollateralizedSecuritizationDataSetDescription).HasColumnName("CollateralizedSecuritizationDataSetDescription");

            MapToStoredProcedures(s =>
                s.Insert((i => i.HasName("InsertCollateralizedSecuritizationDataSet", Constants.DreamSchemaName)
                    .Parameter(p => p.CutOffDate, "CutOffDate")
                    .Parameter(p => p.CollateralizedSecuritizationDataSetDescription, "CollateralizedSecuritizationDataSetDescription")
                    )));
        }
    }
}
