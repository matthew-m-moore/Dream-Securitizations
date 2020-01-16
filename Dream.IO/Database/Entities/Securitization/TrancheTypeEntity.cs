namespace Dream.IO.Database.Entities.Securitization
{
    public class TrancheTypeEntity
    {
        public int TrancheTypeId { get; set; }
        public string TrancheTypeDescription { get; set; }
        public bool IsVisible { get; set; }
        public bool IsFeeTranche { get; set; }
        public bool IsReserveTranche { get; set; }
        public bool? IsInterestPayingTranche { get; set; }
    }
}
