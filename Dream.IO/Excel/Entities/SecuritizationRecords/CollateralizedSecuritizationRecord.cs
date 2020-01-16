namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class CollateralizedSecuritizationRecord
    {
        public string SecuritizationName { get; set; }
        public string PathToUnderlyingSecuritization { get; set; }
        public string CollateralizedTrancheName { get; set; }
        public double PercentageStake { get; set; }
    }
}
