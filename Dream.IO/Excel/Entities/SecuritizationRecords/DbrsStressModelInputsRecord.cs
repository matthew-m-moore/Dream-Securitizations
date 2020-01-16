namespace Dream.IO.Excel.Entities.SecuritizationRecords
{
    public class DbrsStressModelInputsRecord : SecuritizationInputsRecord
    {
        public double DefaultRate { get; set; }
        public double? LossGivenDefault { get; set; }
        public int ForeclosureTermInMonths { get; set; }
        public int ReperformanceTermInMonths { get; set; }
        public string DefaultPropertyState { get; set; }

        public double DefaultRateSecondState { get; set; }
        public double? LossGivenDefaultSecondState { get; set; }
        public int ForeclosureTermInMonthsSecondState { get; set; }
        public int ReperformanceTermInMonthsSecondState { get; set; }
        public string SecondPropertyState { get; set; }

        public double DefaultRateThirdState { get; set; }
        public double? LossGivenDefaultThirdState { get; set; }
        public int ForeclosureTermInMonthsThirdState { get; set; }
        public int ReperformanceTermInMonthsThirdState { get; set; }
        public string ThirdPropertyState { get; set; }

        public int TotalNumberOfDefaultSequences { get; set; }
    }
}
