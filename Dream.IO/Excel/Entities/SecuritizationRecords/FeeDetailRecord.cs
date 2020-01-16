namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class FeeDetailRecord
    {
        public string FeeGroupName { get; set; }
        public string FeeName { get; set; }
        public double AnnualFeeAmount { get; set; }
        public bool? ApplyFeeIncreaseRate { get; set; }
    }
}
