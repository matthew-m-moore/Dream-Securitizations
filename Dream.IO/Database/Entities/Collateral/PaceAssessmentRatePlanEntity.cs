namespace Dream.IO.Database.Entities.Collateral
{
    public class PaceAssessmentRatePlanEntity
    {
        public int PaceAssessmentRatePlanId { get; set; }
        public int PaceAssessmentRatePlanTermSetId { get; set; }
        public int PropertyStateId { get; set; }
        public double CouponRate { get; set; }
        public double BuyDownRate { get; set; }
        public int TermInYears { get; set; }
    }
}
