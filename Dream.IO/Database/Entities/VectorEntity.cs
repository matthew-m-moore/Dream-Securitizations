namespace Dream.IO.Database.Entities
{
    public class VectorEntity
    {
        public int VectorId { get; set; }
        public int VectorParentId { get; set; }
        public int VectorPeriod { get; set; }
        public double VectorValue { get; set; }
    }
}
