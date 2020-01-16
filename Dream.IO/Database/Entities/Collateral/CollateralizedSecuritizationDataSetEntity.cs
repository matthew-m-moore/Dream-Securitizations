using System;

namespace Dream.IO.Database.Entities.Collateral
{
    public class CollateralizedSecuritizationDataSetEntity
    {
        public int CollateralizedSecuritizationDataSetId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string CollateralizedSecuritizationDataSetDescription { get; set; }
    }
}
